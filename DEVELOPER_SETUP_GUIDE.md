# Developer Setup Guide - Gainwell-OnBase-Wi-Custom

## 📋 Table of Contents
1. [Prerequisites](#prerequisites)
2. [Initial Setup](#initial-setup)
3. [Workspace Configuration](#workspace-configuration)
4. [Creating a New Command-Line App](#creating-a-new-command-line-app)
5. [Debugging and Testing](#debugging-and-testing)
6. [Step-by-Step Code Execution](#step-by-step-code-execution)
7. [Troubleshooting](#troubleshooting)
8. [Best Practices](#best-practices)

---

## 🔧 Prerequisites

### Software Requirements
Before starting, ensure you have the following installed:

1. **Git for Windows**
   - Download: https://git-scm.com/download/win
   - Verify installation: `git --version`

2. **.NET SDK (6.0, 8.0, or 9.0)**
   - Download: https://dotnet.microsoft.com/download
   - Verify installation: `dotnet --version`

3. **Visual Studio Code**
   - Download: https://code.visualstudio.com/
   - Required Extensions (will be auto-suggested):
     - C# for Visual Studio Code (`ms-dotnettools.csharp`)
     - C# Dev Kit (`ms-dotnettools.csdevkit`)
     - IntelliCode for C# Dev Kit (`ms-dotnettools.vscodeintellicode-csharp`)

4. **Oracle Client (Optional - for database connectivity)**
   - Oracle Instant Client or Oracle Database

### System Requirements
- Windows 10/11
- Minimum 8GB RAM
- 2GB free disk space

---

## 🚀 Initial Setup

### Step 1: Clone the Repository

1. **Create a source directory** (recommended location):
   ```powershell
   mkdir C:\source
   cd C:\source
   ```

2. **Clone the repository**:
   ```powershell
   git clone https://github.com/mahermere-GWT/Gainwell-OnBase-WI.git
   cd Gainwell-OnBase-WI
   ```

3. **Verify the structure**:
   ```powershell
   ls
   ```
   You should see:
   ```
   📁 .github/
   📁 .vscode/
   📁 demo/
   📄 .gitignore
   📄 Gainwell-OnBase-Wi-Custom.code-workspace
   📄 Gainwell-OnBase-Wi-Custom.sln
   📄 README.md
   ```

### Step 2: Open in Visual Studio Code

1. **Open the workspace**:
   ```powershell
   code Gainwell-OnBase-Wi-Custom.code-workspace
   ```

2. **Install recommended extensions** (VS Code will prompt):
   - Click "Install All" when the extension recommendation notification appears
   - Or manually install:
     - `Ctrl+Shift+X` → Search for "C#" → Install Microsoft C# extensions

3. **Restore packages**:
   ```powershell
   dotnet restore
   ```

4. **Build the solution**:
   ```powershell
   dotnet build
   ```

---

## ⚙️ Workspace Configuration

### Understanding the Workspace Structure

```
Gainwell-OnBase-Wi-Custom/
├── 📁 .github/
│   └── copilot-instructions.md     # Development guidelines
├── 📁 .vscode/
│   ├── launch.json                 # Debug configurations
│   └── tasks.json                  # Build and run tasks
├── 📁 demo/                        # Example console application
│   ├── 📁 Models/                  # Data models
│   ├── 📁 Services/                # Business services
│   ├── 📁 data/                    # Sample data files
│   ├── appsettings.json            # Configuration
│   ├── demo.csproj                 # Project file
│   └── Program.cs                  # Main entry point
├── Gainwell-OnBase-Wi-Custom.sln   # Solution file
└── README.md                       # Documentation
```

### Key Configuration Files

**1. `.vscode/launch.json`** - Debug Configuration:
- Defines how VS Code launches and debugs your applications
- Pre-configured for .NET console applications

**2. `.vscode/tasks.json`** - Build Tasks:
- `build-demo`: Compiles the demo application
- `Build and Run Demo`: Builds and executes the demo

**3. `appsettings.json`** - Application Configuration:
- Database connection strings
- Logging settings
- Application-specific settings

---

## 🆕 Creating a New Command-Line App

### Method 1: Using VS Code Terminal

1. **Open integrated terminal** (`Ctrl+`` ` or View → Terminal)

2. **Create new console application**:
   ```powershell
   dotnet new console -n MyNewApp
   ```

3. **Add to solution**:
   ```powershell
   dotnet sln add MyNewApp/MyNewApp.csproj
   ```

4. **Update project file** for multi-framework support:
   ```xml
   <Project Sdk="Microsoft.NET.Sdk">
     <PropertyGroup>
       <OutputType>Exe</OutputType>
       <TargetFrameworks>net6.0;net8.0;net9.0</TargetFrameworks>
       <ImplicitUsings>enable</ImplicitUsings>
       <Nullable>enable</Nullable>
     </PropertyGroup>
   </Project>
   ```

### Method 2: Copy and Modify Existing Project

1. **Copy the demo folder**:
   ```powershell
   cp -r demo MyNewApp
   ```

2. **Rename project file**:
   ```powershell
   mv MyNewApp/demo.csproj MyNewApp/MyNewApp.csproj
   ```

3. **Update namespaces** in all `.cs` files:
   - Change `namespace Demo` to `namespace MyNewApp`

4. **Add to solution**:
   ```powershell
   dotnet sln add MyNewApp/MyNewApp.csproj
   ```

### Adding Common Dependencies

Add these packages for enterprise applications:

```powershell
cd MyNewApp
dotnet add package Microsoft.Extensions.Configuration
dotnet add package Microsoft.Extensions.Configuration.Json
dotnet add package Microsoft.Extensions.DependencyInjection
dotnet add package Microsoft.Extensions.Hosting
dotnet add package Microsoft.Extensions.Logging
dotnet add package Serilog.Extensions.Hosting
dotnet add package Serilog.Sinks.Console
dotnet add package Serilog.Sinks.File
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