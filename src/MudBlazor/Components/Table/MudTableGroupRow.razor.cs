using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudTableGroupRow<T> : MudComponentBase
    {
        protected string HeaderClassname => new CssBuilder("mud-table-row")
                                .AddClass(HeaderClass)
                                .AddClass($"mud-table-row-group-indented-{GroupDefinition?.Level - 1}", (GroupDefinition?.Indentation ?? false) && GroupDefinition?.Level > 1).Build();

        protected string FooterClassname => new CssBuilder("mud-table-row")
                                .AddClass(FooterClass)
                                .AddClass($"mud-table-row-group-indented-{GroupDefinition?.Level - 1}", (GroupDefinition?.Indentation ?? false) && GroupDefinition?.Level > 1).Build();

        protected string ActionsStylename => new StyleBuilder()
            .AddStyle("padding-left", "34px", GroupDefinition?.IsParentExpandable ?? false).Build();

        [CascadingParameter] public TableContext Context { get; set; }

        private IEnumerable<IGrouping<object, T>> _innerGroupItems;

        /// <summary>
        /// The group definition for this grouping level. It's recursive.
        /// </summary>
        [Parameter] public TableGroupDefinition<T> GroupDefinition { get; set; }

        /// <summary>
        /// Inner Items List for the Group
        /// </summary>
        [Parameter] public IGrouping<object, T> Items { get; set; }

        /// <summary>
        /// Defines Group Header Data Template
        /// </summary>
        [Parameter] public RenderFragment<TableGroupData<object, T>> HeaderTemplate { get; set; }

        /// <summary>
        /// Defines Group Header Data Template
        /// </summary>
        [Parameter] public RenderFragment<TableGroupData<object, T>> FooterTemplate { get; set; }

        /// <summary>
        /// Add a multi-select checkbox that will select/unselect every item in the table
        /// </summary>
        [Parameter] public bool IsCheckable { get; set; }

        [Parameter] public string HeaderClass { get; set; }
        [Parameter] public string FooterClass { get; set; }

        [Parameter] public string HeaderStyle { get; set; }
        [Parameter] public string FooterStyle { get; set; }


        /// <summary>
        /// On click event
        /// </summary>
        [Parameter] public EventCallback<MouseEventArgs> OnRowClick { get; set; }

        private bool _checked;
        public bool IsChecked
        {
            get => _checked;
            set
            {
                if (value != _checked)
                {
                    _checked = value;
                    if (IsCheckable)
                        Table.OnGroupHeaderCheckboxClicked(value, Items.ToList());
                }
            }
        }

        public bool IsExpanded { get; internal set; } = true;

        protected override Task OnInitializedAsync()
        {
            if (GroupDefinition != null)
            {
                ((TableContext<T>)Context)?.GroupRows.Add(this);
                if (GroupDefinition.InnerGroup != null)
                {
                    _innerGroupItems = Table.GetItemsOfGroup(GroupDefinition.InnerGroup, Items);
                }
            }
            return base.OnInitializedAsync();
        }

        public void Dispose()
        {
            ((TableContext<T>)Context)?.GroupRows.Remove(this);
        }

        public void SetChecked(bool b, bool notify)
        {
            if (notify)
                IsChecked = b;
            else
            {
                _checked = b;
                if (IsCheckable)
                    InvokeAsync(StateHasChanged);
            }
        }

        private MudTable<T> Table
        {
            get => (MudTable<T>)((TableContext<T>)Context)?.Table;
        }

    }
}
