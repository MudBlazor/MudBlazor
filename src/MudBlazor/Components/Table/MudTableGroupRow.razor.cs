using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable

    /// <summary>
    /// A grouping of values for a column in a <see cref="MudTable{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of item being grouped.</typeparam>
    public partial class MudTableGroupRow<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] T> : MudComponentBase
    {
        private bool? _checked = false;
        private IGrouping<object, T>? _items = null;
        private IEnumerable<IGrouping<object, T>>? _innerGroupItems = null;

        protected string HeaderClassname => new CssBuilder("mud-table-row")
            .AddClass(HeaderClass)
            .AddClass($"mud-table-row-group-indented-{GroupDefinition?.Level - 1}",
                (GroupDefinition?.Indentation ?? false) && GroupDefinition?.Level > 1)
            .Build();

        protected string FooterClassname => new CssBuilder("mud-table-row")
            .AddClass(FooterClass)
            .AddClass($"mud-table-row-group-indented-{GroupDefinition?.Level - 1}",
                (GroupDefinition?.Indentation ?? false) && GroupDefinition?.Level > 1)
            .Build();

        protected string ActionsStylename => new StyleBuilder()
            .AddStyle("padding-left", "34px", GroupDefinition?.IsParentExpandable ?? false).Build();

        /// <summary>
        /// The current state of the <see cref="MudTable{T}"/> containing this group.
        /// </summary>
        [CascadingParameter]
        public TableContext? Context { get; set; }

        /// <summary>
        /// The definition for this grouping level.
        /// </summary>
        /// <remarks>
        /// Group definitions can be recursive.
        /// </remarks>
        [Parameter]
        public TableGroupDefinition<T>? GroupDefinition { get; set; }

        /// <summary>
        /// The groups and items within this grouping.
        /// </summary>
        [Parameter]
        public IGrouping<object, T>? Items
        {
            get => _items;
            set
            {
                _items = value;
                SyncInnerGroupItems();
            }
        }

        /// <summary>
        /// The custom content for this group's header.
        /// </summary>
        [Parameter]
        public RenderFragment<TableGroupData<object, T>>? HeaderTemplate { get; set; }

        /// <summary>
        /// The custom content for this group's footer.
        /// </summary>
        [Parameter]
        public RenderFragment<TableGroupData<object, T>>? FooterTemplate { get; set; }

        /// <summary>
        /// Displays a checkbox which selects or unselects all items within this group.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        public bool Checkable { get; set; }

        /// <summary>
        /// The CSS classes applied to this group's header.
        /// </summary>
        /// <remarks>
        /// Multiple classes must be separated by spaces.
        /// </remarks>
        [Parameter]
        public string? HeaderClass { get; set; }

        /// <summary>
        /// The CSS classes applied to this group's footer.
        /// </summary>
        /// <remarks>
        /// Multiple classes must be separated by spaces.
        /// </remarks>
        [Parameter]
        public string? FooterClass { get; set; }

        /// <summary>
        /// The CSS styles applied to this group's header.
        /// </summary>
        [Parameter]
        public string? HeaderStyle { get; set; }

        /// <summary>
        /// The CSS styles applied to this group's footer.
        /// </summary>
        [Parameter]
        public string? FooterStyle { get; set; }

        /// <summary>
        /// The icon of the expand button when <see cref="TableGroupDefinition{T}.Expandable"/> is <c>true</c>.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.ExpandMore"/>.
        /// </remarks>
        [Parameter]
        public string ExpandIcon { get; set; } = Icons.Material.Filled.ExpandMore;

        /// <summary>
        /// The icon of the collapse button when <see cref="TableGroupDefinition{T}.Expandable"/> is <c>true</c>.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.ChevronRight"/>.
        /// </remarks>
        [Parameter]
        public string CollapseIcon { get; set; } = Icons.Material.Filled.ChevronRight;

        /// <summary>
        /// Occurs when a grouping row is clicked.
        /// </summary>
        [Parameter]
        public EventCallback<MouseEventArgs> OnRowClick { get; set; }

        /// <summary>
        /// Selects the checkbox for this group's header.
        /// </summary>
        /// <remarks>
        /// Only has an effect when <see cref="Checkable"/> is <c>true</c>.
        /// </remarks>
        public bool? Checked
        {
            get => _checked;
            set
            {
                if (value != _checked)
                {
                    _checked = value;
                    if (Checkable)
                    {
                        Table?.OnGroupHeaderCheckboxClicked(_checked.HasValue && _checked.Value, Items?.ToList() ?? new List<T>());
                    }
                }
            }
        }

        /// <summary>
        /// Shows the items in this group.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        public bool Expanded { get; internal set; } = true;

        protected override Task OnInitializedAsync()
        {
            if (GroupDefinition != null)
            {
                Expanded = GroupDefinition.IsInitiallyExpanded;
                ((TableContext<T>?)Context)?.GroupRows.Add(this);
                SyncInnerGroupItems();
            }
            return base.OnInitializedAsync();
        }

        private void SyncInnerGroupItems()
        {
            if (GroupDefinition?.InnerGroup != null)
            {
                _innerGroupItems = Table?.GetItemsOfGroup(GroupDefinition.InnerGroup, Items);
            }
        }

        /// <summary>
        /// Releases resources used by this group row.
        /// </summary>
        public void Dispose()
        {
            ((TableContext<T>?)Context)?.GroupRows.Remove(this);
        }

        /// <summary>
        /// Sets the <see cref="Checked"/> value and optionally refreshes this group.
        /// </summary>
        /// <param name="checkedState">The new checked state.</param>
        /// <param name="notify">When <c>true</c>, and <see cref="Checkable"/> is <c>true</c>, the <see cref="MudTable{T}.OnGroupHeaderCheckboxClicked(bool, IEnumerable{T})"/> event will occur.</param>
        public void SetChecked(bool? checkedState, bool notify)
        {
            if (_checked != checkedState)
            {
                if (notify)
                    Checked = checkedState;
                else
                {
                    _checked = checkedState;
                    if (Checkable)
                        InvokeAsync(StateHasChanged);
                }
            }
        }

        private MudTable<T>? Table
        {
            get => (MudTable<T>?)((TableContext<T>?)Context)?.Table;
        }
    }
}
