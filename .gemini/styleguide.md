# Gemini Style Guide for MudBlazor

This document provides guidelines for AI-assisted development on the MudBlazor project. It's derived from `CONTRIBUTING.md` and `.editorconfig` to ensure all contributions are consistent with project standards.

-----

## 📜 General Principles

### **Stability and API Change Awareness**

MudBlazor is widely used in production. Given limited testing resources:

  * **Stability is the highest priority.**
  * **Proactively identify and call out potential breaking changes or API changes in PRs.**
  * **Be conservative with changes to public APIs, component parameters, or behaviors.**
  * **Document any risk of breakage or migration impact in the PR description.**
  * **Favor additive, non-breaking changes.**
  * **If in doubt, ask maintainers for review and highlight the risk.**

This approach protects users from regressions and ensures reliability.

1.  **Clarity and Simplicity**: Code should be easy to read. Avoid unnecessary complexity or refactoring in PRs.
2.  **Consistency**: Adhere to established patterns within the MudBlazor codebase.
3.  **Safety**: Prioritize robust, break-safe code, including comprehensive unit testing for new or modified logic.
4.  **Single Responsibility**: Pull requests should be atomic, focusing on one feature or bug fix.

-----

## 💻 C\# and .razor Conventions

These rules are based on the project's `.editorconfig`.

  * **Formatting**:
      * Use **4 spaces** for indentation in `.cs` and `.razor` files.
      * Use **2 spaces** for indentation in `.json`, `.csproj`, and `.scss` files.
      * Place open braces on a **new line** for all blocks (`csharp_new_line_before_open_brace = all`).
      * Ensure all files end with a **final newline**.
  * **Naming Conventions**:
      * Private instance fields: `_camelCase` (e.g., `_myField`).
      * Public properties and methods: `PascalCase` (e.g., `MyProperty`).
      * Constants: `PascalCase`.
      * Parameters and local variables: `camelCase`.
  * **Style**:
      * Use `var` whenever possible (`csharp_style_var_for_built_in_types`, `csharp_style_var_when_type_is_apparent`, `csharp_style_var_elsewhere` are all `true:suggestion`).
      * Avoid `this.` unless necessary.
      * Prefer block bodies for methods and constructors (`csharp_style_expression_bodied_methods = false`).
      * Sort `using` directives with `System.*` first.
      * Do not add XML documentation comments (`CS1591`) or file headers (`IDE0073`).
  * **Code Structure**:
      * Use the `CssBuilder` for constructing `class` and `style` attributes dynamically.
      * Add a `summary` comment for every public property.

-----

## ⛓️ Parameter Handling: The ParameterState Framework

This framework is critical for preventing bugs and ensuring predictable component behavior.

### **Rule: No Logic in `[Parameter]` Property Setters**

All logic reacting to a parameter change **must** be placed in a change handler and registered using the `ParameterState` framework in the component's constructor.

#### ❌ Don't: Logic in Setter

```csharp
// This is forbidden in MudBlazor.
private bool _expanded;

[Parameter]
public bool Expanded
{
    get => _expanded;
    set
    {
        if (_expanded == value)
            return;
        _expanded = value;
        // BAD: This setter contains logic and unobserved async calls.
        _ = ExpandedChanged.InvokeAsync(_expanded);
    }
}
```

#### ✅ Do: Use `ParameterState`

1.  **Declare** the Parameter as an auto-property and a `ParameterState` field.
2.  **Register** the Parameter in the constructor.
3.  **Move the logic** to an `async` change handler.

<!-- end list -->

```csharp
// The correct MudBlazor pattern.

// 1. Declare
private readonly ParameterState<bool> _expandedState;

[Parameter]
public bool Expanded { get; set; }

[Parameter]
public EventCallback<bool> ExpandedChanged { get; set; }

// 2. Register in constructor
public MyComponent()
{
    using var registerScope = CreateRegisterScope();
    _expandedState = registerScope.RegisterParameter<bool>(nameof(Expanded))
        .WithParameter(() => Expanded)
        .WithEventCallback(() => ExpandedChanged)
        .WithChangeHandler(OnExpandedChangedAsync); // Can be sync or async
}

// 3. Move logic to handler
private async Task OnExpandedChangedAsync()
{
    // Logic that was previously in the setter goes here.
    // The new value is available via _expandedState.Value
    // Note: The framework automatically calls ExpandedChanged.InvokeAsync if you use WithEventCallback.
}
```

### **Rule: Do Not Overwrite Parameters Directly**

Never assign a value directly to a parameter property. Use the `ParameterState` object's `SetValueAsync` method.

#### ❌ Don't: Direct Parameter Assignment

```csharp
private Task ToggleAsync()
{
    // BAD: Overwrites the parameter directly.
    Expanded = !Expanded;
    return ExpandedChanged.InvokeAsync(Expanded);
}
```

#### ✅ Do: Use `SetValueAsync`

```csharp
private Task ToggleAsync()
{
    // GOOD: Uses the ParameterState framework to safely update the value and invoke the EventCallback.
    return _expandedState.SetValueAsync(!_expandedState.Value);
}
```

### **Rule: Do Not Set Another Component's Parameters Programmatically**

Component parameters must only be set declaratively in the Razor markup. Do not use `@ref` to get a component instance and then set its parameters in code.

#### ❌ Don't: Imperative Parameter Setting

```razor
<CalendarComponent @ref="@_calendar" />
<button @onclick="Update">Update</button>

@code {
    private CalendarComponent _calendarRef = null!;

    private void Update()
    {
        // BAD: Causes BL0005 warning and is against Blazor principles.
        _calendarRef.ShowOnlyOneCalendar = true;
    }
}
```

#### ✅ Do: Declarative Parameter Binding

```razor
<CalendarComponent ShowOnlyOneCalendar="@_showOnlyOne" />
<button @onclick="Update">Update</button>

@code {
    private bool _showOnlyOne;

    private void Update()
    {
        // GOOD: Update the bound variable, letting Blazor handle the parameter update.
        _showOnlyOne = true;
    }
}
```

-----

## 🧱 Component Design

  * **New Components**: Must add a documentation page with examples ordered from simple to complex. Collapse examples over 15 lines.
  * **RTL Support**: All components must support Right-To-Left (RTL) rendering. If necessary, cascade the `RightToLeft` parameter and apply conditional styles.
  * **CSS**: Use CSS variables from the MudBlazor theme. Avoid hard-coded colors or sizes.

-----

## 🧪 Unit Testing (bUnit)

  * **Coverage is Mandatory**: All non-trivial C\# logic requires a corresponding bUnit test.
  * **Break-Safety**: When fixing a bug or adding a feature, add a test specifically covering the change to prevent regressions.
  * **Test Structure**:
    1.  Create a test component in `MudBlazor.UnitTests.Viewer`.
    2.  In `MudBlazor.UnitTests`, render the test component using `ctx.RenderComponent<T>()`.
    3.  Assert the initial state.
    4.  Interact with the component (e.g., `comp.Find("button").Click()`) or its parameters.
    5.  Assert the expected outcome (e.g., a class change, a property update).

### **Common Testing Pitfalls to Avoid:**

1.  **Do not store `Find` or `FindAll` results in variables.** The DOM can be re-rendered, making the reference stale. Re-query the element each time.
2.  **Always use `InvokeAsync`** when setting a component's parameters or calling its methods directly from a test to ensure the test logic waits for the component to update.

#### ✅ Correct Test Interaction

```csharp
var comp = ctx.RenderComponent<MudTextField<string>>();
var textFieldInstance = comp.Instance;

// Use InvokeAsync to set a parameter value
await comp.InvokeAsync(() => textFieldInstance.Value = "I love dogs");

// Re-query the element before interacting with it
comp.Find("input").Blur();

// Assert the result
comp.Instance.Value.Should().Be("I love dogs");
```

-----

## ♿ Accessibility

MudBlazor components should be usable by everyone.

  * **Semantic HTML**: Use appropriate HTML elements (e.g., `button`, `a`, `h1`).
  * **ARIA Attributes**: When custom components are necessary, use WAI-ARIA attributes (e.g., `aria-label`, `aria-describedby`) to convey roles, states, and properties to assistive technologies.
  * **Keyboard Navigation**: All interactive components must be fully navigable and operable using only the keyboard. Ensure proper focus management, tab order, and support for standard keyboard interactions.
  * **Color Contrast**: Ensure sufficient color contrast for all text and interactive elements to meet WCAG 2.1 AA standards. Avoid relying solely on color to convey information.
  * **Focus Indicators**: Ensure visible and clear focus indicators for all interactive elements.

-----

## ⚡ Performance Considerations

Optimize for fast and responsive MudBlazor components.

  * **Minimize Re-renders**: Understand Blazor's rendering lifecycle and avoid unnecessary re-renders. Use `@bind:get` and `@bind:set` or `EventCallback` with `ParameterState`.
  * **Virtualization**: For large lists or tables, consider Blazor's built-in virtualization.
  * **Asynchronous Operations**: Always use `async` and `await` for I/O-bound or long-running operations to keep the UI responsive. Avoid blocking the UI thread.
  * **CSS Performance**:
      * Minimize expensive CSS properties that trigger layout or paint (e.g., `box-shadow`, `filter`).
      * Prefer CSS transforms and opacity for animations.
  * **Component Initialization**: Defer complex or resource-intensive initialization logic until it's needed.

-----

## 📝 Documentation & Comments

Clear and consistent documentation is vital.

  * **Public API Documentation**: Any new public APIs (components, parameters, methods, events) must be clearly documented in the public-facing MudBlazor documentation, including examples.
  * **Internal Comments**: Use comments judiciously to explain complex logic, non-obvious choices, or workarounds. Comments should explain *why*, not *what*.
  * **Commit Messages**: Follow a clear and descriptive commit message convention (e.g., Conventional Commits) in addition to the PR title format.

-----

## 🚀 Pull Requests

  * **Target Branch**: `dev`
  * **Title Format**: `<component name>: <short description of changes in imperative> (#issue)`
      * Example: `DateRangePicker: Fix initializing DateRange with null values (#1997)`
  * **Description**:
      * Link to related issues using keywords like `Fixes #123` or `Closes #456`.
      * Include a screenshot or GIF for any visual changes.
  * **Branching**: Use `feature/my-new-feature` or `fix/my-bug-fix`. Keep your branch up-to-date by **merging** from `upstream/dev`, not rebasing.
  * **Checks**: All CI checks (build, test, coverage, quality) must pass before a PR can be merged.