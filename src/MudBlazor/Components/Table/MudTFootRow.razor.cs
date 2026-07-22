using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable

    /// <summary>
    /// A footer row displayed at the bottom of a <see cref="MudTable{T}"/> and each group.
    /// </summary>
    public partial class MudTFootRow : MudComponentBase
    {
        private bool? _checked = false;

        protected string Classname => new CssBuilder("mud-table-row")
            .AddClass(Class)
            .Build();

        /// <summary>
        /// The current state of the <see cref="MudTable{T}"/> containing this footer.
        /// </summary>
        [CascadingParameter]
        public TableContext? Context { get; set; }

        /// <summary>
        /// The content within this footer row.
        /// </summary>
        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// Shows a checkbox which selects or deselects every row in the group.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        public bool Checkable { get; set; }

        /// <summary>
        /// Prevents the change of the current selection of rows in the group.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.  Requires <see cref="Checkable"/> to be <c>true</c>.
        /// </remarks>
        [Parameter]
        public bool SelectionChangeable { get; set; } = true;

        /// <summary>
        /// Hides the extra column displayed when <see cref="MudTableBase.MultiSelection"/> is <c>true</c>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        public bool IgnoreCheckbox { get; set; }

        /// <summary>
        /// Hides the extra column displayed when <see cref="MudTableBase.Editable"/> is <c>true</c>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        public bool IgnoreEditable { get; set; }

        /// <summary>
        /// Shows an additional left and right margin when the parent group is expandable.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  Managed automatically by table groups.
        /// </remarks>
        [Parameter]
        public bool Expandable { get; set; }

        /// <summary>
        /// Occurs when this footer row is clicked.
        /// </summary>
        [Parameter]
        public EventCallback<MouseEventArgs> OnRowClick { get; set; }

        /// <summary>
        /// The state of the checkbox when <see cref="Checkable"/> is <c>true</c>.
        /// </summary>
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
            Context?.FooterRows.Add(this);
            return base.OnInitializedAsync();
        }

        /// <summary>
        /// Releases resources used by this footer row.
        /// </summary>
        public void Dispose()
        {
            Context?.FooterRows.Remove(this);
        }

        /// <summary>
        /// Sets <see cref="Checked"/> to the specified value.
        /// </summary>
        /// <param name="checkedState">The new checked state.</param>
        /// <param name="notify">When <c>true</c>, the table's <see cref="MudTable{T}.OnHeaderCheckboxClicked(bool)"/> event occurs.</param>
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
