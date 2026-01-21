# Template Architecture & Structure

## Generated Solution Structure

When you run `dotnet new kry-modular -c Acme -p Products`, you get:

```
Acme.Products/
└── src/
    ├── Acme.Products.sln
    │
    ├── Hosts/                              ← Application Entry Points
    │   ├── Acme.Products.Hosts.WebApi/     
    │   │   ├── Controllers/
    │   │   │   └── WeatherForecastController.cs
    │   │   ├── Properties/
    │   │   │   └── launchSettings.json
    │   │   ├── appsettings.json
    │   │   ├── appsettings.Development.json
    │   │   ├── Program.cs
    │   │   └── Acme.Products.Hosts.WebApi.csproj
    │   │
    │   └── Acme.Products.Hosts.WPF/        [Optional: --include-wpf]
    │       ├── App.xaml
    │       ├── App.xaml.cs
    │       ├── MainWindow.xaml
    │       ├── MainWindow.xaml.cs
    │       └── Acme.Products.Hosts.WPF.csproj
    │
    ├── Modules/                            ← Business Modules
    │   └── Acme.Products.Modules.Auth/     [Optional: --include-auth]
    │       ├── Contracts/
    │       │   └── IJwtTokenService.cs
    │       ├── Implements/
    │       │   └── JwtTokenService.cs
    │       ├── Extensions/
    │       │   ├── JwtOptions.cs
    │       │   └── ServiceCollectionExtensions.cs
    │       └── Acme.Products.Modules.Auth.csproj
    │
    ├── Services/                           ← Infrastructure Services
    │   └── Acme.Products.Services.Excel/   [Optional: --include-excel]
    │       ├── Contracts/
    │       ├── Implements/
    │       ├── Extensions/
    │       └── Acme.Products.Services.Excel.csproj
    │
    └── Shared/                             ← Common Libraries
        ├── Acme.Products.Shared.BaseModule/
        │   ├── Class1.cs
        │   └── Acme.Products.Shared.BaseModule.csproj
        │
        └── Acme.Products.Shared.UnitOfWork/
            ├── Contracts/
            ├── Implements/
            ├── Extensions/
            └── Acme.Products.Shared.UnitOfWork.csproj
```

## Template Configuration Flow

```
User Command:
dotnet new kry-modular -c Acme -p Products -f net9.0 --include-wpf false

          ↓

template.json processes symbols:
┌─────────────────────────────────────────────┐
│ Symbol Processing                           │
├─────────────────────────────────────────────┤
│ CompanyName:  "Acme"                        │
│ ProjectName:  "Products"                    │
│ Framework:    "net9.0"                      │
│ FrameworkWpf: "net9.0-windows" (generated)  │
│ IncludeWPF:   false                         │
│ IncludeAuth:  true (default)                │
│ IncludeExcel: true (default)                │
│ UseSwagger:   true (default)                │
└─────────────────────────────────────────────┘

          ↓

Replacement Process:
┌─────────────────────────────────────────────┐
│ File Content Replacement                    │
├─────────────────────────────────────────────┤
│ CompanyName.ProjectName → Acme.Products     │
│ CompanyName → Acme                          │
│ ProjectName → Products                      │
│ FRAMEWORK_VERSION → net9.0                  │
│ FRAMEWORK_VERSION_WPF → net9.0-windows      │
└─────────────────────────────────────────────┘

          ↓

Source Modifiers Applied:
┌─────────────────────────────────────────────┐
│ Conditional Exclusions                      │
├─────────────────────────────────────────────┤
│ ✗ WPF project excluded (IncludeWPF=false)  │
│ ✓ Auth module included (IncludeAuth=true)  │
│ ✓ Excel service included                   │
│ ✓ Swagger files included                   │
└─────────────────────────────────────────────┘

          ↓

Post Actions:
┌─────────────────────────────────────────────┐
│ Automatic Actions                           │
├─────────────────────────────────────────────┤
│ 1. dotnet restore (restore packages)       │
│ 2. Open in IDE (optional)                  │
└─────────────────────────────────────────────┘

          ↓

Final Output:
Acme.Products solution with 5 projects (no WPF)
Ready to build and run!
```

## Parameter Decision Tree

```
Creating new project...
│
├─ Include WPF Desktop Host?
│  ├─ Yes (--include-wpf true)  → Hosts.WPF included
│  └─ No  (--include-wpf false) → Hosts.WPF excluded
│
├─ Include Authentication Module?
│  ├─ Yes (--include-auth true)  → Modules.Auth included
│  └─ No  (--include-auth false) → Modules.Auth excluded
│
├─ Include Excel Service?
│  ├─ Yes (--include-excel true)  → Services.Excel included
│  └─ No  (--include-excel false) → Services.Excel excluded
│
├─ Include Swagger/OpenAPI?
│  ├─ Yes (--use-swagger true)  → Swagger files included
│  └─ No  (--use-swagger false) → Swagger files excluded
│
└─ Choose Framework Version?
   ├─ net10.0 (default)  → Latest .NET
   ├─ net9.0             → .NET 9 LTS
   └─ net8.0             → .NET 8 LTS
```

## Common Configuration Scenarios

### 1. Full-Stack Enterprise Application
```bash
dotnet new kry-modular -c Enterprise -p ERP
```
**Result:** All features enabled
- WebAPI + WPF
- Authentication
- Excel Services
- All Shared Libraries

### 2. Microservice API
```bash
dotnet new kry-modular -c MyCompany -p OrderService \
  --include-wpf false
```
**Result:** API-focused
- WebAPI only
- Authentication enabled
- Excel Services enabled
- Shared Libraries

### 3. Minimal REST API
```bash
dotnet new kry-modular -c Startup -p MinimalApi \
  --wpf false --auth false --excel false
```
**Result:** Bare minimum
- WebAPI only
- No authentication
- No Excel
- Shared Libraries only

### 4. Desktop Application with Services
```bash
dotnet new kry-modular -c Desktop -p DataManager \
  --include-auth true --include-excel true
```
**Result:** Desktop + Services
- WPF + WebAPI
- Authentication enabled
- Excel enabled
- Full featured

## Project Dependencies

```
┌─────────────────────────────────────────────────────────┐
│                    Acme.Products.sln                    │
└─────────────────────────────────────────────────────────┘
                           │
        ┌──────────────────┼──────────────────┐
        ▼                  ▼                  ▼
   ┌─────────┐        ┌─────────┐      ┌─────────┐
   │  Hosts  │        │ Modules │      │Services │
   └─────────┘        └─────────┘      └─────────┘
        │                  │                  │
   ┌────┴────┐            │                  │
   ▼         ▼            ▼                  ▼
WebApi     WPF         Auth              Excel
   │         │            │                  │
   └─────────┴────────────┴──────────────────┘
                      ▼
              ┌───────────────┐
              │    Shared     │
              ├───────────────┤
              │  BaseModule   │
              │  UnitOfWork   │
              └───────────────┘
```

## Technology Stack

```
┌─────────────────────────────────────────────────────────┐
│                  Technology Layers                      │
├─────────────────────────────────────────────────────────┤
│                                                         │
│  Presentation Layer                                     │
│  ┌─────────────┐              ┌─────────────┐         │
│  │   ASP.NET   │              │     WPF     │         │
│  │   Web API   │              │   Desktop   │         │
│  │  + Swagger  │              │             │         │
│  └─────────────┘              └─────────────┘         │
│                                                         │
│  Business Layer                                         │
│  ┌─────────────┐              ┌─────────────┐         │
│  │     JWT     │              │   Custom    │         │
│  │    Auth     │              │   Modules   │         │
│  │   Module    │              │             │         │
│  └─────────────┘              └─────────────┘         │
│                                                         │
│  Service Layer                                          │
│  ┌─────────────┐              ┌─────────────┐         │
│  │    Excel    │              │   Future    │         │
│  │  Import/    │              │  Services   │         │
│  │   Export    │              │             │         │
│  └─────────────┘              └─────────────┘         │
│                                                         │
│  Data Access Layer                                      │
│  ┌──────────────────────────────────────────┐         │
│  │        Unit of Work + Dapper             │         │
│  │        (Repository Pattern)              │         │
│  └──────────────────────────────────────────┘         │
│                                                         │
│  Infrastructure                                         │
│  ┌──────────────────────────────────────────┐         │
│  │   Dependency Injection                   │         │
│  │   Configuration Management               │         │
│  │   Logging                                │         │
│  └──────────────────────────────────────────┘         │
└─────────────────────────────────────────────────────────┘
```

## NuGet Package Dependencies

```
Core Packages (All Projects):
├─ Microsoft.Extensions.DependencyInjection
└─ Microsoft.Extensions.Hosting

WebApi Packages:
├─ Microsoft.AspNetCore.OpenApi
└─ (Framework Reference: Microsoft.AspNetCore.App)

WPF Packages:
├─ Microsoft.Extensions.DependencyInjection
└─ Microsoft.Extensions.Hosting

Auth Module Packages:
├─ Microsoft.AspNetCore.Authentication.JwtBearer
├─ Microsoft.Extensions.Options
└─ Microsoft.Extensions.Options.ConfigurationExtensions

Excel Service Packages:
├─ ClosedXML
├─ ExcelDataReader
├─ ExcelDataReader.DataSet
├─ Dapper
└─ Microsoft.Data.SqlClient

UnitOfWork Packages:
├─ Dapper
├─ Microsoft.Data.SqlClient
├─ Microsoft.Extensions.Options
└─ Microsoft.Extensions.Options.ConfigurationExtensions
```

## File Organization Best Practices

```
Project Layout Pattern:
├─ Contracts/          ← Interfaces and abstractions
├─ Implements/         ← Concrete implementations
├─ Extensions/         ← Extension methods and DI setup
├─ Models/            ← DTOs and view models
└─ [ProjectName].csproj

Example: Auth Module
Acme.Products.Modules.Auth/
├─ Contracts/
│  └─ IJwtTokenService.cs
├─ Implements/
│  └─ JwtTokenService.cs
├─ Extensions/
│  ├─ JwtOptions.cs
│  └─ ServiceCollectionExtensions.cs
└─ Acme.Products.Modules.Auth.csproj
```

## Getting Started Flowchart

```
Start
  │
  ├─ 1. Install Template
  │    dotnet new install C:\repo\Kry-dotnet-template
  │
  ├─ 2. Verify Installation
  │    dotnet new list | findstr kry-modular
  │
  ├─ 3. Choose Configuration
  │    ├─ Full-featured?   → Use defaults
  │    ├─ API-only?        → --wpf false
  │    └─ Minimal?         → --wpf false --auth false --excel false
  │
  ├─ 4. Create Project
  │    dotnet new kry-modular -c [Company] -p [Project] [options]
  │
  ├─ 5. Navigate to Solution
  │    cd src
  │
  ├─ 6. Build
  │    dotnet build
  │
  ├─ 7. Run Application
  │    ├─ WebAPI:  cd Hosts\[Company].[Project].Hosts.WebApi
  │    │           dotnet run
  │    │           → Browse to https://localhost:5001/swagger
  │    │
  │    └─ WPF:     cd Hosts\[Company].[Project].Hosts.WPF
  │                dotnet run
  │
  └─ 8. Start Development!
```
