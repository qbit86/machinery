# Project Structure

## Root Level Organization
```
├── src/                    # Source code
├── tests/                  # Unit tests
├── samples/                # Example implementations
├── assets/                 # Icons, signing keys, resources
└── .artifacts/             # Build outputs (bin/, obj/)
```

## Source Structure (`src/`)

- **src/Machinery/** - Main library project containing core state machine implementation
- **src/Shared/** - Shared code projects (e.g., NullableAttributes for older frameworks)
- **src/Directory.Build.props** - Source-specific MSBuild properties (packaging, versioning)

## Test Structure (`tests/`)

- **tests/Tests.Machinery/** - Unit tests for the main library
- **tests/Directory.Build.props** - Test-specific MSBuild properties (IsTestProject, target framework)

## Sample Structure (`samples/`)

- **Samples.MonomorphicValueStateDemo** - Value type state example
- **Samples.PolymorphicReferenceStateDemo** - Reference type polymorphic states
- **Samples.PolymorphicValueStateDemo** - Value type polymorphic states  
- **Samples.ContextDemo** - Context usage patterns

## Configuration Files

- **Directory.Build.props** - Root build properties (analysis, nullable, language version)
- **Directory.Packages.props** - Centralized package version management
- **global.json** - .NET SDK version pinning
- **.editorconfig** - Code style and formatting rules
- **Machinery.sln** - Solution file organizing all projects

## Naming Conventions

- **Projects**: PascalCase with descriptive names (e.g., `Tests.Machinery`, `Samples.ContextDemo`)
- **Folders**: Match project names, organized by purpose
- **Namespace**: Root namespace is `Machinery` for all projects

## Build Artifacts

- **Output**: Centralized in `.artifacts/` directory
- **Packages**: Generated with embedded symbols and documentation
- **Assembly Signing**: Strong-named assemblies using `assets/machinery.snk`
