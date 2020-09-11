using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Utilities;
using System.Windows.Input;

namespace MudBlazor
{
    public class ComponentBaseListItem : ComponentBase
    {
        protected string Classname =>
        new CssBuilder("mud-list-item")
          .AddClass($"mud-ripple", !DisableRipple)
          .AddClass(Class)
        .Build();

        [Parameter] public string Class { get; set; }
        [Parameter] public bool Disabled { get; set; }
        [Parameter] public bool DisableRipple { get; set; }
        [Parameter] public string Href { get; set; }
        [Parameter] public RenderFragment ChildContent { get; set; }
        [Parameter] public object CommandParameter { get; set; }
        [Parameter] public ICommand Command { get; set; }
        [Inject] public Microsoft.AspNetCore.Components.NavigationManager UriHelper { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }
        protected void OnClickHandler(MouseEventArgs ev)
        {
            if (Href != null)
            {
                UriHelper.NavigateTo(Href);
            }
            else
            {
                OnClick.InvokeAsync(ev);
                if (Command?.CanExecute(CommandParameter) ?? false)
                {
                    Command.Execute(CommandParameter);
                }
            }
        }
    }
}
