using System;
using System.Windows.Input;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor.Utilities;
using MudBlazor.Extensions;

namespace MudBlazor
{
    public class ComponentBaseIconButton : ComponentBase
    {
        protected string Classname =>
        new CssBuilder("mud-button-root mud-icon-button")
          .AddClass($"mud-icon-button-color-{Color.ToDescriptionString()}")
          .AddClass($"mud-ripple mud-ripple-icon", !DisableRipple)
          .AddClass($"mud-icon-button-size-{Size.ToDescriptionString()}", when: () => Size != Size.Medium)
          .AddClass($"mud-icon-button-edge-{Edge.ToDescriptionString()}", when: () => Edge != Edge.False)
          .AddClass(Class)
        .Build();

        [Inject] public Microsoft.AspNetCore.Components.NavigationManager UriHelper { get; set; }

        [Inject] public IJSRuntime JsRuntime { get; set; }

        [Parameter] public string Icon { get; set; }

        [Parameter] public string ToggleIcon { get; set; }

        [Parameter] public Color Color { get; set; } = Color.Default;

        [Parameter] public Size Size { get; set; } = Size.Medium;

        [Parameter] public Edge Edge { get; set; }

        [Parameter] public string Class { get; set; }

        [Parameter] public bool DisableRipple { get; set; }

        [Parameter] public bool Disabled { get; set; }

        [Parameter] public string Link { get; set; }

        [Parameter] public string Target { get; set; }

        [Parameter] public bool ForceLoad { get; set; }

        [Parameter] public ICommand Command { get; set; }

        [Parameter] public object CommandParameter { get; set; }

        [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }

        protected async Task OnClickHandler(MouseEventArgs ev)
        {
            if (Link != null)
            {
                if (string.IsNullOrWhiteSpace(Target))
                    UriHelper.NavigateTo(Link , ForceLoad);
                else
                    await JsRuntime.InvokeAsync<object>("open", Link, Target);
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
    }
}
