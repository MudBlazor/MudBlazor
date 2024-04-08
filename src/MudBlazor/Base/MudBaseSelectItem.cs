using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace MudBlazor
{
#nullable enable
    public abstract class MudBaseSelectItem : MudComponentBase
    {
        [Inject]
        private NavigationManager UriHelper { get; set; } = null!;

        /// <summary>
        /// If true, the input element will be disabled.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.General.Behavior)]
        public bool Disabled { get; set; }

        /// <summary>
        /// Gets or sets whether to show a ripple effect when the user clicks the button. Default is true.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.General.Appearance)]
        public bool Ripple { get; set; } = true;

        /// <summary>
        /// Link to a URL when clicked.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.General.ClickAction)]
        public string? Href { get; set; }

        /// <summary>
        /// If true, force browser to redirect outside component router-space.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.General.ClickAction)]
        public bool ForceLoad { get; set; }

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.General.Behavior)]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// Click event. Will not be called if <c>Href</c> is also set.
        /// </summary>
        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        protected async Task OnClickHandler(MouseEventArgs ev)
        {
            if (Disabled)
                return;
            if (Href != null)
            {
                UriHelper.NavigateTo(Href, ForceLoad);
            }
            else
            {
                await OnClick.InvokeAsync(ev);
            }
        }
    }
}
