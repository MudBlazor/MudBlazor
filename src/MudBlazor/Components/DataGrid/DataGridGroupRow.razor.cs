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
        public bool Expanded { get; set; }

        [Parameter]
        public required MudDataGrid<T> DataGrid { get; set; }

        /// <summary>
        /// The definition for this grouping level
        /// </summary>
        [Parameter]
        public GroupDefinition<T> GroupDefinition { get; set; } = default!;

        /// <summary>
        /// The groups and items within this grouping.
        /// </summary>
        [Parameter]
        public IGrouping<object, T>? Items { get; set; }

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

        private void GroupExpandClick()
        {
            Expanded = !Expanded;
        }
    }
}
