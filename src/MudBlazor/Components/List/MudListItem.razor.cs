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

        /// <summary>
        /// Avatar to use if set.
        /// </summary>
        [Parameter] public string Avatar { get; set; }

        /// <summary>
        /// Link to a URL when clicked.
        /// </summary>
        [Parameter] public string Href { get; set; }

        /// <summary>
        /// Avatar CSS Class to applie if Avtar is set.
        /// </summary>
        [Parameter] public string AvatarClass { get; set; }

        /// <summary>
        /// If true, disables ripple effect.
        /// </summary>
        [Parameter] public bool DisableRipple { get; set; }

        /// <summary>
        /// Icon to use if set.
        /// </summary>
        [Parameter] public string Icon { get; set; }

        /// <summary>
        /// If true, the List Subheader will be indented.
        /// </summary>
        [Parameter] public bool Inset { get; set; }

        /// <summary>
        /// If Nested list and If Expanded true expands the nested list, otherwise collapse it.
        /// </summary>
        [Parameter] public bool Expanded { get; set; }

        /// <summary>
        /// Command parameter.
        /// </summary>
        [Parameter] public object CommandParameter { get; set; }

        /// <summary>
        /// Command executed when the user clicks on an element.
        /// </summary>
        [Parameter] public ICommand Command { get; set; }
        [Inject] public Microsoft.AspNetCore.Components.NavigationManager UriHelper { get; set; }
        [CascadingParameter] bool Clickable { get; set; }
        [Parameter] public RenderFragment ChildContent { get; set; }
        [Parameter] public RenderFragment NestedList { get; set; }

        /// <summary>
        /// List click event.
        /// </summary>
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
