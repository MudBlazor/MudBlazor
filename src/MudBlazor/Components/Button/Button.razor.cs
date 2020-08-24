using System;
using System.Windows.Input;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

using MudBlazor.Utilities;
using MudBlazor.Extensions;

namespace MudBlazor
{
    public class ComponentBaseButton : ComponentBase
    {
        protected string Classname =>
        new CssBuilder("mud-button-root mud-button")
          .AddClass($"mud-button-{Variant.ToDescriptionString()}")
          .AddClass($"mud-button-{Variant.ToDescriptionString()}-{Color.ToDescriptionString()}")
          .AddClass(Class)
        .Build();

        [Inject] public Microsoft.AspNetCore.Components.NavigationManager UriHelper { get; set; }

        [Parameter] public string Icon { get; set; }

        [Parameter] public Color Color { get; set; } = Color.Default;

        [Parameter] public Size Size { get; set; } = Size.Medium;

        [Parameter] public Variant Variant { get; set; } = Variant.Text;

        [Parameter] public string Class { get; set; }

        [Parameter] public bool Disabled { get; set; }

        [Parameter] public string Link { get; set; }

        [Parameter] public bool ForceLoad { get; set; }

        [Parameter] public ICommand Command { get; set; }

        [Parameter] public object CommandParameter { get; set; }

        [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        protected async Task OnClickHandler(MouseEventArgs ev)
        {
            if (Link != null)
            {
                UriHelper.NavigateTo(Link , ForceLoad);
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
