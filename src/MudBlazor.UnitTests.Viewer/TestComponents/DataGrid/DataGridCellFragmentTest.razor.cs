// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

using Microsoft.AspNetCore.Components;

namespace MudBlazor.UnitTests.TestComponents;

public partial class DataGridCellFragmentTest : ComponentBase
{
    public static readonly TestModel[] Items =
    {
        new("John", 45),
        new("Johanna", 23),
        new("Steve", 32)
    };

    public record TestModel(string Name, int Age);
}

public class CustomPropertyColumn<T, TProperty> : PropertyColumn<T, TProperty>
{
    protected override object CellContent(T item)
    {
        var value = base.CellContent(item);
            
        return value switch
        {
            string sValue => CreateFragment(sValue),
            _ => value
        };
    }

    private static RenderFragment CreateFragment(string content)
    {
        return builder =>
        {
            builder.OpenElement(0, "span");
            builder.AddContent(1, content);
            builder.CloseElement();
        };
    }
}
