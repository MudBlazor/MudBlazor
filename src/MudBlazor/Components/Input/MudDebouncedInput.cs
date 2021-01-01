using System;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.AspNetCore.Components;

namespace MudBlazor
{
    public partial class MudDebouncedInput<T> : MudBaseInput<T>, IDisposable
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

        private bool _updateTextOnTimerComplete;

        protected override void UpdateValueProperty(bool updateText)
        {
            // This method is called when Value property needs to be refreshed from the current Text property, so typically because Text property has changed.
            // If a debounce interval is defined, we want to delay the update of Value property.

            if (DebounceInterval == 0)
            {
                base.UpdateValueProperty(updateText);
            }
            else
            {
                // store the value to use when timer will complete
                _updateTextOnTimerComplete = updateText;

                // restart the timer while user is typing
                _timer.Stop();
                _timer.Start();
            }
        }

        /// <summary>
        /// It triggers when the interval has elapsed
        /// </summary>
        /// <param name="_">discard, not used</param>
        /// <param name="__">discard, not used</param>
        private void OnTimerComplete(object _, ElapsedEventArgs __) => InvokeAsync(OnTimerCompleteGuiThread);
        
        private void OnTimerCompleteGuiThread()
        {
            base.UpdateValueProperty(_updateTextOnTimerComplete);
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
