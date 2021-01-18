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
        private string _selectedLabel;
        private MudRadio _selectedRadio;

        private HashSet<MudRadio> _radios = new HashSet<MudRadio>();

        [Parameter] public RenderFragment ChildContent { get; set; }

        [Parameter] public string Name { get; set; } = Guid.NewGuid().ToString();

        [Parameter]
        public string SelectedOption
        {
            get => _selectedRadio?.Option;
            set => SetSelectedOptionAsync(value, true).AndForget();
        }

        protected async Task SetSelectedOptionAsync(string option, bool updateRadio)
        {
            if (_selectedOption != option)
            {
                _selectedOption = option;

                if (updateRadio)
                    await SetSelectedRadioAsync(_radios.FirstOrDefault(r => r.Option == _selectedOption), false, true);

                await SelectedOptionChanged.InvokeAsync(_selectedOption);
            }
        }

        [Parameter]
        public EventCallback<string> SelectedOptionChanged { get; set; }

        [Parameter]
        public string SelectedLabel
        {
            get => _selectedRadio?.Label;
            set => SetSelectedLabelAsync(value, true).AndForget();
        }

        protected async Task SetSelectedLabelAsync(string label, bool updateRadio)
        {
            if (_selectedLabel != label)
            {
                _selectedLabel = label;

                if (updateRadio)
                    await SetSelectedRadioAsync(_radios.FirstOrDefault(r => r.Label == _selectedLabel), true, false);

                await SelectedLabelChanged.InvokeAsync(_selectedLabel);
            }
        }

        [Parameter]
        public EventCallback<string> SelectedLabelChanged { get; set; }

        internal Task SetSelectedRadioAsync(MudRadio radio)
        {
            return SetSelectedRadioAsync(radio, true, true);
        }

        protected async Task SetSelectedRadioAsync(MudRadio radio, bool updateOption, bool updateLabel)
        {
            if (_selectedRadio != radio)
            {
                _selectedRadio = radio;

                foreach (var item in _radios.ToArray())
                    item.SetChecked(item == _selectedRadio);

                if (updateOption)
                    await SetSelectedOptionAsync(_selectedRadio?.Option, false);

                if (updateLabel)
                    await SetSelectedLabelAsync(_selectedRadio?.Label, false);
            }
        }

        internal Task RegisterRadioAsync(MudRadio radio)
        {
            _radios.Add(radio);

            if (_selectedRadio == null)
            {
                if (radio.Option == _selectedOption)
                    return SetSelectedRadioAsync(radio, false, true);

                if (radio.Label == _selectedLabel)
                    return SetSelectedRadioAsync(radio, true, false);
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
