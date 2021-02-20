﻿using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudCollapse : MudComponentBase, IDisposable
    {
        private enum CollapseState
        {
            Entering, Entered, Exiting, Exited
        }

        private double _height;
        private int _listenerId;
        private bool _expanded, _isRendered;
        private ElementReference _container, _wrapper;
        private CollapseState _state = CollapseState.Exited;

        protected string Stylename =>
            new StyleBuilder()
            .AddStyle("max-height", $"{MaxHeight?.ToString("#.##", CultureInfo.InvariantCulture)}px", MaxHeight != null)
            .AddStyle("height", "auto", _state == CollapseState.Entered)
            .AddStyle("height", $"{_height.ToString("#.##", CultureInfo.InvariantCulture)}px", _state == CollapseState.Entering || _state == CollapseState.Exiting)
            .AddStyle("animation-duration", $"{CalculatedAnimationDuration.ToString("#.##", CultureInfo.InvariantCulture)}s", _state == CollapseState.Entering)
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
                _state = _expanded ? CollapseState.Entering : CollapseState.Exiting;
                if (_isRendered)
                {
                    _ = UpdateHeight();
                }
                _ = ExpandedChanged.InvokeAsync(_expanded);
            }
        }

        /// <summary>
        /// Explicitly sets the height for the Collapse element to override the css default.
        /// </summary>
        [Parameter] public int? MaxHeight { get; set; }

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        [Parameter] public EventCallback OnAnimationEnd { get; set; }

        [Parameter] public EventCallback<bool> ExpandedChanged { get; set; }

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
                    else if (MaxHeight <= 600) return 0.4;
                    else if (MaxHeight <= 1400) return 0.6;
                    return 1;
                }
                return Math.Min(_height / 800.0, 1);
            }
            set { }
        }

        private async Task UpdateHeight()
        {
            _height = (await _wrapper.MudGetBoundingClientRectAsync())?.Height ?? 0;
            if (MaxHeight != null && _height > MaxHeight)
            {
                _height = MaxHeight.Value;
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                if (_expanded)
                {
                    _state = CollapseState.Entered;
                    StateHasChanged();
                }
                _isRendered = true;
                await UpdateHeight();
                _listenerId = await _container.MudAddEventListenerAsync(DotNetObjectReference.Create(this), "animationend", nameof(AnimationEnd));
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        public void Dispose()
        {
            _ = _container.MudRemoveEventListenerAsync("animationend", _listenerId);
        }

        [JSInvokable]
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
    }
}
