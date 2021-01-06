using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Utilities;
using System.Threading.Tasks;

namespace MudBlazor
{
    public partial class MudTreeItem : MudComponentBase
    {
        protected string Classname =>
        new CssBuilder("mud-tree-item")
          .AddClass("mud-tree-item-selected", Selected)
          .AddClass(Class)
        .Build();

        [Parameter] public string Text { get; set; }

        [CascadingParameter] MudTree MudTreeRoot { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        [Parameter] public RenderFragment Content { get; set; }

        public bool Expanded { get; set; } = false;

        [Parameter]
        public bool Selected { get; set; } = false;

        [Parameter]
        public EventCallback<bool> SelectedChanged { get; set; }

        /// <summary>
        /// Tree item click event.
        /// </summary>
        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        protected async Task OnClickHandler(MouseEventArgs ev)
        {
            await Select();

            if (ChildContent != null)
            {
                Expanded = !Expanded;
            }

            StateHasChanged();

            await OnClick.InvokeAsync(ev);
        }

        public async Task Select()
        {
            Selected = true;

            await MudTreeRoot.UpdateSelection(this);

            StateHasChanged();

            await SelectedChanged.InvokeAsync(Selected);
        }

        public async Task UnSelect()
        {
            Selected = false;

            StateHasChanged();

            await SelectedChanged.InvokeAsync(Selected);
        }
    }
}
