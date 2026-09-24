using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Interfaces;
using MudBlazor.Utilities;
using static System.String;

namespace MudBlazor
{
    /// <summary>
    /// Base class for clickable button components such as <see cref="MudButton"/>, <see cref="MudFab"/>, and <see cref="MudIconButton"/>.
    /// </summary>
    public abstract class MudBaseButton : MudComponentBase
    {
        /// <summary>
        /// Stores the rendered element reference.
        /// </summary>
        /// <remarks>
        /// Kept as a field so the capture does not allocate a closure on every render.
        /// </remarks>
        private readonly Action<ElementReference> _captureElementReference;

        protected MudBaseButton()
        {
            _captureElementReference = reference => _elementReference = reference;
        }

        /// <summary>
        /// The custom activation behavior.
        /// </summary>
        /// <remarks>
        /// Default to <c>null</c>.  This property is used to implement a custom behavior beyond a basic button click.  The activation will occur during the <see cref="OnClick"/> event.
        /// </remarks>
        [CascadingParameter]
        protected IActivatable? Activatable { get; set; }

        [CascadingParameter(Name = "ParentDisabled")]
        private bool ParentDisabled { get; set; }

        /// <summary>
        /// The HTML tag rendered for this component.
        /// </summary>
        /// <remarks>
        /// Defaults to <see href="https://developer.mozilla.org/docs/Web/HTML/Element/Button">button</see>,
        /// or <see href="https://developer.mozilla.org/docs/Web/HTML/Element/a">a</see> if <see cref="Href"/> is set.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Button.ClickAction)]
        public string HtmlTag { get; set; } = "button";

        /// <summary>
        /// The type of button.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>Button</c>. Other values are <c>Submit</c> to submit a form, and <c>Reset</c> to clear a form.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Button.ClickAction)]
        public ButtonType ButtonType { get; set; }

        /// <summary>
        /// The URL to navigate to when the button is clicked.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>. When clicked, the browser will navigate to this URL.  Use the <see cref="Target"/> property to target a specific tab.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Button.ClickAction)]
        public string? Href { get; set; }

        /// <summary>
        /// The browser tab/window opened when a click occurs and <see cref="Href"/> is set.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>. This property allows navigation to open a new tab/window or to reuse a specific tab.  Possible values are <c>_blank</c>, <c>_self</c>, <c>_parent</c>, <c>_top</c>, <c>noopener</c>, or the name of an <c>iframe</c> element.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Button.ClickAction)]
        public string? Target { get; set; }

        /// <summary>
        /// The relationship between the current document and the linked document when <see cref="Href"/> is set.
        /// </summary>
        /// <remarks>
        /// This property is typically used by web crawlers to get more information about a link.  Common values can be found here: <see href="https://www.w3schools.com/tags/att_a_rel.asp" />
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Button.ClickAction)]
        public string? Rel { get; set; }

        /// <summary>
        /// Allows the user to interact with this button.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Button.Behavior)]
        public bool Disabled { get; set; }

        /// <summary>
        /// Allows the click event to bubble up to the parent component.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Button.Behavior)]
        public bool ClickPropagation { get; set; }

        /// <summary>
        /// Displays a shadow.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Button.Appearance)]
        public bool DropShadow { get; set; } = true;

        /// <summary>
        /// Shows a ripple effect when the user clicks the button.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Button.Appearance)]
        public bool Ripple { get; set; } = true;

        /// <summary>
        /// Occurs when this button has been clicked.
        /// </summary>
        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        protected bool GetDisabledState() => Disabled || ParentDisabled;

        protected virtual async Task OnClickHandler(MouseEventArgs ev)
        {
            if (GetDisabledState())
                return;
            await OnClick.InvokeAsync(ev);
            Activatable?.Activate(this, ev);
        }

        /// <summary>
        /// The tag rendered for the root element.
        /// </summary>
        /// <remarks>
        /// Derived from the parameters on every render so that clearing <see cref="Href"/> or enabling the button again restores the tag.
        /// A disabled button always renders a <c>button</c> so the browser can disable it.
        /// </remarks>
        private string GetHtmlTag()
        {
            if (GetDisabledState())
            {
                return "button";
            }

            return IsNullOrWhiteSpace(Href) ? HtmlTag : "a";
        }

        protected ElementReference _elementReference;

        protected bool GetClickPropagation() => GetHtmlTag() != "button" || ClickPropagation;

        /// <summary>
        /// Opens the root element with the attributes every button shares.
        /// The caller renders its content and closes the element.
        /// </summary>
        /// <remarks>
        /// <see cref="HtmlTag"/> is a public parameter with no fixed set of values, so the element is opened by name rather than written as markup.
        /// class and style come before the splat so a class or style supplied through <see cref="MudComponentBase.UserAttributes"/> still wins, which is the precedence the MudElement boundary gave them.
        /// </remarks>
        private protected void OpenRoot(RenderTreeBuilder builder, string classname)
        {
            var disabled = GetDisabledState();
            var htmlTag = GetHtmlTag();

            builder.OpenElement(0, htmlTag);
            builder.AddAttribute(1, "class", classname);
            builder.AddAttribute(2, "style", Style);
            builder.AddMultipleAttributes(3, UserAttributes!);
            builder.AddAttribute(4, "onclick", this.AsNonRenderingEventHandler<MouseEventArgs>(OnClickHandler));
            // On an anchor, type is a MIME type hint for the linked resource, so a button type does not belong there.
            builder.AddAttribute(5, "type", htmlTag == "a" ? null : ButtonType.ToStringFast(true));
            builder.AddAttribute(6, "href", disabled ? null : Href);
            builder.AddAttribute(7, "target", disabled ? null : Target);
            builder.AddAttribute(8, "rel", GetRel());
            builder.AddAttribute(9, "disabled", disabled);
            builder.AddEventStopPropagationAttribute(10, "onclick", !GetClickPropagation());
            builder.AddElementReferenceCapture(11, _captureElementReference);
        }

        /// <summary>
        /// Obtains focus for this button.
        /// </summary>
        public ValueTask FocusAsync() => _elementReference.FocusAsync();

        protected string? GetRel()
        {
            if (GetDisabledState())
            {
                return null;
            }

            if (Rel is null && Target == "_blank")
            {
                return "noopener";
            }

            return Rel;
        }
    }
}
