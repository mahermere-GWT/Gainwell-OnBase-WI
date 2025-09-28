# 🚀 Quick Reference - Debugging Cheat Sheet

## Essential Keyboard Shortcuts

| Action | Shortcut | Description |
|--------|----------|-------------|
| Start Debugging | `F5` | Launch with debugger attached |
| Start Without Debug | `Ctrl+F5` | Run without debugger |
| Toggle Breakpoint | `F9` | Add/remove breakpoint on current line |
| Step Over | `F10` | Execute current line without entering functions |
| Step Into | `F11` | Enter function calls on current line |
| Step Out | `Shift+F11` | Exit current function |
| Continue | `F5` | Resume execution to next breakpoint |
| Stop Debugging | `Shift+F5` | Terminate debugging session |
| Restart | `Ctrl+Shift+F5` | Restart current debug session |

## Debug Panel Shortcuts

| Panel | Shortcut | Purpose |
|-------|----------|---------|
| Run and Debug | `Ctrl+Shift+D` | Open debug panel |
| Terminal | `Ctrl+`` ` | Open integrated terminal |
| Command Palette | `Ctrl+Shift+P` | Access all commands |
| Problems | `Ctrl+Shift+M` | View compilation errors |

## Common Debug Commands (Debug Console)

```csharp
// View variable value
myVariable

// Check object properties
myObject.PropertyName

// Evaluate expressions
DateTime.Now.ToString("yyyy-MM-dd")

// Call methods
myObject.DoSomething()

// Check collections
myList.Count
myList[0]
```

## Quick Setup New Project

```powershell
# Create new console app
dotnet new console -n MyApp

# Add to solution
dotnet sln add MyApp/MyApp.csproj

# Add common packages
cd MyApp
dotnet add package Microsoft.Extensions.Hosting
dotnet add package Serilog.Extensions.Hosting

# Build and run
dotnet build
dotnet run
```

## Launch Configuration Template

```json
{
    "name": "Launch MyApp",
    "type": "coreclr",
    "request": "launch",
    "program": "${workspaceFolder}/MyApp/bin/Debug/net8.0/MyApp.dll",
    "args": [],
    "cwd": "${workspaceFolder}/MyApp",
    "console": "integratedTerminal"
}
```

---
📌 **Pin this reference next to your monitor for quick access!**