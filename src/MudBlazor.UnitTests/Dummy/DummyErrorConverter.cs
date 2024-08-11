// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.UnitTests.Dummy;

public class DummyErrorConverter : Converter<int>
{
    public DummyErrorConverter()
    {
        SetFunc = OnSet;
        GetFunc = OnGet;

        UpdateGetError("Conversion error");
    }

    private int OnGet(string _)
    {
        UpdateGetError("Conversion error");
        return 0;
    }

    private string OnSet(int _)
    {
        UpdateSetError("Conversion error");
        return string.Empty;
    }
}
