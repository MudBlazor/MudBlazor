using System;
using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudCarousel<TData> : MudBaseBindableItemsControl<MudCarouselItem, TData>, IDisposable
    {

        protected string Classname =>
                    new CssBuilder("mud-carousel")
                         .AddClass($"mud-carousel-{_currentColor.ToDescriptionString()}")
                                 .AddClass(Class)
                                 .Build();

        protected string NavigationButtonsClassName =>
                    new CssBuilder("align-self-center")
                        .AddClass("mud-carousel-elements-rtl", RightToLeft)
                        .Build();


        private System.Timers.Timer _autoCycleTimer;

        internal Color _currentColor = Color.Inherit;

        [CascadingParameter] public bool RightToLeft { get; set; }

        /// <summary>
        /// Gets or Sets if 'Next' and 'Previous' arrows must be visible
        /// </summary>
        [Parameter] public bool ShowArrows { get; set; } = true;

        /// <summary>
        /// Gets or Sets if bottom bar with Delimiters musb be visible
        /// </summary>
        [Parameter] public bool ShowDelimiters { get; set; } = true;

        private bool _autoCycleField = true;
        /// <summary>
        /// Gets or Sets automatic cycle on item collection
        /// </summary>
        [Parameter] public bool AutoCycle
        {
            get => _autoCycleField;
            set
            {
                _autoCycleField = value;
                if (AutoCycle == true)
                    Timer_Reset();
                else
                    Timer_Stop();
            }
        }

        public TimeSpan _autoCycleTimeField = TimeSpan.FromSeconds(5);
        /// <summary>
        /// Gets or Sets the Auto Cycle time
        /// </summary>
        [Parameter] public TimeSpan AutoCycleTime
        {
            get => _autoCycleTimeField;
            set
            {
                _autoCycleTimeField = value;
                if (AutoCycle == true)
                    Timer_Reset();
            }
        }

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
        [Parameter]  public RenderFragment<bool> DelimiterTemplate { get; set; }

        /// <summary>
        /// Fires when selected Index changed on base class
        /// </summary>
        private void Selection_Changed()
        {
            Timer_Reset();
            _currentColor = (SelectedContainer != null ? SelectedContainer.Color : Color.Inherit);
        }

        /// <summary>
        /// Provides Selection changes by horizontal swipe gesture
        /// </summary>
        private void OnSwipe(SwipeDirection direction)
        {
            if (direction == SwipeDirection.LeftToRight)
                Previous();
            else if (direction == SwipeDirection.RightToLeft)
                Next();
        }

        /// <summary>
        /// Prepares the AutoCycle's timer for start its ticks, based on AutoCycleTime value
        /// </summary>
        private void Timer_Start()
        {
            if (AutoCycle == false)
                return;
            _autoCycleTimer = new System.Timers.Timer(AutoCycleTime.TotalMilliseconds)
            {
                Enabled = true
            };
            _autoCycleTimer.Elapsed += Timer_Tick;
        }

        /// <summary>
        /// Stops the AutoCycle's timer with immediate effect
        /// </summary>
        private void Timer_Stop()
        {
            if (_autoCycleTimer != null)
            {
                _autoCycleTimer.Elapsed -= Timer_Tick;
                _autoCycleTimer.Dispose();
            }
        }

        /// <summary>
        /// Resets the AutoCycle's timer (ex: when user mannualy change the selected index)
        /// </summary>
        private void Timer_Reset()
        {
            Timer_Stop();
            Timer_Start();
        }

        /// <summary>
        /// Changes the SelectedIndex to a next one (or restart on 0)
        /// </summary>
        private void Timer_Tick(object source, System.Timers.ElapsedEventArgs e)
        {
            InvokeAsync(Next);
        }


        protected override void OnInitialized()
        {
            base.OnInitialized();
            SelectedIndexChanged = new EventCallback<int>(this, (Action)Selection_Changed);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Timer_Stop();
            }
        }


    }
}
