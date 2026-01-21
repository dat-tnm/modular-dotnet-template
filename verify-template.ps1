# Template Verification Script
# Run this script to verify the template is correctly configured

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Kry Modular Template Verification" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check template.json exists and is valid JSON
Write-Host "1. Checking template.json..." -ForegroundColor Yellow
$templateJsonPath = "C:\repo\Kry-dotnet-template\.template.config\template.json"
if (Test-Path $templateJsonPath) {
    try {
        $template = Get-Content $templateJsonPath | ConvertFrom-Json
        Write-Host "   ✓ template.json exists and is valid JSON" -ForegroundColor Green
        Write-Host "   - Identity: $($template.identity)" -ForegroundColor Gray
        Write-Host "   - Name: $($template.name)" -ForegroundColor Gray
        Write-Host "   - ShortName: $($template.shortName)" -ForegroundColor Gray
    } catch {
        Write-Host "   ✗ template.json is invalid JSON" -ForegroundColor Red
        exit 1
    }
} else {
    Write-Host "   ✗ template.json not found" -ForegroundColor Red
    exit 1
}

# Check all project files use placeholders
Write-Host "`n2. Checking project files for placeholders..." -ForegroundColor Yellow
$projectFiles = Get-ChildItem "C:\repo\Kry-dotnet-template\content" -Recurse -Filter *.csproj
$hardcodedCount = 0
foreach ($file in $projectFiles) {
    $content = Get-Content $file.FullName -Raw
    if ($content -match "<TargetFramework>net\d+\.\d+(-windows)?</TargetFramework>" -and $content -notmatch "FRAMEWORK_VERSION") {
        Write-Host "   ✗ Hardcoded framework in: $($file.Name)" -ForegroundColor Red
        $hardcodedCount++
    }
}
if ($hardcodedCount -eq 0) {
    Write-Host "   ✓ All project files use FRAMEWORK_VERSION placeholder" -ForegroundColor Green
} else {
    Write-Host "   ✗ Found $hardcodedCount files with hardcoded frameworks" -ForegroundColor Red
}

# Check solution file includes all projects
Write-Host "`n3. Checking solution file..." -ForegroundColor Yellow
$solutionPath = "C:\repo\Kry-dotnet-template\content\src\CompanyName.ProjectName.sln"
if (Test-Path $solutionPath) {
    $solutionContent = Get-Content $solutionPath -Raw
    $expectedProjects = @(
        "CompanyName.ProjectName.Hosts.WebApi",
        "CompanyName.ProjectName.Hosts.WPF",
        "CompanyName.ProjectName.Modules.Auth",
        "CompanyName.ProjectName.Services.Excel",
        "CompanyName.ProjectName.Shared.BaseModule",
        "CompanyName.ProjectName.Shared.UnitOfWork"
    )
    $allFound = $true
    foreach ($project in $expectedProjects) {
        if ($solutionContent -match $project) {
            Write-Host "   ✓ Found: $project" -ForegroundColor Green
        } else {
            Write-Host "   ✗ Missing: $project" -ForegroundColor Red
            $allFound = $false
        }
    }
} else {
    Write-Host "   ✗ Solution file not found" -ForegroundColor Red
}

# Check documentation files exist
Write-Host "`n4. Checking documentation files..." -ForegroundColor Yellow
$docFiles = @(
    "README.md",
    "QUICKSTART.md",
    "USAGE.md",
    "INSTALL-COMMANDS.md",
    "IMPLEMENTATION-SUMMARY.md"
)
foreach ($doc in $docFiles) {
    $docPath = "C:\repo\Kry-dotnet-template\$doc"
    if (Test-Path $docPath) {
        Write-Host "   ✓ Found: $doc" -ForegroundColor Green
    } else {
        Write-Host "   ✗ Missing: $doc" -ForegroundColor Red
    }
}

# Check template config files
Write-Host "`n5. Checking template configuration files..." -ForegroundColor Yellow
$configFiles = @(
    ".template.config\template.json",
    ".template.config\dotnetcli.host.json",
    ".template.config\.templateignore"
)
foreach ($config in $configFiles) {
    $configPath = "C:\repo\Kry-dotnet-template\$config"
    if (Test-Path $configPath) {
        Write-Host "   ✓ Found: $config" -ForegroundColor Green
    } else {
        Write-Host "   ✗ Missing: $config" -ForegroundColor Red
    }
}

# Test template installation (optional)
Write-Host "`n6. Template Installation Test (Optional)" -ForegroundColor Yellow
$response = Read-Host "Do you want to test template installation? (y/n)"
if ($response -eq 'y') {
    Write-Host "   Installing template..." -ForegroundColor Gray
    dotnet new install C:\repo\Kry-dotnet-template
    
    Write-Host "   Checking if template is installed..." -ForegroundColor Gray
    $installed = dotnet new list | Select-String "kry-modular"
    if ($installed) {
        Write-Host "   ✓ Template installed successfully" -ForegroundColor Green
        Write-Host "   $installed" -ForegroundColor Gray
        
        # Ask if user wants to create a test project
        $testResponse = Read-Host "   Create a test project? (y/n)"
        if ($testResponse -eq 'y') {
            $testPath = "C:\Temp\TemplateVerificationTest"
            if (Test-Path $testPath) {
                Remove-Item -Recurse -Force $testPath
            }
            New-Item -ItemType Directory -Path $testPath | Out-Null
            Set-Location $testPath
            
            Write-Host "   Creating test project..." -ForegroundColor Gray
            dotnet new kry-modular -c VerifyCompany -p VerifyProject
            
            Write-Host "   Building test project..." -ForegroundColor Gray
            Set-Location src
            $buildResult = dotnet build 2>&1
            if ($LASTEXITCODE -eq 0) {
                Write-Host "   ✓ Test project built successfully" -ForegroundColor Green
            } else {
                Write-Host "   ✗ Test project build failed" -ForegroundColor Red
                Write-Host $buildResult -ForegroundColor Red
            }
            
            # Clean up
            Set-Location C:\
            $cleanupResponse = Read-Host "   Remove test project? (y/n)"
            if ($cleanupResponse -eq 'y') {
                Remove-Item -Recurse -Force $testPath
                Write-Host "   Test project removed" -ForegroundColor Gray
            }
        }
        
        # Ask if user wants to uninstall
        $uninstallResponse = Read-Host "   Uninstall template? (y/n)"
        if ($uninstallResponse -eq 'y') {
            dotnet new uninstall C:\repo\Kry-dotnet-template
            Write-Host "   Template uninstalled" -ForegroundColor Gray
        }
    } else {
        Write-Host "   ✗ Template installation failed" -ForegroundColor Red
    }
}

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "Verification Complete!" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "1. Review QUICKSTART.md for usage instructions" -ForegroundColor Gray
Write-Host "2. Install template: dotnet new install C:\repo\Kry-dotnet-template" -ForegroundColor Gray
Write-Host "3. Create project: dotnet new kry-modular -c YourCompany -p YourProject" -ForegroundColor Gray
Write-Host ""
