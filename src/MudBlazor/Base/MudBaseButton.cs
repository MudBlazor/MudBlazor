using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace MudBlazor
{
    public abstract class MudBaseButton : ComponentBase
    {
        [Inject] public Microsoft.AspNetCore.Components.NavigationManager UriHelper { get; set; }

        [Inject] public IJSRuntime JsRuntime { get; set; }

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
                    UriHelper.NavigateTo(Link, ForceLoad);
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
