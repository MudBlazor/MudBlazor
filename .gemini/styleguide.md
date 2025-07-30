# Comprehensive Coding Standards and Style Guide

This document outlines the coding standards, best practices, and contribution guidelines for our development team. These rules are organized by severity level and are designed to ensure code quality, security, maintainability, and consistency across all projects.

## Table of Contents

- [General Principles](#general-principles)
- [JavaScript/TypeScript Standards](#javascripttypescript-standards)
- [C# Standards](#c-standards)
- [CSS Standards](#css-standards)
- [Blazor Component Guidelines](#blazor-component-guidelines)
- [Testing Requirements](#testing-requirements)
- [Pull Request Guidelines](#pull-request-guidelines)
- [Project Structure](#project-structure)

## General Principles

### Code Quality Fundamentals
- Write code that is readable, maintainable, and self-documenting
- Follow the principle of least surprise - code should behave as expected
- Prefer composition over inheritance
- Keep functions and methods focused on a single responsibility
- Use meaningful names for variables, functions, and classes
- Include comprehensive documentation and comments where necessary

### Security First
- Never hard-code credentials, API keys, or sensitive information
- Always validate user input and sanitize data
- Use secure cryptographic algorithms and practices
- Implement proper error handling without exposing sensitive information
- Follow the principle of least privilege in access controls

## JavaScript/TypeScript Standards

### Blocker Issues (Must Fix Immediately)

#### Control Flow and Logic
- **S128**: End `switch` cases with an unconditional `break` statement
- **S1314**: Ensure `for` loops have a condition that will eventually be met to prevent infinite loops
- **S1527**: Avoid nesting `switch` statements
- **S2187**: Remove assignments to variables that are never used (dead stores)
- **S2970**: Avoid unnecessary boolean expressions

#### Security Requirements
- **S2068**: Do not hard-code credentials. Specifically, avoid using the terms: `password`, `pwd`, `passwd`
- **S2076**: Do not use `document.write` or `innerHTML` with user-controlled input (XSS prevention)
- **S2083**: Do not create cookies without the `secure` and `HttpOnly` flags
- **S6302**: Do not use hard-coded secrets in code
- **S6418**: Secrets (API keys, tokens, credentials) should not be guessable
- **S6265**: Do not use hard-coded IP addresses for security checks

#### Data Safety
- **S1219**: Do not update a collection while iterating over it
- **S2189**: When catching and rethrowing an exception, preserve the original exception
- **S2699**: Do not use `NaN` in direct comparisons; use `isNaN()` instead
- **S2703**: Ensure server ports are positive numbers
- **S2755**: Always check the return value of `read` and `receive` methods

#### Cryptography and Security
- **S5131**: Do not use insecure cryptographic algorithms like DES
- **S5146**: Do not use weak RSA padding schemes like PKCS1
- **S5147**: Do not use insecure key-stretching algorithms
- **S5334**: Do not use weak pseudo-random number generators like `Math.random()`
- **S6105**: Do not use empty passwords for cryptographic operations

#### Web Security
- **S3516**: Do not use APIs that are known to be vulnerable
- **S3649**: Do not disable server-side certificate validation
- **S5696**: Do not use `postMessage` with a wildcard `*` as the target origin
- **S6096**: Ensure that JSON Web Tokens (JWTs) are validated before use
- **S6268**: Do not use components with known vulnerabilities
- **S6270**: Do not allow unrestricted file uploads (remote code execution risk)
- **S6333**: Do not use insecure templating engines vulnerable to XSS

#### Regular Expressions
- **S3796**: Regular expressions should not be subject to Denial of Service (DoS) attacks
- **S6329**: Ensure regular expressions are resistant to ReDoS attacks

### Critical Issues (High Priority)

#### Code Structure
- **S888**: Remove dead code paths
- **S930**: `return`, `throw`, `continue`, and `break` statements should not be followed by other statements
- **S1143**: Avoid variable shadowing
- **S1186**: Remove empty methods and functions
- **S1523**: `return` statements should not be duplicated in `if/else if` chains
- **S2004**: Do not have more than 4 `return` statements in a function

#### Security and Best Practices
- **S1994**: Using `eval` is a security risk
- **S2245**: Do not use the same value on both sides of a binary operator
- **S2310**: `Promises` should be handled appropriately
- **S2430**: Do not use insecure SSL/TLS protocols
- **S2598**: Regular expression patterns should not be vulnerable to injection attacks
- **S2631**: Do not use HTTP for sensitive data; use HTTPS instead
- **S2819**: Do not use insecure hashing algorithms like MD5 or SHA-1

#### Performance and Logic
- **S2871**: Do not create and start threads in a loop
- **S3504**: Do not use `alert`, `confirm`, or `prompt` in server-side code
- **S3735**: `for` loop counters should not be modified within the loop body
- **S3776**: Cognitive Complexity of functions should not be higher than 15
- **S3972**: Do not perform database queries in a loop

#### JavaScript-Specific
- **S3785**: `String.prototype.split()` should not be used with lookbehind assertions
- **S3812**: Do not use non-cryptographically secure random number generators for security
- **S3834**: Using `this` outside of a class constructor or method can have unintended consequences
- **S3854**: Do not use `delete` on variables; use it only on object properties
- **S4123**: Do not use `arguments.callee` and `arguments.caller`
- **S4125**: Do not define functions in a loop
- **S4275**: `async` functions should contain `await` expressions or return a `Promise`
- **S4423**: Do not modify the query string of a URL directly
- **S4426**: Do not use `__proto__` property
- **S4524**: Do not use `Function` constructor`
- **S5542**: Do not use `with` statement

#### Security Vulnerabilities
- **S4502**: Do not use weak SSL/TLS protocols
- **S4790**: Do not use regular expressions vulnerable to ReDoS
- **S4830**: Do not use weak key-exchange mechanisms
- **S5042**: Do not use insecure randomness sources
- **S5332**: Do not perform redirects to user-controlled URLs without validation
- **S5443**: Do not use insecure XML parsers
- **S5527**: Do not use `this` in a static context to call a non-static method
- **S5547**: Server-side code should not be vulnerable to path traversal attacks
- **S5659**: Do not use `Buffer` constructor without sanitizing input
- **S5852**: Do not use insecure pseudo-random number generators
- **S5856**: Do not use `eval` with expressions from tamperable sources
- **S5876**: Do not disable certificate validation for HTTPS connections
- **S6079**: Do not use `child_process` with unsanitized user input
- **S6249**: Do not use hard-coded credentials
- **S6281**: Do not use insecure template engines
- **S6317**: Do not use `new Function()` with untrusted strings

#### Framework-Specific
- **S6958**: React components should not be vulnerable to XSS attacks
- **S7134**: Module dependencies should not form a cycle
- **S7197**: Avoid oversized modules to maintain clean architecture

### Major Issues (Should Fix)

#### Function Design
- **S107**: Functions should have no more than 7 parameters
- **S108**: Nested blocks of code should not be empty
- **S1119**: `if/else if` chains should not have duplicated conditions
- **S1121**: Do not assign the result of a `new` expression to a variable that is immediately returned

#### Code Maintenance
- **S1134**: `TODO` and `FIXME` tags should be handled
- **S1439**: `switch` statements should have a `default` case
- **S1479**: `switch` statements should have no more than 30 `case` clauses
- **S1854**: Remove unnecessary assignments to variables

#### Security Configuration
- **S2077**: Do not use insecure URLs
- **S2234**: `true` and `false` should not be used as strings
- **S5144**: Use `HttpOnly` flag for session cookies
- **S5691**: Do not use `localStorage` or `sessionStorage` for sensitive information
- **S5693**: Limit file upload size to 8MB and standard request size to 2MB

#### Documentation
- **S2999**: Use JSDoc comments for functions, methods, and classes
- **S3358**: `if` statements should not be nested too deeply

### Minor Issues (Style and Consistency)

#### Naming Conventions
- **S101**: Function names should match PascalCase format: `^[A-Z][a-zA-Z0-9]*$`

#### Code Cleanup
- **S1125**: Remove unnecessary boolean literals
- **S1135**: Remove commented-out code
- **S1199**: `for` loop update clauses should be correct
- **S1264**: `throw` statements should not be nested in `finally` blocks
- **S1472**: `switch` statements should not have too many `case` clauses
- **S1874**: Use `===` and `!==` instead of `==` and `!=`
- **S5883**: Use secure defaults for `Cross-Origin-Resource-Policy` headers

### Info Issues (Documentation and Cleanup)
- **S1135**: Remove or update commented-out code blocks

## C# Standards

### Blocker Issues (Must Fix Immediately)

#### Null Safety
- When a parameter is nullable, its value must be checked for `null` before being used
- Avoid instantiating a new `Random` object for each use; reuse a single instance

#### Security
- Do not hard-code credentials. Avoid using: `password`, `passwd`, `pwd`, or `passphrase`
- Do not use weak cryptographic algorithms
- Do not hard-code certificates or other credentials in code
- Code should not be vulnerable to cross-site scripting (XSS) attacks
- Code should not be vulnerable to SQL injection
- Cookies should be created with the `secure` and `HttpOnly` flags
- Code should not be vulnerable to LDAP injection
- Do not disable server-side certificate validation
- Do not use insecure cryptographic algorithms like DES
- Do not use insecure protocols that accept self-signed certificates
- Do not use weak RSA padding schemes like PKCS1
- Do not use insecure key-stretching algorithms
- Do not use weak pseudo-random number generators
- Ensure that JSON Web Tokens (JWTs) are properly validated before use
- Secrets (like API keys or tokens) should not be guessable

#### Threading and Concurrency
- When using `lock`, it should be on a `private readonly` object
- `Thread.Resume` should not be used
- `Thread.Sleep` should not be used in a `lock`
- `Interlocked.Exchange` should be used to change `ThreadStatic` fields atomically

#### Memory and Resource Management
- Remove assignments to variables that are never used (dead stores)
- Return values from `Stream.Read` and related methods should be checked
- `SafeHandle.ReleaseHandle` should not be called from constructors
- Avoid making calls to `GC.Collect`

#### Data Integrity
- `Equals` methods on value types should be overridden
- A `DataTable` or `DataSet` should not be serialized with untrusted data
- Dynamic binding should not be used with user-controlled strings
- Variables should not be compared to themselves
- `const` and `static readonly` fields should not be changed

#### Async/Await
- Awaiting a `Task` without a timeout can lead to deadlocks

#### System Security
- The `System.Security.Permissions.SecurityAction.RequestMinimum` permission should not be used
- Native methods should not be used
- Non-serializable types should not be used in `Session` state

#### Code Structure
- Use a `while` loop instead of a `for` loop that has no update statement
- `if`/`else if` chains should not have gratuitous `else` clauses
- Members should not be initialized to their default values
- `ISerializable` should be implemented correctly

### Critical Issues (High Priority)

#### Method Design
- Methods should not have identical implementations
- Remove empty methods
- Methods should not be empty
- `async` methods should not be `void`
- Property setters should not be empty

#### Exception Handling
- Exception-handling clauses should not be empty
- `[Serializable]` classes should have a constructor that takes `SerializationInfo` and `StreamingContext`

#### Interface Implementation
- `ICloneable.Clone` should be implemented correctly
- The `IDisposable` interface should be implemented correctly
- `IDisposable` objects should be disposed before all references are out of scope
- Constructor-injected instances should be stored in fields

#### Threading
- Avoid using `Thread.Abort` or `Thread.Suspend`
- Do not use `[DllImport]` on a type that is not a static class

#### Type Design
- Do not use the same value on both sides of a binary comparison
- `GetHashCode` should not be overridden on mutable types
- Overriding `Equals` on a type that does not implement `IEquatable<T>` can be error-prone
- A `[Flags]` enum should not have a member with the value zero

#### Performance
- Cognitive Complexity of functions should not be higher than 15
- Do not perform database queries in a loop

#### Async Patterns
- Do not use `Task.Factory.StartNew` with an `async` lambda
- Do not use `Task.Result` or `Task.Wait()` on a `Task` that is not completed

#### Security
- Do not use `System.Reflection.Assembly.Load` with a byte array
- Avoid using insecure protocols like SSL/TLS
- Regular expressions should not be vulnerable to Denial of Service (DoS) attacks
- Do not use weak random number generators
- Do not allow redirects to user-controlled URLs without validation
- Do not use insecure XML parsers

### Major Issues (Should Fix)

#### Code Structure
- Avoid duplicate `if` statements
- Methods should not have more than 7 parameters
- Empty statements should be removed
- Classes should not have more than 5 levels of inheritance
- Avoid unnecessary `continue` statements
- Finalizers should not be empty

#### Code Maintenance
- `TODO` and `FIXME` comments should be resolved
- Avoid empty `catch` blocks
- Avoid duplicate conditions in `if`/`else if` chains
- `switch` statements should have no more than 30 `case` clauses
- Avoid `goto` statements
- Boolean expressions should not be nested

#### Object-Oriented Design
- Do not call `virtual` methods in constructors
- `switch` statements should have a `default` case
- Avoid nested `if` statements
- `async` methods should have "Async" as a suffix
- Avoid empty interfaces
- Avoid empty `finally` blocks

#### Security
- The `lock` statement should be used to protect shared resources
- The `lock` keyword should be used correctly
- Limit file uploads to a maximum of 8.38MB
- Avoid logging sensitive information

### Minor Issues (Style and Consistency)

#### Naming Conventions
- Class, struct, enum, and interface names should comply with standard naming conventions
- Generic type parameters should be prefixed with "T"
- `[Flags]` enums should be named with a plural
- Logger fields should be `private static readonly`

#### Code Quality
- URIs should not be hard-coded
- Do not declare read-only fields that can be converted to `const`
- Remove parameterless constructors from `structs`
- Do not use `virtual` on `sealed` members
- Boolean literals should not be redundant
- Remove duplicate string literals when they exceed a threshold of 3
- Avoid obsolete `using` directives
- Avoid empty `switch` statements
- Do not use `throw` in a `finally` block
- Avoid unnecessary `public` members in `sealed` classes
- Avoid empty `static` constructors
- Use `nameof()` instead of hard-coded names

### Info Issues (Documentation and Cleanup)
- Deprecated code should be removed
- Commented-out code should be removed

## CSS Standards

### Blocker Issues (Must Fix Immediately)
- Avoid using unknown or misspelled HTML element type selectors
- Use valid and standard CSS units (e.g., `px`, `%`, `em`, `rem`)
- Do not use CSS properties that are unknown or misspelled
- Avoid using pseudo-class selectors that are not standard or are misspelled
- Ensure that strings within selectors are enclosed in single or double quotes

### Critical Issues (High Priority)
- Media feature names used in `@media` queries must be valid and correctly spelled
- Avoid using a shorthand property after a corresponding longhand property within the same rule
- Do not use font family names that are not defined or recognized

### Major Issues (Should Fix)
- Remove commented-out code
- Do not use the `!important` keyword, as it disrupts the natural cascade of styles
- Hex colors should be written in lowercase for consistency
- Avoid using more than three universal selectors (`*`) in a selector list
- Avoid using duplicate selectors within the same stylesheet
- When a property and its value are a fallback, place it before the modern property
- Do not use vendor prefixes for properties that are now standard
- Avoid using unknown or misspelled pseudo-class selectors
- Do not use unknown or non-standard pseudo-element selectors
- Colors should not be specified by their name; use hex, RGB, or HSL values instead
- Avoid using unknown or non-standard at-rules
- Duplicate properties within the same rule should be removed
- Empty comment blocks should be removed

### Minor Issues (Style and Consistency)
- Selectors for IDs should not be overqualified by including a type selector
- Remove commented-out code blocks

## Blazor Component Guidelines

### Parameter Management (Critical)

#### The ParameterState Pattern
**NEVER put logic in parameter getters/setters!** Use the ParameterState framework instead.

**Bad Example:**
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
        // Logic here causes problems!
        _ = UpdateHeight(); // Unobserved async discard!
        _ = ExpandedChanged.InvokeAsync(_expanded); // Dangerous!
    }
}
```

**Good Example:**
```csharp
private readonly ParameterState<bool> _expandedState;

[Parameter]
public bool Expanded { get; set; }

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
        await UpdateHeightAsync(); // Properly awaited!
        _updateHeight = true;
    }
    else if (_expandedState.Value)
    {
        _state = CollapseState.Entered;
    }
    await ExpandedChanged.InvokeAsync(_expandedState.Value); // Properly awaited!
}
```

### Avoid Parameter Overwriting

**Bad Example:**
```csharp
private Task ToggleAsync()
{
    Expanded = !Expanded; // Don't overwrite parameters!
    return ExpandedChanged.InvokeAsync(Expanded);
}
```

**Good Example:**
```csharp
private Task ToggleAsync()
{
    return _expandedState.SetValueAsync(!_expandedState.Value);
}
```

### No External Parameter Assignment

**Bad Example:**
```razor
<CalendarComponent @ref="@_calendar" />
<button @onclick="Update">Update</button>

@code
{
    private CalendarComponent _calendarRef = null!;

    private void Update()
    {
        _calendarRef.ShowOnlyOneCalendar = true; // BL0005 warning!
    }
}
```

**Good Example:**
```razor
<CalendarComponent ShowOnlyOneCalendar="@_showOnlyOne" />
<button @onclick="Update">Update</button>

@code
{
    private bool _showOnlyOne;
    
    private void Update()
    {
        _showOnlyOne = true; // Declarative approach
    }
}
```

### Component Design Requirements

#### RTL Support
- All components must support Right-to-Left (RTL) layouts
- Include `[CascadingParameter] public bool RightToLeft { get; set; }` when necessary
- Apply RTL styles at the component level

#### Documentation and Testing
- Add summary comments for every public property using XML documentation
- Use `CssBuilder` for classes and styles
- Add comprehensive unit tests for any component containing logic
- CSS styling alone requires no testing

#### CSS Variables
- Use CSS variables instead of hard-coding colors or other values
- Follow the established design system patterns

## Testing Requirements

### Unit Testing Principles

#### What Must Be Tested
- All non-trivial C# logic in components
- Two-way bindable properties and their behavior
- Event handling and callbacks
- Component state changes and their effects
- Parameter validation and edge cases

#### What Doesn't Need Testing
- Complete rendered HTML output
- Visual appearance of components
- Simple CSS styling without logic

### Writing bUnit Tests

#### Best Practices
```csharp
// Correct approach - don't save HTML elements in variables
var comp = ctx.RenderComponent<MudTextField<string>>();
comp.Find("input").Change("Garfield");
comp.Find("input").Blur();
comp.FindComponent<MudTextField<string>>().Instance.Value.Should().NotBeNullOrEmpty();
```

```csharp
// Wrong approach - HTML elements become stale after interaction
var comp = ctx.RenderComponent<MudTextField<string>>();
var textField = comp.Find("input"); // Don't do this!
textField.Change("Garfield");
textField.Blur(); // This will fail - element is stale
```

#### Component Interaction
```csharp
// Always use InvokeAsync for component parameter changes
var comp = ctx.RenderComponent<MudTextField<string>>();
var textField = comp.FindComponent<MudTextField<string>>().Instance;

// Wrong
textField.Value = "Garfield";

// Correct
await comp.InvokeAsync(() => textField.Value = "I love dogs");
```

### Test Structure
- Create test components in MudBlazor.UnitTests.Viewer
- Write corresponding tests in MudBlazor.UnitTests
- Assert initial state correctness
- Test parameter changes and their effects
- Test user interactions and event handling
- Verify proper EventCallback invocations

## Pull Request Guidelines

### PR Requirements

#### Content Standards
- **Single Topic**: Each PR must address only one feature, bug fix, or improvement
- **Target Branch**: Always target the `dev` branch
- **Testing**: All logic changes must include corresponding unit tests
- **Documentation**: Include documentation for new features or API changes

#### PR Title Format
```
<component name>: <short description in imperative> (<linked issue>)
```

**Example:**
```
DateRangePicker: Fix initializing DateRange with null values (#1997)
```

#### Description Requirements
- Link related issues using `Fixes #<issue>` for bugs or `Closes #<issue>` for features
- Include screenshots/videos for visual changes
- Describe what was changed and why
- List any breaking changes

#### Technical Requirements
- All tests must pass (automated CI checks)
- Code must be properly formatted
- No unnecessary refactoring
- Build successfully with no warnings
- Maintain backward compatibility unless explicitly breaking

### Branch Management
- Work on descriptive feature branches: `feature/my-new-feature` or `fix/my-bug-fix`
- Keep branches up to date by merging `dev` (don't rebase)
- Use draft PRs for work in progress

### New Component Requirements
- Must support RTL layouts
- Include comprehensive unit tests
- Use CSS variables for styling
- Add documentation page with examples
- Examples over 15 lines should be collapsible
- Include XML summary comments for all public properties

## Project Structure

### Important Directories
- `src/MudBlazor/`: Core component library
- `src/MudBlazor.Docs/`: Documentation and examples
- `src/MudBlazor.UnitTests/`: bUnit test suite
- `src/MudBlazor.UnitTests.Viewer/`: Visual test runner

### Key Files
- **Components**: `src/MudBlazor/Components/` (.razor, .razor.cs)
- **Styles**: `src/MudBlazor/Styles/components/` (.scss)
- **Enums**: `src/MudBlazor/Enums/`
- **Tests**: `src/MudBlazor.UnitTests/Components/`
- **Test Components**: `src/MudBlazor.UnitTests.Viewer/TestComponents/`

### Development Workflow
1. Fork the repository and clone locally
2. Create a descriptive feature branch
3. Make changes and test locally using MudBlazor.Docs.Server
4. Write unit tests for any logic changes
5. Run the full test suite locally
6. Create PR with proper title and description
7. Address review feedback and CI failures
8. Merge when approved and all checks pass

## Continuous Integration

### Automated Checks
- **Build Verification**: Project must compile successfully
- **Test Suite**: All unit tests must pass
- **Code Coverage**: Maintain or improve coverage metrics
- **Code Quality**: Static analysis and linting checks
- **Security Scanning**: Vulnerability detection

### Local Development
- Run tests locally before pushing: `dotnet test`
- Use MudBlazor.Docs.Server for development and testing
- Verify changes in MudBlazor.UnitTests.Viewer when applicable
- Format code according to .NET standards

---

**Remember**: These standards exist to ensure code quality, security, and maintainability. When in doubt, err on the side of caution and ask for clarification. All contributors are expected to follow these guidelines to maintain the high quality of our codebase.
