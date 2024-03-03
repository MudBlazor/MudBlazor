# ![MudBlazor](content/MudBlazor-GitHub-NoBg.png)

# Information and Guidelines for Contributors
Thank you for contributing to MudBlazor and making it even better. We are happy about every contribution! Issues, bug-fixes, new components...

## Code of Conduct
Please make sure that you follow our [code of conduct](/CODE_OF_CONDUCT.md)

## Minimal Prerequisites to Compile from Source

- [.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- [.NET 7.0 SDK](https://dotnet.microsoft.com/download/dotnet/7.0)

## Pull Requests
- Your Pull Request (PR) must only consist of one topic. It is better to split Pull Requests with more than one feature or bug fix in seperate Pull Requests
- First fork the repository and clone your fork locally to make your changes. (The main repository is protected and does not accept direct commits.)
- You should work on a seperate branch with a descriptive name. The following naming convention can be used: `feature/my-new-feature` for new features and enhancements, `fix/my-bug-fix` for bug fixes. For example `fix/button-hover-color` if your PR is about a bug involving the hover color of buttons
- You should build, test and run one of the Docs projects locally to confirm your changes give the expected result. We generally suggest the MudBlazor.Docs.Server project for the best debugging experience.
- Choose `dev` as the target branch
- All tests must pass, when you push, they will be executed on the CI server and you'll receive a test report per email. But you can also execute them locally for quicker feedback.
- You must include tests when your Pull Requests alters any logic. This also ensures that your feature will not break in the future under changes from other contributors. For more information on testing, see the appropriate section below
- If there are new changes in the main repo, you should either merge the main repo's (upstream) dev or rebase your branch onto it.
- Before working on a large change, it is recommended to first open an issue to discuss it with others
- If your Pull Request is still in progress, convert it to a draft Pull Request
- Your commit messages should follow the following format: 
```
<component name>: <short description of changes in imperative> (<linked issue>)
```
For example:
```
 DateRangePicker: Fix initializing DateRange with null values (#1997)
```

- Your Pull Request should not include any unnecessary refactoring
- If there are visual changes, you should include a screenshot, gif or video
- If there are any coresponding issues, link them to the Pull Request. Include `Fixes #<issue nr>` for bug fixes and `Closes #<issue nr>` for other issues in the description ([Link issues guide](https://docs.github.com/en/github/managing-your-work-on-github/linking-a-pull-request-to-an-issue#linking-a-pull-request-to-an-issue-using-a-keyword)) 
- Your code should be formatted correctly ([Format documentation](https://docs.microsoft.com/en-us/dotnet/fundamentals/code-analysis/style-rules/formatting-rules))



### Pull Requests which introduce new components
- MudBlazor supports RTL. It basically mirrors the ui horizontally for languages which are read right-to-left. See [RTL guide](https://rtlstyling.com/posts/rtl-styling)  for more information. Therefore every component should implement this functionality.
If necessary include
```csharp
[CascadingParameter] public bool RightToLeft {get; set;}
```
in your component and apply styles at component level.
- You must add tests if your component contains any logic (CSS styling requires no testing)
- Use our `css variables` if possible. For instance you should not hard code any colors etc.
- Include a summary comment for every public property ([Summary documentation](https://docs.microsoft.com/de-de/dotnet/csharp/programming-guide/xmldoc/summary))
- Use the `CssBuilder` for classes and styles
- Add a doc page and examples which should be ordered from easy to more complex
- Examples with more than 15 lines should be collapsed by default

## Project structure and where to find the most important files
MudBlazor is divided in different projects. The most important ones are:
- [MudBlazor](https://github.com/MudBlazor/MudBlazor/tree/dev/src/MudBlazor): contains all components
- [MudBlazor.Docs](https://github.com/MudBlazor/MudBlazor/tree/dev/src/MudBlazor.Docs): contains the [docs](https://mudblazor.com/)
- [MudBlazor.UnitTests](https://github.com/MudBlazor/MudBlazor/tree/dev/src/MudBlazor.UnitTests): contains bUnit tests for all components
- [MudBlazor.UnitTests.Viewer](https://github.com/MudBlazor/MudBlazor/tree/dev/src/MudBlazor.UnitTests.Viewer): a visual representation of the unit tests. When you launch the project, you can test whether the components look and behave correctly

Most important files:
- Component `.razor` and `.razor.cs` classes ([Link](https://github.com/MudBlazor/MudBlazor/tree/dev/src/MudBlazor/Components))
- Component `.scss` style classes ([Link](https://github.com/MudBlazor/MudBlazor/tree/dev/src/MudBlazor/Styles/components))
- Enums ([Link](https://github.com/MudBlazor/MudBlazor/tree/dev/src/MudBlazor/Enums))
- Component doc pages ([Link](https://github.com/MudBlazor/MudBlazor/tree/dev/src/MudBlazor.Docs/Pages/Components))
- Component tests ([Link](https://github.com/MudBlazor/MudBlazor/tree/dev/src/MudBlazor.UnitTests/Components))
- Test components ([Link](https://github.com/MudBlazor/MudBlazor/tree/dev/src/MudBlazor.UnitTests.Viewer/TestComponents))

## Unit Testing and Continuous Integration

We strive for a complete test coverage in order to keep stuff from breaking and
delivering a rock solid library. For every component that has C# logic we 
require a bUnit test that is checking that logic.

### How not to break stuff

When you are making changes to any components and preparing a PR make
sure you run the entire test suite to see if anything broke. 

### Make your code break-safe

When you are writing non-trivial logic, please add a unit test for it. Basically, think of it like this: By adding 
a test for everything you fear could break you make sure your work is not undone by accident by future additions. 

### How to write a unit test?

It is actually dead simple. Look at some of the simpler tests like 
- StringExtensionTests.cs for normal C# tests
- CheckBoxTests.cs or RadioTests.cs for bUnit tests

### How to write a bUnit test

Lets say we want to test whether a component's two-way bindable property works

In MudBlazor.UnitTests.Viewer create a razor file that instantiates your component
and binds it to a public field.

In MudBlazor.UnitTests create another test (i.e. by copying CheckBoxTests.cs and renaming it)
In the Test make sure to instantiate the razor file you just prepared above.
 - Assert that the initial state is correct
 - Make changes to the public field of the test component and assert that it changes  what it should change in the component
 - Call Click or other events on the componet and check that the public field was updated properly
 
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

As soon as you interact with html elements they are potentially re-rendered and your variable becomes stale.

```c#
   var comp = ctx.RenderComponent<MudTextField<string>>();
   
   // correct   
   comp.Find("input").Change("Garfield");
   comp.Find("input").Blur();
   comp.FindComponent<MudTextField<string>>().Instance.Value.NotBeNullOrEmpty();
```
So never save html element references in a variable in a bUnit test. Note: you can save component references
in variables just fine, so don't confuse that.

#### Always use InvokeAsync to set parameter values on a component

The bUnit test logic is not running on the blazor UI-thread, so whenever directly interacting with a component's 
parameters or methods you need to use `await comp.InvokeAsync(()=> ... )`. That way the following test logic happens only after the 
interaction with the component has been concluded.

```c#
   var comp = ctx.RenderComponent<MudTextField<string>>();
   var textField=comp.FindComponent<MudTextField<string>>().Instance;
   
   // wrong!
   textField.Value="Garfield";
   // correct
   await comp.InvokeAsync(()=>textField.Value="I love dogs");
```

### What does not need to be tested?

We don't need to test the complete rendered HTML of a component, or the appearance
of a component. Test the logic, not the HTML. When checking changes in the HTML
do simple checks like "does the HTML element exist that depends on a state". 

### What is the MudBlazor.UnitTests.Viewer for?

Two things. 
- It holds all the test components which are required by the bUnit tests. 
- You can run it and try your test components out if you need to debug them.

### What are the auto-generated tests for?

The documentation has lots of examples for every component. We use those 
examples as unit tests by instantiating them in a bUnit context and checking
whether rendering them throws an error or not. While this is not comparable
to a good hand-written unit test we can at least catch exceptions thrown by
the render logic. These tests are generated automatically on build and their
cs files start with an underscore.

### Continuous Integration

We have an Azure DevOps pipeline which will automatically execute the entire
test suite on all pushes and PRs. So if your commit or PR breaks the tests
you'll be notified.
