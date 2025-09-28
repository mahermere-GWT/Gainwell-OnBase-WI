using CsvHelper;
using CsvHelper.Configuration;
using Demo.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Globalization;
using System.Text;

namespace Demo.Services
{
    /// <summary>
    /// Interface for CSV processing and bulk loading
    /// </summary>
    public interface ICsvBulkLoadService
    {
        Task<ExecutionResult> ProcessCsvFileAsync(string csvFilePath);
        Task<ExecutionResult> BulkLoadDataAsync(IEnumerable<DurQuarterlyLoadRecord> records);
    }

    /// <summary>
    /// CSV processing and Oracle bulk loading service
    /// </summary>
    public class CsvBulkLoadService : ICsvBulkLoadService
    {
        private readonly IOracleConnectionService _oracleService;
        private readonly BulkLoadSettings _bulkSettings;
        private readonly OracleConnectionSettings _oracleSettings;
        private readonly ILogger<CsvBulkLoadService> _logger;

        public CsvBulkLoadService(
            IOracleConnectionService oracleService,
            IOptions<BulkLoadSettings> bulkSettings,
            IOptions<OracleConnectionSettings> oracleSettings,
            ILogger<CsvBulkLoadService> logger)
        {
            _oracleService = oracleService ?? throw new ArgumentNullException(nameof(oracleService));
            _bulkSettings = bulkSettings?.Value ?? throw new ArgumentNullException(nameof(bulkSettings));
            _oracleSettings = oracleSettings?.Value ?? throw new ArgumentNullException(nameof(oracleSettings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Process CSV file and bulk load data
        /// </summary>
        public async Task<ExecutionResult> ProcessCsvFileAsync(string csvFilePath)
        {
            var result = new ExecutionResult();

            try
            {
                _logger.LogInformation("Starting CSV file processing: {FilePath}", csvFilePath);

                if (!File.Exists(csvFilePath))
                {
                    result.Message = $"CSV file not found: {csvFilePath}";
                    _logger.LogError(result.Message);
                    return result;
                }

                var records = await ReadCsvFileAsync(csvFilePath);
                if (!records.Any())
                {
                    result.Success = true;
                    result.Message = "CSV file is empty or contains no valid records";
                    _logger.LogWarning(result.Message);
                    return result;
                }

                _logger.LogInformation("Read {RecordCount} records from CSV file", records.Count());

                // Bulk load the data
                var bulkLoadResult = await BulkLoadDataAsync(records);
                result.Success = bulkLoadResult.Success;
                result.Message = bulkLoadResult.Message;
                result.RecordsProcessed = bulkLoadResult.RecordsProcessed;
                result.Exception = bulkLoadResult.Exception;

                return result;
            }
            catch (Exception ex)
            {
                result.Exception = ex;
                result.Message = $"CSV processing failed: {ex.Message}";
                _logger.LogError(ex, "CSV file processing failed: {Message}", ex.Message);
                return result;
            }
        }

        /// <summary>
        /// Read CSV file and parse records
        /// </summary>
        private async Task<IEnumerable<DurQuarterlyLoadRecord>> ReadCsvFileAsync(string csvFilePath)
        {
            var records = new List<DurQuarterlyLoadRecord>();

            try
            {
                _logger.LogDebug("Reading CSV file: {FilePath}", csvFilePath);

                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true,
                    MissingFieldFound = null,
                    BadDataFound = null,
                    TrimOptions = TrimOptions.Trim
                };

                using var reader = new StreamReader(csvFilePath, Encoding.UTF8);
                using var csv = new CsvReader(reader, config);

                await foreach (var record in csv.GetRecordsAsync<DurQuarterlyLoadRecord>())
                {
                    records.Add(record);
                }

                _logger.LogDebug("Successfully read {RecordCount} records from CSV", records.Count);
                return records;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to read CSV file: {Message}", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Bulk load data into Oracle table
        /// </summary>
        public async Task<ExecutionResult> BulkLoadDataAsync(IEnumerable<DurQuarterlyLoadRecord> records)
        {
            var result = new ExecutionResult();

            try
            {
                _logger.LogInformation("Starting bulk load operation for {RecordCount} records", records.Count());

                var connection = _oracleService.GetSharedConnection();
                if (connection == null)
                {
                    result.Message = "Oracle connection is not available for bulk loading";
                    _logger.LogError(result.Message);
                    return result;
                }

                var recordList = records.ToList();
                int totalRecords = recordList.Count;
                int processedRecords = 0;

                // Process in batches
                for (int i = 0; i < totalRecords; i += _bulkSettings.BatchSize)
                {
                    var batch = recordList.Skip(i).Take(_bulkSettings.BatchSize);
                    var batchResult = await ProcessBatchAsync(connection, batch, i + 1);

                    if (!batchResult.Success)
                    {
                        result.Message = batchResult.Message;
                        result.Exception = batchResult.Exception;
                        result.RecordsProcessed = processedRecords;
                        return result;
                    }

                    processedRecords += batchResult.RecordsProcessed;
                    _logger.LogDebug("Processed batch {BatchNumber}: {ProcessedRecords}/{TotalRecords} records", 
                        (i / _bulkSettings.BatchSize) + 1, processedRecords, totalRecords);
                }

                result.Success = true;
                result.RecordsProcessed = processedRecords;
                result.Message = $"Successfully bulk loaded {processedRecords} records";
                _logger.LogInformation("Bulk load completed successfully: {RecordsProcessed} records", processedRecords);

                return result;
            }
            catch (Exception ex)
            {
                result.Exception = ex;
                result.Message = $"Bulk load failed: {ex.Message}";
                _logger.LogError(ex, "Bulk load operation failed: {Message}", ex.Message);
                return result;
            }
        }

        /// <summary>
        /// Process a batch of records
        /// </summary>
        private async Task<ExecutionResult> ProcessBatchAsync(OracleConnection connection, IEnumerable<DurQuarterlyLoadRecord> batch, int batchNumber)
        {
            var result = new ExecutionResult();

            try
            {
                var batchList = batch.ToList();
                var insertSql = BuildInsertSql();

                using var transaction = connection.BeginTransaction();
                try
                {
                    foreach (var record in batchList)
                    {
                        using var command = new OracleCommand(insertSql, connection);
                        command.Transaction = transaction;
                        command.CommandTimeout = _oracleSettings.CommandTimeout;

                        // Add parameters - adjust based on your table structure
                        command.Parameters.Add("Column1", OracleDbType.Varchar2).Value = record.Column1 ?? (object)DBNull.Value;
                        command.Parameters.Add("Column2", OracleDbType.Varchar2).Value = record.Column2 ?? (object)DBNull.Value;
                        command.Parameters.Add("Column3", OracleDbType.Varchar2).Value = record.Column3 ?? (object)DBNull.Value;
                        command.Parameters.Add("Column4", OracleDbType.Varchar2).Value = record.Column4 ?? (object)DBNull.Value;
                        command.Parameters.Add("Column5", OracleDbType.Varchar2).Value = record.Column5 ?? (object)DBNull.Value;

                        await command.ExecuteNonQueryAsync();
                    }

                    await transaction.CommitAsync();
                    result.Success = true;
                    result.RecordsProcessed = batchList.Count;
                    return result;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                result.Exception = ex;
                result.Message = $"Batch {batchNumber} processing failed: {ex.Message}";
                _logger.LogError(ex, "Batch processing failed for batch {BatchNumber}: {Message}", batchNumber, ex.Message);
                return result;
            }
        }

        /// <summary>
        /// Build INSERT SQL statement
        /// </summary>
        private string BuildInsertSql()
        {
            var tableName = $"{_oracleSettings.Schema}.{_oracleSettings.TableName}";
            
            // Adjust column names based on your actual table structure
            return $@"
                INSERT INTO {tableName} 
                (COLUMN1, COLUMN2, COLUMN3, COLUMN4, COLUMN5, CREATED_DATE) 
                VALUES 
                (:Column1, :Column2, :Column3, :Column4, :Column5, SYSTIMESTAMP)";
        }
    }
}