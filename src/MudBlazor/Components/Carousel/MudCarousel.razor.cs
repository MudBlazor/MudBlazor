using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudCarousel<TData> : MudBaseBindableItemsControl<MudCarouselItem, TData>, IAsyncDisposable
    {
        protected string Classname =>
                    new CssBuilder("mud-carousel")
                         .AddClass($"mud-carousel-{_currentColor.ToDescriptionString()}")
                                 .AddClass(Class)
                                 .Build();

        protected string NavigationButtonsClassName =>
                    new CssBuilder()
                        .AddClass($"align-self-{ConvertArrowsPosition(ArrowsPosition).ToDescriptionString()}", !(NavigationButtonsClass ?? "").Contains("align-self-"))
                        .AddClass("mud-carousel-elements-rtl", RightToLeft)
                        .AddClass(NavigationButtonsClass)
                        .Build();

        protected string DelimitersButtonsClassName =>
                    new CssBuilder()
                        .AddClass("align-self-center", !(DelimitersClass ?? "").Contains("align-self-"))
                        .AddClass(DelimitersClass)
                        .Build();

        private Timer _timer;
        private bool _autoCycle = true;
        private Color _currentColor = Color.Inherit;
        private TimeSpan _cycleTimeout = TimeSpan.FromSeconds(5);
        private void _timerElapsed(object stateInfo) => InvokeAsync(async () => await TimerTickAsync());

        private Position ConvertArrowsPosition(Position position)
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

        [CascadingParameter] public bool RightToLeft { get; set; }


        /// <summary>
        /// Gets or Sets if 'Next' and 'Previous' arrows must be visible
        /// </summary>
        [Parameter] public bool ShowArrows { get; set; } = true;

        /// <summary>
        /// Sets the position of the arrows. By default, the position is the Center position
        /// </summary>
        [Parameter] public Position ArrowsPosition { get; set; } = Position.Center;

        /// <summary>
        /// Gets or Sets if bottom bar with Delimiters musb be visible
        /// </summary>
        [Parameter] public bool ShowDelimiters { get; set; } = true;


        /// <summary>
        /// Gets or Sets automatic cycle on item collection
        /// </summary>
        [Parameter]
        public bool AutoCycle
        {
            get => _autoCycle;
            set
            {
                _autoCycle = value;

                if (_autoCycle)
                    InvokeAsync(async () => await ResetTimerAsync());

                else
                    InvokeAsync(async () => await StopTimerAsync());
            }
        }


        /// <summary>
        /// Gets or Sets the Auto Cycle time
        /// </summary>
        [Parameter]
        public TimeSpan AutoCycleTime
        {
            get => _cycleTimeout;
            set
            {
                _cycleTimeout = value;

                if (_autoCycle == true)
                    InvokeAsync(async () => await ResetTimerAsync());

                else
                    InvokeAsync(async () => await StopTimerAsync());
            }
        }


        /// <summary>
        /// Gets or Sets custom class(es) for 'Next' and 'Previous' arrows
        /// </summary>
        [Parameter] public string NavigationButtonsClass { get; set; }

        /// <summary>
        /// Gets or Sets custom class(es) for Delimiters buttons
        /// </summary>
        [Parameter] public string DelimitersClass { get; set; }

        /// <summary>
        /// Custom previous navigation icon.
        /// </summary>
        [Parameter] public string PreviousIcon { get; set; } = Icons.Material.Filled.NavigateBefore;

        /// <summary>
        /// Custom selected delimiter icon.
        /// </summary>
        [Parameter] public string CheckedIcon { get; set; } = Icons.Material.Filled.RadioButtonChecked;

        /// <summary>
        /// Custom unselected delimiter icon.
        /// </summary>
        [Parameter] public string UncheckedIcon { get; set; } = Icons.Material.Filled.RadioButtonUnchecked;

        /// <summary>
        /// Custom next navigation icon.
        /// </summary>
        [Parameter] public string NextIcon { get; set; } = Icons.Material.Filled.NavigateNext;

        /// <summary>
        /// Gets or Sets the Template for the Left Arrow
        /// </summary>
        [Parameter] public RenderFragment NextButtonTemplate { get; set; }


        /// <summary>
        /// Gets or Sets the Template for the Right Arrow
        /// </summary>
        [Parameter] public RenderFragment PreviousButtonTemplate { get; set; }


        /// <summary>
        /// Gets or Sets the Template for Delimiters
        /// </summary>
        [Parameter] public RenderFragment<bool> DelimiterTemplate { get; set; }


        /// <summary>
        /// Fires when selected Index changed on base class
        /// </summary>
        private void SelectionChanged()
        {
            InvokeAsync(async () => await ResetTimerAsync());

            _currentColor = SelectedContainer?.Color ?? Color.Inherit;
        }


        /// <summary>
        /// Provides Selection changes by horizontal swipe gesture
        /// </summary>
        private void OnSwipe(SwipeDirection direction)
        {
            switch (direction)
            {
                case SwipeDirection.LeftToRight:
                    Previous();
                    break;

                case SwipeDirection.RightToLeft:
                    Next();
                    break;
            }
        }

        /// <summary>
        /// Immediately starts the AutoCycle timer
        /// </summary>
        private ValueTask StartTimerAsync()
        {
            if (AutoCycle)
                _timer?.Change(AutoCycleTime, TimeSpan.Zero);

            return ValueTask.CompletedTask;
        }

        /// <summary>
        /// Immediately stops the AutoCycle timer
        /// </summary>
        private ValueTask StopTimerAsync()
        {
            _timer?.Change(Timeout.Infinite, Timeout.Infinite);
            return ValueTask.CompletedTask;
        }

        /// <summary>
        /// Stops and restart the AutoCycle timer
        /// </summary>
        private async ValueTask ResetTimerAsync()
        {
            await StopTimerAsync();
            await StartTimerAsync();
        }


        /// <summary>
        /// Changes the SelectedIndex to a next one (or restart on 0)
        /// </summary>
        private async ValueTask TimerTickAsync()
        {
            await InvokeAsync(Next);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                SelectedIndexChanged = new EventCallback<int>(this, (Action)SelectionChanged);

                _timer = new Timer(_timerElapsed, null, AutoCycle ? AutoCycleTime : Timeout.InfiniteTimeSpan, AutoCycleTime);
            }
        }

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
