# AGENTS.md - AI Coding Agent Guide for MudBlazor

## Overview

MudBlazor: Material Design component library for Blazor (Server/WebAssembly)
- **Stack:** .NET (8.0/9.0/10.0), SCSS, minimal JS in TScripts/, bUnit tests
- **Scale:** ~450 components, ~3,700+ tests

## Prerequisites

- .NET 10.0 SDK (10.0.100+): `dotnet --version`

## Core Commands

**CRITICAL:** Target specific projects only. Solution-wide commands are too slow.

### Target Projects by Change Type

- **Components:** `src/MudBlazor/MudBlazor.csproj` + `src/MudBlazor.UnitTests/MudBlazor.UnitTests.csproj`
- **Docs:** `src/MudBlazor.Docs.Compiler/MudBlazor.Docs.Compiler.csproj` + `src/MudBlazor.Docs/MudBlazor.Docs.csproj`
- **Analyzers:** `src/MudBlazor.Analyzers/MudBlazor.Analyzers.csproj` or `src/MudBlazor.SourceGenerator/MudBlazor.SourceGenerator.csproj`

### Clean, Build, Test

```bash
# Clean (when build fails)
dotnet clean <project.csproj>

# Build
dotnet build <project.csproj> -c Release --nologo

# Test
dotnet test src/MudBlazor.UnitTests/MudBlazor.UnitTests.csproj --filter "FullyQualifiedName~MudButton" --no-build -c Release --nologo --blame-hang --blame-hang-timeout 60s
```

### Formatting (REQUIRED)

```bash
# Format specific files/directories (use --include)
dotnet format <project.csproj> --include <path/to/changed/files>
```
CI will fail if code is not formatted.

### Other Commands

```bash
# Run docs locally
dotnet run --project src/MudBlazor.Docs.Server/MudBlazor.Docs.Server.csproj

# Run test viewer
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
├── MudBlazor.UnitTests/               # bUnit tests
├── MudBlazor.UnitTests.Viewer/        # Visual test runner
├── MudBlazor.Analyzers/               # Roslyn analyzers
└── MudBlazor.SourceGenerator/         # Source generators
```

**Key config files:**
- `src/.editorconfig` - Code style rules
- `src/Directory.Build.props` - MSBuild properties
- `.github/workflows/build-test-mudblazor.yml` - CI/CD

## bUnit Testing

### Critical Rules
1. Never save `Find()` or `FindAll()` results - elements become stale after re-render
2. Always use `InvokeAsync()` for parameter changes/method calls

```csharp
// GOOD
var comp = ctx.RenderComponent<MudTextField<string>>();
await comp.Find("input").ChangeAsync("Garfield");  // Query each time
await comp.Find("input").BlurAsync();

// BAD
var input = comp.Find("input");  // Becomes stale
await input.ChangeAsync("Garfield");
await input.BlurAsync();  // Will fail
```

### Test Organization
- Test components: `src/MudBlazor.UnitTests.Viewer/TestComponents/<ComponentName>/`
- Tests: `src/MudBlazor.UnitTests/Components/<ComponentName>Tests.cs`

### Test Naming
- Remove `Test`/`Async` suffixes from test method names (e.g., `Toggle_OpenAsync` -> `Toggle_Open`)
- Avoid embedding `Test_` in the middle of names (e.g., `AlertTest_Click` -> `Alert_Click`)
- No trailing underscores or double underscores in test method names (e.g., `BarChart_CanHideSeries_` -> `BarChart_CanHideSeries`)

## Blazor Component Patterns

### ParameterState Pattern (REQUIRED)

Never put logic in parameter getters/setters. Use ParameterState framework.

```csharp
// GOOD
private readonly ParameterState<bool> _expandedState;

[Parameter]
public bool Expanded { get; set; }  // Auto-property only

public MudCollapse()
{
    using var registerScope = CreateRegisterScope();
    _expandedState = registerScope.RegisterParameter<bool>(nameof(Expanded))
        .WithParameter(() => Expanded)
        .WithEventCallback(() => ExpandedChanged)
        .WithChangeHandler(OnExpandedChangedAsync);
}

private async Task OnExpandedChangedAsync()
{
    if (_isRendered)
    {
        _state = _expandedState.Value ? CollapseState.Entering : CollapseState.Exiting;
        await UpdateHeightAsync();
        _updateHeight = true;
    }
    await ExpandedChanged.InvokeAsync(_expandedState.Value);
}

// BAD
private bool _expanded;
[Parameter]
public bool Expanded
{
    get => _expanded;
    set
    {
        if (_expanded == value) return;
        _expanded = value;
        _ = UpdateHeight(); // FORBIDDEN - unobserved async discard
        _ = ExpandedChanged.InvokeAsync(_expanded);
    }
}
```

### Key Rules
- Never overwrite parameters directly
- Never set external component parameters (BL0005)
- Use declarative binding instead

## Component Requirements

- RTL support: `[CascadingParameter] public bool RightToLeft { get; set; }`
- XML documentation for all public properties
- Unit tests for components with logic
- Use `CssBuilder` for classes and styles
- Use CSS variables (no hard-coded colors)
- Documentation page: `src/MudBlazor.Docs/Pages/Components/<ComponentName>.razor`

## Code Style

**Naming:**
- Instance/static fields: `_camelCase`
- Constants/public members: `PascalCase`
- Async methods: `Async` suffix

**Critical Rules:**
- CS4014: No unobserved async discards (`_ = SomeAsync()` is ERROR)
- BL0007: Parameter auto-properties (suggestion)
- File header required (copyright notice)
- Max 7 parameters, complexity ≤15, ≤4 returns per function
