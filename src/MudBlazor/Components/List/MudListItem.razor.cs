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
          .AddClass("mud-list-item-dense", Dense || MudList?.Dense==true)
          .AddClass("mud-list-item-gutters", !DisableGutters && !(MudList?.DisableGutters==true))
          .AddClass("mud-list-item-clickable", MudList?.Clickable)
          .AddClass($"mud-ripple", MudList?.Clickable==true && !DisableRipple)
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
        /// If true, compact vertical padding will be used.
        /// </summary>
        [Parameter] public bool Dense { get; set; }

        /// <summary>
        /// If true, the left and right padding is removed.
        /// </summary>
        [Parameter] public bool DisableGutters { get; set; }

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
        [CascadingParameter] MudList MudList { get; set; }
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

        public Typo textTypo { get; set; }

        protected override void OnParametersSet()
        {
           if(Dense || MudList?.Dense==true)
           {
                textTypo = Typo.body2;
           }
            else if(!Dense || !MudList?.Dense==true)
           {
                textTypo = Typo.body1;
            }
        }
    }
}
