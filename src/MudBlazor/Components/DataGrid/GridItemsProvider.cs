// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Web.Virtualization;

namespace MudBlazor.Components.DataGrid
{
    /// <summary>
    /// A callback that provides data for a MudBlazorGrid"/>.
    /// </summary>
    /// <typeparam name="TGridItem">The type of data represented by each row in the grid.</typeparam>
    /// <param name="request">Parameters describing the data being requested.</param>
    /// <returns>A <see cref="ValueTask{GridItemsProviderResult}" /> that gives the data to be displayed.</returns>
    public delegate ValueTask<ItemsProviderResult<TGridItem>> GridItemsProvider<TGridItem>(
    GridItemsProviderRequest<TGridItem> request);
}
