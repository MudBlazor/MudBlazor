using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;
using MudBlazor.Utilities.Exceptions;

namespace MudBlazor
{
    public partial class MudRadioGroup<T> : MudFormComponent<T, T>, IMudRadioGroup
    {
        public MudRadioGroup() : base(new Converter<T, T>()) { }

        private MudRadio<T> _selectedRadio;

        private HashSet<MudRadio<T>> _radios = new();

        protected string Classname =>
        new CssBuilder("mud-input-control-boolean-input")
            .AddClass(Class)
            .Build();

        private string GetInputClass() =>
        new CssBuilder("mud-radio-group")
            .AddClass(InputClass)
            .Build();

        /// <summary>
        /// User class names for the input, separated by space
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Radio.Appearance)]
        public string InputClass { get; set; }

        /// <summary>
        /// User style definitions for the input
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Radio.Appearance)]
        public string InputStyle { get; set; }

        [Parameter]
        [Category(CategoryTypes.Radio.Behavior)]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        [Category(CategoryTypes.Radio.Behavior)]
        public string Name { get; set; } = Guid.NewGuid().ToString();

        public void CheckGenericTypeMatch(object select_item)
        {
            var itemT = select_item.GetType().GenericTypeArguments[0];
            if (itemT != typeof(T))
                throw new GenericTypeMismatchException("MudRadioGroup", "MudRadio", typeof(T), itemT);
        }

        [Parameter]
        [Category(CategoryTypes.Radio.Data)]
        public T SelectedOption
        {
            get => _value;
            set => SetSelectedOptionAsync(value, true).AndForget();
        }

        protected async Task SetSelectedOptionAsync(T option, bool updateRadio)
        {
            if (!OptionEquals(_value, option))
            {
                _value = option;

                if (updateRadio)
                    await SetSelectedRadioAsync(_radios.FirstOrDefault(r => OptionEquals(r.Option, _value)), false);

                await SelectedOptionChanged.InvokeAsync(_value);

                BeginValidate();
                FieldChanged(_value);
            }
        }

        [Parameter]
        public EventCallback<T> SelectedOptionChanged { get; set; }

        internal Task SetSelectedRadioAsync(MudRadio<T> radio)
        {
            Touched = true;
            return SetSelectedRadioAsync(radio, true);
        }

        protected async Task SetSelectedRadioAsync(MudRadio<T> radio, bool updateOption)
        {
            if (_selectedRadio != radio)
            {
                _selectedRadio = radio;

                foreach (var item in _radios.ToArray())
                    item.SetChecked(item == _selectedRadio);

                if (updateOption)
                    await SetSelectedOptionAsync(GetOptionOrDefault(_selectedRadio), false);
            }
        }

        internal Task RegisterRadioAsync(MudRadio<T> radio)
        {
            _radios.Add(radio);

            if (_selectedRadio == null)
            {
                if (OptionEquals(radio.Option, _value))
                    return SetSelectedRadioAsync(radio, false);
            }
            return Task.CompletedTask;
        }

        internal void UnregisterRadio(MudRadio<T> radio)
        {
            _radios.Remove(radio);

            if (_selectedRadio == radio)
                _selectedRadio = null;
        }

        protected override void ResetValue()
        {
            if (_selectedRadio != null)
            {
                _selectedRadio.SetChecked(false);
                _selectedRadio = null;
            }

            base.ResetValue();
        }

        private static T GetOptionOrDefault(MudRadio<T> radio)
        {
            return radio != null ? radio.Option : default;
        }

        private static bool OptionEquals(T option1, T option2)
        {
            return EqualityComparer<T>.Default.Equals(option1, option2);
        }
    }
}
