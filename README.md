# Tnm Modular Monolith Dotnet Template

A comprehensive .NET solution template for building modular monolith applications with multiple hosting options, authentication, and services.

## Features

- **Multi-Host Architecture**: Includes both WebAPI and WPF desktop application hosts
- **Modular Design**: Organized into Modules, Services, and Shared libraries
- **JWT Authentication**: Optional authentication module with JWT token services
- **Excel Services**: Optional Excel import/export capabilities
- **Clean Architecture**: Separation of concerns with clear project boundaries
- **Modern .NET**: Support for .NET 8.0, 9.0, and 10.0

## Solution Structure

```
YourSolution/
├── src/
│   ├── Hosts/
│   │   ├── YourCompany.YourProject.Hosts.WebApi    # ASP.NET Core Web API
│   │   └── YourCompany.YourProject.Hosts.WPF       # WPF Desktop Application
│   ├── Modules/
│   │   └── YourCompany.YourProject.Modules.Auth    # JWT Authentication Module
│   ├── Services/
│   │   └── YourCompany.YourProject.Services.Excel  # Excel Import/Export Service
│   └── Shared/
│       ├── YourCompany.YourProject.Shared.BaseModule
│       └── YourCompany.YourProject.Shared.UnitOfWork
└── tests/
```

## Installation

### Install from NuGet (Recommended)

[![NuGet](https://img.shields.io/nuget/v/tnm.ModularMonolith.Template.svg)](https://www.nuget.org/packages/tnm.ModularMonolith.Template)

```bash
dotnet new install tnm.ModularMonolith.Template
```

📦 **NuGet Package**: [https://www.nuget.org/packages/tnm.ModularMonolith.Template](https://www.nuget.org/packages/tnm.ModularMonolith.Template)

### Install from Local Path (Development)

```bash
dotnet new install C:\path\to\modular-dotnet-template
```

### Verify Installation

```bash
dotnet new list | findstr kry-modular
```

## Usage

### Create a New Solution

```bash
# Basic usage with default options
dotnet new kry-modular -n MyCompany.MyProject

# With custom company and project names
dotnet new kry-modular -c Acme -p ProductCatalog

# With specific framework version
dotnet new kry-modular -n MyCompany.MyProject -f net9.0

# Without WPF project
dotnet new kry-modular -n MyCompany.MyProject --include-wpf false

# Minimal configuration (API only, no Auth, no Excel)
dotnet new kry-modular -n MyCompany.MyProject --include-wpf false --include-auth false --include-excel false
```

### Command-Line Parameters

| Parameter | Short | Type | Default | Description |
|-----------|-------|------|---------|-------------|
| `--company-name` | `-c` | string | CompanyName | The company name for namespaces |
| `--project-name` | `-p` | string | ProjectName | The project name for namespaces |
| `--framework` | `-f` | choice | net10.0 | Target framework (net8.0, net9.0, net10.0) |
| `--include-wpf` | `--wpf` | bool | true | Include WPF desktop host |
| `--include-auth` | `--auth` | bool | true | Include JWT authentication module |
| `--include-excel` | `--excel` | bool | true | Include Excel service |
| `--use-swagger` | `--swagger` | bool | true | Include Swagger/OpenAPI in WebAPI |

### Examples

#### Full-Featured Solution
```bash
dotnet new kry-modular -n Contoso.ERP -c Contoso -p ERP -f net10.0
```

#### API-Only Microservice
```bash
dotnet new kry-modular -n MyApi -c MyCompany -p Api --include-wpf false
```

#### Desktop Application with Services
```bash
dotnet new kry-modular -n DesktopApp -c MyCompany -p Desktop --include-auth true --include-excel true
```

## Post-Creation Steps

After creating your solution:

1. **Restore NuGet packages** (done automatically via post-action):
   ```bash
   dotnet restore
   ```

2. **Build the solution**:
   ```bash
   dotnet build
   ```

3. **Run the WebAPI**:
   ```bash
   cd src/Hosts/YourCompany.YourProject.Hosts.WebApi
   dotnet run
   ```

4. **Run the WPF Application** (if included):
   ```bash
   cd src/Hosts/YourCompany.YourProject.Hosts.WPF
   dotnet run
   ```

## Projects Overview

### Hosts

- **WebApi**: ASP.NET Core Web API with OpenAPI/Swagger support
- **WPF**: Windows Presentation Foundation desktop application

### Modules

- **Auth**: JWT authentication and authorization services
  - JWT token generation and validation
  - Configurable options for token settings

### Services

- **Excel**: Excel file processing capabilities
  - ClosedXML for creating Excel files
  - ExcelDataReader for reading Excel files
  - Dapper for database operations

### Shared

- **BaseModule**: Base classes and common functionality
- **UnitOfWork**: Unit of Work pattern implementation with Dapper

## Development

### Adding New Modules

1. Create a new class library in the `Modules` folder
2. Follow the naming convention: `YourCompany.YourProject.Modules.[ModuleName]`
3. Add project reference to the solution
4. Register services in your host application

### Adding New Services

1. Create a new class library in the `Services` folder
2. Follow the naming convention: `YourCompany.YourProject.Services.[ServiceName]`
3. Add project reference to the solution
4. Inject services in your host application

## Configuration

### JWT Authentication

Configure JWT settings in `appsettings.json`:

```json
{
  "Jwt": {
    "Key": "your-secret-key-here",
    "Issuer": "your-issuer",
    "Audience": "your-audience",
    "ExpiryMinutes": 60
  }
}
```

### Database Connection

Configure connection strings in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=YourDb;Trusted_Connection=True;"
  }
}
```

## Uninstall Template

```bash
dotnet new uninstall C:\path\to\Kry-dotnet-template
```

## License

[Specify your license here]

## Support

For issues or questions, please [create an issue](https://github.com/dat-tnm/modular-dotnet-template/issues) in the repository.