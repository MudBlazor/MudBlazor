// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using MudBlazor.State;
using MudBlazor.Utilities;

#nullable enable

namespace MudBlazor
{
    /// <summary>
    /// Represents group data for a DataGrid
    /// </summary>
    public class DataGridGroupData<TKey, T>
    {
        public TKey? Key { get; }
        public List<T> Items { get; }

        public DataGridGroupData(TKey? key, List<T> items)
        {
            Key = key;
            Items = items;
        }
    }

    public partial class DataGridGroupRow<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T> : MudComponentBase
    {
        private IEnumerable<IGrouping<object, T>>? _innerGroupItems = null;
        private MudVirtualize<IndexBag<T>>? _mudVirtualize;

        public DataGridGroupRow()
        {
            // TODO make sure Expanded is somehow bindable from GroupExpanded property
        }

        protected string GroupClassname => new CssBuilder("mud-table-cell")
            .AddClass("mud-datagrid-group")
            .AddClass($"mud-table-row-group-indented-{(GroupDefinition.Indentation ? Math.Min(GroupDefinition.Level, 5) : 0)}")
            .AddClass(GroupClassFunc?.Invoke(GroupDefinition))
            .AddClass(GroupClass)
            .Build();

        protected string GroupStylename => new StyleBuilder()
            .AddStyle(GroupStyle)
            .AddStyle(GroupStyleFunc?.Invoke(GroupDefinition))
            .Build();

        [Parameter]
        public bool Expanded { get; set; }

        [Parameter]
        public MudDataGrid<T>? DataGrid { get; set; }

        /// <summary>
        /// The definition for this grouping level
        /// </summary>
        [Parameter]
        public GroupDefinition<T> GroupDefinition { get; set; } = default!;

        /// <summary>
        /// The groups and items within this grouping.
        /// </summary>
        [Parameter]
        public IGrouping<object, T> Items { get; set; }

        [Parameter]
        public string? GroupClass { get; set; }

        [Parameter]
        public string? GroupStyle { get; set; }

        [Parameter]
        public Func<GroupDefinition<T>, string>? GroupClassFunc { get; set; }

        [Parameter]
        public Func<GroupDefinition<T>, string>? GroupStyleFunc { get; set; }

        [Parameter]
        public string? StyleClass { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            // Initialize expansion state from GroupDefinition
            if (GroupDefinition != null)
            {
                Expanded = GroupDefinition.Expanded;
            }
        }

        private async Task OnExpandedChangeHandlerAsync(ParameterChangedEventArgs<bool> args)
        {
            await Task.CompletedTask;
        }
    }
}
