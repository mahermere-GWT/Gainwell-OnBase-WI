# Gainwell-OnBase-Wi-Custom Development Workspace

## Overview
This workspace is designed for multi-developer C# .NET development with Oracle database connectivity, comprehensive logging, and command-line application support. The workspace provides a foundation for building enterprise applications with proper dependency injection, configuration management, and error handling.

## Projects

### Demo Application
The `demo` project demonstrates the following workflow:
1. **Variable Initialization** - Sets up all application variables with default values
2. **Logging Setup** - Configures file and console logging to the `log` directory
3. **Oracle Connection Testing** - Tests Oracle database connectivity and logs results
4. **Shared Connection Management** - Establishes and manages shared Oracle connections
5. **CSV Processing & Bulk Loading** - Reads CSV files and bulk loads data into Oracle table `hsicustom.ccBulk_DUR_QTR_LOAD`

## Prerequisites

### Software Requirements
- .NET SDK 6.0, 8.0, or 9.0
- Visual Studio Code
- Git

### Database Requirements
- Oracle Database (11g or higher)
- Oracle schema: `hsicustom`
- Target table: `ccBulk_DUR_QTR_LOAD`

## Getting Started

### 1. Clone the Repository
```bash
git clone https://github.com/mahermere-GWT/Gainwell-OnBase-WI.git
cd Gainwell-OnBase-WI
```

### 2. Open in VS Code
```bash
code Gainwell-OnBase-Wi-Custom.code-workspace
```

### 3. Install Dependencies
```bash
dotnet restore
```

### 4. Configure Settings
Edit `demo/appsettings.json` to configure:
- Oracle connection string
- CSV file path
- Logging settings
- Bulk load parameters

### 5. Build the Application
```bash
dotnet build
```

### 6. Run the Demo
```bash
cd demo
dotnet run --framework net8.0
```

## Configuration

### Oracle Connection Settings
```json
{
  "OracleConnection": {
    "ConnectionString": "Data Source=localhost:1521/XE;User Id=hsicustom;Password=your_password;",
    "Schema": "hsicustom",
    "TableName": "ccBulk_DUR_QTR_LOAD",
    "CommandTimeout": 30,
    "ConnectionTimeout": 15
  }
}
```

### Application Settings
```json
{
  "ApplicationSettings": {
    "ApplicationName": "Gainwell OnBase Wi Custom Demo",
    "Version": "1.0.0",
    "LogDirectory": "log",
    "CsvFilePath": "data/sample.csv"
  }
}
```

## Development Standards

### Framework Support
- Multi-targeting: .NET 6.0, 8.0, 9.0
- Dependency injection using Microsoft.Extensions.*
- Configuration management with appsettings.json
- Comprehensive logging to files and console

### Code Organization
```
demo/
├── Models/              # Data models and configuration classes
├── Services/            # Business logic and service implementations
├── data/               # Sample data files
├── log/                # Application log files (created at runtime)
├── appsettings.json    # Application configuration
└── Program.cs          # Application entry point
```

### Logging
- Console output for real-time feedback
- File logging with daily rotation
- Structured logging with different levels (Information, Warning, Error)
- Exception logging with full stack traces

### Error Handling
- Graceful degradation when Oracle connection fails
- Comprehensive exception logging
- Transactional operations with rollback capability
- Retry logic for bulk operations

## Features

### Oracle Database Integration
- Connection testing and validation
- Shared connection management
- Bulk data loading with transactions
- Configurable timeouts and retry policies

### CSV Processing
- Flexible CSV parsing with CsvHelper
- Batch processing for large files
- Error handling for malformed data
- Progress reporting

### Logging & Monitoring
- File-based logging with rotation
- Console output for immediate feedback
- Structured logging for debugging
- Performance metrics and timing

## Testing
- Sample CSV file included in `demo/data/sample.csv`
- Configurable test data structure
- Mock-friendly service interfaces
- Comprehensive error simulation

## Deployment
- Self-contained executable option
- Configuration-based deployment
- Log directory auto-creation
- Minimal external dependencies

## Contributing

### Development Environment
1. Install recommended VS Code extensions
2. Configure Oracle database connection
3. Run `dotnet restore` to install packages
4. Use `dotnet build` to verify setup

### Code Standards
- Follow Microsoft C# coding conventions
- Use dependency injection for all services
- Implement comprehensive logging
- Include error handling in all operations
- Write unit tests for business logic

### Git Workflow
- Create feature branches from main
- Submit pull requests for review
- Include tests with new features
- Update documentation as needed

## Support
For issues or questions, please create an issue in the GitHub repository or contact the development team.

## License
This project is proprietary to Gainwell Technologies.