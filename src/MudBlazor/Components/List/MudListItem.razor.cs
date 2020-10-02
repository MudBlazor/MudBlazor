using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Utilities;
using System.Windows.Input;

namespace MudBlazor
{
    public partial class MudListItem : MudComponentBase
    {
        protected string Classname =>
        new CssBuilder("mud-list-item")
          .AddClass("mud-list-item-gutters")
          .AddClass("mud-list-item-clickable", Clickable)
          .AddClass($"mud-ripple", Clickable && !DisableRipple)
          .AddClass(Class)
        .Build();
        [Parameter] public string Text { get; set; }
        [Parameter] public string Avatar { get; set; }
        [Parameter] public string Href { get; set; }
        [Parameter] public string AvatarClass { get; set; }
        [Parameter] public bool DisableRipple { get; set; }
        [Parameter] public string Icon { get; set; }
        [Parameter] public bool Inset { get; set; }
        [Parameter] public bool Expanded { get; set; }
        [Parameter] public object CommandParameter { get; set; }
        [Parameter] public ICommand Command { get; set; }
        [Inject] public Microsoft.AspNetCore.Components.NavigationManager UriHelper { get; set; }
        [CascadingParameter] bool Clickable { get; set; }
        [Parameter] public RenderFragment ChildContent { get; set; }
        [Parameter] public RenderFragment NestedList { get; set; }
        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }
        
        protected void OnClickHandler(MouseEventArgs ev)
        {
            if (NestedList != null)
            {
                Expanded = !Expanded;
            }
            else if (Href != null)
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
