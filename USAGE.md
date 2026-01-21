# Template Testing and Usage Guide

## Installation

```powershell
# Install from current directory
dotnet new install .

# Or install from specific path
dotnet new install C:\repo\Kry-dotnet-template
```

## Verify Installation

```powershell
dotnet new list
# Look for: kry-modular
```

## Test Scenarios

### Scenario 1: Default Configuration (All Features)

```powershell
mkdir TestProject1
cd TestProject1
dotnet new kry-modular -c TestCompany -p TestProject
cd src
dotnet build
```

**Expected Output:**
- Solution with 6 projects
- WebApi project with Swagger
- WPF project
- Auth module
- Excel service
- 2 Shared libraries

### Scenario 2: API-Only Configuration

```powershell
mkdir TestProject2
cd TestProject2
dotnet new kry-modular -c TestCompany -p ApiProject --include-wpf false
cd src
dotnet build
```

**Expected Output:**
- Solution with 5 projects (no WPF)
- WebApi with Auth and Excel
- Shared libraries

### Scenario 3: Minimal Configuration

```powershell
mkdir TestProject3
cd TestProject3
dotnet new kry-modular `
  -c Minimal `
  -p App `
  --include-wpf false `
  --include-auth false `
  --include-excel false
cd src
dotnet build
```

**Expected Output:**
- Solution with 3 projects
- WebApi only
- 2 Shared libraries

### Scenario 4: Framework Version Test

```powershell
# Test with .NET 9.0
mkdir TestNet9
cd TestNet9
dotnet new kry-modular -c TestCo -p Net9App -f net9.0
cd src
dotnet build

# Verify framework version in .csproj files
Get-Content Hosts\TestCo.Net9App.Hosts.WebApi\TestCo.Net9App.Hosts.WebApi.csproj | Select-String "TargetFramework"
# Should show: <TargetFramework>net9.0</TargetFramework>
```

## Running the Generated Projects

### Run WebAPI

```powershell
cd src\Hosts\YourCompany.YourProject.Hosts.WebApi
dotnet run

# API should be available at:
# https://localhost:5001
# Swagger UI: https://localhost:5001/swagger
```

### Run WPF Application

```powershell
cd src\Hosts\YourCompany.YourProject.Hosts.WPF
dotnet run
```

## Validation Checklist

After creating a project from the template, verify:

- [ ] Solution file opens without errors
- [ ] All projects are included in the solution
- [ ] Projects restore successfully (`dotnet restore`)
- [ ] Solution builds without errors (`dotnet build`)
- [ ] Correct framework version is set in all .csproj files
- [ ] Namespace replacement worked correctly (no "CompanyName.ProjectName" remains)
- [ ] WebApi runs and Swagger page is accessible
- [ ] WPF application launches (if included)
- [ ] Auth module is included/excluded based on parameter
- [ ] Excel service is included/excluded based on parameter

## Troubleshooting

### Build Errors

If you encounter build errors:

```powershell
# Clean and restore
dotnet clean
dotnet restore
dotnet build
```

### Template Not Found

```powershell
# List installed templates
dotnet new list

# If not listed, reinstall
dotnet new uninstall C:\repo\Kry-dotnet-template
dotnet new install C:\repo\Kry-dotnet-template
```

### Wrong Framework Version

Check that the placeholder was replaced:

```powershell
# Search for unreplaced placeholders
Get-ChildItem -Recurse -Filter *.csproj | ForEach-Object {
    $content = Get-Content $_.FullName
    if ($content -match "FRAMEWORK_VERSION") {
        Write-Host "Unreplaced placeholder found in: $($_.FullName)"
    }
}
```

## Uninstall Template

```powershell
dotnet new uninstall C:\repo\Kry-dotnet-template
```

## Template Modification Workflow

When you need to update the template:

1. **Uninstall current version:**
   ```powershell
   dotnet new uninstall C:\repo\Kry-dotnet-template
   ```

2. **Make your changes** to the template files

3. **Reinstall:**
   ```powershell
   dotnet new install C:\repo\Kry-dotnet-template
   ```

4. **Test:** Create a new project to verify changes

## Advanced Usage

### Custom Parameters via CLI

```powershell
# Using short names
dotnet new kry-modular -c Acme -p Products -f net9.0 --wpf false --auth true

# Using long names
dotnet new kry-modular `
  --company-name "MyCompany" `
  --project-name "MyProject" `
  --framework net10.0 `
  --include-wpf true `
  --include-auth true `
  --include-excel true `
  --use-swagger true
```

### Batch Testing

```powershell
# Create multiple test projects
$configs = @(
    @{name="FullFeature"; wpf=$true; auth=$true; excel=$true},
    @{name="ApiOnly"; wpf=$false; auth=$true; excel=$true},
    @{name="Minimal"; wpf=$false; auth=$false; excel=$false}
)

foreach ($config in $configs) {
    $folder = "Test_$($config.name)"
    mkdir $folder
    cd $folder
    dotnet new kry-modular `
        -c TestCo `
        -p $config.name `
        --include-wpf $config.wpf `
        --include-auth $config.auth `
        --include-excel $config.excel
    cd src
    dotnet build
    cd ..\..
}
```

## Publishing to NuGet

### Create Package

```powershell
cd C:\repo\Kry-dotnet-template
nuget pack template.nuspec
```

### Install from Package

```powershell
dotnet new install .\Kry.ModularMonolith.Template.1.0.0.nupkg
```

### Publish to NuGet.org

```powershell
nuget push Kry.ModularMonolith.Template.1.0.0.nupkg -Source https://api.nuget.org/v3/index.json -ApiKey YOUR_API_KEY
```

## Getting Help

```powershell
# Get detailed help for the template
dotnet new kry-modular --help

# List all available parameters
dotnet new kry-modular --help | Select-String "Options:"
```
