using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    public partial class MudTHeadRow : MudComponentBase
    {
        private bool? _checked = false;

        protected string Classname => new CssBuilder("mud-table-row")
            .AddClass(Class)
            .Build();

        [CascadingParameter]
        public TableContext? Context { get; set; }

        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// Add a multi-select checkbox that will select/unselect every item in the table
        /// </summary>
        [Parameter]
        public bool Checkable { get; set; }

        /// <summary>
        /// Specify behavior in case the table is multi-select mode. If set to <c>true</c>, it won't render an additional empty column.
        /// </summary>
        [Parameter]
        public bool IgnoreCheckbox { get; set; }

        /// <summary>
        /// Specify behavior in case the table is editable. If set to <c>true</c>, it won't render an additional empty column.
        /// </summary>
        [Parameter]
        public bool IgnoreEditable { get; set; }

        [Parameter]
        public bool Expandable { get; set; }

        /// <summary>
        /// On click event
        /// </summary>
        [Parameter]
        public EventCallback<MouseEventArgs> OnRowClick { get; set; }

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
                        Context?.Table?.OnHeaderCheckboxClicked(_checked.HasValue && _checked.Value);
                    }
                }
            }
        }

        protected override Task OnInitializedAsync()
        {
            Context?.HeaderRows.Add(this);
            return base.OnInitializedAsync();
        }

        public void Dispose()
        {
            Context?.HeaderRows.Remove(this);
        }

        public void SetChecked(bool? checkedState, bool notify)
        {
            if (_checked != checkedState)
            {
                if (notify)
                {
                    Checked = checkedState;
                }
                else
                {
                    _checked = checkedState;
                    if (Checkable)
                    {
                        InvokeAsync(StateHasChanged);
                    }
                }
            }
        }
    }
}
