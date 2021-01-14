using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudLink : MudComponentBase
    {
        protected string Classname =>
        new CssBuilder("mud-typography mud-link")
           .AddClass($"mud-{Color.ToDescriptionString()}-text")
          .AddClass($"mud-link-underline-{Underline.ToDescriptionString()}")
          .AddClass($"mud-typography-{Typo.ToDescriptionString()}")
          .AddClass(Class)
        .Build();

        /// <summary>
        /// The color of the component. It supports the theme colors.
        /// </summary>
        [Parameter] public Color Color { get; set; } = Color.Primary;

        /// <summary>
        /// Typography variant to use.
        /// </summary>
        [Parameter] public Typo Typo { get; set; } = Typo.body1;

        /// <summary>
        /// Controls when the link should have an underline.
        /// </summary>
        [Parameter] public Underline Underline { get; set; } = Underline.Hover;

        /// <summary>
        /// The URL, which is the actual link.
        /// </summary>
        [Parameter] public string Href { get; set; }


        [Parameter] public string Target { get; set; }

        /// <summary>
        /// Event to handle OnClick.  Proceeds to Href after click if specified.
        /// </summary>
        [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }

        [Inject] private NavigationManager UriHelper { get; set; }

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        protected async Task OnClickHandler(MouseEventArgs args)
        {
            await OnClick.InvokeAsync(args);
            if (!string.IsNullOrWhiteSpace(Href))
            {
                UriHelper.NavigateTo(Href);
            }
        }
    }
}
