# .NET CLI Quick Reference

Common .NET CLI commands for creating a solution, creating projects, adding projects to a solution, and adding NuGet packages.

## Create a solution

```powershell
dotnet new sln -n MySolution
```

## Create projects

# Console app
```powershell
dotnet new console -o MyApp
```

# Class library
```powershell
dotnet new classlib -o MyLib
```

## ASP.NET Core Web API

```powershell
dotnet new webapi -o MyApi
```

## Add project(s) to the solution

```powershell
dotnet sln MySolution.sln add MyApp/MyApp.csproj
dotnet sln MySolution.sln add MyLib/MyLib.csproj
```

## Add a project reference

```powershell
dotnet add MyApp/MyApp.csproj reference ..\MyLib\MyLib.csproj
```

## Add a NuGet package to a project

```powershell
dotnet add MyApp/MyApp.csproj package Newtonsoft.Json --version 13.0.1
```

## List packages for a project

```powershell
dotnet list MyApp/MyApp.csproj package
```

## Restore, build, and run

```powershell
dotnet restore
dotnet build MySolution.sln
dotnet run --project MyApp/MyApp.csproj
```

## Remove package or project from solution

```powershell
dotnet remove MyApp/MyApp.csproj package Newtonsoft.Json
dotnet sln MySolution.sln remove MyLib/MyLib.csproj
```

## Rename projects

$oldName = "OldName"
$newName = "NewName"

# 1. Copy the directory
Copy-Item -Path ".\$oldName" -Destination ".\$newName" -Recurse

# 2. Rename the .csproj file
Rename-Item -Path ".\$newName\$oldName.csproj" -NewName "$newName.csproj"

# 3. Replace the namespace and references inside all .cs and .csproj files
Get-ChildItem -Path ".\$newName" -Recurse -Include *.cs, *.csproj | ForEach-Object {
    (Get-Content $_.FullName) -replace $oldName, $newName | Set-Content $_.FullName
}

# 4. Add the new project to the solution file
dotnet sln add ".\$newName\$newName.csproj"