using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable

    /// <summary>
    /// A row of data within a <see cref="MudTable{T}"/>.
    /// </summary>
    public partial class MudTr : MudComponentBase
    {
        private bool _checked;
        private bool _hasBeenCanceled;
        private bool _hasBeenCommitted;
        private bool _hasBeenClickedFirstTime;

        protected string Classname => new CssBuilder("mud-table-row")
            .AddClass(Class)
            .Build();

        protected string ActionsStylename => new StyleBuilder()
            .AddStyle("padding-left", "34px", Expandable)
            .Build();

        /// <summary>
        /// The current state of the <see cref="MudTable{T}"/> containing this data row.
        /// </summary>
        [CascadingParameter]
        public TableContext? Context { get; set; }

        /// <summary>
        /// The content within this data row.
        /// </summary>
        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// The data being displayed for this row.
        /// </summary>
        [Parameter]
        public object? Item { get; set; }

        /// <summary>
        /// Displays a checkbox at the start of this row.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  Managed automatically by the parent <see cref="MudTable{T}"/>.
        /// </remarks>
        [Parameter]
        public bool Checkable { get; set; }

        /// <summary>
        /// Prevents the change of the current selection.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.  Requires <see cref="Checkable"/> to be <c>true</c>.  Managed automatically by the parent <see cref="MudTable{T}"/>.
        /// </remarks>
        [Parameter]
        public bool SelectionChangeable { get; set; } = true;

        /// <summary>
        /// Allows this row to be edited.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  Managed automatically by the parent <see cref="MudTable{T}"/>.
        /// </remarks>
        [Parameter]
        public bool Editable { get; set; }

        /// <summary>
        /// Allows this row to expand to display nested content.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  Managed automatically by the parent <see cref="MudTable{T}"/>.
        /// </remarks>
        [Parameter]
        public bool Expandable { get; set; }

        /// <summary>
        /// Occurs when <see cref="Checked"/> has changed.
        /// </summary>
        [Parameter]
        public EventCallback<bool> CheckedChanged { get; set; }

        /// <summary>
        /// The state of the checkbox when <see cref="Checkable"/> is <c>true</c>.
        /// </summary>
        [Parameter]
        public bool Checked
        {
            get => _checked;
            set
            {
                if (value != _checked)
                {
                    _checked = value;
                    CheckedChanged.InvokeAsync(value);
                }
            }
        }

        /// <summary>
        /// Occurs when this row is clicked.
        /// </summary>
        /// <param name="args">The mouse coordinates of the click.</param>
        public async Task OnRowClickedAsync(MouseEventArgs args)
        {
            var table = Context?.Table;
            if (table is null)
            {
                return;
            }

            table.SetSelectedItem(Item);
            StartEditingItem(buttonClicked: false);
            if (table is { MultiSelection: true, SelectionChangeable: true, SelectOnRowClick: true, Editable: false })
            {
                Checked = !Checked;
            }

            await table.FireRowClickEventAsync(args, this, Item);
        }

        /// <summary>
        /// Occurs when the pointer enters this row.
        /// </summary>
        /// <param name="args">The coordinates of the pointer.</param>
        public async Task OnRowMouseEnterAsync(PointerEventArgs args)
        {
            var table = Context?.Table;
            if (table is null)
            {
                return;
            }

            await table.FireRowMouseEnterEventAsync(args, this, Item);
        }

        /// <summary>
        /// Occurs when the pointer leaves this row.
        /// </summary>
        /// <param name="args">The coordinates of the pointer.</param>
        public async Task OnRowMouseLeaveAsync(PointerEventArgs args)
        {
            var table = Context?.Table;
            if (table is null)
            {
                return;
            }

            await table.FireRowMouseLeaveEventAsync(args, this, Item);
        }

        private EventCallback<PointerEventArgs> RowMouseEnterEventCallback
        {
            get
            {
                var hasEventHandler = Context?.Table?.HasRowMouseEnterEventHandler ?? false;

                if (hasEventHandler)
                {
                    return EventCallback.Factory.Create<PointerEventArgs>(this, OnRowMouseEnterAsync);
                }

                return default;
            }
        }

        private EventCallback<PointerEventArgs> RowMouseLeaveEventCallback
        {
            get
            {
                var hasEventHandler = Context?.Table?.HasRowMouseLeaveEventHandler ?? false;

                if (hasEventHandler)
                {
                    return EventCallback.Factory.Create<PointerEventArgs>(this, OnRowMouseLeaveAsync);
                }

                return default;
            }
        }

        private void StartEditingItem() => StartEditingItem(buttonClicked: true);

        private void StartEditingItem(bool buttonClicked)
        {
            if (!Editable) return;

            if (Context?.Table?.Editable == true && Context?.Table.Editing == true && Context?.Table.IsEditRowSwitchingBlocked == true) return;

            if ((Context?.Table?.EditTrigger == TableEditTrigger.RowClick && buttonClicked) || (Context?.Table?.EditTrigger == TableEditTrigger.EditButton && !buttonClicked)) return;

            // Manage any previous edited row
            Context?.ManagePreviousEditedRow(this);

            if (!(Context?.Table?.Validator.IsValid ?? true))
                return;

            // Manage edition the first time the row is clicked and if the table is editable
            if (!_hasBeenClickedFirstTime && Editable)
            {
                // Sets hasBeenClickedFirstTime to true
                _hasBeenClickedFirstTime = true;

                // Set to false that the item has been committed
                // Set to false that the item has been canceled
                _hasBeenCanceled = false;
                _hasBeenCommitted = false;

                // Trigger the preview event
                Context?.Table?.OnPreviewEditHandler(Item);

                // Trigger the row edit preview event
                Context?.Table?.RowEditPreview?.Invoke(Item);

                Context?.Table?.SetEditingItem(Item);

                if (Context?.Table is not null)
                {
                    Context.Table.Validator.Model = Item;
                }
            }
        }

        protected override Task OnInitializedAsync()
        {
            Context?.Add(this, Item);
            return base.OnInitializedAsync();
        }

        /// <summary>
        /// Releases resources used by this row.
        /// </summary>
        public void Dispose()
        {
            Context?.Remove(this, Item);
        }

        /// <summary>
        /// Sets <see cref="Checked"/> to the specified value.
        /// </summary>
        /// <param name="checkedState">The new checked state.</param>
        /// <param name="notify">When <c>true</c>, the table's <see cref="MudTable{T}.OnHeaderCheckboxClicked(bool)"/> event occurs.</param>
        public void SetChecked(bool checkedState, bool notify)
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

        private void FinishEdit(MouseEventArgs ev)
        {
            // Check the validity of the item
            if (!(Context?.Table?.Validator.IsValid ?? true)) return;

            // Set the item value to cancel edit mode
            Context?.Table?.SetEditingItem(null);

            // Trigger the commit event
            Context?.Table?.OnCommitEditHandler(ev, Item);

            // Trigger the row edit commit event
            Context?.Table?.RowEditCommit?.Invoke(Item);

            // Set to true that the item has been committed
            // Set to false that the item has been canceled
            _hasBeenCommitted = true;
            _hasBeenCanceled = false;

            // Set hasBeenClickedFirstTime to false 
            _hasBeenClickedFirstTime = false;
        }

        private void CancelEdit(MouseEventArgs ev)
        {
            // Set the item value to cancel edit mode
            Context?.Table?.SetEditingItem(null);

            // Trigger the cancel event
            Context?.Table?.OnCancelEditHandler(ev);

            // Trigger the row edit cancel event
            Context?.Table?.RowEditCancel?.Invoke(Item);

            // Set to true that the item has been canceled
            // Set to false that the items has been committed
            _hasBeenCanceled = true;
            _hasBeenCommitted = false;

            // Set hasBeenClickedFirstTime to false 
            _hasBeenClickedFirstTime = false;
        }

        /// <summary>
        /// Resets this row's editing state.
        /// </summary>
        /// <remarks>
        /// Typically occurs when another row has been selected.  Managed automatically by the parent table.
        /// </remarks>
        public void ManagePreviousEdition()
        {
            // Reset the item to its original value if no cancellation and no commit has been done
            if (!_hasBeenCanceled && !_hasBeenCommitted)
            {
                // Set the item value to cancel edit mode
                Context?.Table?.SetEditingItem(null);

                // Force/indicate a refresh on the component to remove the edition mode for the row
                StateHasChanged();

                // Trigger the row edit cancel event
                Context?.Table?.RowEditCancel?.Invoke(Item);
            }

            // Reset the variables
            _hasBeenCanceled = false;
            _hasBeenCommitted = false;
            _hasBeenClickedFirstTime = false;
        }
    }
}
