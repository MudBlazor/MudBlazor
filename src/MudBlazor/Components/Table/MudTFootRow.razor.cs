using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudTFootRow : MudComponentBase
    {
        protected string Classname => new CssBuilder("mud-table-row")
            .AddClass(Class).Build();

        [CascadingParameter] public TableContext Context { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Add a multi-select checkbox that will select/unselect every item in the table
        /// </summary>
        [Parameter] public bool IsCheckable { get; set; }

        /// <summary>
        /// Specify behavior in case the table is multi-select mode. If set to <code>true</code>, it won't render an additional empty column.
        /// </summary>
        [Parameter] public bool IgnoreCheckbox { get; set; }

        /// <summary>
        /// Specify behavior in case the table is editable. If set to <code>true</code>, it won't render an additional empty column.
        /// </summary>
        [Parameter] public bool IgnoreEditable { get; set; }

        [Parameter] public bool IsExpandable { get; set; }

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
                        Context.Table.OnHeaderCheckboxClicked(value);
                }
            }
        }

        protected override Task OnInitializedAsync()
        {
            Context?.FooterRows.Add(this);
            return base.OnInitializedAsync();
        }

        public void Dispose()
        {
            Context?.FooterRows.Remove(this);
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
    }
}
