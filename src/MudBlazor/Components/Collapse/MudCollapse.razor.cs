using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor.State;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// Represents a container for content which can be collapsed and expanded.
    /// </summary>
    public partial class MudCollapse : MudComponentBase
    {
        internal enum CollapseState
        {
            Entering, Entered, Exiting, Exited
        }

        internal double _height;
        private readonly ParameterState<bool> _expandedState;
        private bool _isRendered;
        private bool _updateHeight;
        private ElementReference _wrapper;
        internal CollapseState _state = CollapseState.Exited;

        protected string Stylename => new StyleBuilder()
            .AddStyle("max-height", MaxHeight.ToPx(), MaxHeight != null)
            .AddStyle("height", "auto", _state == CollapseState.Entered)
            .AddStyle("height", _height.ToPx(), _state is CollapseState.Entering or CollapseState.Exiting)
            .AddStyle("animation-duration", $"{CalculatedAnimationDuration.ToString("#.##", CultureInfo.InvariantCulture)}s", _state == CollapseState.Entering)
            .AddStyle(Style)
            .Build();

        protected string Classname => new CssBuilder("mud-collapse-container")
            .AddClass($"mud-collapse-entering", _state == CollapseState.Entering)
            .AddClass($"mud-collapse-entered", _state == CollapseState.Entered)
            .AddClass($"mud-collapse-exiting", _state == CollapseState.Exiting)
            .AddClass(Class)
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

        private async Task OnExpandedParameterChangedAsync()
        {
            if (_isRendered)
            {
                _state = _expandedState.Value ? CollapseState.Entering : CollapseState.Exiting;
                await UpdateHeightAsync();
                _updateHeight = true;
            }
            else if (_expandedState.Value)
            {
                _state = CollapseState.Entered;
            }

            await ExpandedChanged.InvokeAsync(_expandedState.Value);
        }

        /// <summary>
        /// Modified Animation duration that scales with height parameter.
        /// Basic implementation for now but should be a math formula to allow it to scale between 0.1s and 1s for the effect to be consistently smooth.
        /// </summary>
        private double CalculatedAnimationDuration
        {
            get
            {
                return MaxHeight switch
                {
                    null => Math.Min(_height / 800.0, 1),
                    <= 200 => 0.2,
                    <= 600 => 0.4,
                    <= 1400 => 0.6,
                    _ => 1
                };
            }
        }

        internal async Task UpdateHeightAsync()
        {
            try
            {
                _height = (await _wrapper.MudGetBoundingClientRectAsync())?.Height ?? 0;
            }
            catch (Exception ex) when (ex is JSDisconnectedException or TaskCanceledException)
            {
                _height = 0;
            }

            if (_height > MaxHeight)
            {
                _height = MaxHeight.Value;
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _isRendered = true;
                await UpdateHeightAsync();
            }
            else if (_updateHeight && _state is CollapseState.Entering or CollapseState.Exiting)
            {
                _updateHeight = false;
                await UpdateHeightAsync();
                StateHasChanged();
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        /// <summary>
        /// Completes an ongoing animation.
        /// </summary>
        public Task AnimationEndAsync()
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
