namespace Demo.Models
{
    /// <summary>
    /// Application configuration settings
    /// </summary>
    public class ApplicationSettings
    {
        public string ApplicationName { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string LogDirectory { get; set; } = "log";
        public string CsvFilePath { get; set; } = string.Empty;
    }

    /// <summary>
    /// Oracle database connection configuration
    /// </summary>
    public class OracleConnectionSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string Schema { get; set; } = "hsicustom";
        public string TableName { get; set; } = "ccBulk_DUR_QTR_LOAD";
        public int CommandTimeout { get; set; } = 30;
        public int ConnectionTimeout { get; set; } = 15;
    }

    /// <summary>
    /// Bulk loading configuration settings
    /// </summary>
    public class BulkLoadSettings
    {
        public int BatchSize { get; set; } = 1000;
        public int MaxRetries { get; set; } = 3;
        public int RetryDelayMs { get; set; } = 1000;
    }

    /// <summary>
    /// Data model for DUR Quarterly Load record
    /// </summary>
    public class DurQuarterlyLoadRecord
    {
        public string? Column1 { get; set; }
        public string? Column2 { get; set; }
        public string? Column3 { get; set; }
        public string? Column4 { get; set; }
        public string? Column5 { get; set; }
        // Add more columns as needed based on your CSV structure
    }

    /// <summary>
    /// Application execution result
    /// </summary>
    public class ExecutionResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int RecordsProcessed { get; set; }
        public Exception? Exception { get; set; }
    }

    /// <summary>
    /// Connection status information
    /// </summary>
    public class ConnectionStatus
    {
        public bool IsConnected { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime TestedAt { get; set; } = DateTime.UtcNow;
        public Exception? Exception { get; set; }
    }
}