using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Components;

namespace MudBlazor
{
    public partial class MudRadioGroup : MudComponentBase
    {
        [Parameter] public RenderFragment ChildContent { get; set; }

        [Parameter] public string Name { get; set; } = Guid.NewGuid().ToString();

        [Parameter]
        public EventCallback<string> SelectedLabelChanged { get; set; }

        [Parameter]
        public string SelectedLabel
        {
            get => _selectedLabel;
            set
            {
                if (_selectedLabel == value)
                    return;
                _selectedLabel = value;
                SetSelectedRadio(_radios.FirstOrDefault(r=>r.Label==value));
                SelectedLabelChanged.InvokeAsync(value);
            }
        }
        private string _selectedLabel;

        [Parameter]
        public string SelectedOption
        {
            get => _selectedOption;
            set
            {
                if (_selectedOption == value)
                    return;
                _selectedOption = value;
                SetSelectedRadio(_radios.FirstOrDefault(r => r.Option == value));
                SelectedOptionChanged.InvokeAsync(value);
            }
        }
        private string _selectedOption;

        [Parameter]
        public EventCallback<string> SelectedOptionChanged { get; set; }

        HashSet<MudRadio> _radios=new HashSet<MudRadio>();



        internal void SetSelectedRadio(MudRadio radio)
        {
            if (radio==null || radio.Checked)
                return;
            foreach (var registered_radio in _radios.ToArray())
                registered_radio.Checked = (registered_radio == radio);
            SelectedLabel = radio.Label;
            SelectedOption = radio.Option;
        }

        internal void RegisterRadio(MudRadio radio)
        {
            if (radio == null)
                return;
            _radios.Add(radio);
            if (!string.IsNullOrWhiteSpace(radio.Label) && radio.Label == SelectedLabel 
                || !string.IsNullOrWhiteSpace(radio.Option) && radio.Option == SelectedOption)
            {
                radio.Checked = true;
                SelectedLabel = radio.Label;
                SelectedOption = radio.Option;
            }
        }

        internal void UnregisterRadio(MudRadio radio)
        {
            if (radio == null)
                return;
            _radios.Remove(radio);
        }

    }
}
