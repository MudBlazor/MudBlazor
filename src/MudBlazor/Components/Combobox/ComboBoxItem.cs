// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
#nullable enable
namespace MudBlazor.Components.Combobox
{
    public record ComboBoxItem<T>(T Value, Func<bool> IsSelected, Func<bool> IsDisabled, RenderFragment? ChildContent, Func<Task> ToggleSelectedItem, Func<T?, string?>? ToStringFunc)
    {
        public RenderFragment DisplayFragment()
        {
            if (ChildContent != null)
            {
                return ChildContent;
            }
            return StringFragment(ToStringFunc?.Invoke(Value) ?? Value?.ToString() ?? string.Empty);
        }

        private RenderFragment StringFragment(string stringVal) => __builder =>
        {
            __builder.AddContent(0, stringVal);
        };

        public string CheckBoxIcon()
        {
            if (IsSelected.Invoke())
                return Icons.Material.Filled.CheckBox;
            return Icons.Material.Filled.CheckBoxOutlineBlank;
        }
    }
}
