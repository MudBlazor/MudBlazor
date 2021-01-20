using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudCollapse : MudComponentBase
    {
        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        private ElementReference _container;
        private bool _transitionEnds { get; set; }

        /// <summary>
        /// If true, expands the panel, otherwise collapse it. Setting this prop enables control over the panel.
        /// </summary>
        [Parameter] public bool Expanded { get; set; }

        /// <summary>
        /// Explicitly sets the height for the Collapse element to override the css default.
        /// </summary>
        [Parameter] public int? MaxHeight { get; set; }

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        [Parameter] public EventCallback OnTransitionEnd { get; set; }

        /// <summary>
        /// Modified transition duration that scales with height parameter.
        /// Basic implementation for now but should be a math formula to allow it to scale between 0.1s and 1s for the effect to be consistently smooth.
        /// </summary>
        private decimal CalculatedTransitionDuration
        {
            get
            {
                if (MaxHeight.HasValue && Expanded)
                {
                    if (MaxHeight <= 200) { return 0.2m; }
                    else if (MaxHeight <= 600) { return 0.4m; }
                    else if (MaxHeight <= 1400) { return 0.6m; };
                }
                return 1;
            }
            set { }
        }

        public override Task SetParametersAsync(ParameterView parameters)
        {
            var expanded = parameters.GetValueOrDefault<bool>(nameof(Expanded));
            if (expanded != Expanded)
            {
                _transitionEnds = false;
            }

            return base.SetParametersAsync(parameters);
        }

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                JSRuntime.InvokeVoidAsync("addTranstionEndListener", _container, DotNetObjectReference.Create(this));
            }
            return base.OnAfterRenderAsync(firstRender);
        }

        [JSInvokable]
        public void TransitionEnd()
        {
            if (!_transitionEnds)
            {
                _transitionEnds = true;
                OnTransitionEnd.InvokeAsync(Expanded);
                StateHasChanged();
            }
        }

        protected string Classname =>
            new CssBuilder("mud-collapse-container")
            .AddClass($"mud-collapse-expand", Expanded)
            .AddClass($"mud-collapse-expanded", Expanded && _transitionEnds)
            .AddClass(Class)
            .Build();
    }
}
