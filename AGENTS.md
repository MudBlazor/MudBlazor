# AGENTS.md - AI Coding Agent Guide for MudBlazor

## What is MudBlazor?

MudBlazor is a comprehensive, production-ready Material Design component library for Blazor applications. This is a large-scale .NET project with:
- **Primary Language:** C# (.NET 8 and .NET 9)
- **UI Framework:** Blazor (both Server and WebAssembly)
- **Styling:** SCSS (compiled to CSS)
- **JavaScript:** Minimal JS interop files in TScripts/
- **Testing:** bUnit for component testing
- **Size:** ~450 components, ~3,700+ unit tests, extensive documentation
- **Target Frameworks:** .NET 8.0 and .NET 9.0

The project follows Material Design guidelines and provides a complete set of UI components for building modern web applications with Blazor.

## Prerequisites

- **.NET 9.0 SDK** (version 9.0.305 or later) - Required for building
- Check your version: `dotnet --version`
- The solution targets both .NET 8.0 and .NET 9.0

## Dev Environment Tips

### Build Commands (CRITICAL TIMINGS)

**ALWAYS follow this exact sequence:**

1. **Clean (when needed):**
```bash
dotnet clean src/MudBlazor.sln
```
- Runs in ~2-3 seconds
- Use when: Build failures occur, switching branches, or unexplained issues
- No warnings or errors expected

2. **Build:**
```bash
dotnet build src/MudBlazor.sln -c Release --nologo
```
- **Duration: ~2-2.5 minutes** (this is NORMAL - do NOT timeout before 150 seconds)
- Builds 15+ projects including:
  - MudBlazor (core library) - targets net8.0 and net9.0
  - MudBlazor.Docs.Compiler - generates 745+ documentation files
  - MudBlazor.UnitTests.Docs.Generator - generates test files
  - Multiple doc hosting projects (Server, Wasm, WasmHost)
  - Analyzers, source generators, and test projects
- Expected output: "Build succeeded" with 0 warnings, 0 errors
- JavaScript files are compiled: wwwroot/MudBlazor.min.js
- SCSS is compiled to CSS automatically
- **IMPORTANT:** The build generates files during compilation - this is expected and not an error

3. **Test:**
```bash
dotnet test src/MudBlazor.UnitTests/MudBlazor.UnitTests.csproj --no-build -c Release --nologo
```
- **Duration: ~1.5 minutes (90 seconds)** - do NOT timeout before 120 seconds
- Runs 3,734+ tests (some skipped performance tests)
- Expected output: "Passed! - Failed: 0, Passed: 3734, Skipped: 10"
- **ALWAYS use `--no-build`** to avoid rebuilding (saves time)
- Tests must pass before submitting PRs

### Running Docs Locally

```bash
dotnet run --project src/MudBlazor.Docs.Server/MudBlazor.Docs.Server.csproj
```
- Launches at https://localhost:5001 (or http://localhost:5000)
- Best for debugging visual changes and testing components interactively

### Build Troubleshooting

**If build fails:**
1. Run `dotnet clean src/MudBlazor.sln` first
2. Check that .NET 9.0 SDK is installed: `dotnet --version`
3. Ensure you're in the repository root directory
4. Check for file permission issues

**If tests fail:**
1. Ensure build completed successfully first
2. Use `--no-build` flag to avoid rebuild
3. Check that you haven't broken existing tests with your changes
4. Review test output for specific failure reasons

**If CI formatting check fails:**
1. Run `dotnet format src/MudBlazor.sln` to auto-fix formatting issues
2. Commit the formatting changes
3. Common issues: blank lines after attributes, missing UTF-8 BOM, incorrect indentation

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
└── src/MudBlazor.sln         # Main solution file
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
- **src/.editorconfig** - C# code style rules (Microsoft Roslyn defaults with MudBlazor team overrides)
  - Instance fields: `_camelCase` with underscore prefix
  - File header template required (copyright notice)
  - CS4014 (unawaited async) set to ERROR
  - BL0007 (parameter auto-properties) set to SUGGESTION
- **src/Directory.Build.props** - Shared MSBuild properties
- **src/Directory.Build.targets** - Shared MSBuild targets
- **.github/workflows/build-test-mudblazor.yml** - CI/CD pipeline (builds, tests, ESLint, coverage)

## Testing Instructions

### Running Tests
```bash
# Build first (required)
dotnet build src/MudBlazor.sln -c Release --nologo

# Run all unit tests (ALWAYS use --no-build to save time)
dotnet test src/MudBlazor.UnitTests/MudBlazor.UnitTests.csproj --no-build -c Release --nologo

# Run specific test
dotnet test src/MudBlazor.UnitTests/MudBlazor.UnitTests.csproj --filter "TestName" --no-build -c Release
```

### Writing bUnit Tests

**Critical Rules:**
1. **Never save HTML elements from `Find()` or `FindAll()` in variables** - they become stale after interaction
2. **Always use `InvokeAsync()` when setting component parameters or calling methods**

**✅ GOOD Example:**
```csharp
var comp = ctx.RenderComponent<MudTextField<string>>();
comp.Find("input").Change("Garfield");  // Query each time
comp.Find("input").Blur();
comp.FindComponent<MudTextField<string>>().Instance.Value.Should().NotBeNullOrEmpty();
```

**❌ BAD Example:**
```csharp
var textField = comp.Find("input");  // DON'T DO THIS
textField.Change("Garfield");
textField.Blur();  // Will fail - element is stale
```

**Why it matters:** HTML elements become stale after any interaction that triggers a re-render.

### Using InvokeAsync

**❌ BAD:**
```csharp
var textField = comp.FindComponent<MudTextField<string>>().Instance;
textField.Value = "Garfield"; // WRONG - not on UI thread
```

**✅ GOOD:**
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

**❌ BAD (FORBIDDEN):**
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

**✅ GOOD (REQUIRED):**
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

**❌ BAD:**
```csharp
private Task ToggleAsync()
{
    Expanded = !Expanded; // DON'T OVERWRITE PARAMETERS!
    return ExpandedChanged.InvokeAsync(Expanded);
}
```

**✅ GOOD:**
```csharp
private Task ToggleAsync()
{
    return _expandedState.SetValueAsync(!_expandedState.Value);
}
```

### Never Set External Component Parameters (BL0005 Warning)

**❌ BAD:**
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

**✅ GOOD:**
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
<component name>: <short description in imperative> (<linked issue>)
```
Example: `DateRangePicker: Fix initializing DateRange with null values (#1997)`

### PR Requirements
- Single topic per PR (one feature/bug fix)
- Target the `dev` branch
- All tests must pass
- Include unit tests for logic changes
- No unnecessary refactoring
- Link related issues using `Fixes #<issue>` (bugs) or `Closes #<issue>` (features)
- Include screenshots/videos for visual changes
- Code must be properly formatted per .editorconfig

### Branch Management
- Use descriptive branches: `feature/my-new-feature` or `fix/my-bug-fix`
- Keep branches up to date by merging `dev` (don't rebase)
- Use draft PRs for work in progress

## Build and Validation Workflow

### Development Workflow by Task Type

**For Component Changes:**
1. Locate files:
   - Component code: `src/MudBlazor/Components/<ComponentName>/`
   - Component styles: `src/MudBlazor/Styles/components/_<componentname>.scss`
   - Component tests: `src/MudBlazor.UnitTests/Components/<ComponentName>Tests.cs`
   - Test components: `src/MudBlazor.UnitTests.Viewer/TestComponents/<ComponentName>/`

2. Make your changes (follow ParameterState pattern)

3. Build and test iteratively:
```bash
dotnet build src/MudBlazor.sln -c Release --nologo
dotnet test src/MudBlazor.UnitTests/MudBlazor.UnitTests.csproj --no-build -c Release --nologo
```

4. Run docs locally to verify (optional):
```bash
dotnet run --project src/MudBlazor.Docs.Server/MudBlazor.Docs.Server.csproj
```

**For Documentation Changes:**
1. Edit in `src/MudBlazor.Docs/Pages/Components/<ComponentName>.razor`
2. Build to generate files (runs MudBlazor.Docs.Compiler):
```bash
dotnet build src/MudBlazor.sln -c Release --nologo
```
3. Preview locally with docs server

**For Test Changes:**
1. Create test component in `src/MudBlazor.UnitTests.Viewer/TestComponents/<ComponentName>/`
2. Write bUnit test in `src/MudBlazor.UnitTests/Components/<ComponentName>Tests.cs`
3. Build and run tests
4. Debug visually if needed:
```bash
dotnet run --project src/MudBlazor.UnitTests.Viewer/MudBlazor.UnitTests.Viewer.csproj
```

### Before Making Changes
```bash
# Always check initial state
dotnet build src/MudBlazor.sln -c Release --nologo
dotnet test src/MudBlazor.UnitTests/MudBlazor.UnitTests.csproj --no-build -c Release --nologo
```

### After Making Changes
```bash
# Clean if needed (switching branches or unexplained issues)
dotnet clean src/MudBlazor.sln

# Build
dotnet build src/MudBlazor.sln -c Release --nologo

# Run tests
dotnet test src/MudBlazor.UnitTests/MudBlazor.UnitTests.csproj --no-build -c Release --nologo
```

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
5. **Breaking existing tests** - Run full test suite before submitting PR
6. **Targeting wrong branch** - Always target `dev`, not `master`
7. **Multiple topics in one PR** - Keep PRs focused on single issue
8. **Build timeouts** - Set timeout to at least 180 seconds for builds, 120 seconds for tests
9. **Missing `--no-build` flag** - Always use when running tests after a successful build

## Continuous Integration

The GitHub Actions workflow (`.github/workflows/build-test-mudblazor.yml`) runs:
1. **Build** - Compiles all 15+ projects
2. **Test** - Runs 3,734+ unit tests
3. **ESLint** - Checks JavaScript files in `src/MudBlazor/TScripts/`
4. **Code Coverage** - Publishes to Codecov
5. **Code Quality** - SonarCloud analysis
6. **Security Scanning** - Dependency checks

**All checks must pass before merging.**

## Additional Resources

- **CONTRIBUTING.md** - Detailed contribution guidelines
- **README.md** - Quick start and installation
- **Documentation Site** - https://mudblazor.com
- **Discord** - https://discord.gg/mudblazor

## Quick Reference

```bash
# Check .NET version
dotnet --version  # Should be 9.0.305 or later

# Full build and test cycle
dotnet clean src/MudBlazor.sln
dotnet build src/MudBlazor.sln -c Release --nologo  # ~2 minutes
dotnet test src/MudBlazor.UnitTests/MudBlazor.UnitTests.csproj --no-build -c Release --nologo  # ~1.5 minutes

# Test specific component
dotnet test src/MudBlazor.UnitTests/MudBlazor.UnitTests.csproj --filter "ButtonTests" --no-build -c Release

# Run docs locally (server mode - best for debugging)
dotnet run --project src/MudBlazor.Docs.Server/MudBlazor.Docs.Server.csproj

# Run test viewer (for visual debugging of tests)
dotnet run --project src/MudBlazor.UnitTests.Viewer/MudBlazor.UnitTests.Viewer.csproj

# Pack for local testing
dotnet pack src/MudBlazor/MudBlazor.csproj -c Release -o ./LocalNuGet -p:Version=8.0.0-custom
```

## Validation Steps Before PR

**ALWAYS run this sequence before creating/updating a PR:**

```bash
# 1. Clean (if switching branches or weird issues)
dotnet clean src/MudBlazor.sln

# 2. Format code (REQUIRED - CI will fail if not formatted)
dotnet format src/MudBlazor.sln --verify-no-changes
# Expected: Exit code 0 with no formatting errors
# If formatting errors are found, run without --verify-no-changes to auto-fix:
# dotnet format src/MudBlazor.sln

# 3. Build
dotnet build src/MudBlazor.sln -c Release --nologo
# Expected: "Build succeeded" with 0 warnings, 0 errors in ~2 minutes

# 4. Test
dotnet test src/MudBlazor.UnitTests/MudBlazor.UnitTests.csproj --no-build -c Release --nologo
# Expected: "Passed! - Failed: 0, Passed: 3734+, Skipped: 10" in ~1.5 minutes

# 5. (Optional) Test docs locally
dotnet run --project src/MudBlazor.Docs.Server/MudBlazor.Docs.Server.csproj
```

---

When in doubt, check the existing code patterns, follow the guidelines in CONTRIBUTING.md, and ask questions on Discord before implementing.
