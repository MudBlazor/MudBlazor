using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudTr : MudComponentBase
    {
        protected string Classname => new CssBuilder("mud-table-row")
            .AddClass(Class).Build();

        [CascadingParameter] public TableContext Context { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        [Parameter] public object Item { get; set; }

        [Parameter] public bool IsCheckable { get; set; }

        [Parameter] public bool IsEditable { get; set; }

        [Parameter] public bool IsHeader { get; set; }
        [Parameter] public bool IsFooter { get; set; }

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
            if (IsHeader || !(Context?.Table.Validator.IsValid ?? true))
                return;

            Context?.Table.SetSelectedItem(Item);
            Context?.Table.SetEditingItem(Item);

            if (Context?.Table.MultiSelection == true && !IsHeader)
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
            if (!Context?.Table.Validator.IsValid ?? true) return;
            Context?.Table.SetEditingItem(null);
            Context?.Table.OnCommitEditHandler(ev, Item);
        }
    }
}
