using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    public partial class MudCollapse : MudComponentBase
    {
        internal enum CollapseState
        {
            Entering, Entered, Exiting, Exited
        }

        internal double _height;
        private bool _expanded, _isRendered, _updateHeight;
        private ElementReference _wrapper;
        internal CollapseState _state = CollapseState.Exited;

        protected string Stylename =>
            new StyleBuilder()
                .AddStyle("max-height", MaxHeight.ToPx(), MaxHeight != null)
                .AddStyle("height", "auto", _state == CollapseState.Entered)
                .AddStyle("height", _height.ToPx(), _state is CollapseState.Entering or CollapseState.Exiting)
                .AddStyle("animation-duration", $"{CalculatedAnimationDuration.ToString("#.##", CultureInfo.InvariantCulture)}s", _state == CollapseState.Entering)
                .AddStyle(Style)
                .Build();

        protected string Classname =>
            new CssBuilder("mud-collapse-container")
                .AddClass($"mud-collapse-entering", _state == CollapseState.Entering)
                .AddClass($"mud-collapse-entered", _state == CollapseState.Entered)
                .AddClass($"mud-collapse-exiting", _state == CollapseState.Exiting)
                .AddClass(Class)
                .Build();

        /// <summary>
        /// If true, expands the panel, otherwise collapse it. Setting this prop enables control over the panel.
        /// </summary>
        [Parameter]
        public bool Expanded
        {
            get => _expanded;
            set
            {
                if (_expanded == value)
                    return;
                _expanded = value;

                if (_isRendered)
                {
                    _state = _expanded ? CollapseState.Entering : CollapseState.Exiting;
                    _ = UpdateHeight();
                    _updateHeight = true;
                }
                else if (_expanded)
                {
                    _state = CollapseState.Entered;
                }

                _ = ExpandedChanged.InvokeAsync(_expanded);
            }
        }

        /// <summary>
        /// Explicitly sets the height for the Collapse element to override the css default.
        /// </summary>
        [Parameter]
        public int? MaxHeight { get; set; }

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        [Parameter]
        public EventCallback OnAnimationEnd { get; set; }

        [Parameter]
        public EventCallback<bool> ExpandedChanged { get; set; }

        /// <summary>
        /// Modified Animation duration that scales with height parameter.
        /// Basic implementation for now but should be a math formula to allow it to scale between 0.1s and 1s for the effect to be consistently smooth.
        /// </summary>
        private double CalculatedAnimationDuration
        {
            get
            {
                if (MaxHeight != null)
                {
                    if (MaxHeight <= 200) return 0.2;
                    if (MaxHeight <= 600) return 0.4;
                    if (MaxHeight <= 1400) return 0.6;
                    return 1;
                }
                return Math.Min(_height / 800.0, 1);
            }
        }

        internal async Task UpdateHeight()
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
                await UpdateHeight();
            }
            else if (_updateHeight && _state is CollapseState.Entering or CollapseState.Exiting)
            {
                _updateHeight = false;
                await UpdateHeight();
                StateHasChanged();
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        [Obsolete($"Use {nameof(AnimationEndAsync)} instead. This will be removed in v7")]
        public void AnimationEnd()
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
            OnAnimationEnd.InvokeAsync(Expanded);
        }

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
            return OnAnimationEnd.InvokeAsync(Expanded);
        }
    }
}
