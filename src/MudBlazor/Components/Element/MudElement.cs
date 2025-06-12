using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// A primitive component which allows dynamically changing the HTML element rendered under the hood.
    /// </summary>
    public class MudElement : MudComponentBase
    {
        /// <summary>
        /// The content within this element.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Element.Misc)]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// The HTML tag rendered for this element.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>span</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Element.Misc)]
        public string HtmlTag { get; set; } = "span";

        /// <summary>
        /// The <see cref="ElementReference"/> to bind to.
        /// </summary>
        /// <remarks>
        /// This is typically bound via <c>@bind-Ref="myRef"</c>.  When this property changes, the <see cref="RefChanged"/> event occurs.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Element.Misc)]
        public ElementReference? Ref { get; set; }

        /// <summary>
        /// Occurs when <see cref="Ref"/> has changed.
        /// </summary>
        [Parameter]
        public EventCallback<ElementReference> RefChanged { get; set; }

        /// <summary>
        /// Propagates click events beyond this element.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Button.Behavior)]
        public bool ClickPropagation { get; set; } = true;

        /// <summary>
        /// Prevents the default action when this element is clicked.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>, allowing default actions.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Button.Behavior)]
        public bool PreventDefault { get; set; }

        /// <summary>
        /// Makes the html element follow the dark/light mode of the application.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Button.Appearance)]
        public bool FollowThemeColorScheme { get; set; } = false;

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            base.BuildRenderTree(builder);

            // Initialize the sequence number.
            // https://learn.microsoft.com/aspnet/core/blazor/advanced-scenarios.
            var seq = 0;

            // Open element.
            builder.OpenElement(seq++, HtmlTag);

            // Splatted attributes.
            builder.AddMultipleAttributes(seq++, UserAttributes!);

            string StyleWithColorScheme = new StyleBuilder()
                .AddStyle(Style)
                .AddStyle("color-scheme", "var(--mud-native-html-color-scheme)", FollowThemeColorScheme)
                .Build();

            // Add class and style attributes.
            builder.AddAttribute(seq++, "class", Class);
            builder.AddAttribute(seq++, "style", StyleWithColorScheme);

            // Add event attributes.
            builder.AddEventStopPropagationAttribute(seq++, "onclick", !ClickPropagation);
            builder.AddEventPreventDefaultAttribute(seq++, "onclick", PreventDefault);

            // Capture the element reference if specified.
            if (Ref != null)
            {
                builder.AddElementReferenceCapture(seq++, async capturedRef =>
                {
                    Ref = capturedRef;
                    await RefChanged.InvokeAsync(Ref.Value);
                });
            }

            // Add child content.
            builder.AddContent(seq++, ChildContent);

            // Close element.
            builder.CloseElement();
        }
    }
}
