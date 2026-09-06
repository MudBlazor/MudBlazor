// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor.State;
using MudBlazor.Utilities;

namespace MudBlazor
{

    /// <summary>
    /// Displays additional context when users hover over or focus on an element.
    /// </summary>
    public partial class MudTooltip : MudComponentBase, IAsyncDisposable
    {
        private int _parentUpdateCount;
        private readonly ParameterState<bool> _visibleState;
        private Origin _anchorOrigin;
        private Origin _transformOrigin;
        private ElementReference _rootRef;
        private readonly Lazy<DotNetObjectReference<MudTooltip>> _dotNetReferenceLazy;

        [Inject]
        private IJSRuntime JsRuntime { get; set; } = null!;

        [DynamicDependency(nameof(OnHoverChangedAsync))]
        public MudTooltip()
        {
            _dotNetReferenceLazy = new Lazy<DotNetObjectReference<MudTooltip>>(() => DotNetObjectReference.Create(this));

            using var registerScope = CreateRegisterScope();
            _visibleState = registerScope.RegisterParameter<bool>(nameof(Visible))
                .WithParameter(() => Visible)
                .WithEventCallback(() => VisibleChanged);
        }

        protected string ContainerClass => new CssBuilder("mud-tooltip-root")
            .AddClass("mud-tooltip-inline", Inline)
            .AddClass(RootClass)
            .Build();

        protected string Classname => new CssBuilder("mud-tooltip")
            .AddClass("d-flex")
            .AddClass("mud-tooltip-default", Color == Color.Default)
            .AddClass($"mud-tooltip-{ConvertPlacement().ToStringFast(true)}")
            .AddClass("mud-tooltip-arrow", Arrow)
            .AddClass($"mud-border-{Color.ToStringFast(true)}", Arrow && Color != Color.Default)
            .AddClass($"mud-theme-{Color.ToStringFast(true)}", Color != Color.Default)
            .AddClass(Class)
            .Build();

        /// <summary>
        /// Displays content right-to-left.
        /// </summary>
        [CascadingParameter(Name = "RightToLeft")]
        public bool RightToLeft { get; set; }

        /// <summary>
        /// The tooltip color.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Color.Default"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Tooltip.Appearance)]
        public Color Color { get; set; } = Color.Default;

        /// <summary>
        /// The tooltip text.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Tooltip.Behavior)]
        public string? Text { get; set; } = string.Empty;

        /// <summary>
        /// Displays an arrow pointing towards the tooltip content.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Tooltip.Appearance)]
        public bool Arrow { get; set; } = false;

        /// <summary>
        /// The length of time to animate the opening transition.
        /// </summary>
        /// <remarks>
        /// Defaults to 251ms in <see cref="MudGlobal.TooltipDefaults.Duration"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Tooltip.Appearance)]
        public double Duration { get; set; } = MudGlobal.TooltipDefaults.Duration.TotalMilliseconds;

        /// <summary>
        /// The amount of time, in milliseconds, to wait from opening the popover before performing the transition. 
        /// </summary>
        /// <remarks>
        /// Defaults to 0ms in <see cref="MudGlobal.TooltipDefaults.Delay"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Tooltip.Appearance)]
        public double Delay { get; set; } = MudGlobal.TooltipDefaults.Delay.TotalMilliseconds;

        /// <summary>
        /// The location of the tooltip relative to its content.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Placement.Bottom"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Tooltip.Appearance)]
        public Placement Placement { get; set; } = Placement.Bottom;

        /// <summary>
        /// The content described by this tooltip.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Tooltip.Behavior)]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// The content of the tooltip.
        /// </summary>
        /// <remarks>
        /// Can contain any valid HTML.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Tooltip.Behavior)]
        public RenderFragment? TooltipContent { get; set; }

        /// <summary>
        /// Displays this tooltip inline with its container.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>. When <c>false</c>, the content will display as a block element.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Tooltip.Appearance)]
        public bool Inline { get; set; } = true;

        /// <summary>
        /// Any CSS styles applied to the tooltip.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Tooltip.Appearance)]
        public string? RootStyle { get; set; }

        /// <summary>
        /// Any CSS classes applied to the tooltip.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Tooltip.Appearance)]
        public string? RootClass { get; set; }

        /// <summary>
        /// Shows this tooltip when hovering over its content.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Tooltip.Appearance)]
        public bool ShowOnHover { get; set; } = true;

        /// <summary>
        /// Shows this tooltip when its content is focused.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Tooltip.Appearance)]
        public bool ShowOnFocus { get; set; } = true;

        /// <summary>
        /// Shows this tooltip when its content is clicked.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Tooltip.Appearance)]
        public bool ShowOnClick { get; set; } = false;

        /// <summary>
        /// Shows this tooltip.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter, ParameterState]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool Visible { get; set; }

        /// <summary>
        /// Occurs when <see cref="Visible"/> has changed.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public EventCallback<bool> VisibleChanged { get; set; }

        /// <summary>
        /// Prevents this tooltip from being displayed.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool Disabled { get; set; }

        /// <summary>
        /// Gets whether the tooltip should be shown.
        /// </summary>
        /// <remarks>
        /// The tooltip will be displayed if not disabled, not already visible, and either <see cref="TooltipContent"/> or <see cref="Text"/> is specified.
        /// </remarks>
        internal bool ShowToolTip()
        {
            return !Disabled && (TooltipContent is not null || !string.IsNullOrEmpty(Text));
        }

        /// <inheritdoc />
        public override Task SetParametersAsync(ParameterView parameters)
        {
            unchecked { _parentUpdateCount++; }

            return base.SetParametersAsync(parameters);
        }

        /// <inheritdoc />
        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            ConvertPlacement();
        }

        /// <inheritdoc />
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                // The wrapper is display:contents (issue #1167) and has no box, so pointerenter/leave
                // never fire on it. Bridge hover through the bubbling pointerover/pointerout events.
                await JsRuntime.InvokeVoidAsyncWithErrorHandling("mudElementRef.addTooltipHover", _rootRef, _dotNetReferenceLazy.Value);
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            if (IsJSRuntimeAvailable)
            {
                await JsRuntime.InvokeVoidAsyncWithErrorHandling("mudElementRef.removeTooltipHover", _rootRef);
            }

            if (_dotNetReferenceLazy.IsValueCreated)
            {
                _dotNetReferenceLazy.Value.Dispose();
            }

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Invoked from JavaScript (via the hover bridge installed by <c>mudElementRef.addTooltipHover</c>)
        /// when the pointer enters or leaves the tooltip's content. The bridge is used because the
        /// <c>display:contents</c> wrapper generates no box, so the native pointerenter/leave events
        /// never fire on it; the bridge listens to the bubbling pointerover/pointerout events and ignores
        /// moves that stay within the wrapper subtree.
        /// </summary>
        /// <param name="hovered">Whether the pointer is now within the tooltip's content.</param>
        [JSInvokable]
        public async Task OnHoverChangedAsync(bool hovered)
        {
            if (!ShowOnHover)
            {
                return;
            }

            await _visibleState.SetValueAsync(hovered);
            // Unlike the UI event handlers (focus/click), a JS interop callback does not trigger an
            // automatic re-render, so request one explicitly to open/close the popover.
            await InvokeAsync(StateHasChanged);
        }

        private Task HandleFocusInAsync()
        {
            return ShowOnFocus ? _visibleState.SetValueAsync(true) : Task.CompletedTask;
        }

        private Task HandleFocusOutAsync()
        {
            return ShowOnFocus ? _visibleState.SetValueAsync(false) : Task.CompletedTask;
        }

        private Task HandlePointerUpAsync()
        {
            return ShowOnClick ? _visibleState.SetValueAsync(!_visibleState.Value) : Task.CompletedTask;
        }

        private Origin ConvertPlacement()
        {
            if (Placement == Placement.Bottom)
            {
                _anchorOrigin = Origin.BottomCenter;
                _transformOrigin = Origin.TopCenter;

                return Origin.BottomCenter;
            }

            if (Placement == Placement.Top)
            {
                _anchorOrigin = Origin.TopCenter;
                _transformOrigin = Origin.BottomCenter;

                return Origin.TopCenter;
            }

            if (Placement == Placement.Left || (Placement == Placement.Start && !RightToLeft) || (Placement == Placement.End && RightToLeft))
            {
                _anchorOrigin = Origin.CenterLeft;
                _transformOrigin = Origin.CenterRight;

                return Origin.CenterLeft;
            }

            if (Placement == Placement.Right || (Placement == Placement.End && !RightToLeft) || (Placement == Placement.Start && RightToLeft))
            {
                _anchorOrigin = Origin.CenterRight;
                _transformOrigin = Origin.CenterLeft;

                return Origin.CenterRight;
            }

            return Origin.BottomCenter;
        }
    }
}
