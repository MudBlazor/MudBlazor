using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
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
        /// </summary>
        [Parameter] public string HtmlTag { get; set; }

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

        [Inject] public IJSRuntime JSRuntime { get; set; }

        protected async Task OnClickHandler(MouseEventArgs ev)
        {
            await OnClick.InvokeAsync(ev);
            if (Command?.CanExecute(CommandParameter) ?? false)
            {
                Command.Execute(CommandParameter);
            }
            Activateable?.Activate(this, ev);
        }

        protected override void OnInitialized()
        {
            // Use anchor element if Link property is set
            if (!IsNullOrWhiteSpace(Link))
            {
                HtmlTag = "a";
            }
            else
                // Default tag for a MudButton is "button" if no HtmlTag defined
                if (IsNullOrWhiteSpace(HtmlTag))
            {
                HtmlTag = "button";
            }

            base.OnInitialized();
        }

        protected ElementReference _elementReference;

        public ValueTask FocusAsync()
        {
            return JSRuntime.InvokeVoidAsync("elementReference.focus", _elementReference);
        }
    }
}
