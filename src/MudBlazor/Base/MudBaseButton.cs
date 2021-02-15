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
        [CascadingParameter] protected IActivatable Activateable { get; set; }

        /// <summary>
        /// The HTML element that will be rendered in the root by the component
        /// By default, is a button
        /// </summary>
        [Parameter] public string HtmlTag { get; set; } = "button";

        /// <summary>
        /// The button Type (Button, Submit, Refresh)
        /// </summary>
        [Parameter] public ButtonType ButtonType { get; set; }

        /// <summary>
        /// If set to a URL, clicking the button will open the referenced document. Use Target to specify where
        /// </summary>
        [Parameter] public string Link { get; set; }

        /// <summary>
        /// The target attribute specifies where to open the link, if Link is specified. Possible values: _blank | _self | _parent | _top | <i>framename</i>
        /// </summary>
        [Parameter] public string Target { get; set; }

        /// <summary>
        /// If true, the button will be disabled.
        /// </summary>
        [Parameter] public bool Disabled { get; set; }

        /// <summary>
        /// If true, no drop-shadow will be used.
        /// </summary>
        [Parameter] public bool DisableElevation { get; set; }

        /// <summary>
        /// If true, disables ripple effect.
        /// </summary>
        [Parameter] public bool DisableRipple { get; set; }

        /// <summary>
        /// Command executed when the user clicks on an element.
        /// </summary>
        [Parameter] public ICommand Command { get; set; }

        /// <summary>
        /// Command parameter.
        /// </summary>
        [Parameter] public object CommandParameter { get; set; }

        /// <summary>
        /// Button click event.
        /// </summary>
        [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }

        protected async Task OnClickHandler(MouseEventArgs ev)
        {
            if (Disabled)
                return;
            await OnClick.InvokeAsync(ev);
            if (Command?.CanExecute(CommandParameter) ?? false)
            {
                Command.Execute(CommandParameter);
            }
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
            if (Disabled)
            {
                HtmlTag = "button";
                Link = null;
                Target = null;
                return;
            }

            // Render an anchor element if Link property is set and is not disabled
            if (!IsNullOrWhiteSpace(Link))
            {
                HtmlTag = "a";
            }
        }

        protected ElementReference _elementReference;

        public ValueTask FocusAsync() => _elementReference.FocusAsync();
    }
}
