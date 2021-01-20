using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace MudBlazor
{
    public partial class MudRadioGroup : MudComponentBase
    {
        private string _selectedOption;
        private MudRadio _selectedRadio;

        private HashSet<MudRadio> _radios = new HashSet<MudRadio>();

        [Parameter] public RenderFragment ChildContent { get; set; }

        [Parameter] public string Name { get; set; } = Guid.NewGuid().ToString();

        [Parameter]
        public string SelectedOption
        {
            get => _selectedOption;
            set => SetSelectedOptionAsync(value, true).AndForget();
        }

        protected async Task SetSelectedOptionAsync(string option, bool updateRadio)
        {
            if (_selectedOption != option)
            {
                _selectedOption = option;

                if (updateRadio)
                    await SetSelectedRadioAsync(_radios.FirstOrDefault(r => r.Option == _selectedOption), false);

                await SelectedOptionChanged.InvokeAsync(_selectedOption);
            }
        }

        [Parameter]
        public EventCallback<string> SelectedOptionChanged { get; set; }

        internal Task SetSelectedRadioAsync(MudRadio radio)
        {
            return SetSelectedRadioAsync(radio, true);
        }

        protected async Task SetSelectedRadioAsync(MudRadio radio, bool updateOption)
        {
            if (_selectedRadio != radio)
            {
                _selectedRadio = radio;

                foreach (var item in _radios.ToArray())
                    item.SetChecked(item == _selectedRadio);

                if (updateOption)
                    await SetSelectedOptionAsync(_selectedRadio?.Option, false);
            }
        }

        internal Task RegisterRadioAsync(MudRadio radio)
        {
            _radios.Add(radio);

            if (_selectedRadio == null)
            {
                if (radio.Option == _selectedOption)
                    return SetSelectedRadioAsync(radio, false);
            }
            return Task.CompletedTask;
        }

        internal void UnregisterRadio(MudRadio radio)
        {
            _radios.Remove(radio);

            if (_selectedRadio == radio)
                _selectedRadio = null;
        }
    }
}
