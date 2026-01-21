# Quick Start Guide

## Installing the Template

### From Local Folder
```powershell
# Navigate to the template folder or use full path
dotnet new install .

# Or with full path
dotnet new install C:\repo\Kry-dotnet-template
```

### Verify Installation
```powershell
dotnet new list | findstr kry-modular
```

You should see:
```
kry-modular    Kry Modular Monolith Solution    [C#]    Solution/Web/API/Desktop/Modular
```

## Creating Your First Project

### Example 1: Full-Featured Solution
```powershell
# Create a new folder and navigate to it
mkdir MyNewProject
cd MyNewProject

# Create the solution
dotnet new kry-modular -c MyCompany -p MyProject

# Result: MyCompany.MyProject solution with all features
```

### Example 2: API-Only Solution
```powershell
mkdir MyApi
cd MyApi

# Create without WPF
dotnet new kry-modular -c Acme -p ProductApi --include-wpf false

# Result: Acme.ProductApi with WebAPI only
```

### Example 3: Different Framework Version
```powershell
mkdir LegacyProject
cd LegacyProject

# Target .NET 8.0
dotnet new kry-modular -c Legacy -p System -f net8.0

# Result: Legacy.System targeting .NET 8.0
```

## Testing the Template

### Test the WebAPI
```powershell
cd src\Hosts\MyCompany.MyProject.Hosts.WebApi
dotnet run

# Open browser to: https://localhost:5001/swagger
```

### Test the WPF App (if included)
```powershell
cd src\Hosts\MyCompany.MyProject.Hosts.WPF
dotnet run
```

## Common Issues

### Issue: Template not found
**Solution**: Make sure you installed from the correct path:
```powershell
dotnet new list
# If not listed, reinstall
dotnet new install C:\repo\Kry-dotnet-template
```

### Issue: Project doesn't build
**Solution**: Restore NuGet packages:
```powershell
dotnet restore
dotnet build
```

### Issue: Want to modify the template
**Solution**: Uninstall, make changes, reinstall:
```powershell
dotnet new uninstall C:\repo\Kry-dotnet-template
# Make your changes
dotnet new install C:\repo\Kry-dotnet-template
```

## All Available Options

```powershell
dotnet new kry-modular --help
```

### Quick Reference
```powershell
# Minimal syntax
dotnet new kry-modular -n MyProject

# Full syntax with all options
dotnet new kry-modular `
  --company-name MyCompany `
  --project-name MyProject `
  --framework net10.0 `
  --include-wpf true `
  --include-auth true `
  --include-excel true `
  --use-swagger true
```

## Next Steps

1. Review the generated solution structure
2. Update `appsettings.json` with your configuration
3. Customize the projects to fit your needs
4. Add your business logic
5. Set up your database connection
6. Configure authentication if using Auth module

## Updating the Template

To update after making changes:
```powershell
# Uninstall current version
dotnet new uninstall C:\repo\Kry-dotnet-template

# Reinstall updated version
dotnet new install C:\repo\Kry-dotnet-template

# Verify
dotnet new list | findstr kry-modular
```

## Packaging for Distribution

To create a NuGet package:
```powershell
cd C:\repo\Kry-dotnet-template
dotnet pack -c Release

# The .nupkg file will be in bin\Release\
```

Install from NuGet package:
```powershell
dotnet new install .\bin\Release\Kry.ModularMonolith.Template.1.0.0.nupkg
```
