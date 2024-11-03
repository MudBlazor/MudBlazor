using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// Represents a base class for designing selection items.
    /// </summary>
    public abstract class MudBaseSelectItem : MudComponentBase
    {
        [Inject]
        private NavigationManager UriHelper { get; set; } = null!;

        /// <summary>
        /// Prevents the user from interacting with this item.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.General.Behavior)]
        public bool Disabled { get; set; }

        /// <summary>
        /// Shows a ripple effect when the user clicks the button.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.General.Appearance)]
        public bool Ripple { get; set; } = true;

        /// <summary>
        /// The URL to navigate to when this item is clicked.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.General.ClickAction)]
        public string? Href { get; set; }

        /// <summary>
        /// Performs a full page load during navigation.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>. When <c>true</c>, client-side routing is bypassed and the browser is forced to load the new page from the server.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.General.ClickAction)]
        public bool ForceLoad { get; set; }

        /// <summary>
        /// The content within this item.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.General.Behavior)]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// Occurs when the item has been clicked.
        /// </summary>
        /// <remarks>
        /// This event only occurs when the <see cref="Href"/> property is not set.
        /// </remarks>
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
