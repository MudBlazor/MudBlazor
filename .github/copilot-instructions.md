# MudBlazor AI Coding Agent Instructions

## Project Overview
MudBlazor is a Material Design component library for Blazor, written mostly in C#. It features minimal JavaScript, extensive documentation, and a strong focus on test coverage and code quality. The repo is organized into several key projects:
- `src/MudBlazor`: Core components, styles, enums, services, and utilities
- `src/MudBlazor.Docs`: Documentation site and examples
- `src/MudBlazor.UnitTests`: bUnit and C# tests for components
- `src/MudBlazor.Docs.Compiler`: Generates API docs from source

## Architecture & Patterns
- **Components**: Located in `src/MudBlazor/Components`. Each component typically has `.razor` and `.razor.cs` files, with styles in `src/MudBlazor/Styles/components`.
- **ParameterState Pattern**: Component parameters must be auto-properties (no logic in getter/setter). Use the `ParameterState` registration pattern in the constructor for change handling. See `CONTRIBUTING.md` for examples.
- **RTL Support**: Components should support right-to-left layouts via `[CascadingParameter] public bool RightToLeft {get; set;}`.
- **Services**: Provided via DI, registered in `Program.cs` with `builder.Services.AddMudServices();`. See `src/MudBlazor/Services` for service implementations.
- **Enums & Extensions**: Shared enums in `src/MudBlazor/Enums`, helpers in `src/MudBlazor/Extensions`.
- **JavaScript Interop**: Minimal JS, located in `src/MudBlazor/TScripts`.

## Developer Workflows
- **Build**: Use the VS Code task `build` or run `dotnet build src/MudBlazor.sln`.
- **Test**: Use the VS Code task `test` or run `dotnet test src/MudBlazor.UnitTests/MudBlazor.UnitTests.csproj`. All logic changes require tests.
- **Docs Preview**: Run `MudBlazor.Docs.WasmHost` locally to preview documentation changes.
- **Coverage**: Use the VS Code task `coverage report` to view test coverage HTML report.
- **Local PR Testing**: See `TESTING.md` for instructions to pack and test MudBlazor locally in your app.

## Conventions & Best Practices
- **No logic in `[Parameter]` setters/getters**; use `ParameterState` for change handling.
- **Do not overwrite parameters in components**; use `ParameterState.SetValueAsync()`.
- **Do not set component parameters outside their markup**; use declarative syntax.
- **Tests**: bUnit tests for components, C# tests for logic. Do not save HTML element references in variables in bUnit tests.
- **Branching**: PRs should target `dev` and follow naming conventions (`feature/...`, `fix/...`).
- **Formatting**: Follow .NET formatting rules.

## Integration Points
- **NuGet**: Main package is `MudBlazor`. Local builds can be tested via custom NuGet source (see `TESTING.md`).
- **Docs Compiler**: API docs are generated from source via `src/MudBlazor.Docs.Compiler`.
- **CI**: GitHub Actions run build, test, coverage, and quality checks on all PRs.

## Key Files & Directories
- `src/MudBlazor/Components/`: Core component implementations
- `src/MudBlazor/Styles/components/`: SCSS styles for components
- `src/MudBlazor/Enums/`: Shared enums
- `src/MudBlazor/Extensions/`: Extension methods
- `src/MudBlazor.UnitTests/`: bUnit and logic tests
- `src/MudBlazor.Docs/Pages/Components/`: Docs pages for components
- `CONTRIBUTING.md`, `TESTING.md`, `README.md`: Essential workflow and architecture guidance

---

If any section is unclear or missing important project-specific details, please provide feedback so this guide can be improved for future AI agents.
