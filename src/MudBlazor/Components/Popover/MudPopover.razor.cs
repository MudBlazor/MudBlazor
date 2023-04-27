using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Diagnostics.CodeAnalysis;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudPopover : MudComponentBase, IAsyncDisposable
    {
        private bool _afterFirstRender;

        [Inject] 
        public IMudPopoverService Service { get; set; }

        protected string PopoverClass =>
           new CssBuilder("mud-popover")
            .AddClass($"mud-popover-fixed", Fixed)
            .AddClass($"mud-popover-open", Open)
            .AddClass($"mud-popover-{TransformOrigin.ToDescriptionString()}")
            .AddClass($"mud-popover-anchor-{AnchorOrigin.ToDescriptionString()}")
            .AddClass($"mud-popover-overflow-{OverflowBehavior.ToDescriptionString()}")
            .AddClass($"mud-popover-relative-width", RelativeWidth)
            .AddClass($"mud-paper", Paper)
            .AddClass($"mud-paper-square", Paper && Square)
            .AddClass($"mud-elevation-{Elevation}", Paper)
            .AddClass($"overflow-y-auto", MaxHeight != null)
            .AddClass(Class)
           .Build();

        protected string PopoverStyles =>
            new StyleBuilder()
            .AddStyle("transition-duration", $"{Duration}ms")
            .AddStyle("transition-delay", $"{Delay}ms")
            .AddStyle("max-height", MaxHeight.ToPx(), MaxHeight != null)
            .AddStyle(Style)
            .Build();

        internal Direction ConvertDirection(Direction direction)
        {
            return direction switch
            {
                Direction.Start => RightToLeft ? Direction.Right : Direction.Left,
                Direction.End => RightToLeft ? Direction.Left : Direction.Right,
                _ => direction
            };
        }

        [CascadingParameter(Name = "RightToLeft")] 
        public bool RightToLeft { get; set; }

        /// <summary>
        /// Sets the maxheight the popover can have when open.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Popover.Appearance)]
        public int? MaxHeight { get; set; } = null;

        /// <summary>
        /// If true, will apply default MudPaper classes.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Popover.Appearance)]
        public bool Paper { get; set; } = true;

        /// <summary>
        /// The higher the number, the heavier the drop-shadow.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Popover.Appearance)]
        public int Elevation { set; get; } = 8;

        /// <summary>
        /// If true, border-radius is set to 0.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Popover.Appearance)]
        public bool Square { get; set; }

        /// <summary>
        /// If true, the popover is visible.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Popover.Behavior)]
        public bool Open { get; set; }

        /// <summary>
        /// If true the popover will be fixed position instead of absolute.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Popover.Behavior)]
        public bool Fixed { get; set; }

        /// <summary>
        /// Sets the length of time that the opening transition takes to complete.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Popover.Appearance)]
        public double Duration { get; set; } = 251;

        /// <summary>
        /// Sets the amount of time in milliseconds to wait from opening the popover before beginning to perform the transition. 
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Popover.Appearance)]
        public double Delay { get; set; } = 0;

        /// <summary>
        /// Sets the direction the popover will start from relative to its parent.
        /// </summary>
        /// 
        [Obsolete("Use AnchorOrigin and TransformOrigin instead.", true)]
        [Parameter] 
        public Direction Direction { get; set; } = Direction.Bottom;

        /// <summary>
        /// Set the anchor point on the element of the popover.
        /// The anchor point will determinate where the popover will be placed.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Popover.Appearance)]
        public Origin AnchorOrigin { get; set; } = Origin.TopLeft;

        /// <summary>
        /// Sets the intersection point if the anchor element. At this point the popover will lay above the popover. 
        /// This property in conjunction with AnchorPlacement determinate where the popover will be placed.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Popover.Appearance)]
        public Origin TransformOrigin { get; set; } = Origin.TopLeft;

        /// <summary>
        /// Set the overflow behavior of a popover and controls how the element should react if there is not enough space for the element to be visible
        /// Defaults to none, which doens't apply any overflow logic
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Popover.Appearance)]
        public OverflowBehavior OverflowBehavior { get; set; } = OverflowBehavior.FlipOnOpen;

        /// <summary>
        /// If true, the select menu will open either above or bellow the input depending on the direction.
        /// </summary>
        [ExcludeFromCodeCoverage]
        [Obsolete("Use AnchorOrigin and TransformOrigin instead.", true)]
        [Parameter] public bool OffsetX { get; set; }

        /// <summary>
        /// If true, the select menu will open either before or after the input depending on the direction.
        /// </summary>
        [ExcludeFromCodeCoverage]
        [Obsolete("Use AnchorOrigin and TransformOrigin instead.", true)]
        [Parameter] public bool OffsetY { get; set; }

        /// <summary>
        /// If true, the popover will have the same width at its parent element, default to false
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Popover.Appearance)]
        public bool RelativeWidth { get; set; } = false;

        /// <summary>
        /// Child content of the component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Popover.Behavior)]
        public RenderFragment ChildContent { get; set; }

        private MudPopoverHandler _handler;

        protected override void OnInitialized()
        {
            _handler = Service.Register(ChildContent ?? new RenderFragment((x) => { }));
            _handler.SetComponentBaseParameters(this, PopoverClass, PopoverStyles, Open);
            base.OnInitialized();
        }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            // henon: this change by PR #3776 caused problems on BSS (#4303)
            //// Only update the fragment if the popover is currently shown or will show
            //// This prevents unnecessary renders and popover handle locking
            //if (!_handler.ShowContent && !Open)
            //    return;

            if (_afterFirstRender)
            {
                await _handler.UpdateFragmentAsync(ChildContent, this, PopoverClass, PopoverStyles, Open);
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await _handler.Initialize();
                await Service.InitializeIfNeeded();
                await _handler.UpdateFragmentAsync(ChildContent, this, PopoverClass, PopoverStyles, Open);
                _afterFirstRender = true;
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        [ExcludeFromCodeCoverage]
        public async ValueTask DisposeAsync()
        {
            try
            {
                await Service.Unregister(_handler);
            }
            catch (JSDisconnectedException) { }
            catch (TaskCanceledException) { }
        }
    }
}
