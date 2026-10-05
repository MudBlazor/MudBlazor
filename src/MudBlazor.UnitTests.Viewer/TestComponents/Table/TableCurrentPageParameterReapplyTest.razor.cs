// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using Microsoft.AspNetCore.Components;

namespace MudBlazor.UnitTests.TestComponents.Table;

public partial class TableCurrentPageParameterReapplyTest : ComponentBase
{
    public static string __description__ = "Table - Re-applying an unchanged CurrentPage parameter must not reset the pager (same class as #13462)";

    public MudTable<int> Table { get; set; } = null!;

    private readonly int[] _items = Enumerable.Range(1, 200).ToArray();

    // One-way bound: the pager mutates the table's page without writing back here, so a plain
    // re-render re-applies this unchanged value. Changing it from code drives a genuine navigation.
    private int _currentPage;

    private void Rerender() => StateHasChanged();

    private void SetPageFromCode() => _currentPage = 5;
}
