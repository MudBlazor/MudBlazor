// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;

namespace MudBlazor.Components.Combobox
{
    public record ComboBoxItem<T>(T Value, bool IsSelected, bool IsDisabled, bool IsHovered, Func<Task> ToggleSelectedItem)
    {
        public RenderFragment? DisplayFragment()
        {
            if (Value is null)
                return null;
            return StringFragment(Value.ToString());
        }

        private RenderFragment StringFragment(string stringVal) => __builder =>
        {
            __builder.AddContent(0, stringVal);
        };

        public string CheckBoxIcon()
        {
            if (IsSelected)
                return Icons.Material.Filled.CheckBox;
            return Icons.Material.Filled.CheckBoxOutlineBlank;
        }
    }
}
