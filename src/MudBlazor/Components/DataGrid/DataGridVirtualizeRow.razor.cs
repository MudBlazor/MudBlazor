// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
#nullable enable
namespace MudBlazor
{
    public partial class DataGridVirtualizeRow<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T> : MudComponentBase
    {
        [Parameter]
        public required MudDataGrid<T> DataGrid { get; set; }

        [Parameter]
        public IGrouping<object, T>? GroupedItems { get; set; }
    }
}
