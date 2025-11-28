// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Utilities.Converter;

public class EmptyConverter<T> : IReversibleConverter<T, T>
{
    public T Convert(T input)
    {
        return input;
    }

    public T ConvertBack(T output)
    {
        return output;
    }
}
