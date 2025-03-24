// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

#nullable enable

namespace MudBlazor
{
    public partial class DataGridGroupRow<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T> : MudComponentBase
    {
        protected string GroupClassname => new CssBuilder("mud-table-cell")
            .AddClass("mud-datagrid-group")
            .AddClass($"mud-row-group-indented-{(GroupDefinition.Indentation ? Math.Min(GroupDefinition.Level, 5) : 0)}")
            .AddClass(GroupClassFunc?.Invoke(GroupDefinition))
            .AddClass(GroupClass)
            .Build();

        protected string GroupStylename => new StyleBuilder()
            .AddStyle(GroupStyle)
            .AddStyle(GroupStyleFunc?.Invoke(GroupDefinition))
            .Build();

        [Parameter]
        [Category(CategoryTypes.DataGrid.Behavior)]
        public bool Expanded { get; set; }

        [Parameter, EditorRequired]
        [Category(CategoryTypes.DataGrid.Grouping)]
        public MudDataGrid<T> DataGrid { get; set; } = default!;

        /// <summary>
        /// The definition for this grouping level
        /// </summary>
        [Parameter, EditorRequired]
        [Category(CategoryTypes.DataGrid.Grouping)]
        public GroupDefinition<T> GroupDefinition { get; set; } = default!;

        /// <summary>
        /// The groups and items within this grouping.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.DataGrid.Grouping)]
        public IGrouping<object?, T>? Items { get; set; }

        [Parameter]
        [Category(CategoryTypes.DataGrid.Appearance)]
        public string? GroupClass { get; set; }

        [Parameter]
        [Category(CategoryTypes.DataGrid.Appearance)]
        public string? GroupStyle { get; set; }

        [Parameter]
        [Category(CategoryTypes.DataGrid.Appearance)]
        public Func<GroupDefinition<T>, string>? GroupClassFunc { get; set; }

        [Parameter]
        [Category(CategoryTypes.DataGrid.Appearance)]
        public Func<GroupDefinition<T>, string>? GroupStyleFunc { get; set; }

        [Parameter]
        [Category(CategoryTypes.DataGrid.Appearance)]
        public string? StyleClass { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
        }

        private void GroupExpandClick()
        {
            Expanded = !Expanded;
            if (Items != null)
            {
                var key = new { GroupDefinition.Title, Items?.Key };
                if (DataGrid._groupExpansionsDict.ContainsKey(key))
                {
                    if (Expanded == GroupDefinition.Expanded)
                        DataGrid._groupExpansionsDict.Remove(key);
                    else
                        DataGrid._groupExpansionsDict[key] = Expanded;
                }
                else
                {
                    DataGrid._groupExpansionsDict.TryAdd(key, Expanded);
                }
            }
            DataGrid._groupInitialExpanded = false;
        }
    }
}
