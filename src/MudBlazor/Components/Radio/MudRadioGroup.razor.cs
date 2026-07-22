using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;
using MudBlazor.Utilities.Exceptions;

namespace MudBlazor
{
#nullable enable
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
        /// User class names for the input, separated by space
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Radio.Appearance)]
        public string? InputClass { get; set; }

        /// <summary>
        /// User style definitions for the input
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Radio.Appearance)]
        public string? InputStyle { get; set; }

        [Parameter]
        [Category(CategoryTypes.Radio.Behavior)]
        public RenderFragment? ChildContent { get; set; }

        [Parameter]
        [Category(CategoryTypes.Radio.Behavior)]
        public string Name { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// If true, the input will be disabled.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool Disabled { get; set; }

        /// <summary>
        /// If true, the input will be read-only.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool ReadOnly { get; set; }

        [Parameter]
        [Category(CategoryTypes.Radio.Data)]
        public T? Value
        {
            get => _value;
            set => SetSelectedOptionAsync(value, true).CatchAndLog();
        }

        [Parameter]
        public EventCallback<T> ValueChanged { get; set; }

        internal bool GetDisabledState() => Disabled || ParentDisabled; //internal because the MudRadio reads this value directly

        internal bool GetReadOnlyState() => ReadOnly || ParentReadOnly; //internal because the MudRadio reads this value directly

        protected async Task SetSelectedOptionAsync(T? option, bool updateRadio)
        {
            if (!OptionEquals(_value, option))
            {
                _value = option;

                if (updateRadio)
                {
                    var radio = _radios.FirstOrDefault(r => OptionEquals(r.Value, _value));
                    await SetSelectedRadioAsync(radio, false);
                }

                await ValueChanged.InvokeAsync(_value);

                await BeginValidateAsync();
                FieldChanged(_value);
            }
        }

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
