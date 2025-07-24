// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Components.DropDown
{
    public record DropDownItem<T>(T Item, bool IsSelected, bool IsDisabled, bool IsHovered, Func<Task> ToggleSelectedItem);
}
