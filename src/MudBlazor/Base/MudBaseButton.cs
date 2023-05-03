using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Interfaces;
using static System.String;

namespace MudBlazor
{
    public abstract class MudBaseButton : MudComponentBase
    {
        /// <summary>
        /// Potential activation target for this button. This enables RenderFragments with user-defined
        /// buttons which will automatically activate the intended functionality. 
        /// </summary>
        [CascadingParameter] 
        protected IActivatable Activateable { get; set; }

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
        public string Href { get; set; }
        /// <summary>
        /// If set to a URL, clicking the button will open the referenced document. Use Target to specify where (Obsolete replaced by Href)
        /// </summary>
        
        [Obsolete("Use Href Instead.", false)]
        [Parameter]
        [Category(CategoryTypes.Button.ClickAction)]
        public string Link
        {
            get => Href;
            set => Href = value;
        }

        /// <summary>
        /// The target attribute specifies where to open the link, if Link is specified. Possible values: _blank | _self | _parent | _top | <i>framename</i>
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.ClickAction)]
        public string Target { get; set; }

        /// <summary>
        /// The value of rel attribute for web crawlers. Overrides "noopener" set by <see cref="Target"/> attribute.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.ClickAction)]
        public string Rel { get; set; }

        /// <summary>
        /// If true, the button will be disabled.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Behavior)]
        public bool Disabled { get; set; }
        [CascadingParameter(Name = "ParentDisabled")]
        private bool ParentDisabled { get; set; }
        protected bool GetDisabledState() => Disabled || ParentDisabled;

        /// <summary>
        /// If true, no drop-shadow will be used.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Appearance)]
        public bool DisableElevation { get; set; }

        /// <summary>
        /// If true, disables ripple effect.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Appearance)]
        public bool DisableRipple { get; set; }

        /// <summary>
        /// Command executed when the user clicks on an element.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.ClickAction)]
        [Obsolete($"Use {nameof(OnClick)} instead. This will be removed in v7.")]
        public ICommand Command { get; set; }

        /// <summary>
        /// Command parameter.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.ClickAction)]
        [Obsolete("This will be removed in v7.")]
        public object CommandParameter { get; set; }

        /// <summary>
        /// Button click event.
        /// </summary>
        [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }

        protected virtual async Task OnClickHandler(MouseEventArgs ev)
        {
            if (GetDisabledState())
                return;
            await OnClick.InvokeAsync(ev);
#pragma warning disable CS0618
            if (Command?.CanExecute(CommandParameter) ?? false)
            {
                Command.Execute(CommandParameter);
            }
#pragma warning restore CS0618
            Activateable?.Activate(this, ev);
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

        //Set the default value for HtmlTag, Link and Target 
        private void SetDefaultValues()
        {
            if (GetDisabledState())
            {
                HtmlTag = "button";
                Href = null;
                Target = null;
                return;
            }

            // Render an anchor element if Link property is set and is not disabled
            if (!IsNullOrWhiteSpace(Href))
            {
                HtmlTag = "a";
            }
        }

        protected ElementReference _elementReference;

        public ValueTask FocusAsync() => _elementReference.FocusAsync();

        protected string GetRel()
        {
            if (Rel == null && Target == "_blank")
                return "noopener";
            return Rel;
        }
    }
}
