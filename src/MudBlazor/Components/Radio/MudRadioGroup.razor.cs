using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace MudBlazor
{
    public partial class MudRadioGroup<T> : MudComponentBase
    {
        private T _selectedOption;
        private MudRadio<T> _selectedRadio;

        private HashSet<MudRadio<T>> _radios = new HashSet<MudRadio<T>>();

        [Parameter] public RenderFragment ChildContent { get; set; }

        [Parameter] public string Name { get; set; } = Guid.NewGuid().ToString();

        [Parameter]
        public T SelectedOption
        {
            get => _selectedOption;
            set => SetSelectedOptionAsync(value, true).AndForget();
        }

        protected async Task SetSelectedOptionAsync(T option, bool updateRadio)
        {
            if (!OptionEquals(_selectedOption, option))
            {
                _selectedOption = option;

                if (updateRadio)
                    await SetSelectedRadioAsync(_radios.FirstOrDefault(r => OptionEquals(r.Option, _selectedOption)), false);

                await SelectedOptionChanged.InvokeAsync(_selectedOption);
            }
        }

        [Parameter]
        public EventCallback<T> SelectedOptionChanged { get; set; }

        internal Task SetSelectedRadioAsync(MudRadio<T> radio)
        {
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
                if (OptionEquals(radio.Option, _selectedOption))
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
