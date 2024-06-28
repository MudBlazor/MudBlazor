// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.State;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// Represents a form input component which stores a boolean value.
    /// </summary>
    /// <typeparam name="T">The type of item managed by this component.</typeparam>
    public abstract class MudBooleanInput<T> : MudFormComponent<T?, bool?>
    {
        private readonly ParameterState<T?> _valueState;

        protected MudBooleanInput() : base(new BoolConverter<T?>())
        {
            using var registerScope = CreateRegisterScope();
            _valueState = registerScope.RegisterParameter<T?>(nameof(Value))
                .WithParameter(() => Value)
                .WithEventCallback(() => ValueChanged)
                .WithChangeHandler(OnValueChangedAsync);
        }

        /// <summary>
        /// Prevents the user from interacting with this input.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool Disabled { get; set; }

        [CascadingParameter(Name = "ParentDisabled")]
        private bool ParentDisabled { get; set; }

        protected bool GetDisabledState() => Disabled || ParentDisabled;

        /// <summary>
        /// Prevents the user from changing the input.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  When <c>true</c>, the user can copy the input but cannot change it.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool ReadOnly { get; set; }

        [CascadingParameter(Name = "ParentReadOnly")]
        private bool ParentReadOnly { get; set; }

        protected bool GetReadOnlyState() => ReadOnly || ParentReadOnly;

        /// <summary>
        /// The currently selected value.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Data)]
        public T? Value { get; set; }

        /// <summary>
        /// Prevents the parent component from receiving click events.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.  When <c>true</c>, the click will not bubble up to parent components.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool StopClickPropagation { get; set; } = true;

        /// <summary>
        /// Occurs when the <see cref="Value"/> has changed.
        /// </summary>
        [Parameter]
        public EventCallback<T?> ValueChanged { get; set; }

        protected bool? BoolValue => Converter.Set(_valueState.Value);

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                // ensure underlying form component has correct initial value
                _value = Value;
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        protected virtual Task OnChange(ChangeEventArgs args)
            => SetBoolValueAsync((bool?)args.Value, true);

        protected Task SetBoolValueAsync(bool? value, bool? markAsTouched = null)
        {
            if (GetReadOnlyState() is false
                && GetDisabledState() is false
                && markAsTouched is true)
            {
                Touched = true;
            }
            return SetCheckedAsync(Converter.Get(value));
        }

        protected override bool SetConverter(Converter<T?, bool?> value)
        {
            var changed = base.SetConverter(value);
            if (changed)
            {
                SetBoolValueAsync(Converter.Set(_valueState.Value)).CatchAndLog();
            }

            return changed;
        }

        /// <summary>
        /// A value is required, so if not checked we return ERROR.
        /// </summary>
        protected override bool HasValue(T? value)
        {
            return BoolValue == true;
        }

        protected override async Task ResetValueAsync()
        {
            await _valueState.SetValueAsync(default, ParameterStateValueChangeTiming.AfterEventCallbacks);
            await base.ResetValueAsync();
        }


        protected virtual Task OnValueChangedAsync(ParameterChangedEventArgs<T?> args)
            => UpdateFormValueAsync(args.Value);

        private async Task SetCheckedAsync(T? value)
        {
            if (GetReadOnlyState() || GetDisabledState())
            {
                return;
            }

            // let parameter state handle the value, then update the input state
            await _valueState.SetValueAsync(value, ParameterStateValueChangeTiming.AfterEventCallbacks);
            // ParameterState won't call ChangeHandler without ValueChanged, so we have to update the form value manually
            if (ValueChanged.HasDelegate is false)
            {
                await UpdateFormValueAsync(value);
            }
        }

        private async Task UpdateFormValueAsync(T? value)
        {
            _value = value;
            await BeginValidateAsync();
            FieldChanged(_value);
        }
    }
}
