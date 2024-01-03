// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace MudBlazor
{
    public enum SelectionMode
    {
        [Description("single-selection")]
        SingleSelection,

        [Description("multi-selection")]
        MultiSelection,

        [Description("toggle-selection")]
        ToggleSelection
    }
}
