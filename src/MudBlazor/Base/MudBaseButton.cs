using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Interfaces;
using static System.String;

namespace MudBlazor
{
#nullable enable
    public abstract class MudBaseButton : MudComponentBase
    {
        /// <summary>
        /// Potential activation target for this button. This enables RenderFragments with user-defined
        /// buttons which will automatically activate the intended functionality.
        /// </summary>
        [CascadingParameter]
        protected IActivatable? Activatable { get; set; }

        [CascadingParameter(Name = "ParentDisabled")]
        private bool ParentDisabled { get; set; }

        /// <summary>
        /// The HTML element that will be rendered in the root by the component
        /// By default, is a button
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.ClickAction)]
        public string HtmlTag { get; set; } = "button";

        /// <summary>
        /// The button Type (Button, Submit, Refresh)
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.ClickAction)]
        public ButtonType ButtonType { get; set; }

        /// <summary>
        /// If set to a URL, clicking the button will open the referenced document. Use Target to specify where
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.ClickAction)]
        public string? Href { get; set; }

        /// <summary>
        /// The target attribute specifies where to open the link, if Href is specified. Possible values: _blank | _self | _parent | _top | <i>framename</i>
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.ClickAction)]
        public string? Target { get; set; }

        /// <summary>
        /// The value of rel attribute for web crawlers. Overrides "noopener" set by <see cref="Target"/> attribute.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.ClickAction)]
        public string? Rel { get; set; }

        /// <summary>
        /// If true, the button will be disabled.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Behavior)]
        public bool Disabled { get; set; }

        /// <summary>
        /// Title of the button, used for accessibility.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Behavior)]
        public string? Title { get; set; }

        /// <summary>
        /// If true, the click event bubbles up to the containing/parent component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Behavior)]
        public bool ClickPropagation { get; set; }

        /// <summary>
        /// Determines whether the component has a drop-shadow. Default is true
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Appearance)]
        public bool DropShadow { get; set; } = true;

        /// <summary>
        /// Gets or sets whether to show a ripple effect when the user clicks the button. Default is true.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Appearance)]
        public bool Ripple { get; set; } = true;

        /// <summary>
        /// Button click event.
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

        protected override void OnInitialized()
        {
            SetDefaultValues();
        }

        protected override void OnParametersSet()
        {
            //if params change, must set default values again
            SetDefaultValues();
        }

        //Set the default value for HtmlTag, Href and Target
        private void SetDefaultValues()
        {
            if (GetDisabledState())
            {
                HtmlTag = "button";
                Href = null;
                Target = null;
                return;
            }

            // Render an anchor element if Href property is set and is not disabled
            if (!IsNullOrWhiteSpace(Href))
            {
                HtmlTag = "a";
            }
        }

        protected ElementReference _elementReference;

        public ValueTask FocusAsync() => _elementReference.FocusAsync();

        protected string? GetRel()
        {
            if (Rel is null && Target == "_blank")
            {
                return "noopener";
            }

            return Rel;
        }
    }
}
