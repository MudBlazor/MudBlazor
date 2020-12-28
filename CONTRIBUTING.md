# ![MudBlazor](content/MudBlazor-GitHub.png)

# Information and Guidelines for Contributors

## Prerequisites to Compile from Source

- .NET Core 3.1
- Visual Studio 2019 with the ASP.NET and Web development.

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
whether or not rendering them throws an error or not. While this is not a 
replacing of a good hand-written unit test we can at least catch very basic
breakages which would throw a runtime exception on render. The auto-tests are 
all generated into the file _AllComponents.cs under MudBlazor.UnitTests.  

### Continuous Integration

We have an Azure DevOps pipeline which will automatically execute the entire
test suite on all pushes to the master branch. 
