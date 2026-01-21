# Template Installation Commands

## Quick Installation & Testing

```powershell
# 1. Install the template
dotnet new install C:\repo\Kry-dotnet-template

# 2. Verify installation
dotnet new list | findstr kry-modular

# 3. Get help for the template
dotnet new kry-modular --help

# 4. Create a test project
mkdir C:\Temp\TemplateTest
cd C:\Temp\TemplateTest
dotnet new kry-modular -c TestCompany -p TestProject

# 5. Build and test
cd src
dotnet restore
dotnet build
cd Hosts\TestCompany.TestProject.Hosts.WebApi
dotnet run

# 6. Clean up test
cd C:\Temp
Remove-Item -Recurse -Force TemplateTest

# 7. Uninstall template (if needed)
dotnet new uninstall C:\repo\Kry-dotnet-template
```

## Test Different Configurations

```powershell
# Test 1: Full configuration (default)
mkdir Test1 && cd Test1
dotnet new kry-modular -c Company1 -p Project1
cd src && dotnet build && cd ..\..

# Test 2: Without WPF
mkdir Test2 && cd Test2
dotnet new kry-modular -c Company2 -p Project2 --include-wpf false
cd src && dotnet build && cd ..\..

# Test 3: Minimal (API only)
mkdir Test3 && cd Test3
dotnet new kry-modular -c Company3 -p Project3 --wpf false --auth false --excel false
cd src && dotnet build && cd ..\..

# Test 4: Different framework
mkdir Test4 && cd Test4
dotnet new kry-modular -c Company4 -p Project4 -f net9.0
cd src && dotnet build && cd ..\..

# Clean up all tests
cd ..
Remove-Item -Recurse -Force Test1, Test2, Test3, Test4
```

## Verify Template Configuration

```powershell
# Check template.json syntax
Get-Content C:\repo\Kry-dotnet-template\.template.config\template.json | ConvertFrom-Json

# List all .csproj files (should all have FRAMEWORK_VERSION placeholder)
Get-ChildItem C:\repo\Kry-dotnet-template\content -Recurse -Filter *.csproj | ForEach-Object {
    $content = Get-Content $_.FullName -Raw
    if ($content -match "net10.0" -and $content -notmatch "FRAMEWORK_VERSION") {
        Write-Warning "Hardcoded framework found in: $($_.Name)"
    }
}
```

## Update Template Workflow

```powershell
# 1. Uninstall
dotnet new uninstall C:\repo\Kry-dotnet-template

# 2. Make changes to template files

# 3. Reinstall
dotnet new install C:\repo\Kry-dotnet-template

# 4. Test
mkdir C:\Temp\QuickTest && cd C:\Temp\QuickTest
dotnet new kry-modular -c Test -p App
cd src && dotnet build
```
