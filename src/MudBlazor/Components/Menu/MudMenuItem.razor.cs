using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace MudBlazor
{
    public partial class MudMenuItem : MudComponentBase
    {
        [CascadingParameter] public MudMenu MudMenu { get; set; }

        [Parameter] [Category(CategoryTypes.Menu.Behavior)] public RenderFragment ChildContent { get; set; }
        [Parameter] [Category(CategoryTypes.Menu.Behavior)] public bool Disabled { get; set; }

        [Inject] public NavigationManager UriHelper { get; set; }
        [Inject] public IJsApiService JsApiService { get; set; }

        [Parameter] [Category(CategoryTypes.Menu.ClickAction)] public string Link { get; set; }
        [Parameter] [Category(CategoryTypes.Menu.ClickAction)] public string Target { get; set; }
        [Parameter] [Category(CategoryTypes.Menu.ClickAction)] public bool ForceLoad { get; set; }
        [Parameter] [Category(CategoryTypes.Menu.ClickAction)] public ICommand Command { get; set; }
        [Parameter] [Category(CategoryTypes.Menu.ClickAction)] public object CommandParameter { get; set; }

        [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }
        [Parameter] public EventCallback<TouchEventArgs> OnTouch { get; set; }

        protected async Task OnClickHandler(MouseEventArgs ev)
        {
            if (Disabled)
                return;
            MudMenu.CloseMenu();

            if (Link != null)
            {
                if (string.IsNullOrWhiteSpace(Target))
                    UriHelper.NavigateTo(Link, ForceLoad);
                else
                    await JsApiService.Open(Link, Target);
            }
            else
            {
                await OnClick.InvokeAsync(ev);
                if (Command?.CanExecute(CommandParameter) ?? false)
                {
                    Command.Execute(CommandParameter);
                }
            }
        }

        protected internal async Task OnTouchHandler(TouchEventArgs ev)
        {
            if (Disabled)
                return;
            MudMenu.CloseMenu();

            if (Link != null)
            {
                if (string.IsNullOrWhiteSpace(Target))
                    UriHelper.NavigateTo(Link, ForceLoad);
                else
                    await JsApiService.Open(Link, Target);
            }
            else
            {
                await OnTouch.InvokeAsync(ev);
                if (Command?.CanExecute(CommandParameter) ?? false)
                {
                    Command.Execute(CommandParameter);
                }
            }
        }
    }
}
