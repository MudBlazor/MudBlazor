using System;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.AspNetCore.Components;

namespace MudBlazor
{
    public abstract class MudDebouncedInput<T> : MudBaseInput<T>, IDisposable
    {
        private Timer _timer;
        private double _debounceInterval;

        /// <summary>
        /// Interval to be awaited in milliseconds before changing the Text value
        /// </summary>
        [Parameter] public double DebounceInterval
        {
            get => _debounceInterval;
            set => SetDebounceIntervalAsync(value).AndForget();
        }

        protected async Task SetDebounceIntervalAsync(double interval)
        {
            if (_debounceInterval != interval)
            {
                _debounceInterval = interval;

                if (_debounceInterval > 0)
                {
                    SetTimer();
                }
                else
                {
                    if (ClearTimer())
                        await OnTimerCompleteGuiThread();
                }
            }
        }

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

            if (DebounceInterval <= 0)
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

        private Task OnTimerCompleteGuiThread()
        {
            base.UpdateValueProperty(_updateTextOnTimerComplete);
            return OnDebounceIntervalElapsed.InvokeAsync(Text);
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            // if input is to be debounced, makes sense to bind the change of the text to oninput
            // so we set Immediate to true
            if (DebounceInterval > 0)
                Immediate = true;
        }

        private void SetTimer()
        {
            if (_timer == null)
            {
                _timer = new Timer();
                _timer.Elapsed += (o, a) => InvokeAsync(OnTimerCompleteGuiThread).AndForget();
                _timer.AutoReset = false;
            }
            _timer.Interval = DebounceInterval;
        }

        private bool ClearTimer()
        {
            var wasStarted = false;

            if (_timer != null)
            {
                wasStarted = _timer.Enabled;

                _timer.Stop();
                _timer.Dispose();
                _timer = null;
            }

            return wasStarted;
        }

        public void Dispose()
        {
            ClearTimer();
        }
    }
}
