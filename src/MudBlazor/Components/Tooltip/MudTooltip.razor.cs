using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.State;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    public partial class MudTooltip : MudComponentBase
    {
        private readonly ParameterState<bool> _visibleState;
        private Origin _anchorOrigin;
        private Origin _transformOrigin;

        public MudTooltip()
        {
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
            .AddClass($"mud-tooltip-{ConvertPlacement().ToDescriptionString()}")
            .AddClass("mud-tooltip-arrow", Arrow)
            .AddClass($"mud-border-{Color.ToDescriptionString()}", Arrow && Color != Color.Default)
            .AddClass($"mud-theme-{Color.ToDescriptionString()}", Color != Color.Default)
            .AddClass(Class)
            .Build();

        [CascadingParameter(Name = "RightToLeft")]
        public bool RightToLeft { get; set; }

        /// <summary>
        /// The color of the component. It supports the theme colors.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Tooltip.Appearance)]
        public Color Color { get; set; } = Color.Default;

        /// <summary>
        /// Sets the text to be displayed inside the tooltip.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Tooltip.Behavior)]
        public string? Text { get; set; } = string.Empty;

        /// <summary>
        /// If true, an arrow will be displayed pointing towards the content from the tooltip.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Tooltip.Appearance)]
        public bool Arrow { get; set; } = false;

        /// <summary>
        /// Sets the length of time that the opening transition takes to complete.
        /// </summary>
        /// <remarks>
        /// Set globally via <see cref="MudGlobal.TransitionDefaults.Duration"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Tooltip.Appearance)]
        public double Duration { get; set; } = MudGlobal.TransitionDefaults.Duration.TotalMilliseconds;

        /// <summary>
        /// Sets the amount of time in milliseconds to wait from opening the popover before beginning to perform the transition. 
        /// </summary>
        /// <remarks>
        /// Set globally via <see cref="MudGlobal.TooltipDefaults.Delay"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Tooltip.Appearance)]
        public double Delay { get; set; } = MudGlobal.TooltipDefaults.Delay.TotalMilliseconds;

        /// <summary>
        /// Tooltip placement.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Tooltip.Appearance)]
        public Placement Placement { get; set; } = Placement.Bottom;

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Tooltip.Behavior)]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// Tooltip content. May contain any valid html
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Tooltip.Behavior)]
        public RenderFragment? TooltipContent { get; set; }

        /// <summary>
        /// Determines if this component should be inline with it's surrounding (default) or if it should behave like a block element.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Tooltip.Appearance)]
        public bool Inline { get; set; } = true;

        /// <summary>
        /// Styles applied directly to root component of the tooltip
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Tooltip.Appearance)]
        public string? RootStyle { get; set; }

        /// <summary>
        /// Classes applied directly to root component of the tooltip
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Tooltip.Appearance)]
        public string? RootClass { get; set; }

        /// <summary>
        /// Determines on which events the tooltip will act
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Tooltip.Appearance)]
        public bool ShowOnHover { get; set; } = true;

        /// <summary>
        /// Determines on which events the tooltip will act
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Tooltip.Appearance)]
        public bool ShowOnFocus { get; set; } = true;

        [Parameter]
        [Category(CategoryTypes.Tooltip.Appearance)]
        public bool ShowOnClick { get; set; } = false;

        /// <summary>
        /// The visible state of the Tooltip.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool Visible { get; set; }

        /// <summary>
        /// An event triggered when the state of Visible has changed
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public EventCallback<bool> VisibleChanged { get; set; }

        /// <summary>
        /// If true, the tooltip will be disabled; the popover will not be visible.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool Disabled { get; set; }

        private Task HandlePointerEnterAsync()
        {
            return ShowOnHover ? _visibleState.SetValueAsync(true) : Task.CompletedTask;
        }

        private Task HandlePointerLeaveAsync()
        {
            return ShowOnHover ? _visibleState.SetValueAsync(false) : Task.CompletedTask;
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
