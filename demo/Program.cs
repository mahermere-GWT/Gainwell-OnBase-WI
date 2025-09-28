using Demo.Models;
using Demo.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Demo
{
    internal class Program
    {
        static async Task<int> Main(string[] args)
        {
            try
            {
                Console.WriteLine("=================================");
                Console.WriteLine("Gainwell OnBase Wi Custom Demo");
                Console.WriteLine("=================================");

                // Step 1: Initialize all variables and set default values
                Console.WriteLine("\n1. Initializing application variables...");
                var startTime = DateTime.UtcNow;
                var executionResults = new List<ExecutionResult>();

                // Build configuration
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production"}.json", optional: true)
                    .AddCommandLine(args)
                    .Build();

                // Get application settings
                var appSettings = new ApplicationSettings();
                configuration.GetSection("ApplicationSettings").Bind(appSettings);

                Console.WriteLine($"Application: {appSettings.ApplicationName} v{appSettings.Version}");
                Console.WriteLine($"Started at: {startTime:yyyy-MM-dd HH:mm:ss} UTC");

                // Step 2: Begin logging to file
                Console.WriteLine("\n2. Setting up logging system...");
                
                // Ensure log directory exists
                var logDirectory = Path.Combine(Directory.GetCurrentDirectory(), appSettings.LogDirectory);
                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                    Console.WriteLine($"Created log directory: {logDirectory}");
                
                // Configure Serilog
                var logFilePath = Path.Combine(logDirectory, $"demo-{DateTime.Now:yyyyMMdd}.log");
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Information()
                    .WriteTo.Console()
                    .WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day)
                    .CreateLogger();
                }

                // Build host with dependency injection and logging
                var host = Host.CreateDefaultBuilder(args)
                    .ConfigureServices((context, services) =>
                    {
                        // Configure settings
                        services.Configure<ApplicationSettings>(configuration.GetSection("ApplicationSettings"));
                        services.Configure<OracleConnectionSettings>(configuration.GetSection("OracleConnection"));
                        services.Configure<BulkLoadSettings>(configuration.GetSection("BulkLoadSettings"));
                        
                        // Register services
                        services.AddSingleton<IOracleConnectionService, OracleConnectionService>();
                        services.AddTransient<ICsvBulkLoadService, CsvBulkLoadService>();
                        services.AddTransient<DemoApplication>();
                    })
                    .ConfigureLogging(logging =>
                    {
                        logging.ClearProviders();
                        logging.AddSerilog();
                        logging.SetMinimumLevel(LogLevel.Information);
                    })
                    .UseConsoleLifetime()
                    .Build();

                Console.WriteLine($"Logging configured. Log file location: {logDirectory}");

                // Run the application
                var app = host.Services.GetRequiredService<DemoApplication>();
                var exitCode = await app.RunAsync();

                Console.WriteLine($"\n=================================");
                Console.WriteLine($"Application completed with exit code: {exitCode}");
                Console.WriteLine($"Total execution time: {DateTime.UtcNow.Subtract(startTime).TotalSeconds:F2} seconds");
                Console.WriteLine($"=================================");

                // Close and flush the log
                Log.CloseAndFlush();

                return exitCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nFATAL ERROR: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return 1;
            }
        }
    }

    /// <summary>
    /// Main application class that orchestrates the demo workflow
    /// </summary>
    public class DemoApplication
    {
        private readonly IOracleConnectionService _oracleService;
        private readonly ICsvBulkLoadService _csvService;
        private readonly ApplicationSettings _appSettings;
        private readonly ILogger<DemoApplication> _logger;

        public DemoApplication(
            IOracleConnectionService oracleService,
            ICsvBulkLoadService csvService,
            Microsoft.Extensions.Options.IOptions<ApplicationSettings> appSettings,
            ILogger<DemoApplication> logger)
        {
            _oracleService = oracleService ?? throw new ArgumentNullException(nameof(oracleService));
            _csvService = csvService ?? throw new ArgumentNullException(nameof(csvService));
            _appSettings = appSettings?.Value ?? throw new ArgumentNullException(nameof(appSettings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<int> RunAsync()
        {
            try
            {
                _logger.LogInformation("Demo application starting execution");

                // Step 3: Find Oracle configuration and test connection
                Console.WriteLine("\n3. Testing Oracle database connection...");
                _logger.LogInformation("Testing Oracle database connection");

                var connectionStatus = await _oracleService.TestConnectionAsync();
                
                if (connectionStatus.IsConnected)
                {
                    Console.WriteLine("✓ Oracle connection successful!");
                    _logger.LogInformation("Oracle connection test successful");

                    // Step 4: Add connection to shared memory location
                    Console.WriteLine("\n4. Establishing shared Oracle connection...");
                    var sharedConnection = await _oracleService.GetConnectionAsync();
                    if (sharedConnection != null)
                    {
                        _oracleService.SetSharedConnection(sharedConnection);
                        Console.WriteLine("✓ Shared Oracle connection established");
                        _logger.LogInformation("Shared Oracle connection established for application use");
                    }
                    else
                    {
                        Console.WriteLine("✗ Failed to establish shared connection");
                        _logger.LogError("Failed to establish shared Oracle connection");
                    }
                }
                else
                {
                    Console.WriteLine($"✗ Oracle connection failed: {connectionStatus.Message}");
                    _logger.LogError("Oracle connection test failed: {Message}", connectionStatus.Message);
                    if (connectionStatus.Exception != null)
                    {
                        _logger.LogError(connectionStatus.Exception, "Oracle connection exception details");
                    }
                }

                // Step 5: Read CSV file and bulk load data
                Console.WriteLine("\n5. Processing CSV file and bulk loading data...");
                _logger.LogInformation("Starting CSV file processing and bulk load operation");

                var csvFilePath = Path.Combine(Directory.GetCurrentDirectory(), _appSettings.CsvFilePath);
                Console.WriteLine($"CSV file path: {csvFilePath}");

                if (!connectionStatus.IsConnected)
                {
                    Console.WriteLine("⚠ Skipping CSV processing - Oracle connection not available");
                    _logger.LogWarning("Skipping CSV processing due to Oracle connection failure");
                    return 1;
                }

                var processingResult = await _csvService.ProcessCsvFileAsync(csvFilePath);
                
                if (processingResult.Success)
                {
                    Console.WriteLine($"✓ CSV processing completed successfully!");
                    Console.WriteLine($"  Records processed: {processingResult.RecordsProcessed}");
                    _logger.LogInformation("CSV processing completed successfully. Records processed: {RecordsProcessed}", 
                        processingResult.RecordsProcessed);
                }
                else
                {
                    Console.WriteLine($"✗ CSV processing failed: {processingResult.Message}");
                    _logger.LogError("CSV processing failed: {Message}", processingResult.Message);
                    if (processingResult.Exception != null)
                    {
                        _logger.LogError(processingResult.Exception, "CSV processing exception details");
                    }
                    return 1;
                }

                _logger.LogInformation("Demo application completed successfully");
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Application error: {ex.Message}");
                _logger.LogError(ex, "Demo application failed with exception: {Message}", ex.Message);
                return 1;
            }
        }
    }
}
