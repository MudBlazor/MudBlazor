// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.UnitTests.Dummy;

public class DummyErrorConverter : IReversibleConverter<int, string>
{
    public string Convert(int input)
    {
        throw new InvalidOperationException("Conversion error");
    }

    public int ConvertBack(string input)
    {
        throw new InvalidOperationException("Conversion error");
    }
}
