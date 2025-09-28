using Demo.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace Demo.Services
{
    /// <summary>
    /// Interface for Oracle database connection management
    /// </summary>
    public interface IOracleConnectionService
    {
        Task<ConnectionStatus> TestConnectionAsync();
        Task<OracleConnection?> GetConnectionAsync();
        void SetSharedConnection(OracleConnection? connection);
        OracleConnection? GetSharedConnection();
    }

    /// <summary>
    /// Oracle database connection service implementation
    /// </summary>
    public class OracleConnectionService : IOracleConnectionService, IDisposable
    {
        private readonly OracleConnectionSettings _settings;
        private readonly ILogger<OracleConnectionService> _logger;
        private OracleConnection? _sharedConnection;
        private readonly object _lockObject = new();

        public OracleConnectionService(
            IOptions<OracleConnectionSettings> settings,
            ILogger<OracleConnectionService> logger)
        {
            _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Test Oracle database connection
        /// </summary>
        public async Task<ConnectionStatus> TestConnectionAsync()
        {
            var result = new ConnectionStatus();

            try
            {
                _logger.LogInformation("Testing Oracle database connection...");
                _logger.LogDebug("Connection string configured for schema: {Schema}", _settings.Schema);

                if (string.IsNullOrWhiteSpace(_settings.ConnectionString))
                {
                    result.Message = "Oracle connection string is not configured";
                    _logger.LogError(result.Message);
                    return result;
                }

                using var connection = new OracleConnection(_settings.ConnectionString);
                
                // Note: ConnectionTimeout is read-only, set via connection string

                await connection.OpenAsync();
                
                // Test with a simple query
                using var command = connection.CreateCommand();
                command.CommandText = "SELECT 1 FROM DUAL";
                command.CommandTimeout = _settings.CommandTimeout;

                var testResult = await command.ExecuteScalarAsync();

                result.IsConnected = testResult != null;
                result.Message = result.IsConnected 
                    ? "Oracle connection successful" 
                    : "Oracle connection test failed";

                _logger.LogInformation("Oracle connection test result: {Result}", result.Message);
                return result;
            }
            catch (Exception ex)
            {
                result.Exception = ex;
                result.Message = $"Oracle connection failed: {ex.Message}";
                _logger.LogError(ex, "Oracle connection test failed: {Message}", ex.Message);
                return result;
            }
        }

        /// <summary>
        /// Get a new Oracle connection
        /// </summary>
        public async Task<OracleConnection?> GetConnectionAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_settings.ConnectionString))
                {
                    _logger.LogWarning("Oracle connection string is not configured");
                    return null;
                }

                var connection = new OracleConnection(_settings.ConnectionString);
                // Note: ConnectionTimeout is read-only, set via connection string
                
                await connection.OpenAsync();
                _logger.LogDebug("New Oracle connection opened successfully");
                return connection;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create Oracle connection: {Message}", ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Set the shared connection for use across the application
        /// </summary>
        public void SetSharedConnection(OracleConnection? connection)
        {
            lock (_lockObject)
            {
                _sharedConnection?.Dispose();
                _sharedConnection = connection;
                
                if (connection != null)
                {
                    _logger.LogInformation("Shared Oracle connection established");
                }
                else
                {
                    _logger.LogWarning("Shared Oracle connection cleared");
                }
            }
        }

        /// <summary>
        /// Get the shared connection
        /// </summary>
        public OracleConnection? GetSharedConnection()
        {
            lock (_lockObject)
            {
                if (_sharedConnection?.State != ConnectionState.Open)
                {
                    _logger.LogWarning("Shared Oracle connection is not available or not open");
                    return null;
                }
                
                return _sharedConnection;
            }
        }

        /// <summary>
        /// Dispose of resources
        /// </summary>
        public void Dispose()
        {
            lock (_lockObject)
            {
                _sharedConnection?.Dispose();
                _sharedConnection = null;
            }
            GC.SuppressFinalize(this);
        }
    }
}