# Developer Setup Guide - Gainwell-OnBase-WI

Welcome to the Gainwell OnBase Wisconsin Custom Development workspace! This guide will walk you through setting up your development environment, understanding the workspace, and becoming productive with debugging and development.

## 📋 Table of Contents
1. [Repository Download & Setup](#-repository-download--setup)
2. [Visual Studio Code Setup](#-visual-studio-code-setup)  
3. [Workspace Understanding](#-workspace-understanding)
4. [Creating a New Application](#-creating-a-new-application)
5. [Making Changes & Running Debugger](#-making-changes--running-debugger)
6. [Advanced Debugging Tutorial](#-advanced-debugging-tutorial)
7. [Troubleshooting](#-troubleshooting)
8. [Best Practices](#-best-practices)

---

## 🔽 Repository Download & Setup

### Prerequisites
Before starting, ensure you have these installed:

1. **Git for Windows**
   ```powershell
   # Verify installation
   git --version
   ```
   Download: https://git-scm.com/download/win

2. **.NET SDK (6.0, 8.0, and 9.0)**
   ```powershell
   # Verify installation
   dotnet --version
   dotnet --list-sdks
   ```
   Download: https://dotnet.microsoft.com/download
   > **Note**: You need all three versions for full compatibility testing

3. **Visual Studio Code**
   Download: https://code.visualstudio.com/

### Step 1: Clone the Repository

1. **Choose your development location** (recommended):
   ```powershell
   # Create and navigate to your source directory
   mkdir C:\source
   cd C:\source
   ```

2. **Clone the repository**:
   ```powershell
   git clone https://github.com/mahermere-GWT/Gainwell-OnBase-WI.git
   cd Gainwell-OnBase-WI
   ```

3. **Verify repository structure**:
   ```powershell
   ls
   ```
   You should see:
   ```
   📁 .github/               # GitHub configuration & guidelines
   📁 .vscode/               # VS Code debugging & build configurations  
   📁 bulkDURLoader/         # Primary bulk data loader application
   📁 demo/                  # Demo application with Oracle & CSV features
   📁 FileConventionValidator/ # File naming convention validator
   📁 FileProcessor/         # File processing utilities
   📁 SampleApp/            # Sample console application
   📄 .gitignore
   📄 Gainwell-OnBase-WI.code-workspace  # Main workspace file
   📄 README.md
   ```

---

## 💻 Visual Studio Code Setup

### Step 1: Open the Workspace

**Always use the workspace file** (not just the folder):
```powershell
code Gainwell-OnBase-WI.code-workspace
```

> **Important**: Opening the `.code-workspace` file gives you access to all 24 debug configurations and optimized settings.

### Step 2: Install Required Extensions

VS Code will automatically prompt you to install recommended extensions:

1. **Click "Install All"** when the notification appears, or
2. **Manual installation**: `Ctrl+Shift+X` → Search and install:
   - **C# for Visual Studio Code** (`ms-dotnettools.csharp`)
   - **C# Dev Kit** (`ms-dotnettools.csdevkit`)
   - **.NET Install Tool** (`ms-dotnettools.vscode-dotnet-runtime`)

### Step 3: Restore and Build

1. **Restore NuGet packages**:
   ```powershell
   dotnet restore
   ```

2. **Build all projects**:
   ```powershell
   dotnet build
   ```

3. **Verify all frameworks build successfully**:
   ```powershell
   # Test each major project
   cd demo && dotnet build
   cd ../FileProcessor && dotnet build  
   cd ../FileConventionValidator && dotnet build
   cd ../bulkDURLoader && dotnet build
   ```

### Step 4: Verify VS Code Configuration

1. **Open Command Palette** (`Ctrl+Shift+P`)
2. **Type**: "C#: Restart OmniSharp" and run it
3. **Check debug configurations**: `Ctrl+Shift+D` - you should see 24+ configurations
4. **Verify IntelliSense**: Open any `.cs` file and confirm code completion works

---

## 🏗️ Workspace Understanding

### Workspace Architecture

```
Gainwell-OnBase-WI/
├── 📁 .vscode/
│   ├── launch.json        # 24 debug configurations (6 per major app)
│   └── tasks.json         # Build tasks for all .NET frameworks
│
├── 📁 bulkDURLoader/      # Primary Application
│   ├── Models/            # Database and business models
│   ├── Services/          # Core business logic
│   ├── bulkDURLoader.csproj # Multi-framework project (.NET 6/8/9)
│   └── Program.cs         # Entry point
│
├── 📁 demo/               # Demo Application (Full Enterprise Pattern)  
│   ├── Models/            # ApplicationSettings, ExecutionResult models
│   ├── Services/          # Oracle, CSV, logging services
│   ├── appsettings.json   # Configuration for all environments
│   ├── demo.csproj        # Multi-framework targeting
│   └── Program.cs         # Dependency injection & hosting pattern
│
├── 📁 FileProcessor/      # File Processing Utility
│   ├── Models/            # File processing models
│   ├── Services/          # File processing business logic
│   ├── TestData/          # Sample input/output files
│   └── Program.cs         # Command-line processing app
│
├── 📁 FileConventionValidator/  # File Naming Validator
│   ├── Services/          # Validation logic
│   ├── TestFiles/         # Test scenarios for validation
│   └── Program.cs         # File convention validation
│
└── 📁 SampleApp/          # Basic Console App Template
    └── Program.cs         # Simple logging example
```

### Debug Configurations Available

**Your workspace provides 24 debug configurations**:

#### **Demo App (6 configs)**:
- Demo App .NET 6.0 (console/internal)
- Demo App .NET 8.0 (console/internal)  
- Demo App .NET 9.0 (console/internal)

#### **FileProcessor (6 configs)**:
- FileProcessor .NET 6.0 (console/internal)
- FileProcessor .NET 8.0 (console/internal)
- FileProcessor .NET 9.0 (console/internal)

#### **FileConventionValidator (6 configs)**:
- FileConventionValidator .NET 6.0 (console/internal)
- FileConventionValidator .NET 8.0 (console/internal)
- FileConventionValidator .NET 9.0 (console/internal)

#### **Plus**: bulkDURLoader (4 configs) and SampleApp (2 configs)

---

## 🆕 Creating a New Application

### Method 1: Copy from Demo (Recommended for Enterprise Apps)

The `demo` project is a full enterprise template with Oracle connectivity, CSV processing, logging, and dependency injection.

1. **Copy the demo folder**:
   ```powershell
   cp -r demo MyNewApp
   cd MyNewApp
   ```

2. **Rename project file**:
   ```powershell
   mv demo.csproj MyNewApp.csproj
   ```

3. **Update all namespace references**:
   ```powershell
   # PowerShell: Replace namespace in all C# files
   Get-ChildItem *.cs -Recurse | ForEach-Object {
       (Get-Content $_.FullName) | ForEach-Object { 
           $_ -replace 'namespace Demo', 'namespace MyNewApp' 
       } | Set-Content $_.FullName
   }
   ```

4. **Update using statements**:
   ```powershell
   # Replace using statements
   Get-ChildItem *.cs -Recurse | ForEach-Object {
       (Get-Content $_.FullName) | ForEach-Object { 
           $_ -replace 'using Demo', 'using MyNewApp' 
       } | Set-Content $_.FullName
   }
   ```

5. **Update configuration files**:
   ```json
   // appsettings.json - Update application name
   {
     "ApplicationSettings": {
       "ApplicationName": "MyNewApp",
       "Version": "1.0.0",
       // ... rest of config
     }
   }
   ```

### Method 2: Copy from FileProcessor (For File Processing Apps)

1. **Copy and rename**:
   ```powershell
   cp -r FileProcessor MyFileApp
   cd MyFileApp
   mv FileProcessor.csproj MyFileApp.csproj
   ```

2. **Update namespaces** (same PowerShell commands as above, replacing `FileProcessor` with `MyFileApp`)

### Method 3: Create from Scratch (Simple Apps)

1. **Create new console app**:
   ```powershell
   dotnet new console -n MySimpleApp
   cd MySimpleApp
   ```

2. **Update for multi-framework support**:
   ```xml
   <!-- MySimpleApp.csproj -->
   <Project Sdk="Microsoft.NET.Sdk">
     <PropertyGroup>
       <OutputType>Exe</OutputType>
       <TargetFrameworks>net6.0;net8.0;net9.0</TargetFrameworks>
       <ImplicitUsings>enable</ImplicitUsings>
       <Nullable>enable</Nullable>
     </PropertyGroup>
     
     <!-- Add common packages -->
     <ItemGroup>
       <PackageReference Include="Microsoft.Extensions.Hosting" Version="[6.0.0,10.0.0)" />
       <PackageReference Include="Microsoft.Extensions.Logging.Console" Version="[6.0.0,10.0.0)" />
       <PackageReference Include="Serilog.Extensions.Hosting" Version="8.0.0" />
       <PackageReference Include="Serilog.Sinks.Console" Version="6.0.0" />
       <PackageReference Include="Serilog.Sinks.File" Version="6.0.0" />
     </ItemGroup>
   </Project>
   ```

### Step 4: Add Debug Configurations

1. **Open `.vscode/launch.json`**
2. **Add debug configurations** for your new app:
   ```json
   {
       "name": "MyNewApp .NET 8.0 (console)",
       "type": "coreclr", 
       "request": "launch",
       "preLaunchTask": "build-mynewapp-net8",
       "program": "${workspaceFolder}/MyNewApp/bin/Debug/net8.0/MyNewApp.dll",
       "args": [],
       "cwd": "${workspaceFolder}/MyNewApp", 
       "console": "externalTerminal",
       "stopAtEntry": false
   }
   ```

3. **Add build task** in `.vscode/tasks.json`:
   ```json
   {
       "label": "build-mynewapp-net8",
       "type": "shell",
       "command": "dotnet",
       "args": ["build", "--framework", "net8.0", "--verbosity", "quiet"],
       "options": {
           "cwd": "${workspaceFolder}/MyNewApp"
       },
       "group": "build",
       "presentation": {
           "reveal": "silent"
       },
       "problemMatcher": "$msCompile"
   }
   ```

4. **Build and test**:
   ```powershell
   dotnet build
   ```

---

## 🔧 Making Changes & Running Debugger

### Scenario: Adding a New Feature to Demo App

Let's walk through adding a new feature to the demo application.

#### Step 1: Make Code Changes

1. **Open `demo/Program.cs`**
2. **Add a new step** after the existing 5 steps:

```csharp
// Add this after Step 5 in the DemoApplication.RunAsync() method
// Step 6: Process additional data
Console.WriteLine("\n6. Processing additional business logic...");
_logger.LogInformation("Starting additional business logic processing");

var additionalResult = await ProcessAdditionalDataAsync();
if (additionalResult.Success)
{
    Console.WriteLine("✓ Additional processing completed successfully!");
    _logger.LogInformation("Additional processing completed successfully");
}
else
{
    Console.WriteLine($"✗ Additional processing failed: {additionalResult.Message}");
    _logger.LogError("Additional processing failed: {Message}", additionalResult.Message);
}
```

3. **Add the new method** to the `DemoApplication` class:

```csharp
private async Task<(bool Success, string Message)> ProcessAdditionalDataAsync()
{
    try
    {
        // Simulate some business logic
        await Task.Delay(1000);
        
        var random = new Random();
        var success = random.NextDouble() > 0.2; // 80% success rate
        
        if (success)
        {
            return (true, "Additional data processed successfully");
        }
        else
        {
            return (false, "Simulated processing failure");
        }
    }
    catch (Exception ex)
    {
        return (false, ex.Message);
    }
}
```

#### Step 2: Build and Test Changes

1. **Build the project**:
   ```powershell
   cd demo
   dotnet build
   ```

2. **Run without debugger** to test:
   ```powershell
   dotnet run --framework net8.0
   ```

#### Step 3: Debug Your Changes

1. **Set breakpoints**:
   - Click in the gutter next to line numbers in your new `ProcessAdditionalDataAsync` method
   - Set breakpoint on the `var success = random.NextDouble() > 0.2;` line

2. **Start debugging**:
   - Press `Ctrl+Shift+D` to open Debug panel
   - Select **"Demo App .NET 8.0 (internal console)"**
   - Press `F5` or click the green play button

3. **Step through execution**:
   - When it hits your breakpoint, examine variables
   - Use `F10` to step over lines
   - Use `F11` to step into methods
   - Check the Debug Console for variable values

---

## 🎯 Advanced Debugging Tutorial

### Tutorial: Deep Debugging Session

Let's debug the Oracle connection logic in the demo app to understand how it works.

#### Step 1: Set Strategic Breakpoints

1. **Open `demo/Services/OracleConnectionService.cs`**
2. **Set breakpoints** on these key lines:
   - `TestConnectionAsync()` method entry
   - Inside the try block where connection opens
   - The return statement

3. **Open `demo/Program.cs`** 
4. **Set breakpoint** where Oracle connection is tested

#### Step 2: Start Advanced Debug Session

1. **Launch debugger**: Select **"Demo App .NET 8.0 (console)"** and press `F5`
2. **When first breakpoint hits**: Examine the call stack

#### Step 3: Use Debug Features

**Variables Panel**:
- Expand objects to see all properties
- Right-click variables → "Add to Watch"

**Watch Panel**:
- Add expressions like `connectionString.Length`
- Add `DateTime.Now.ToString()` to track timing

**Debug Console**:
```csharp
// Type these in Debug Console while debugging:
connectionString
connectionString.Contains("localhost")
System.Environment.MachineName
```

**Call Stack**:
- Click different stack frames to navigate
- See how methods are called in sequence

#### Step 4: Modify Values During Debug

1. **In Variables panel**, double-click a string value
2. **Change it** and continue execution
3. **See how** the change affects program flow

#### Step 5: Exception Handling

1. **Force an exception**:
   - In Debug Console: `throw new Exception("Test exception")`
   - Or modify connection string to cause failure

2. **Observe** how VS Code handles the exception:
   - Exception details in Debug Console
   - Call stack at time of exception
   - Variable states when exception occurred

### Advanced Debugging Scenarios

#### Debugging Different .NET Frameworks

Test your app across frameworks:

1. **Debug with .NET 6.0**: "Demo App .NET 6.0 (console)"
2. **Debug with .NET 8.0**: "Demo App .NET 8.0 (console)"  
3. **Debug with .NET 9.0**: "Demo App .NET 9.0 (console)"

**Compare behavior**:
- Performance differences
- API availability
- Framework-specific features

#### Debugging with Command Line Arguments

1. **Modify launch configuration** to pass arguments:
   ```json
   {
       "name": "Demo App with Args",
       "type": "coreclr",
       "request": "launch", 
       "preLaunchTask": "build-demo",
       "program": "${workspaceFolder}/demo/bin/Debug/net8.0/demo.dll",
       "args": ["--environment", "Development", "--verbose"],
       "cwd": "${workspaceFolder}/demo",
       "console": "externalTerminal",
       "stopAtEntry": false
   }
   ```

2. **Debug and examine** `args` parameter in `Main()` method

#### Debugging File Processing Apps

1. **Create test input** for FileProcessor:
   ```powershell
   cd FileProcessor/TestData
   echo "Test line 1" > test-input.txt
   echo "Test line 2" >> test-input.txt
   ```

2. **Modify launch.json** to pass file arguments:
   ```json
   {
       "name": "FileProcessor with Test Files",
       "type": "coreclr",
       "request": "launch",
       "preLaunchTask": "build-fileprocessor-net8", 
       "program": "${workspaceFolder}/FileProcessor/bin/Debug/net8.0/FileProcessor.dll",
       "args": ["TestData/test-input.txt", "TestData/output.txt"],
       "cwd": "${workspaceFolder}/FileProcessor",
       "console": "externalTerminal",
       "stopAtEntry": false
   }
   ```

### Debugging Best Practices

#### 1. Use Conditional Breakpoints
```csharp
// Set breakpoint, right-click → "Add Conditional Breakpoint"
// Condition: i > 10
// Or: connectionStatus.IsConnected == false
```

#### 2. Use Logpoints for High-Frequency Code
```csharp
// Right-click → "Add Logpoint" 
// Message: Processing record {recordId} at {DateTime.Now}
```

#### 3. Debug Output Windows
- **Output Panel**: Build errors and warnings
- **Problems Panel**: Code issues and suggestions  
- **Terminal**: Command execution results
- **Debug Console**: Interactive debugging

---

## 🛠️ Troubleshooting

### Common Setup Issues

#### **Issue**: "No debug configurations available"
**Solution**:
1. Ensure you opened the `.code-workspace` file, not just the folder
2. Check that `.vscode/launch.json` exists and has configurations
3. Restart VS Code: `Ctrl+Shift+P` → "Developer: Reload Window"

#### **Issue**: "Cannot find project DLL"
**Solution**:
1. Check the `program` path in launch.json points to correct DLL
2. Ensure project is built: `dotnet build`
3. Verify framework version matches: net6.0, net8.0, or net9.0

#### **Issue**: "Breakpoints not being hit"
**Solution**:
1. Ensure you're in Debug configuration (not Release)
2. Check that you have the correct launch configuration selected
3. Verify breakpoint is on executable code (not comments/blank lines)
4. Try "Clean" then "Rebuild": `dotnet clean && dotnet build`

#### **Issue**: "appsettings.json not found"
**Solution**:
1. Check `cwd` setting in launch.json points to correct directory
2. Ensure appsettings.json exists in project root
3. Verify file is set to "Copy to Output Directory"

### Oracle Connection Issues

#### **Issue**: Oracle connection fails
**Solution**:
1. Check connection string in `appsettings.json`
2. Ensure Oracle client is installed
3. Test connectivity outside the application first
4. Check firewall and network access

### Performance Issues

#### **Issue**: Slow debugging or IntelliSense
**Solution**:
1. Close unnecessary files and tabs
2. Disable unused extensions
3. Increase VS Code memory: `--max-memory=4096`
4. Check if antivirus is scanning build outputs

---

## 📚 Best Practices

### Project Organization

```
YourNewApp/
├── 📁 Models/              # Data models, DTOs, settings classes
├── 📁 Services/            # Business logic, data access, external APIs
├── 📁 Interfaces/          # Service contracts and abstractions  
├── 📁 Configuration/       # Configuration binding classes
├── 📁 Data/               # Sample data, test files, schemas
├── 📁 Logs/               # Application log files (gitignored)
├── appsettings.json       # Main configuration
├── appsettings.Development.json  # Dev environment overrides
├── Program.cs             # Application entry point
└── YourNewApp.csproj      # Project file with multi-framework support
```

### Code Quality Standards

#### **1. Use Dependency Injection Pattern**:
```csharp
// In Program.cs
services.AddTransient<IMyService, MyService>();
services.AddSingleton<IConfiguration>(configuration);
services.Configure<MySettings>(configuration.GetSection("MySettings"));

// In your service classes
public class MyService : IMyService
{
    private readonly ILogger<MyService> _logger;
    private readonly MySettings _settings;
    
    public MyService(ILogger<MyService> logger, IOptions<MySettings> settings)
    {
        _logger = logger;
        _settings = settings.Value;
    }
}
```

#### **2. Implement Structured Logging**:
```csharp
// Good logging practices
_logger.LogInformation("Processing started for user {UserId} at {StartTime}", userId, DateTime.UtcNow);
_logger.LogError(ex, "Failed to process record {RecordId}: {ErrorMessage}", recordId, ex.Message);
_logger.LogWarning("Retrying operation {Operation} - attempt {Attempt}", operation, attempt);
```

#### **3. Use Configuration Pattern**:
```csharp
// appsettings.json
{
  "MyAppSettings": {
    "ConnectionString": "...",
    "BatchSize": 100,
    "RetryCount": 3
  }
}

// Configuration class
public class MyAppSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public int BatchSize { get; set; } = 100;
    public int RetryCount { get; set; } = 3;
}

// Registration
services.Configure<MyAppSettings>(configuration.GetSection("MyAppSettings"));
```

### Debugging Excellence

#### **1. Strategic Breakpoint Placement**:
- **Entry points**: Start of methods, especially public ones
- **Decision points**: if/switch statements, loop conditions
- **Error handling**: catch blocks, validation logic
- **Exit points**: return statements, dispose operations

#### **2. Effective Variable Watching**:
```csharp
// In Watch panel, add expressions like:
myObject.State.ToString()
myCollection.Count
DateTime.Now.ToString("HH:mm:ss.fff")
myVariable ?? "NULL"
```

#### **3. Debug Configuration Naming**:
```json
// Use descriptive names that indicate purpose
"Demo App .NET 8.0 (console)"           // External terminal  
"Demo App .NET 8.0 (internal console)"  // VS Code integrated
"Demo App .NET 8.0 with Test Data"      // Special test scenario
```

### Multi-Framework Development

#### **Target Framework Strategy**:
- **NET 6.0**: Legacy compatibility, long-term support
- **NET 8.0**: Current stable, recommended for production  
- **NET 9.0**: Latest features, cutting-edge development

#### **Build and Test Across Frameworks**:
```powershell
# Build all frameworks
dotnet build

# Test specific framework  
dotnet run --framework net8.0
dotnet test --framework net8.0

# Publish for different frameworks
dotnet publish -f net8.0 -r win-x64
```

---

## 🔗 Additional Resources

### Microsoft Documentation
- [.NET Documentation](https://docs.microsoft.com/en-us/dotnet/)
- [ASP.NET Core Documentation](https://docs.microsoft.com/en-us/aspnet/core/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)

### VS Code & C# Development  
- [VS Code C# Documentation](https://code.visualstudio.com/docs/languages/csharp)
- [C# Dev Kit Extension](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit)
- [VS Code Debugging](https://code.visualstudio.com/docs/editor/debugging)

### Logging & Configuration
- [Serilog Documentation](https://serilog.net/)
- [Microsoft.Extensions.Hosting](https://docs.microsoft.com/en-us/dotnet/core/extensions/generic-host)
- [Configuration in .NET](https://docs.microsoft.com/en-us/dotnet/core/extensions/configuration)

### Database & Data Access
- [Oracle .NET Documentation](https://docs.oracle.com/en/database/oracle/oracle-data-access-components/19.3/odpnt/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)

---

## 📞 Getting Help

### When You Need Support:

1. **Check this guide first** - Most common scenarios are covered
2. **Search the codebase** - Look for similar implementations in other projects
3. **Check project documentation** - README files and code comments
4. **Debug step-by-step** - Use the debugging techniques in this guide
5. **Create detailed issue** - Include steps to reproduce, error messages, screenshots

### Creating Good Issue Reports:

```markdown
## Issue Description
Brief description of the problem

## Steps to Reproduce  
1. Open project X
2. Run debug configuration Y
3. Set breakpoint at line Z
4. Expected vs actual behavior

## Environment
- OS: Windows 10/11
- .NET Version: 8.0.x  
- VS Code Version: x.x.x
- Extension Versions: C# x.x.x

## Screenshots/Logs
[Attach relevant screenshots or log files]
```

---

## 🎉 You're Ready!

**Congratulations!** You now have:

✅ **Complete development environment** with VS Code and .NET  
✅ **Full workspace understanding** with 24 debug configurations  
✅ **Hands-on experience** creating new applications  
✅ **Advanced debugging skills** across multiple .NET frameworks  
✅ **Best practices** for professional development  

### Next Steps:
1. **Pick a project** from the workspace to explore
2. **Set some breakpoints** and run through the debugging tutorial
3. **Create your first new application** using the copy method
4. **Start building** your features!

**Happy Coding! 🚀**

---

*Last updated: September 2025 - VS Code Multi-Framework Debugging Environment*
```

### Basic Application Template

```csharp
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace MyNewApp
{
    internal class Program
    {
        static async Task<int> Main(string[] args)
        {
            try
            {
                Console.WriteLine("MyNewApp Starting...");

                // Configure Serilog
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Information()
                    .WriteTo.Console()
                    .WriteTo.File("logs/myapp-.log", rollingInterval: RollingInterval.Day)
                    .CreateLogger();

                // Build configuration
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddCommandLine(args)
                    .Build();

                // Build host
                var host = Host.CreateDefaultBuilder(args)
                    .ConfigureServices((context, services) =>
                    {
                        services.AddTransient<MyAppService>();
                    })
                    .ConfigureLogging(logging =>
                    {
                        logging.ClearProviders();
                        logging.AddSerilog();
                    })
                    .Build();

                // Run application
                var app = host.Services.GetRequiredService<MyAppService>();
                var result = await app.RunAsync();

                Log.CloseAndFlush();
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fatal error: {ex.Message}");
                Log.CloseAndFlush();
                return 1;
            }
        }
    }

    public class MyAppService
    {
        private readonly ILogger<MyAppService> _logger;

        public MyAppService(ILogger<MyAppService> logger)
        {
            _logger = logger;
        }

        public async Task<int> RunAsync()
        {
            _logger.LogInformation("MyAppService starting execution");
            
            // Your application logic here
            
            _logger.LogInformation("MyAppService completed successfully");
            return 0;
        }
    }
}
```

---

## 🐛 Debugging and Testing

### Setting Up Debug Configuration

1. **Update `.vscode/launch.json`** to include your new app:
   ```json
   {
       "name": "Launch MyNewApp",
       "type": "coreclr",
       "request": "launch",
       "preLaunchTask": "build-mynewapp",
       "program": "${workspaceFolder}/MyNewApp/bin/Debug/net8.0/MyNewApp.dll",
       "args": [],
       "cwd": "${workspaceFolder}/MyNewApp",
       "console": "integratedTerminal",
       "stopAtEntry": false
   }
   ```

2. **Add build task** in `.vscode/tasks.json`:
   ```json
   {
       "label": "build-mynewapp",
       "type": "shell",
       "command": "dotnet",
       "args": ["build", "--framework", "net8.0"],
       "options": {
           "cwd": "${workspaceFolder}/MyNewApp"
       },
       "group": "build",
       "problemMatcher": "$msCompile"
   }
   ```

### Running and Debugging

**Method 1: Debug Panel**
1. Press `Ctrl+Shift+D` to open Run and Debug
2. Select your application from the dropdown
3. Press `F5` to start debugging

**Method 2: Command Palette**
1. Press `Ctrl+Shift+P`
2. Type "Debug: Select and Start Debugging"
3. Choose your configuration

**Method 3: Terminal**
```powershell
cd MyNewApp
dotnet run --framework net8.0
```

---

## 🔍 Step-by-Step Code Execution

### Setting Breakpoints

1. **Set breakpoints**:
   - Click in the gutter (left of line numbers) to set breakpoints
   - Red dots indicate active breakpoints
   - Or press `F9` on the desired line

2. **Types of breakpoints**:
   - **Line breakpoint**: Stops at specific line
   - **Conditional breakpoint**: Right-click → "Add Conditional Breakpoint"
   - **Logpoint**: Right-click → "Add Logpoint" (logs without stopping)

### Debug Controls

Once debugging starts, use these controls:

| Key | Action | Description |
|-----|--------|-------------|
| `F5` | Continue | Resume execution until next breakpoint |
| `F10` | Step Over | Execute current line, don't enter functions |
| `F11` | Step Into | Enter functions/methods on current line |
| `Shift+F11` | Step Out | Exit current function and return to caller |
| `Ctrl+Shift+F5` | Restart | Restart debugging session |
| `Shift+F5` | Stop | Stop debugging |

### Debug Windows

**1. Variables Panel**:
- View local variables, parameters, and their values
- Expand objects to see properties
- Right-click to "Add to Watch"

**2. Watch Panel**:
- Monitor specific expressions
- Add variables you want to track continuously
- Evaluate complex expressions

**3. Call Stack**:
- Shows the sequence of method calls
- Click to navigate between call frames

**4. Debug Console**:
- Evaluate expressions during debugging
- Type variable names to see values
- Execute methods and property getters

### Advanced Debugging Techniques

**1. Evaluate Expressions**:
```csharp
// In Debug Console, type:
myVariable.PropertyName
myObject.ToString()
DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
```

**2. Modify Values During Debug**:
- In Variables panel, double-click a value to change it
- Or use Debug Console: `myVariable = "new value"`

**3. Exception Handling**:
- VS Code automatically breaks on unhandled exceptions
- Configure exception settings in Debug panel

---

## 🛠️ Troubleshooting

### Common Issues

**1. "Cannot find appsettings.json"**
```
Solution: Ensure your launch.json has correct "cwd" setting:
"cwd": "${workspaceFolder}/YourAppName"
```

**2. "Project file not found"**
```
Solution: Check that the "program" path in launch.json points to correct DLL:
"program": "${workspaceFolder}/YourApp/bin/Debug/net8.0/YourApp.dll"
```

**3. "Dependencies not restored"**
```
Solution: Run in terminal:
dotnet restore
dotnet build
```

**4. "Extension not working"**
```
Solution: 
1. Reload VS Code window (Ctrl+Shift+P → "Developer: Reload Window")
2. Check that C# extension is installed and enabled
3. Restart VS Code
```

### Debug Issues

**1. Breakpoints not hit**:
- Ensure you're running in Debug configuration
- Check that breakpoints are set in reachable code
- Verify the correct launch configuration is selected

**2. Variables showing "optimized away"**:
- Build in Debug mode: `dotnet build -c Debug`
- Ensure `<Optimize>false</Optimize>` in project file

---

## 📚 Best Practices

### Project Structure
```
YourNewApp/
├── 📁 Models/          # Data models and DTOs
├── 📁 Services/        # Business logic services
├── 📁 Interfaces/      # Service contracts
├── 📁 Configuration/   # Configuration classes
├── 📁 Data/           # Sample data, test files
├── appsettings.json   # Configuration file
├── Program.cs         # Application entry point
└── YourNewApp.csproj  # Project file
```

### Code Organization

1. **Use dependency injection**:
   ```csharp
   services.AddTransient<IMyService, MyService>();
   services.AddSingleton<IConfiguration>(configuration);
   ```

2. **Implement proper logging**:
   ```csharp
   _logger.LogInformation("Operation started");
   _logger.LogError(ex, "Operation failed: {Message}", ex.Message);
   ```

3. **Use configuration pattern**:
   ```csharp
   services.Configure<MySettings>(configuration.GetSection("MySettings"));
   ```

### Debugging Best Practices

1. **Use meaningful breakpoints**:
   - Set breakpoints at decision points
   - Use conditional breakpoints for loops
   - Set logpoints for high-frequency operations

2. **Leverage debug features**:
   - Use "Pin to Source" for frequently watched variables
   - Create watch expressions for complex calculations
   - Use "Run to Cursor" (Ctrl+F10) for quick navigation

3. **Test incrementally**:
   - Build and test small features individually
   - Use unit tests for isolated testing
   - Debug with realistic test data

---

## 🔗 Additional Resources

- [.NET Documentation](https://docs.microsoft.com/en-us/dotnet/)
- [VS Code C# Documentation](https://code.visualstudio.com/docs/languages/csharp)
- [Serilog Documentation](https://serilog.net/)
- [Microsoft.Extensions.Hosting](https://docs.microsoft.com/en-us/dotnet/core/extensions/generic-host)

---

## 📞 Support

If you encounter issues:

1. Check this troubleshooting guide first
2. Search the project's issue tracker on GitHub
3. Contact the development team
4. Create a detailed issue with steps to reproduce

---

**Happy Coding! 🚀**