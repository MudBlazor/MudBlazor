using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// Represents a set of slides which transition after a delay.
    /// </summary>
    /// <typeparam name="TData">The kind of item to display.</typeparam>
    public partial class MudCarousel<TData> : MudBaseBindableItemsControl<MudCarouselItem, TData>, IAsyncDisposable
    {
        private Timer? _timer;
        private bool _autoCycle = true;
        private Color _currentColor = Color.Inherit;
        private TimeSpan _cycleTimeout = TimeSpan.FromSeconds(5);

        protected string Classname => new CssBuilder("mud-carousel")
            .AddClass($"mud-carousel-{(BulletsColor ?? _currentColor).ToDescriptionString()}")
            .AddClass(Class)
            .Build();

        protected string NavigationButtonsClassName => new CssBuilder()
            .AddClass($"align-self-{ConvertPosition(ArrowsPosition).ToDescriptionString()}", !(NavigationButtonsClass ?? "").Contains("align-self-"))
            .AddClass("mud-carousel-elements-rtl", RightToLeft)
            .AddClass(NavigationButtonsClass)
            .Build();

        protected string BulletsButtonsClassName => new CssBuilder()
            .AddClass(BulletsClass)
            .Build();

        [CascadingParameter(Name = "RightToLeft")]
        public bool RightToLeft { get; set; }

        /// <summary>
        /// Displays "Next" and "Previous" arrows.
        /// </summary>
        /// <reamrks>
        /// Defaults to <c>true</c>.  
        /// </reamrks>
        [Parameter]
        [Category(CategoryTypes.Carousel.Behavior)]
        public bool ShowArrows { get; set; } = true;

        /// <summary>
        /// The position where the arrows are displayed, if <see cref="ShowArrows"/> is <c>true</c>.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Position.Center"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Carousel.Appearance)]
        public Position ArrowsPosition { get; set; } = Position.Center;

        /// <summary>
        /// Displays a bullet for each <see cref="MudCarouselItem"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Carousel.Behavior)]
        public bool ShowBullets { get; set; } = true;

        /// <summary>
        /// The location of the bullets when <see cref="ShowBullets"/> is <c>true</c>.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Position.Bottom"/>.  
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Carousel.Appearance)]
        public Position BulletsPosition { get; set; } = Position.Bottom;

        /// <summary>
        /// The color of bullets when <see cref="ShowBullets"/> is <c>true</c>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  When <c>null</c> the <see cref="MudCarouselItem.Color"/> property is used.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Carousel.Appearance)]
        public Color? BulletsColor { get; set; }

        /// <summary>
        /// Automatically cycles items based on <see cref="AutoCycleTime"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  When <c>true</c>, the <see cref="MudCarouselItem"/> items will be rotated after the delay specified in <see cref="AutoCycleTime" />.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Carousel.Behavior)]
        public bool AutoCycle
        {
            get => _autoCycle;
            set
            {
                _autoCycle = value;

                if (_autoCycle)
                {
                    InvokeAsync(async () => await ResetTimerAsync());
                }
                else
                {
                    InvokeAsync(async () => await StopTimerAsync());
                }
            }
        }

        /// <summary>
        /// The delay before displaying the next <see cref="MudCarouselItem"/> when <see cref="AutoCycle"/> is <c>true</c>.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="TimeSpan.Zero"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Carousel.Behavior)]
        public TimeSpan AutoCycleTime
        {
            get => _cycleTimeout;
            set
            {
                _cycleTimeout = value;

                if (_autoCycle)
                {
                    InvokeAsync(async () => await ResetTimerAsync());
                }
                else
                {
                    InvokeAsync(async () => await StopTimerAsync());
                }
            }
        }

        /// <summary>
        /// The custom CSS classes for the "Next" and "Previous" icons when <see cref="ShowArrows"/> is <c>true</c>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  Separate each CSS class with spaces.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Carousel.Appearance)]
        public string? NavigationButtonsClass { get; set; }

        /// <summary>
        /// The custom CSS classes for bullets when <see cref="ShowBullets"/> is <c>true</c>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  Separate each CSS class with spaces.
        /// </remarks>
        [Category(CategoryTypes.Carousel.Appearance)]
        [Parameter]
        public string? BulletsClass { get; set; }

        /// <summary>
        /// The "Previous" button icon when <see cref="ShowBullets" /> is <c>true</c> and no <see cref="PreviousButtonTemplate"/> is set.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.NavigateBefore" />.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Carousel.Appearance)]
        public string PreviousIcon { get; set; } = Icons.Material.Filled.NavigateBefore;

        /// <summary>
        /// The icon displayed for the current <see cref="MudCarouselItem"/> when no <see cref="BulletTemplate"/> is set.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Carousel.Appearance)]
        public string CheckedIcon { get; set; } = Icons.Material.Filled.RadioButtonChecked;

        /// <summary>
        /// The icon displayed for unselected <see cref="MudCarouselItem"/>s when no <see cref="BulletTemplate"/> is set.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Carousel.Appearance)]
        public string UncheckedIcon { get; set; } = Icons.Material.Filled.RadioButtonUnchecked;

        /// <summary>
        /// The "Next" button icon when <see cref="ShowBullets" /> is <c>true</c> and no <see cref="NextButtonTemplate"/> is set.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.NavigateNext" />.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Carousel.Appearance)]
        public string NextIcon { get; set; } = Icons.Material.Filled.NavigateNext;

        /// <summary>
        /// The custom template for the "Next" button.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Carousel.Appearance)]
        public RenderFragment? NextButtonTemplate { get; set; }

        /// <summary>
        /// The custom template for the "Previous" button.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Carousel.Appearance)]
        public RenderFragment? PreviousButtonTemplate { get; set; }

        /// <summary>
        /// The custom template for bullets.
        /// </summary>
        /// <remarks>
        /// When set, the template will be used and the <see cref="CheckedIcon"/> and <see cref="UncheckedIcon"/> properties will be ignored.
        /// </remarks>
        [Category(CategoryTypes.Carousel.Appearance)]
        [Parameter]
        public RenderFragment<bool>? BulletTemplate { get; set; }

        /// <summary>
        /// Allows swipe gestures for touch devices.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.  When <c>true</c>, swipe gestures on touch devices can be used to change the current <see cref="MudCarouselItem"/>.
        /// </remarks>
        [Category(CategoryTypes.Carousel.Behavior)]
        [Parameter]
        public bool EnableSwipeGesture { get; set; } = true;

        /// <summary>
        /// Occurs when the <c>SelectedIndex</c> has changed.
        /// </summary>
        protected override void SelectionChanged()
        {
            InvokeAsync(async () => await ResetTimerAsync());

            _currentColor = SelectedContainer?.Color ?? Color.Inherit;
        }

        /// <inheritdoc />
        public override void AddItem(MudCarouselItem item)
        {
            Items.Add(item);
            if (Items.Count - 1 == SelectedIndex)
            {
                _currentColor = item.Color;
                StateHasChanged();
            }
        }

        private void TimerElapsed(object? stateInfo) => InvokeAsync(async () => await TimerTickAsync());

        private static Position ConvertPosition(Position position)
        {
            return position switch
            {
                Position.Top => Position.Start,
                Position.Start => Position.Start,
                Position.Bottom => Position.End,
                Position.End => Position.End,
                _ => position
            };
        }

        /// <summary>
        /// Occurs when a horizontal swipe gesture has completed.
        /// </summary>
        /// <param name="e">A <see cref="SwipeEventArgs"/> describing the swipe direction.</param>
        private void OnSwipeEnd(SwipeEventArgs e)
        {
            if (!EnableSwipeGesture)
            {
                return;
            }

            switch (e.SwipeDirection)
            {
                case SwipeDirection.LeftToRight:
                    if (RightToLeft) Next();
                    else Previous();
                    break;

                case SwipeDirection.RightToLeft:
                    if (RightToLeft) Previous();
                    else Next();
                    break;
            }
        }

        /// <summary>
        /// Starts the auto-cycle timer if <see cref="AutoCycle"/> is <c>true</c>.
        /// </summary>
        private ValueTask StartTimerAsync()
        {
            if (AutoCycle)
            {
                _timer?.Change(AutoCycleTime, TimeSpan.Zero);
            }

            return ValueTask.CompletedTask;
        }

        /// <summary>
        /// Stops the auto-cycle timer.
        /// </summary>
        private ValueTask StopTimerAsync()
        {
            _timer?.Change(Timeout.Infinite, Timeout.Infinite);

            return ValueTask.CompletedTask;
        }

        /// <summary>
        /// Stops and restarts the auto-cycle timer.
        /// </summary>
        private async ValueTask ResetTimerAsync()
        {
            await StopTimerAsync();
            await StartTimerAsync();
        }

        /// <summary>
        /// Changes the selected <see cref="MudCarouselItem"/> to the next one, or restarts at <c>0</c>.
        /// </summary>
        private async ValueTask TimerTickAsync()
        {
            await InvokeAsync(Next);
        }

        /// <inheritdoc />
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                _timer = new Timer(TimerElapsed, null, AutoCycle ? AutoCycleTime : Timeout.InfiniteTimeSpan, AutoCycleTime);
            }
        }

        /// <summary>
        /// Releases resources used by this component.
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            await DisposeAsync(true);
            GC.SuppressFinalize(this);
        }

        protected virtual async ValueTask DisposeAsync(bool disposing)
        {
            if (disposing)
            {
                await StopTimerAsync();

                var timer = _timer;
                if (timer != null)
                {
                    _timer = null;
                    await timer.DisposeAsync();
                }
            }
        }
    }
}
