// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

#nullable enable

namespace MudBlazor
{
    public partial class DataGridGroupRow<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] T> : MudComponentBase
    {

        private bool? _checked = false;
        private IEnumerable<IGrouping<object, T>>? _innerGroupItems = null;

        protected string GroupClassname => new CssBuilder("mud-table-cell")
            .AddClass("mud-datagrid-group")
            .AddClass(GroupClass)
            .Build();

        /// <summary>
        /// The definitiong for this grouping level
        /// </summary>
        /// <remarks>
        /// </remarks>
        [Parameter]
        public GroupDefinition<T>? GroupDefinition { get; set; }

        /// <summary>
        /// The groups and items within this grouping.
        /// </summary>
        [Parameter]
        public IGrouping<object, T>? Items { get; set; }

        [Parameter]
        public string? GroupClass { get; set; }

        [Parameter]
        public string? StyleClass { get; set; }
    }
}
