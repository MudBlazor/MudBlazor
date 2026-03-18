using Microsoft.AspNetCore.Components;
using MudBlazor.State;
using MudBlazor.Utilities.Debounce;

namespace MudBlazor
{
    /// <summary>
    /// A base class for designing input components which update after a delay.
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

            // Debounce the update - use fire-and-forget pattern to match the old Timer implementation.
            _ = _debouncer.DebounceAsync(OnDebouncedUpdate);
            return Task.CompletedTask;
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
        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            // if input is to be debounced, makes sense to bind the change of the text to oninput
            // so we set Immediate to true
            if (DebounceInterval > 0)
            {
                // TODO: Don't write to parameter directly
                Immediate = true;
            }
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
                _debouncer = new DebounceDispatcher(TimeSpan.FromMilliseconds(args.Value), false, TimeProvider);
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
