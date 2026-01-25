# AGENTS.md - AI Coding Agent Guide for MudBlazor

## Overview

MudBlazor: Material Design component library for Blazor (Server/WebAssembly)

- **Stack:** .NET (8.0/9.0/10.0), SCSS, minimal JS in TScripts/, bUnit tests
- **Scale:** ~450 components, ~3,700+ tests

## Prerequisites

- .NET 10.0 SDK (10.0.100+) required for tests; library targets net8.0/net9.0/net10.0. Verify with `dotnet --version`.

## Core Commands

**CRITICAL:** Target specific projects only. Solution-wide commands are too slow.

### Target Projects by Change Type

- **Components:** `src/MudBlazor/MudBlazor.csproj` + `src/MudBlazor.UnitTests/MudBlazor.UnitTests.csproj`
- **Docs:** `src/MudBlazor.Docs.Compiler/MudBlazor.Docs.Compiler.csproj` + `src/MudBlazor.Docs/MudBlazor.Docs.csproj`
- **Analyzers:** `src/MudBlazor.Analyzers/MudBlazor.Analyzers.csproj` or `src/MudBlazor.SourceGenerator/MudBlazor.SourceGenerator.csproj`

### Clean, Build, Test

```bash
dotnet clean <project.csproj>
dotnet build <project.csproj> -c Release --nologo
dotnet test src/MudBlazor.UnitTests/MudBlazor.UnitTests.csproj --filter "FullyQualifiedName~MudButton" --no-build -c Release --nologo --blame-hang --blame-hang-timeout 60s
```

Use `dotnet build /p:SkipBunCompile=true` to skip Bun JS/SCSS compilation for C#-only changes (bUnit doesn't depend on CSS/JS). Avoid this for styling/JS changes or full builds.

### Formatting (REQUIRED)

```bash
dotnet format <project.csproj> --include <path/to/changed/files>
```

CI will fail if code is not formatted.

### Run Locally

```bash
# Docs preview (preferred for docs changes)
dotnet run --project src/MudBlazor.Docs.WasmHost/MudBlazor.Docs.WasmHost.csproj

# Docs server (debugging)
dotnet run --project src/MudBlazor.Docs.Server/MudBlazor.Docs.Server.csproj

# Test viewer
dotnet run --project src/MudBlazor.UnitTests.Viewer/MudBlazor.UnitTests.Viewer.csproj
```

## Project Structure

```
src/
├── MudBlazor/                         # Core library
│   ├── Components/                    # Blazor components
│   ├── Styles/                        # SCSS files
│   └── TScripts/                      # JavaScript interop
├── MudBlazor.Docs/                    # Documentation site
├── MudBlazor.Docs.Compiler/           # Auto-generates docs
├── MudBlazor.Docs.WasmHost/           # Docs preview host
├── MudBlazor.UnitTests/               # bUnit tests
├── MudBlazor.UnitTests.Viewer/        # Visual test runner
├── MudBlazor.Analyzers/               # Roslyn analyzers
└── MudBlazor.SourceGenerator/         # Source generators
```

**Key config files:**

- `src/.editorconfig` - Code style rules
- `src/Directory.Build.props` - MSBuild properties (warnings are errors)
- `.github/workflows/build-test-mudblazor.yml` - CI/CD

## Component Rules

- Parameters are auto-properties only; no logic in getters/setters. Use ParameterState registration and change handlers.
- Do not overwrite component parameters; use the backing `ParameterState<T>` (`.Value`/`SetValueAsync`) for updates.
- Never set other component parameters via `@ref` (BL0005). Use declarative binding.
- RTL support: `[CascadingParameter] public bool RightToLeft { get; set; }` when layout depends on direction.
- Use `CssBuilder` for classes/styles and CSS variables (no hard-coded colors).
- Add XML `<summary>` for all public properties and use the file header template from `src/.editorconfig`.
- Components with logic require bUnit tests and a docs page: `src/MudBlazor.Docs/Pages/Components/<ComponentName>.razor`.

## Docs Rules

- Order examples from simple to complex.
- Collapse examples longer than 15 lines by default.
- Docs examples are exercised by generated tests; keep them rendering without exceptions.

## Testing Rules

- bUnit: never cache `Find()`/`FindAll()` results; re-query after interactions.
- bUnit: always use `InvokeAsync()` for parameter changes or method calls.
- Keep tests isolated for parallel execution; rework tests to run in parallel instead of using `[NonParallelizable]` when possible.
- If a test must be `[NonParallelizable]`, restore any global state in teardown.
- Prefer `TimeProvider`/`FakeTimeProvider` over `Task.Delay`.
- Prefer async bUnit interactions (`ClickAsync`, `ChangeAsync`, `BlurAsync`, `InputAsync`) over sync methods.
- Test logic, not full HTML snapshots; use focused assertions.
- Test components live in `src/MudBlazor.UnitTests.Viewer/TestComponents/<ComponentName>/`.
- Tests live in `src/MudBlazor.UnitTests/Components/<ComponentName>Tests.cs`.
- Test naming: no `Test`/`Async` suffixes, no `Test_` in the middle, no trailing underscores.

## Code Style and Analyzers

- Follow `src/.editorconfig` (indentation, using placement, file headers).
- Treat warnings as errors; fix new warnings instead of suppressing them.
- CS4014: no unobserved async discards (`_ = SomeAsync()` is an error).
- BL0007: component parameters should be auto-properties.
