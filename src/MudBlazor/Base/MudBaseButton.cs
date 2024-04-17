﻿using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Interfaces;
using static System.String;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// Represents a base class for designing button components.
    /// </summary>
    public abstract class MudBaseButton : MudComponentBase
    {
        /// <summary>
        /// Gets or sets any custom activation behavior.
        /// </summary>
        /// <remarks>
        /// Default to <c>null</c>.  This property is used to implement a custom behavior beyond a basic button click.  The activation will occur during the <see cref="OnClick"/> event.
        /// </remarks>
        [CascadingParameter]
        protected IActivatable? Activatable { get; set; }

        [CascadingParameter(Name = "ParentDisabled")]
        private bool ParentDisabled { get; set; }

        /// <summary>
        /// Gets the HTML tag rendered for this component.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>button</c>. 
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Button.ClickAction)]
        public string HtmlTag { get; set; } = "button";

        /// <summary>
        /// Gets or sets the type of button.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>Button</c>. Other values are <c>Submit</c>to submit a form, and <c>Reset</c> to clear a form.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Button.ClickAction)]
        public ButtonType ButtonType { get; set; }

        /// <summary>
        /// Gets or sets the URL to navigate to when the button is clicked.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>. When clicked, the browser will navigate to this URL.  Use the <see cref="Target"/> property to target a specific tab.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Button.ClickAction)]
        public string? Href { get; set; }

        /// <summary>
        /// Gets or sets the browser tab/window opened when a click occurs and <see cref="Href"/> is set.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>. This property allows navigation to open a new tab/window or to reuse a specific tab.  Possible values are <c>_blank</c>, <c>_self</c>, <c>_parent</c>, <c>_top</c>, <c>noopener</c>, or the name of an <c>iframe</c> element.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Button.ClickAction)]
        public string? Target { get; set; }

        /// <summary>
        /// Gets or sets the relationship between the current document and the linked document when <see cref="Href"/> is set.
        /// </summary>
        /// <remarks>
        /// This property is typically used by web crawlers to get more information about a link.  Common values can be found here: <see href="https://www.w3schools.com/tags/att_a_rel.asp" />
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Button.ClickAction)]
        public string? Rel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user can interact with this button.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Button.Behavior)]
        public bool Disabled { get; set; }

        /// <summary>
        /// Gets or sets the title of this button.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  This property is typically used to improve accessibility.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Button.Behavior)]
        public string? Title { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a click event is bubbled up to the parent component.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Button.Behavior)]
        public bool ClickPropagation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a shadow is displayed.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Button.Appearance)]
        public bool DropShadow { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether a ripple effect is shown when the user clicks the button.
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

        /// <summary>
        /// Obtains focus for this button.
        /// </summary>
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
