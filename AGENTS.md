# AGENTS.md - AI Coding Agent Guide for MudBlazor

## What is MudBlazor?

MudBlazor is a comprehensive, production-ready Material Design component library for Blazor applications. This is a large-scale .NET project with:
- **UI Framework:** Blazor (both Server and WebAssembly)
- **Styling:** SCSS (compiled to CSS)
- **JavaScript:** Minimal JS interop files in TScripts/
- **Testing:** bUnit for component testing
- **Size:** ~450 components, ~3,700+ unit tests, extensive documentation

The project follows Material Design guidelines and provides a complete set of UI components for building modern web applications with Blazor.

## Prerequisites

- **.NET 10.0 SDK** (version 10.0.100 or later) - Required for building
- Check your version: `dotnet --version`
- The solution targets .NET 8.0, .NET 9.0, and .NET 10.0

## Core Commands and Timings

### Build and Test (CRITICAL: Use Targeted Commands)

**IMPORTANT:** Only build, clean, and format the specific projects/files affected by your changes. Running commands on the entire solution is too slow.

#### Determining Target Projects

Based on what you're changing, target these specific projects:

- **Component changes** (`.razor`, `.razor.cs` in `MudBlazor/Components/`):
  - Build: `src/MudBlazor/MudBlazor.csproj`
  - Test: `src/MudBlazor.UnitTests/MudBlazor.UnitTests.csproj`
  
- **Documentation changes** (files in `MudBlazor.Docs/`):
  - Build: `src/MudBlazor.Docs.Compiler/MudBlazor.Docs.Compiler.csproj` and `src/MudBlazor.Docs/MudBlazor.Docs.csproj`
  - Test: `src/MudBlazor.UnitTests.Docs/MudBlazor.UnitTests.Docs.csproj` (if docs examples changed)

- **Analyzer/Source Generator changes**:
  - Build: `src/MudBlazor.Analyzers/MudBlazor.Analyzers.csproj` or `src/MudBlazor.SourceGenerator/MudBlazor.SourceGenerator.csproj`

- **Test-only changes** (files in `MudBlazor.UnitTests/`):
  - Build: `src/MudBlazor.UnitTests/MudBlazor.UnitTests.csproj`
  - Test: `src/MudBlazor.UnitTests/MudBlazor.UnitTests.csproj`

#### 1. Clean (when needed)

**Target specific project:**
```bash
# For component changes
dotnet clean src/MudBlazor/MudBlazor.csproj

# For test changes
dotnet clean src/MudBlazor.UnitTests/MudBlazor.UnitTests.csproj
```
- Use when: Build failures occur or unexplained issues
- No warnings or errors expected

#### 2. Build

**Target specific project:**
```bash
# For component changes
dotnet build src/MudBlazor/MudBlazor.csproj -c Release --nologo

# For documentation changes
dotnet build src/MudBlazor.Docs.Compiler/MudBlazor.Docs.Compiler.csproj -c Release --nologo

# For test changes
dotnet build src/MudBlazor.UnitTests/MudBlazor.UnitTests.csproj -c Release --nologo
```
- Expected output: "Build succeeded" with 0 warnings, 0 errors
- JavaScript files are compiled when building `MudBlazor.csproj`
- SCSS is compiled to CSS automatically when building `MudBlazor.csproj`

#### 3. Test

**Target specific test project or test filter:**
```bash
# Run tests for specific component
dotnet test src/MudBlazor.UnitTests/MudBlazor.UnitTests.csproj --filter "FullyQualifiedName~MudButton" --no-build -c Release --nologo

# Run tests in specific test file
dotnet test src/MudBlazor.UnitTests/MudBlazor.UnitTests.csproj --filter "FullyQualifiedName~MudButtonTests" --no-build -c Release --nologo
```
- **ALWAYS use `--no-build`** to avoid rebuilding
- Use test filters to run only relevant tests

### Formatting (REQUIRED)

**Target specific files or folders:**
```bash
# Format only changed files in a specific directory
dotnet format src/MudBlazor/MudBlazor.csproj --include Components/Button/

# Format specific changed files
dotnet format src/MudBlazor/MudBlazor.csproj --include Components/Button/MudButton.razor.cs

# Format all files in a project (if many files changed)
dotnet format src/MudBlazor/MudBlazor.csproj

# Format test files
dotnet format src/MudBlazor.UnitTests/MudBlazor.UnitTests.csproj --include Components/ButtonTests.cs
```
- MUST run before finalizing changes
- CI will fail if code is not properly formatted
- **Use `--include` parameter to format only the files you changed**

### Running Docs Locally

```bash
dotnet run --project src/MudBlazor.Docs.Server/MudBlazor.Docs.Server.csproj
```
- Launches at https://localhost:5001 (or http://localhost:5000)
- Best for debugging visual changes and testing components interactively

### Other Commands

```bash
# Run test viewer (for visual debugging of tests)
dotnet run --project src/MudBlazor.UnitTests.Viewer/MudBlazor.UnitTests.Viewer.csproj

# Pack for local testing
dotnet pack src/MudBlazor/MudBlazor.csproj -c Release -o ./LocalNuGet -p:Version=8.0.0-custom
```

### Troubleshooting

**If build fails:**
1. Run `dotnet clean <project.csproj>` on the specific project first (e.g., `src/MudBlazor/MudBlazor.csproj`)
2. Check that .NET 10.0 SDK is installed: `dotnet --version`
3. Ensure you're in the repository root directory
4. Check for file permission issues

**If tests fail:**
1. Ensure build completed successfully first
2. Use `--no-build` flag to avoid rebuild
3. Use `--filter` to run only the failing tests for faster iteration
4. Check that you haven't broken existing tests with your changes
5. Review test output for specific failure reasons

**If CI formatting check fails:**
1. Run `dotnet format <project.csproj> --include <path/to/changed/files>` to auto-fix formatting issues
2. Common issues: blank lines after attributes, missing UTF-8 BOM, incorrect indentation
3. Example: `dotnet format src/MudBlazor/MudBlazor.csproj --include Components/Button/`

## Project Structure

### Root Directory Layout
```
/home/runner/work/MudBlazor/MudBlazor/
├── src/                      # All source code
├── content/                  # Images and media assets
├── tools/                    # PowerShell scripts for icon/CSS generation
├── .github/                  # GitHub Actions workflows and config
│   └── workflows/
│       └── build-test-mudblazor.yml  # Main CI workflow
├── CONTRIBUTING.md           # Detailed contribution guidelines
├── README.md                 # Project overview and quick start
├── AGENTS.md                 # This file - AI agent guide
└── src/MudBlazor.slnx        # Main solution file
```

### Source Directory Structure
```
src/
├── .editorconfig                      # C#/Razor code style (Roslyn defaults + MudBlazor overrides)
├── Directory.Build.props              # MSBuild properties
├── Directory.Build.targets            # MSBuild targets
├── MudBlazor/                         # Core library
│   ├── Components/                    # All Blazor components (.razor, .razor.cs)
│   ├── Styles/                        # SCSS files
│   │   ├── components/                # Component-specific styles
│   │   ├── abstracts/                 # SCSS variables, mixins
│   │   ├── utilities/                 # Utility classes
│   │   └── MudBlazor.scss             # Main SCSS entry point
│   ├── TScripts/                      # JavaScript interop files (checked by ESLint)
│   ├── Enums/                         # Shared enumerations
│   └── MudBlazor.csproj               # Core project file
├── MudBlazor.Docs/                    # Documentation site components
│   └── Pages/Components/              # Component documentation pages
├── MudBlazor.Docs.Server/             # Server-side docs project (for local dev)
├── MudBlazor.Docs.WasmHost/           # WASM docs project (for local dev)
├── MudBlazor.Docs.Compiler/           # Auto-generates documentation files
├── MudBlazor.UnitTests/               # bUnit tests
│   └── Components/                    # Component test files
├── MudBlazor.UnitTests.Viewer/        # Visual test runner
│   └── TestComponents/                # Test components used by bUnit tests
├── MudBlazor.UnitTests.Docs/          # Auto-generated tests from docs
├── MudBlazor.Analyzers/               # Roslyn analyzers
└── MudBlazor.SourceGenerator/         # Source generators
```

### Important Configuration Files
- **src/.editorconfig** - C# code style rules (Roslyn defaults + MudBlazor team overrides)
  - Instance fields: `_camelCase` with underscore prefix
  - File header template required (copyright notice)
  - CS4014 (unawaited async) set to ERROR
  - BL0007 (parameter auto-properties) set to SUGGESTION
- **src/Directory.Build.props** - Shared MSBuild properties
- **src/Directory.Build.targets** - Shared MSBuild targets
- **.github/workflows/build-test-mudblazor.yml** - CI/CD pipeline (builds, tests, ESLint, coverage)

## Testing Instructions

### Running Tests
- Build the test project first: `dotnet build src/MudBlazor.UnitTests/MudBlazor.UnitTests.csproj -c Release --nologo`
- Always use `--no-build` for tests after a successful build.
- **Always use `--filter` to run only relevant tests** for faster iteration:
```bash
# Run tests for a specific component
dotnet test src/MudBlazor.UnitTests/MudBlazor.UnitTests.csproj --filter "FullyQualifiedName~MudButton" --no-build -c Release --nologo

# Run a specific test method
dotnet test src/MudBlazor.UnitTests/MudBlazor.UnitTests.csproj --filter "FullyQualifiedName~MudButtonTests.MudButton_ClickAsync" --no-build -c Release --nologo

# Run all tests in a test class
dotnet test src/MudBlazor.UnitTests/MudBlazor.UnitTests.csproj --filter "FullyQualifiedName~MudButtonTests" --no-build -c Release --nologo
```

### Writing bUnit Tests

**Critical Rules:**
1. **Never save HTML elements from `Find()` or `FindAll()` in variables** - they become stale after interaction
2. **Always use `InvokeAsync()` when setting component parameters or calling methods**

**GOOD example:**
```csharp
var comp = ctx.RenderComponent<MudTextField<string>>();
comp.Find("input").Change("Garfield");  // Query each time
comp.Find("input").Blur();
comp.FindComponent<MudTextField<string>>().Instance.Value.Should().NotBeNullOrEmpty();
```

**BAD example:**
```csharp
var textField = comp.Find("input");  // DON'T DO THIS
textField.Change("Garfield");
textField.Blur();  // Will fail - element is stale
```

**Why it matters:** HTML elements become stale after any interaction that triggers a re-render.

### Using InvokeAsync

**BAD:**
```csharp
var textField = comp.FindComponent<MudTextField<string>>().Instance;
textField.Value = "Garfield"; // WRONG - not on UI thread
```

**GOOD:**
```csharp
var textField = comp.FindComponent<MudTextField<string>>().Instance;
await comp.InvokeAsync(() => textField.Value = "Garfield");
```

**Why:** bUnit test logic is not running on the Blazor UI-thread.

### Test Organization
- Create test components in `src/MudBlazor.UnitTests.Viewer/TestComponents/<ComponentName>/`
- Write corresponding tests in `src/MudBlazor.UnitTests/Components/<ComponentName>Tests.cs`
- Test logic, not complete HTML output or visual appearance
- Add tests for any component containing C# logic
- Assert initial state correctness
- Test parameter changes and their effects
- Test user interactions (clicks, input, etc.)
- Verify EventCallback invocations

## Critical Blazor Component Patterns

### ParameterState Pattern (REQUIRED)

**NEVER put logic in parameter getters/setters!** Use the ParameterState framework instead. This prevents unobserved async discards and update loops.

**BAD (FORBIDDEN):**
```csharp
private bool _expanded;

[Parameter]
public bool Expanded
{
    get => _expanded;
    set
    {
        if (_expanded == value) return;
        _expanded = value;
        _ = UpdateHeight(); // UNOBSERVED ASYNC DISCARD - FORBIDDEN!
        _ = ExpandedChanged.InvokeAsync(_expanded); // DANGEROUS!
    }
}
```

**GOOD (REQUIRED):**
```csharp
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
        await UpdateHeightAsync(); // Properly awaited
        _updateHeight = true;
    }
    await ExpandedChanged.InvokeAsync(_expandedState.Value); // Properly awaited
}
```

### Never Overwrite Parameters

**BAD:**
```csharp
private Task ToggleAsync()
{
    Expanded = !Expanded; // DON'T OVERWRITE PARAMETERS!
    return ExpandedChanged.InvokeAsync(Expanded);
}
```

**GOOD:**
```csharp
private Task ToggleAsync()
{
    return _expandedState.SetValueAsync(!_expandedState.Value);
}
```

### Never Set External Component Parameters (BL0005 Warning)

**BAD:**
```razor
<CalendarComponent @ref="@_calendarRef" />
@code {
    private CalendarComponent _calendarRef = null!;
    private void Update()
    {
        _calendarRef.ShowOnlyOneCalendar = true; // BL0005 WARNING!
    }
}
```

**GOOD:**
```razor
<CalendarComponent ShowOnlyOneCalendar="@_showOnlyOne" />
@code {
    private bool _showOnlyOne;
    private void Update()
    {
        _showOnlyOne = true; // Declarative approach
    }
}
```

## Component Design Requirements

### Must-Have Features
- **RTL Support:** Use `[CascadingParameter] public bool RightToLeft { get; set; }` when necessary
- **XML Documentation:** Add summary comments for all public properties
- **Unit Tests:** Comprehensive tests for any component containing logic
- **CssBuilder:** Use for classes and styles
- **CSS Variables:** Use for styling (avoid hard-coded colors)

### Documentation Requirements
- Add documentation page in `src/MudBlazor.Docs/Pages/Components/<ComponentName>.razor`
- Include examples ordered from simple to complex
- Collapse examples with more than 15 lines by default
- Add screenshots/videos for visual changes

## PR Guidelines

### Title Format
```
<component name>: <short description in imperative>
```
Example: `DateRangePicker: Fix initializing DateRange with null values`

## Workflow Checkpoints (REQUIRED)

### Before Starting Work
- Identify which project(s) your changes will affect
- Build only the specific project(s) you'll be working on
- Run tests with `--filter` for the component you'll be changing

### After Changes
1. Clean the specific project if needed (e.g., `dotnet clean src/MudBlazor/MudBlazor.csproj`)
2. Format only the files you changed (REQUIRED - use `--include` parameter)
3. Build only the affected project(s)
4. Test only the affected components (use `--filter`)
5. (Optional) Run docs locally with the Docs Server command above

## Development Workflow by Task Type

**For Component Changes:**
1. Locate files:
   - Component code: `src/MudBlazor/Components/<ComponentName>/`
   - Component styles: `src/MudBlazor/Styles/components/_<componentname>.scss`
   - Component tests: `src/MudBlazor.UnitTests/Components/<ComponentName>Tests.cs`
   - Test components: `src/MudBlazor.UnitTests.Viewer/TestComponents/<ComponentName>/`
2. Make your changes (follow ParameterState pattern)
3. Build only the affected project:
   ```bash
   dotnet build src/MudBlazor/MudBlazor.csproj -c Release --nologo
   ```
4. Test only the affected component:
   ```bash
   dotnet test src/MudBlazor.UnitTests/MudBlazor.UnitTests.csproj --filter "FullyQualifiedName~<ComponentName>" --no-build -c Release --nologo
   ```
5. Before finalizing, format only your changed files:
   ```bash
   dotnet format src/MudBlazor/MudBlazor.csproj --include Components/<ComponentName>/
   dotnet format src/MudBlazor.UnitTests/MudBlazor.UnitTests.csproj --include Components/<ComponentName>Tests.cs
   ```
6. Run docs locally to verify (optional)

**For Documentation Changes:**
1. Edit in `src/MudBlazor.Docs/Pages/Components/<ComponentName>.razor`
2. Build the docs compiler to generate files:
   ```bash
   dotnet build src/MudBlazor.Docs.Compiler/MudBlazor.Docs.Compiler.csproj -c Release --nologo
   ```
3. Format your changed documentation files:
   ```bash
   dotnet format src/MudBlazor.Docs/MudBlazor.Docs.csproj --include Pages/Components/<ComponentName>.razor
   ```
4. Preview locally with docs server

**For Test Changes:**
1. Create test component in `src/MudBlazor.UnitTests.Viewer/TestComponents/<ComponentName>/`
2. Write bUnit test in `src/MudBlazor.UnitTests/Components/<ComponentName>Tests.cs`
3. Build only the test project:
   ```bash
   dotnet build src/MudBlazor.UnitTests/MudBlazor.UnitTests.csproj -c Release --nologo
   ```
4. Run only the new/modified tests:
   ```bash
   dotnet test src/MudBlazor.UnitTests/MudBlazor.UnitTests.csproj --filter "FullyQualifiedName~<ComponentName>Tests" --no-build -c Release --nologo
   ```
5. Format your test files:
   ```bash
   dotnet format src/MudBlazor.UnitTests/MudBlazor.UnitTests.csproj --include Components/<ComponentName>Tests.cs
   ```
6. Debug visually if needed with the test viewer command

## Code Style Highlights

### C# Naming Conventions (from .editorconfig)
- **Instance fields:** `_camelCase` with underscore prefix
- **Static fields:** `_camelCase` with underscore prefix
- **Constants:** `PascalCase`
- **Public properties/methods:** `PascalCase`
- **Local variables/parameters:** `camelCase`
- **Async methods:** Add `Async` suffix (e.g., `UpdateHeightAsync`)

### Security Rules (BLOCKERS)
- **Never** hard-code credentials, API keys, or secrets
- **Never** use weak cryptographic algorithms (MD5, SHA-1, DES)
- **Always** validate user input
- **Always** sanitize data before using in HTML
- **Never** use `eval`, `innerHTML` with user input
- **Always** use secure cookies (HttpOnly, Secure flags)

### Code Quality Rules
- Maximum 7 parameters per function
- Maximum cognitive complexity of 15
- Maximum 4 return statements per function
- No dead code or unused methods
- No empty methods
- Async methods must contain `await` or return `Task`
- **No unobserved async discards** (`_ = SomeAsync()` is ERROR per CS4014)

### Blazor-Specific Rules
- Component parameters must be auto-properties (no logic in getter/setter)
- Use ParameterState framework for parameter change handling
- Support RTL layouts when necessary
- Add XML summary comments for all public properties
- Use `CssBuilder` for classes and styles
- Use CSS variables for styling (no hard-coded colors)
- Analyzer BL0007 set to `suggestion` (parameter auto-properties)
- CS4014 set to `error` (unawaited async calls)
- File header template required (copyright notice)

## Common Pitfalls to Avoid

1. **Logic in parameter setters** - Use ParameterState framework instead (see Critical Patterns)
2. **Stale HTML element references in tests** - Always re-query with `Find()` instead of saving elements
3. **Direct parameter assignment on component refs** - Use declarative binding (BL0005 warning)
4. **Missing `InvokeAsync` in tests** - Required for parameter changes in bUnit tests
5. **Breaking existing tests** - Run relevant tests with `--filter` during development
6. **Running solution-wide commands** - Always target specific projects/files to save time
7. **Missing `--no-build` flag** - Always use when running tests after a successful build
8. **Forgetting to run `dotnet format`** - MUST format your changed files before finalizing (use `--include` to target specific files)

## Continuous Integration

The GitHub Actions workflow (`.github/workflows/build-test-mudblazor.yml`) runs:
1. **Build** - Compiles all 15+ projects
2. **Test** - Runs 3,734+ unit tests
3. **ESLint** - Checks JavaScript files in `src/MudBlazor/TScripts/`
4. **Code Coverage** - Publishes to Codecov
5. **Code Quality** - SonarCloud analysis
6. **Security Scanning** - Dependency checks

**Keep changes compatible with all checks.**
