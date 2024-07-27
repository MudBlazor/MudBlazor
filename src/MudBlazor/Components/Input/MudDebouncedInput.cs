using System.Timers;
using Microsoft.AspNetCore.Components;
using MudBlazor.State;
using Timer = System.Timers.Timer;

namespace MudBlazor
{
    /// <summary>
    /// A base class for designing input components which update after a delay.
    /// </summary>
    /// <typeparam name="T">The type of object managed by this input.</typeparam>
    public abstract class MudDebouncedInput<T> : MudBaseInput<T>
    {
        private Timer _timer;
        private readonly ParameterState<double> _debounceIntervalState;

        protected MudDebouncedInput()
        {
            using var registerScope = CreateRegisterScope();
            _debounceIntervalState = registerScope.RegisterParameter<double>(nameof(DebounceInterval))
                .WithParameter(() => DebounceInterval)
                .WithComparer(DoubleEpsilonEqualityComparer.Default)
                .WithChangeHandler(OnDebounceIntervalChanged);
        }

        /// <summary>
        /// The number of milliseconds to wait before updating the <see cref="MudBaseInput{T}.Text"/> value.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public double DebounceInterval { get; set; }

        /// <summary>
        /// Occurs when the <see cref="DebounceInterval"/> has elapsed.
        /// </summary>
        /// <remarks>
        /// The value in <see cref="MudBaseInput{T}.Text"/> is included in this event.
        /// </remarks>
        [Parameter]
        public EventCallback<string> OnDebounceIntervalElapsed { get; set; }

        protected Task OnChange()
        {
            if (_debounceIntervalState.Value > 0 && _timer != null)
            {
                _timer.Stop();
                return base.UpdateValuePropertyAsync(false);
            }

            return Task.CompletedTask;
        }

        protected override Task UpdateTextPropertyAsync(bool updateValue)
        {
            var suppressTextUpdate = !updateValue
                                     && _debounceIntervalState.Value > 0
                                     && _timer is { Enabled: true }
                                     && (!Value?.Equals(Converter.Get(Text)) ?? false);

            return suppressTextUpdate
                ? Task.CompletedTask
                : base.UpdateTextPropertyAsync(updateValue);
        }

        protected override Task UpdateValuePropertyAsync(bool updateText)
        {
            // This method is called when Value property needs to be refreshed from the current Text property, so typically because Text property has changed.
            // We want to debounce only text-input, not a value being set, so the debouncing is only done when updateText==false (because that indicates the
            // change came from a Text setter)
            if (updateText)
            {
                // we have a change coming not from the Text setter, no debouncing is needed
                return base.UpdateValuePropertyAsync(updateText);
            }
            // if debounce interval is 0 we update immediately
            if (_debounceIntervalState.Value <= 0 || _timer == null)
                return base.UpdateValuePropertyAsync(updateText);
            // If a debounce interval is defined, we want to delay the update of Value property.
            _timer.Stop();
            // restart the timer while user is typing
            _timer.Start();
            return Task.CompletedTask;
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            // if input is to be debounced, makes sense to bind the change of the text to oninput
            // so we set Immediate to true
            if (_debounceIntervalState.Value > 0)
                Immediate = true;
        }

        private void OnDebounceIntervalChanged(ParameterChangedEventArgs<double> args)
        {
            if (args.Value == 0)
            {
                // not debounced, dispose timer if any
                ClearTimer(suppressTick: false);
                return;
            }
            SetTimer();
        }

        private void SetTimer()
        {
            if (_timer == null)
            {
                _timer = new System.Timers.Timer();
                _timer.Elapsed += OnTimerTick;
                _timer.AutoReset = false;
            }
            _timer.Interval = _debounceIntervalState.Value;
        }

        private void OnTimerTick(object sender, ElapsedEventArgs e)
        {
            InvokeAsync(OnTimerTickGuiThread).CatchAndLog();
        }

        private async Task OnTimerTickGuiThread()
        {
            await base.UpdateValuePropertyAsync(false);
            await OnDebounceIntervalElapsed.InvokeAsync(Text);
        }

        private void ClearTimer(bool suppressTick = false)
        {
            if (_timer == null)
                return;
            var wasEnabled = _timer.Enabled;
            _timer.Stop();
            _timer.Elapsed -= OnTimerTick;
            _timer.Dispose();
            _timer = null;
            if (wasEnabled && !suppressTick)
                OnTimerTickGuiThread().CatchAndLog();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            ClearTimer(suppressTick: true);
        }
    }
}
