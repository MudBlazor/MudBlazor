// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;

namespace MudBlazor
{
    public partial class SelectColumn<T>
    {
        /// <summary>
        /// Specifies wheter theres a select all box in the datagrid header
        /// </summary>
        [Parameter] public bool ShowInHeader { get; set; } = true;
        /// <summary>
        /// Specifies wheter theres a select all box in the datagrid footer
        /// </summary>
        [Parameter] public bool ShowInFooter { get; set; } = true;
        /// <summary>
        /// Specifies the size parameter for the MudCheckBox used in the column
        /// </summary>
        [Parameter] public Size Size { get; set; } = Size.Medium;
    }
}
