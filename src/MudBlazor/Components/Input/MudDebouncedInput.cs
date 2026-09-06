using Microsoft.AspNetCore.Components;
using MudBlazor.State;
using MudBlazor.Utilities.Debounce;

namespace MudBlazor
{
    /// <summary>
    /// Base class for MudBlazor inputs that wait for typing to pause before updating their value, such as <see cref="MudTextField{T}"/> and <see cref="MudNumericField{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of object managed by this input.</typeparam>
    public abstract class MudDebouncedInput<T> : MudBaseInput<T>
    {
        private DebounceDispatcher? _debouncer;

        protected MudDebouncedInput()
        {
            using var registerScope = CreateRegisterScope();
            registerScope.RegisterParameter<double>(nameof(DebounceInterval))
                .WithParameter(() => DebounceInterval)
                .WithComparer(DoubleEpsilonEqualityComparer.Default)
                .WithChangeHandler(OnDebounceIntervalChangedAsync);
        }

        [Inject]
        private TimeProvider TimeProvider { get; set; } = null!;

        /// <summary>
        /// The number of milliseconds to wait before updating the <see cref="MudBaseInput{T}.Text"/> value.
        /// </summary>
        [Parameter, ParameterState(ParameterUsage = ParameterUsageOptions.None)]
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

        /// <inheritdoc />
        protected internal override bool EffectiveImmediate => Immediate || DebounceInterval > 0;

        /// <inheritdoc />
        protected override Task UpdateTextPropertyAsync(bool updateValue)
        {
            // Don't update text if we're debouncing and the value hasn't actually changed
            var suppressTextUpdate = !updateValue
                                     && DebounceInterval > 0
                                     && _debouncer is not null
                                     && _debouncer.IsPending;

            return suppressTextUpdate
                ? Task.CompletedTask
                : base.UpdateTextPropertyAsync(updateValue);
        }

        /// <inheritdoc />
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
            // if debounce interval is 0 or no debouncer, we update immediately
            if (DebounceInterval <= 0 || _debouncer is null)
            {
                return base.UpdateValuePropertyAsync(updateText);
            }

            // Don't wait out the interval, but do wait for the debounce to be scheduled.
            // Scheduling happens after an await inside the debouncer, so returning before it lets a caller act on a debounce that does not exist yet.
            var scheduled = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            _ = DebounceAndSignalAsync(scheduled);

            return scheduled.Task;
        }

        /// <summary>
        /// Runs the debounce and completes <paramref name="scheduled"/> once its timer exists.
        /// </summary>
        /// <remarks>
        /// The task is also completed when the debounce ends without scheduling, such as after disposal, so a caller is never left waiting.
        /// </remarks>
        private async Task DebounceAndSignalAsync(TaskCompletionSource scheduled)
        {
            try
            {
                await _debouncer!.DebounceCoreAsync(OnDebouncedUpdate, () => scheduled.TrySetResult());
            }
            finally
            {
                scheduled.TrySetResult();
            }
        }

        /// <inheritdoc />
        protected override async Task ValidateValue()
        {
            if (await SynchronizePendingValueForValidationAsync())
            {
                return;
            }

            await base.ValidateValue();
        }

        /// <inheritdoc />
        protected override void OnInitialized()
        {
            base.OnInitialized();
            // The DebounceInterval change handler only runs for values that arrive through the ParameterView.
            // A value coming from a property initializer or from the constructor of a derived component never
            // does, so seed the debouncer here from the initial value.
            _debouncer ??= CreateDebouncer(DebounceInterval);
        }

        private async Task OnDebounceIntervalChangedAsync(ParameterChangedEventArgs<double> args)
        {
            if (args.Value <= 0)
            {
                // not debounced, dispose debouncer if any
                _debouncer?.Dispose();
                _debouncer = null;
                return;
            }

            // Create debouncer if we don't have one
            if (_debouncer is null)
            {
                _debouncer = CreateDebouncer(args.Value);
            }
            else
            {
                // Only update interval if it has meaningfully changed
                // Use DoubleEpsilonEqualityComparer to avoid unnecessary updates due to floating-point precision
                if (!DoubleEpsilonEqualityComparer.Default.Equals(args.LastValue, args.Value))
                {
                    await _debouncer.UpdateIntervalAsync(TimeSpan.FromMilliseconds(args.Value));
                }
            }
        }

        private DebounceDispatcher? CreateDebouncer(double intervalMilliseconds) => intervalMilliseconds > 0
            ? new DebounceDispatcher(TimeSpan.FromMilliseconds(intervalMilliseconds), false, TimeProvider)
            : null;

        private async Task<bool> SynchronizePendingValueForValidationAsync()
        {
            if (DebounceInterval <= 0 || _debouncer is null || !_debouncer.IsPending)
            {
                return false;
            }

            var pendingValue = ConvertGet(ReadText);
            var pendingValueChanged = !EqualityComparer<T?>.Default.Equals(ReadValue, pendingValue);

            await _debouncer.CancelAsync();

            if (!pendingValueChanged)
            {
                return false;
            }

            // SetValueAndUpdateTextAsync already triggers FieldChanged and BeginValidateAsync,
            // so the synced validation happens there and this call can stop.
            await SetValueAndUpdateTextAsync(pendingValue, updateText: false);
            return true;
        }

        private Task OnDebouncedUpdate()
        {
            return InvokeAsync(async () =>
            {
                await base.UpdateValuePropertyAsync(false);
                await OnDebounceIntervalElapsed.InvokeAsync(ReadText);
            });
        }

        /// <inheritdoc />
        protected override async ValueTask DisposeAsyncCore()
        {
            await base.DisposeAsyncCore();
            _debouncer?.Dispose();
            _debouncer = null;
        }
    }
}
