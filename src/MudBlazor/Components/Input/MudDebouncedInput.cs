using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;
using System.Timers;

namespace MudBlazor
{
    public abstract partial class MudDebouncedInput<T> : MudBaseInput<T>, IDisposable
    {

        private Timer _timer;

        /// <summary>
        /// Interval to be awaited in milliseconds before changing the Text value
        /// </summary>
        [Parameter] public double DebounceInterval { get; set; }

        /// <summary>
        /// callback to be called when the debounce interval has elapsed
        /// receives the Text as a parameter
        /// </summary>
        [Parameter] public EventCallback<string> OnDebounceIntervalElapsed { get; set; }

        protected override void StringValueChanged(string text)
        {
            //setting the Text property

            if (DebounceInterval == 0)
            {
                base.StringValueChanged(text);
                return;
            }
            //stops previous timer
            _timer.Stop();

            //starts the timer while user is typing
            _timer.Start();
        }

        /// <summary>
        /// It triggers when the interval has elapsed
        /// </summary>
        /// <param name="_">discard, not used</param>
        /// <param name="__">discard, not used</param>
        private void OnTimerComplete(object _, ElapsedEventArgs __)
        {
            base.StringValueChanged(Text);
            OnDebounceIntervalElapsed.InvokeAsync(Text);
        }

        protected override Task OnParametersSetAsync()
        {
            if (DebounceInterval != 0)
            {
                //if input is to be debounced, makes sense to bind the change of the text to oninput
                //so we set Immediate to true
                Immediate = true;
                SetTimer();
            }
            return base.OnParametersSetAsync();
        }

        /// <summary>
        /// creates the timer 
        /// </summary>
        private void SetTimer()
        {
            _timer = new Timer(DebounceInterval);
            _timer.Elapsed += OnTimerComplete;
            _timer.AutoReset = false;
        }

        /// <summary>
        /// cleaning
        /// </summary>
        public void Dispose()
        {
            if (_timer == null) return;
            _timer.Elapsed -= OnTimerComplete;
            _timer.Dispose();
        }


    }


}
