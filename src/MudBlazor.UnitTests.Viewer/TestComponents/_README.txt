All test components must have the substring "Test" in their names in order to be loaded by the viewer.

It is advised to name the test component exactly the same as the bUnit testcase under UnitTests.

In order to have the test description show up, make sure the component implements a static field __description__ in the @code section:

@code {
    public static string __description__ = " ... ";
}