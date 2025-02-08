// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;
using MudBlazor.Utilities.Exceptions;

namespace MudBlazor
{
#nullable enable

    /// <summary>
    /// A group of <see cref="MudRadio{T}"/> components.
    /// </summary>
    /// <typeparam name="T">The type of value being selected.</typeparam>
    public partial class MudRadioGroup<T> : MudFormComponent<T, T>, IMudRadioGroup
    {
        private MudRadio<T>? _selectedRadio;
        private HashSet<MudRadio<T>> _radios = new();

        public MudRadioGroup() : base(new Converter<T, T>()) { }

        protected string Classname =>
            new CssBuilder("mud-input-control-boolean-input")
                .AddClass(Class)
                .Build();

        private string GetInputClass() =>
            new CssBuilder("mud-radio-group")
                .AddClass(InputClass)
                .Build();

        [CascadingParameter(Name = "ParentDisabled")]
        private bool ParentDisabled { get; set; }

        [CascadingParameter(Name = "ParentReadOnly")]
        private bool ParentReadOnly { get; set; }

        /// <summary>
        /// The CSS classes for this button group.
        /// </summary>
        /// <remarks>
        /// Multiple classes must be separated by spaces.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Radio.Appearance)]
        public string? InputClass { get; set; }

        /// <summary>
        /// The CSS styles for this button group.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Radio.Appearance)]
        public string? InputStyle { get; set; }

        /// <summary>
        /// The content within this button group.
        /// </summary>
        /// <remarks>
        /// Usually a set of <see cref="MudRadio{T}"/> components.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Radio.Behavior)]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// The unique name for this button group.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Radio.Behavior)]
        public string Name { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Prevents the user from interacting with this group.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool Disabled { get; set; }

        /// <summary>
        /// Prevents the selected value from being changed.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool ReadOnly { get; set; }

        /// <summary>
        /// The current value.
        /// </summary>
        /// <remarks>
        /// When this value changes, the <see cref="ValueChanged"/> event occurs.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Radio.Data)]
        public T? Value
        {
            get => _value;
            set => SetSelectedOptionAsync(value, true, updateValue: false).CatchAndLog();
        }

        /// <summary>
        /// Occurs whenever <see cref="Value"/> has changed.
        /// </summary>
        [Parameter]
        public EventCallback<T> ValueChanged { get; set; }

        internal bool GetDisabledState() => Disabled || ParentDisabled; //internal because the MudRadio reads this value directly

        internal bool GetReadOnlyState() => ReadOnly || ParentReadOnly; //internal because the MudRadio reads this value directly

        protected async Task SetSelectedOptionAsync(T? option, bool updateRadio, bool updateValue = true)
        {
            if (!OptionEquals(_value, option))
            {
                _value = option;

                if (updateRadio)
                {
                    var radio = _radios.FirstOrDefault(r => OptionEquals(r.Value, _value));
                    await SetSelectedRadioAsync(radio, false);
                }

                if (updateValue)
                    await ValueChanged.InvokeAsync(_value);

                await BeginValidateAsync();
                FieldChanged(_value);
            }
        }

        /// <summary>
        /// Tests whether the specified value is valid for this button.
        /// </summary>
        /// <param name="selectItem">The value to examine.</param>
        /// <exception cref="GenericTypeMismatchException">Raised if the specified value does not match <c>T</c>.</exception>
        public void CheckGenericTypeMatch(object selectItem)
        {
            var itemT = selectItem.GetType().GenericTypeArguments[0];
            if (itemT != typeof(T))
            {
                throw new GenericTypeMismatchException("MudRadioGroup", "MudRadio", typeof(T), itemT);
            }
        }

        internal Task SetSelectedRadioAsync(MudRadio<T> radio)
        {
            Touched = true;
            return SetSelectedRadioAsync(radio, true);
        }

        protected async Task SetSelectedRadioAsync(MudRadio<T>? radio, bool updateOption)
        {
            if (_selectedRadio != radio)
            {
                _selectedRadio = radio;

                foreach (var item in _radios)
                {
                    item.SetChecked(item == _selectedRadio);
                }

                if (updateOption)
                {
                    await SetSelectedOptionAsync(GetValueOrDefault(_selectedRadio), false);
                }
            }
        }

        internal Task RegisterRadioAsync(MudRadio<T> radio)
        {
            _radios.Add(radio);

            if (_selectedRadio is null)
            {
                if (OptionEquals(radio.Value, _value))
                {
                    return SetSelectedRadioAsync(radio, false);
                }
            }
            return Task.CompletedTask;
        }

        internal void UnregisterRadio(MudRadio<T> radio)
        {
            _radios.Remove(radio);

            if (_selectedRadio == radio)
            {
                _selectedRadio = null;
            }
        }

        protected override Task ResetValueAsync()
        {
            if (_selectedRadio is not null)
            {
                _selectedRadio.SetChecked(false);
                _selectedRadio = null;
            }

            return base.ResetValueAsync();
        }

        private static T? GetValueOrDefault(MudRadio<T>? radio)
        {
            return radio is not null ? radio.Value : default;
        }

        private static bool OptionEquals(T? option1, T? option2)
        {
            return EqualityComparer<T>.Default.Equals(option1, option2);
        }
    }
}
