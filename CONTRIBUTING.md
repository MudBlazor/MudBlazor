# ![MudBlazor](content/MudBlazor-GitHub-NoBg.png)

<!-- TOC start (generated with https://github.com/derlin/bitdowntoc) -->

- [](#)
- [Information and Guidelines for Contributors](#information-and-guidelines-for-contributors)
  - [Code of Conduct](#code-of-conduct)
  - [Minimal Prerequisites to Compile from Source](#minimal-prerequisites-to-compile-from-source)
  - [Pull Requests](#pull-requests)
    - [Pull Requests which introduce new components](#pull-requests-which-introduce-new-components)
  - [Project structure and where to find the most important files](#project-structure-and-where-to-find-the-most-important-files)
  - [Coding Dos and Don'ts](#coding-dos-and-donts)
  - [Parameter Registration or Why we can't have Logic in Parameter Setters](#parameter-registration-or-why-we-cant-have-logic-in-parameter-setters)
    - [Example of a bad Parameter definition](#example-of-a-bad-parameter-definition)
    - [Example of a good Parameter definition](#example-of-a-good-parameter-definition)
    - [Can I share change handlers between parameters?](#can-i-share-change-handlers-between-parameters)
    - [What about the bad parameters all over the MudBlazor code base?](#what-about-the-bad-parameters-all-over-the-mudblazor-code-base)
  - [Avoid overwriting parameters in Blazor Components](#avoid-overwriting-parameters-in-blazor-components)
    - [Example of a bad code](#example-of-a-bad-code)
    - [Example of a good code](#example-of-a-good-code)
  - [Blazor Component parameter should not be set outside of its component.](#blazor-component-parameter-should-not-be-set-outside-of-its-component)
    - [Example of a bad code](#example-of-a-bad-code-1)
    - [Example of a good code](#example-of-a-good-code-1)
  - [Unit Testing and Continuous Integration](#unit-testing-and-continuous-integration)
    - [How not to break stuff](#how-not-to-break-stuff)
    - [Make your code break-safe](#make-your-code-break-safe)
    - [How to write a unit test?](#how-to-write-a-unit-test)
    - [How to write a bUnit test](#how-to-write-a-bunit-test)
    - [What are common errors when writing tests?](#what-are-common-errors-when-writing-tests)
      - [Do not save html elements you query via `Find` or `FindAll` in a variable!](#do-not-save-html-elements-you-query-via-find-or-findall-in-a-variable)
      - [Always use InvokeAsync to set parameter values on a component](#always-use-invokeasync-to-set-parameter-values-on-a-component)
    - [What does not need to be tested?](#what-does-not-need-to-be-tested)
    - [What is the MudBlazor.UnitTests.Viewer for?](#what-is-the-mudblazorunittestsviewer-for)
    - [What are the auto-generated tests for?](#what-are-the-auto-generated-tests-for)
    - [Continuous Integration](#continuous-integration)

<!-- TOC end -->

# Information and Guidelines for Contributors

Thank you for contributing to MudBlazor and making it even better. We are happy about every contribution! Issues, bug-fixes, new components...

## Code of Conduct

Please make sure that you follow our [code of conduct](/CODE_OF_CONDUCT.md)

## Minimal Prerequisites to Compile from Source

-   [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

## Pull Requests
- Your Pull Request (PR) must only consist of one topic. It is better to split Pull Requests with more than one feature or bug fix in separate Pull Requests
- First fork the repository and clone your fork locally to make your changes. (The main repository is protected and does not accept direct commits.)
- You should work on a separate branch with a descriptive name. The following naming convention can be used: `feature/my-new-feature` for new features and enhancements, `fix/my-bug-fix` for bug fixes. For example, `fix/button-hover-color` if your PR is about a bug involving the hover color of buttons
- You should build, test and run one of the Docs projects locally to confirm your changes give the expected result. We generally suggest the MudBlazor.Docs.Server project for the best debugging experience.
- Choose `dev` as the target branch
- All tests must pass, when you push, they will be executed on the CI server, and you'll receive a test report per email. But you can also execute them locally for quicker feedback.
- You must include tests when your Pull Requests alters any logic. This also ensures that your feature will not break in the future under changes from other contributors. For more information on testing, see the appropriate section below
- If there are new changes in the main repo, you should either merge the main repo's (upstream) dev or rebase your branch onto it.
- Before working on a large change, it is recommended to first open an issue to discuss it with others
- If your Pull Request is still in progress, convert it to a draft Pull Request
- The PR Title should follow the following format: 
```
<component name>: <short description of changes in imperative> (<linked issue>)
```

For example:

```
 DateRangePicker: Fix initializing DateRange with null values (#1997)
```
- To keep your branch up to date with the `dev` branch simply merge `dev`. **Don't rebase** because if you rebase the wrong direction your PR will include tons of unrelated commits from dev.
- Your Pull Request should not include any unnecessary refactoring
- If there are visual changes, you should include a screenshot, gif or video
- If there are any corresponding issues, link them to the Pull Request. Include `Fixes #<issue nr>` for bug fixes and `Closes #<issue nr>` for other issues in the description ([Link issues guide](https://docs.github.com/en/github/managing-your-work-on-github/linking-a-pull-request-to-an-issue#linking-a-pull-request-to-an-issue-using-a-keyword)) 
- Your code should be formatted correctly ([Format documentation](https://docs.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/formatting-rules))



### Pull Requests which introduce new components

-   MudBlazor supports RTL. It basically mirrors the ui horizontally for languages which are read right-to-left. See [RTL guide](https://rtlstyling.com/posts/rtl-styling) for more information. Therefore every component should implement this functionality. If necessary include

```csharp
[CascadingParameter] public bool RightToLeft {get; set;}
```

in your component and apply styles at component level.
- You must add tests if your component contains any logic (CSS styling requires no testing)
- Use our `css variables` if possible. For instance, you should not hard code any colors etc.
- Include a summary comment for every public property ([Summary documentation](https://learn.microsoft.com/dotnet/csharp/language-reference/xmldoc/recommended-tags))
- Use the `CssBuilder` for classes and styles
- Add a doc page and examples which should be ordered from easy to more complex
- Examples with more than 15 lines should be collapsed by default

## Project structure and where to find the most important files
MudBlazor is divided into different projects. The most important ones are:
- [MudBlazor](https://github.com/MudBlazor/MudBlazor/tree/dev/src/MudBlazor): contains all components
- [MudBlazor.Docs](https://github.com/MudBlazor/MudBlazor/tree/dev/src/MudBlazor.Docs): contains the [docs](https://mudblazor.com/)
- [MudBlazor.Docs.WasmHost](https://github.com/MudBlazor/MudBlazor/tree/dev/src/MudBlazor.Docs.WasmHost): local copy of the docs that can be set as the startup project and run locally to review changes before submission.
- [MudBlazor.UnitTests](https://github.com/MudBlazor/MudBlazor/tree/dev/src/MudBlazor.UnitTests): contains bUnit tests for all components
- [MudBlazor.UnitTests.Viewer](https://github.com/MudBlazor/MudBlazor/tree/dev/src/MudBlazor.UnitTests.Viewer): a visual representation of the unit tests. When you launch the project, you can test whether the components look and behave correctly

Most important files:

-   Component `.razor` and `.razor.cs` classes ([Link](https://github.com/MudBlazor/MudBlazor/tree/dev/src/MudBlazor/Components))
-   Component `.scss` style classes ([Link](https://github.com/MudBlazor/MudBlazor/tree/dev/src/MudBlazor/Styles/components))
-   Enums ([Link](https://github.com/MudBlazor/MudBlazor/tree/dev/src/MudBlazor/Enums))
-   Component doc pages ([Link](https://github.com/MudBlazor/MudBlazor/tree/dev/src/MudBlazor.Docs/Pages/Components))
-   Component tests ([Link](https://github.com/MudBlazor/MudBlazor/tree/dev/src/MudBlazor.UnitTests/Components))
-   Test components ([Link](https://github.com/MudBlazor/MudBlazor/tree/dev/src/MudBlazor.UnitTests.Viewer/TestComponents))

## Coding Dos and Don'ts

-   **No code in parameter getter/setter!** See section *Parameter Registration or Why we can't have Logic in Parameter Setters* below
-   **Don't overwrite parameters in components!** See section *Avoid overwriting parameters in Blazor Components* below
-   **No programmatic assignments to another component's parameters** See section *Blazor Component parameter should not be set outside of its component.* below
-   **Don't break stuff!** See section *Unit Testing and Continuous Integration* below
-   **Add a test to guard against others breaking your feature/fix!** See section *Unit Testing and Continuous Integration* below

## Parameter Registration or Why we can't have Logic in Parameter Setters

MudBlazor parameters shall be auto-properties, meaning that there must not be logic in the property getter or setter. This rule prevents update-loops and other nasty bugs such as swallowed exceptions due to unobserved async discards. "This is quite inconvenient" you may say, where do I call the EventCallback and how to react to parameter changes? Luckily the MudBlazor team has got your back. Thanks to our ParameterState framework you don't need to keep track of old parameter values in fields and mess around with `SetParametersAsync`.

**TLDR; Register parameters in the constructor with a change handler that contains all the code that needs to be executed when the parameter value changes.**

**NB: Code in** `[Parameter]` **attributed property setters is no longer allowed in MudBlazor!** (No matter if async functions are called in them or not.)

### Example of a bad Parameter definition

Here is a real example of a parameter with additional logic in the setter, which is now forbidden.

```c#
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
        if (_isRendered)
        {
            _state = _expanded ? CollapseState.Entering : CollapseState.Exiting;
            _ = UpdateHeight();  // <-- unobserved async discard !!!
            _updateHeight = true;
        }
        else if (_expanded)
        {
            _state = CollapseState.Entered;
        }
        _ = ExpandedChanged.InvokeAsync(_expanded); // <-- unobserved async discard !!!
    }
}
```

Note how the setter is invoking async functions which cannot be awaited, because property setters can only have synchronous code. As a result, the async 
functions are invoked and their return value `Task` is discarded. This not only creates hard to test multi-threaded behavior, but it also prevents the user of this 
component from being able to catch any errors in the async functions. Any exceptions that happen in these asynchronous functions may or may not bubble up
to the user. In some cases, Blazor just catches them and they are silently ignored, in other cases they may cause application crashes that can't be prevented with `try catch`. 

The alternative would be to move the code from the setter into `SetParametersAsync` and depending on the component you would also need code in `OnInitializedAsync`. This is cumbersome and error prone and requires you to keep track of the old parameter value in a field and write a series of `if` statements in `SetParametersAsync` if there are multiple parameters.

Using our new `ParameterState` pattern all this is not required.

### Example of a good Parameter definition

```c#
private readonly ParameterState<bool> _expandedState;

[Parameter]
public bool Expanded { get; set; }
```

In the constructor, we register the parameter so that the base class can manage it for us automatically behind the scenes:

```c#
public MudCollapse()
{
    using var registerScope = CreateRegisterScope();
    _expandedState = registerScope.RegisterParameter<bool>(nameof(Expanded)) // the property name is needed for automatic value change detection in SetParametersAsync
        .WithParameter(() => Expanded) // a get func enabling the ParameterState to read the parameter value w/o resorting to Reflection
        .WithEventCallback(() => ExpandedChanged) // a get func enabling the ParameterState to get the EventCallback of the parameter (if the param is two-way bindable)
        .WithChangeHandler(OnExpandedChangedAsync); // the change handler 
}
```

The code from the setter moves into the change handler function which is async so the called functions can be awaited.

```c#
private async Task OnExpandedChangedAsync()
{
    if (_isRendered)
    {
        _state = _expandedState.Value ? CollapseState.Entering : CollapseState.Exiting;
        await UpdateHeightAsync();  // async Task not discarded
        _updateHeight = true;
    }
    else if (_expandedState.Value)
    {
        _state = CollapseState.Entered;
    }
    await ExpandedChanged.InvokeAsync(_expandedState.Value); // async Task not discarded
}
```

There are a couple of builders for the `RegisterParameter` method for different use-cases. For instance, you don't always need an `EventCallback` for every parameter. 
Some parameters need async logic in their change handler, others don't, etc.

### Can I share change handlers between parameters?

Yes, if you pass them as a method group like in the example below, shared parameter change handlers will be called only once, even if multiple parameters change at the same time.

```c#
    // Param1 and Param2 share the same change handler
    using var registerScope = CreateRegisterScope();
    _param1State = registerScope.RegisterParameter<int>(nameof(Param1)).WithParameter(() => Param1).WithChangeHandler(OnParametersChanged);
    _param2State = registerScope.RegisterParameter<int>(nameof(Param2)).WithParameter(() => Param2).WithChangeHandler(OnParametersChanged);
```

**NB**: if you pass lambda functions as change handlers they will be called once each for every changed parameter even if they contain the same code!

### What about the bad parameters all over the MudBlazor code base?

We are slowly but surely refactoring all of those, you can help if you like.

## Avoid overwriting parameters in Blazor Components

The `ParameterState` framework offers a solution to prevent parameter overwriting issues. For a detailed explanation of this problem, refer to the [article](https://learn.microsoft.com/aspnet/core/blazor/components/overwriting-parameters?view=aspnetcore-8.0#overwritten-parameters).

### Example of a bad code

```c#
[Parameter]
public bool Expanded { get; set; }

[Parameter]
public EventCallback<bool> ExpandedChanged { get; set; }


private Task ToggleAsync()
{
	Expanded = !Expanded;
	return ExpandedChanged.InvokeAsync(Expanded);
}
```

### Example of a good code

```c#
private readonly ParameterState<bool> _expandedState;

[Parameter]
public bool Expanded { get; set; }

[Parameter]
public EventCallback<bool> ExpandedChanged { get; set; }

public MudTreeViewItemToggleButton()
{
    using var registerScope = CreateRegisterScope();
    _expandedState = registerScope.RegisterParameter<bool>(nameof(Expanded))
        .WithParameter(() => Expanded)
        .WithEventCallback(() => ExpandedChanged);
}

private Task ToggleAsync()
{
	return _expandedState.SetValueAsync(!_expandedState.Value);
}
```

## Blazor Component parameter should not be set outside of its component.

Consider a hypothetical `CalendarComponent`:

```c#
public class CalendarComponent : ComponentBase
{
	[Parameter]
	public ShowOnlyOneCalendar { get;set; }
}
```

### Example of a bad code

```razor
<CalendarComponent @ref="@_calendar" />
<button @onclick="Update">
        Update
</button>

@code
{
    private CalendarComponent _calendarRef = null!;

    private void Update()
    {
        _calendarRef.ShowOnlyOneCalendar = true;
    }
}
```

This code would result in a [BL0005](https://learn.microsoft.com/aspnet/core/diagnostics/bl0005?view=aspnetcore-8.0) warning.

### Example of a good code

Instead of using an imperative programming approach (`component.Parameter1 = v1`), a Component Parameter is supposed to be passed in a declarative syntax:

```razor
<CalendarComponent ShowOnlyOneCalendar="@_showOnlyOne"  />
<button @onclick="Update">
        Update
</button>

@code
{
    private bool _showOnlyOne;;
	
    private void Update()
    {
        _showOnlyOne = true;
    }
}
```

In the improved version, we pass `ShowOnlyOneCalendar` as a parameter to `CalendarComponent` directly in the markup, using a variable (`_showOnlyOne`) that can be manipulated within the component's code. This adheres to the recommended Blazor coding practices and avoids errors like `BL0005`.

## Unit Testing and Continuous Integration

We strive for complete test coverage to keep stuff from breaking and
deliver a rock-solid library. For every component that has C# logic we 
require a bUnit test that checks its logic.

### How not to break stuff

When you are making changes to any components and preparing a PR make sure you run the entire test suite to see if anything broke.

Documentation changes should be reviewed by locally previewing with 
MudBlazor.Docs.WasmHost as the startup project.

Once your PR is merged into the dev branch, it can be viewed at https://dev.mudblazor.com/ even before the next release.

### Make your code break-safe

When you are writing non-trivial logic, please add a unit test for it. Basically, think of it like this: By adding a test for everything you fear could break you make sure your work is not undone by accident by future additions.

### How to write a unit test?

Simply follow the example of some of the simpler tests like: 
- StringExtensionTests.cs for normal C# tests
- CheckBoxTests.cs or RadioTests.cs for bUnit tests

### How to write a bUnit test

Let's say we want to test whether a component's two-way bindable property works

In MudBlazor.UnitTests.Viewer create a razor file that instantiates your component and binds it to a public field.

In MudBlazor.UnitTests create another test (i.e. by copying CheckBoxTests.cs and renaming it)
In the Test make sure to instantiate the razor file you just prepared above.
 - Assert that the initial state is correct
 - Make changes to the public field of the test component and assert that it changes what it should change in the component
 - Call Click or other events on the component and check that the public field was updated properly
 
 You can print the components rendered HTML to the console at different locations of the test to 
 see how state changes affect the HTML or the class attributes. Then write 
 assertions that enforce those changes i.e. by checking that a certain html exists 
 or a certain class is contained or not contained in the class attributes of an element. 
 
### What are common errors when writing tests?

#### Do not save html elements you query via `Find` or `FindAll` in a variable!

```c#
   var comp = ctx.RenderComponent<MudTextField<string>>();
   
   // wrong - this will fail:
   var textField = comp.Find("input");
   textField.Change("Garfield");
   textField.Blur();
   comp.FindComponent<MudTextField<string>>().Instance.Value.NotBeNullOrEmpty();
```

As soon as you interact with html elements they are potentially re-rendered, and your variable becomes stale.

```c#
   var comp = ctx.RenderComponent<MudTextField<string>>();
   
   // correct   
   comp.Find("input").Change("Garfield");
   comp.Find("input").Blur();
   comp.FindComponent<MudTextField<string>>().Instance.Value.NotBeNullOrEmpty();
```

So never save html element references in a variable in a bUnit test. Note: you can save component references in variables just fine, so don't confuse that.

#### Always use InvokeAsync to set parameter values on a component

The bUnit test logic is not running on the Blazor UI-thread, so whenever directly interacting with a component's parameters or methods you need to use `await comp.InvokeAsync(()=> ... )`. That way the following test logic happens only after the interaction with the component has been concluded.

```c#
   var comp = ctx.RenderComponent<MudTextField<string>>();
   var textField=comp.FindComponent<MudTextField<string>>().Instance;
   
   // wrong!
   textField.Value="Garfield";
   // correct
   await comp.InvokeAsync(()=>textField.Value="I love dogs");
```

### What does not need to be tested?

We don't need to test the complete rendered HTML of a component, or the appearance of a component. Test the logic, not the HTML. When checking changes in the HTML do simple checks like "does the HTML element exist that depends on a state".

### What is the MudBlazor.UnitTests.Viewer for?

Two things.

-   It holds all the test components which are required by the bUnit tests.
-   You can run it and try your test components out if you need to debug them.

### What are the auto-generated tests for?

The documentation has lots of examples for every component. We use those 
examples as unit tests by instantiating them in a bUnit context and checking
whether rendering them throws an error or not. While this is not comparable
to a good hand-written unit test, we can at least catch exceptions thrown by
the render logic. These tests are generated automatically on build and their
cs files start with an underscore.

### Continuous Integration

We have an Azure DevOps pipeline which will automatically execute the entire
test suite on all pushes and PRs. If your commit or PR breaks the tests
you'll be notified.
