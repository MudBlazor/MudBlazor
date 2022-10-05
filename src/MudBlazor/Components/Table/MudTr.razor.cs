using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudTr : MudComponentBase
    {
        private bool hasBeenCanceled;
        private bool hasBeenCommitted;
        private bool hasBeenClickedFirstTime;

        internal object _itemCopy;

        protected string Classname => new CssBuilder("mud-table-row")
            .AddClass(Class).Build();

        protected string ActionsStylename => new StyleBuilder()
            .AddStyle("padding-left", "34px", IsExpandable).Build();


        [CascadingParameter] public TableContext Context { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        [Parameter] public object Item { get; set; }

        [Parameter] public bool IsCheckable { get; set; }

        [Parameter] public bool IsEditable { get; set; }

        [Parameter] public bool IsEditing { get; set; }

        [Parameter] public bool IsEditSwitchBlocked { get; set; }

        [Parameter] public bool IsExpandable { get; set; }


        [Parameter]
        public EventCallback<bool> IsCheckedChanged { get; set; }

        private bool _checked;
        [Parameter]
        public bool IsChecked
        {
            get => _checked;
            set
            {
                if (value != _checked)
                {
                    _checked = value;
                    IsCheckedChanged.InvokeAsync(value);
                }
            }
        }

        public void OnRowClicked(MouseEventArgs args)
        {
            if (Context?.Table.IsEditable == true && Context?.Table.IsEditing == true && Context?.Table.IsEditRowSwitchingBlocked == true) return;

            // Manage any previous edited row
            Context.ManagePreviousEditedRow(this);

            if (!(Context?.Table.Validator.IsValid ?? true))
                return;

            Context?.Table.SetSelectedItem(Item);

            // Manage edition the first time the row is clicked and if the table is editable
            if (!hasBeenClickedFirstTime && IsEditable)
            {
                // Sets hasBeenClickedFirstTime to true
                hasBeenClickedFirstTime = true;

                // Set to false that the item has been committed
                // Set to false that the item has been cancelled
                hasBeenCanceled = false;
                hasBeenCommitted = false;

                // Trigger the preview event
                Context?.Table.OnPreviewEditHandler(Item);

                // Trigger the row edit preview event
                Context.Table.RowEditPreview?.Invoke(Item);

                Context?.Table.SetEditingItem(Item);
            }

            if (Context?.Table.MultiSelection == true && !(Context?.Table.IsEditable == true))
            {
                IsChecked = !IsChecked;
            }
            Context?.Table.FireRowClickEvent(args, this, Item);
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

        public void SetChecked(bool b, bool notify)
        {
            if (notify)
                IsChecked = b;
            else
            {
                _checked = b;
                InvokeAsync(StateHasChanged);
            }
        }

        private void FinishEdit(MouseEventArgs ev)
        {
            // Check the validity of the item
            if (!Context?.Table.Validator.IsValid ?? true) return;

            // Set the item value to cancel edit mode
            Context?.Table.SetEditingItem(null);

            // Trigger the commit event
            Context?.Table.OnCommitEditHandler(ev, Item);

            // Trigger the row edit commit event
            Context.Table.RowEditCommit?.Invoke(Item);

            // Set to true that the item has been committed
            // Set to false that the item has been cancelled
            hasBeenCommitted = true;
            hasBeenCanceled = false;

            // Set hasBeenClickedFirstTime to false 
            hasBeenClickedFirstTime = false;
        }

        private void CancelEdit(MouseEventArgs ev)
        {
            // Set the item value to cancel edit mode
            Context?.Table.SetEditingItem(null);

            // Trigger the cancel event
            Context?.Table.OnCancelEditHandler(ev);

            // Trigger the row edit cancel event
            Context?.Table.RowEditCancel?.Invoke(Item);

            // Set to true that the item has been canceled
            // Set to false that the items has been committed
            hasBeenCanceled = true;
            hasBeenCommitted = false;

            // Set hasBeenClickedFirstTime to false 
            hasBeenClickedFirstTime = false;
        }

        public void ManagePreviousEdition()
        {
            // Reset the item to its original value if no cancellation and no commit has been done
            if (!hasBeenCanceled && !hasBeenCommitted)
            {
                // Set the item value to cancel edit mode
                Context?.Table.SetEditingItem(null);

                // Force/indicate a refresh on the component to remove the edition mode for the row
                StateHasChanged();

                // Trigger the row edit cancel event
                Context.Table.RowEditCancel?.Invoke(Item);
            }

            // Reset the variables
            hasBeenCanceled = false;
            hasBeenCommitted = false;
            hasBeenClickedFirstTime = false;
        }
    }
}
