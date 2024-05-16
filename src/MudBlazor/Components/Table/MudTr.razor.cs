using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
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

        [CascadingParameter]
        public TableContext? Context { get; set; }

        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        [Parameter]
        public object? Item { get; set; }

        [Parameter]
        public bool Checkable { get; set; }

        [Parameter]
        public bool Editable { get; set; }

        [Parameter]
        public bool Expandable { get; set; }

        [Parameter]
        public EventCallback<bool> CheckedChanged { get; set; }

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

        public async Task OnRowClickedAsync(MouseEventArgs args)
        {
            var table = Context?.Table;
            if (table is null)
            {
                return;
            }

            table.SetSelectedItem(Item);
            StartEditingItem(buttonClicked: false);
            if (table is { MultiSelection: true, SelectOnRowClick: true, Editable: false })
            {
                Checked = !Checked;
            }

            await table.FireRowClickEventAsync(args, this, Item);
        }

        public async Task OnRowMouseEnterAsync(MouseEventArgs args)
        {
            var table = Context?.Table;
            if (table is null)
            {
                return;
            }

            await table.FireRowMouseEnterEventAsync(args, this, Item);
        }

        public async Task OnRowMouseLeaveAsync(MouseEventArgs args)
        {
            var table = Context?.Table;
            if (table is null)
            {
                return;
            }

            await table.FireRowMouseLeaveEventAsync(args, this, Item);
        }

        private EventCallback<MouseEventArgs> RowMouseEnterEventCallback
        {
            get
            {
                var hasEventHandler = Context?.Table?.HasRowMouseEnterEventHandler ?? false;

                if (hasEventHandler)
                {
                    return EventCallback.Factory.Create<MouseEventArgs>(this, OnRowMouseEnterAsync);
                }

                return default;
            }
        }

        private EventCallback<MouseEventArgs> RowMouseLeaveEventCallback
        {
            get
            {
                var hasEventHandler = Context?.Table?.HasRowMouseLeaveEventHandler ?? false;

                if (hasEventHandler)
                {
                    return EventCallback.Factory.Create<MouseEventArgs>(this, OnRowMouseLeaveAsync);
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

        public void Dispose()
        {
            Context?.Remove(this, Item);
        }

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
