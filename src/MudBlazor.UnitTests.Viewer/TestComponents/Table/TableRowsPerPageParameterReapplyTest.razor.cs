// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using Microsoft.AspNetCore.Components;

namespace MudBlazor.UnitTests.TestComponents.Table;

public partial class TableRowsPerPageParameterReapplyTest : ComponentBase
{
    public static string __description__ = "Table - Re-applying an unchanged RowsPerPage parameter must not reset the pager (#13462)";

    public MudTable<int> Table { get; set; } = null!;

    private readonly int[] _items = Enumerable.Range(1, 200).ToArray();

    private void Rerender() => StateHasChanged();
}
