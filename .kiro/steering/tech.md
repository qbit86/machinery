# Technology Stack

## .NET Framework & Language

- **Target Frameworks**: .NET Framework 4.6.1, .NET Standard 2.0, .NET Standard 2.1
- **Language**: C# 13 (LangVersion 13 for main library, C# 9 for source projects)
- **SDK**: .NET 9.0.201 (specified in global.json)

## Build System

- **MSBuild** with modern SDK-style projects
- **Central Package Management** via Directory.Packages.props
- **Artifacts Output** using UseArtifactsOutput for build outputs in .artifacts/

## Code Quality & Analysis

- **Nullable Reference Types** enabled
- **ImplicitUsings** disabled (explicit using statements required)
- **AnalysisLevel**: latest with AnalysisMode: All
- **EnforceCodeStyleInBuild**: true
- **EnableNETAnalyzers**: true

## Testing Framework

- **xUnit v3** (1.1.0) for unit testing
- **Microsoft.NET.Test.Sdk** (17.13.0)
- **Target Framework for Tests**: .NET 9.0

## Package Management

- **NuGet** with centralized package version management
- **Reproducible Builds** via DotNet.ReproducibleBuilds package
- **Strong Name Signing** with machinery.snk key file

## Common Commands

### Build

```bash
dotnet build
dotnet build --configuration Release
```

### Test

```bash
dotnet test
dotnet test --configuration Release
```

### Pack

```bash
dotnet pack --configuration Release
```

### Restore

```bash
dotnet restore
```

## Development Tools

- **EditorConfig** for consistent code formatting
- **ReSharper** settings via .DotSettings files
- **Git** for version control with .gitattributes and .gitignore
