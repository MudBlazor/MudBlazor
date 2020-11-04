using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;
using MudBlazor.Extensions;
using System.Windows.Input;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudBlazor
{
    public partial class MudChipSet : MudComponentBase
    {
  
        protected string Classname =>
        new CssBuilder("mud-chipset")
          .AddClass(Class)
        .Build();

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Allows single selection from a set of options. If combined with Filter the selected value can be unselected.
        /// </summary>
        [Parameter]
        public bool Choice { get; set; } = true;

        /// <summary>
        ///  Enables multiple-choice selection from the set of chips. Chips must be "Checkable" for this to work.
        /// </summary>
        [Parameter]
        public bool Filter { get; set; }

        /// <summary>
        /// The currently selected chip in Choice mode
        /// </summary>
        [Parameter]
        public MudChip SelectedChip
        {
            get { return _chips.OfType<MudChip>().Where(x => x.IsSelected).FirstOrDefault(); }
            set
            {
                if (value == null)
                {
                    foreach (var chip in _chips)
                    {
                        chip.IsSelected = false;
                    }
                }
                else
                {
                    foreach (var chip in _chips)
                    {
                        chip.IsSelected = (chip == value);
                    }
                }
                this.InvokeAsync(StateHasChanged);
            }
        }

        /// <summary>
        /// Called when the selected chip changes, in Choice mode
        /// </summary>
        [Parameter]
        public EventCallback<MudChip> SelectedChipChanged { get; set; }

        /// <summary>
        /// The currently selected chips in Filter mode
        /// </summary>
        [Parameter]
        public MudChip[] SelectedChips
        {
            get { return _chips.OfType<MudChip>().Where(x => x.IsSelected).ToArray(); }
            set
            {
                if (value == null || value.Length == 0)
                {
                    foreach (var chip in _chips)
                    {
                        chip.IsSelected = false;
                    }
                }
                else
                {
                    var selected = new HashSet<MudChip>(value);
                    foreach (var chip in _chips)
                    {
                        chip.IsSelected = selected.Contains(chip);
                    }
                }
                this.InvokeAsync(StateHasChanged);
            }
        }

        /// <summary>
        /// Called when the selection changed, in Filter mode
        /// </summary>
        [Parameter]
        public EventCallback<MudChip[]> SelectedChipsChanged { get; set; }

        /// <summary>
        /// Called when a Chip was deleted (by click on the close icon)
        /// </summary>
        [Parameter]
        public EventCallback<MudChip> ChipDeleted { get; set; }

        internal void Add(MudChip chip)
        {
            _chips.Add(chip);
        }

        private  void OnChipClickHandler(MouseEventArgs args) {}

        internal void Remove(MudChip chip)
        {
            _chips.Remove(chip);
        }

        private HashSet<MudChip> _chips = new HashSet<MudChip>();

        internal async Task OnChipClicked(MudChip chip)
        {
            if (Filter)
            {
                chip.IsSelected = !chip.IsSelected;
            }
            else if (Choice)
            {
                foreach (var ch in _chips)
                {
                     ch.IsSelected = (ch==chip); // <-- exclusively select the one chip only, thus all others must be deselected
                }
            }
            await NotifySelection();
        }

        private async Task NotifySelection()
        {
            await InvokeAsync(StateHasChanged);
            await SelectedChipChanged.InvokeAsync(SelectedChip);
            await SelectedChipsChanged.InvokeAsync(SelectedChips);
        }

        public void OnChipDeleted(MudChip chip)
        {
            Remove(chip);
            ChipDeleted.InvokeAsync(chip);
        }
    }
}
