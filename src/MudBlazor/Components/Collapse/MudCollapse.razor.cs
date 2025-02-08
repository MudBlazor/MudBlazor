using Microsoft.AspNetCore.Components;
using MudBlazor.State;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// Represents a container for content which can be collapsed and expanded.
    /// </summary>
    /// <seealso cref="MudExpansionPanels"/>
    /// <seealso cref="MudExpansionPanel"/>
    public partial class MudCollapse : MudComponentBase
    {
        private enum CollapseState
        {
            Entering, Entered, Exiting, Exited
        }

        private readonly ParameterState<bool> _expandedState;
        private CollapseState _state = CollapseState.Exited;

        protected string Classname => new CssBuilder("mud-collapse-container")
            .AddClass($"mud-collapse-entering", _state == CollapseState.Entering)
            .AddClass($"mud-collapse-entered", _state == CollapseState.Entered)
            .AddClass($"mud-collapse-exiting", _state == CollapseState.Exiting)
            .AddClass(Class)
            .Build();

        protected string Stylename => new StyleBuilder()
            .AddStyle("max-height", MaxHeight.ToPx(), MaxHeight != null)
            .AddStyle(Style)
            .Build();

        /// <summary>
        /// Displays content within this panel.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        public bool Expanded { get; set; }

        /// <summary>
        /// The maximum allowed height of this panel, in pixels.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.
        /// </remarks>
        [Parameter]
        public int? MaxHeight { get; set; }

        /// <summary>
        /// The content within this panel.
        /// </summary>
        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// Occurs when the collapse or expand animation has finished.
        /// </summary>
        [Parameter]
        public EventCallback OnAnimationEnd { get; set; }

        /// <summary>
        /// Occurs when the <see cref="Expanded"/> property has changed.
        /// </summary>
        [Parameter]
        public EventCallback<bool> ExpandedChanged { get; set; }

        public MudCollapse()
        {
            using var register = CreateRegisterScope();
            _expandedState = register.RegisterParameter<bool>(nameof(Expanded))
                .WithParameter(() => Expanded)
                .WithEventCallback(() => ExpandedChanged)
                .WithChangeHandler(OnExpandedParameterChangedAsync);
        }

        private Task OnExpandedParameterChangedAsync(ParameterChangedEventArgs<bool> args)
        {
            _state = args.Value ? CollapseState.Entering : CollapseState.Exiting;

            return Task.CompletedTask;
        }

        private Task AnimationEndAsync()
        {
            if (_state == CollapseState.Entering)
            {
                _state = CollapseState.Entered;
                StateHasChanged();
            }
            else if (_state == CollapseState.Exiting)
            {
                _state = CollapseState.Exited;
                StateHasChanged();
            }

            return OnAnimationEnd.InvokeAsync(_expandedState.Value);
        }
    }
}
