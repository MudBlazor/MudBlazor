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
        /// Allows to select more than one chip.
        /// </summary>
        [Parameter]
        public bool MultiSelection { get; set; } = false;

        /// <summary>
        /// Will not allow to deselect the selected chip in single selection mode.
        /// </summary>
        [Parameter]
        public bool Mandatory { get; set; } = false;

        /// <summary>
        /// Will make all chips closable.
        /// </summary>
        [Parameter]
        public bool AllClosable { get; set; } = false;

        /// <summary>
        ///  Will show a check-mark for the selected components.
        /// </summary>
        [Parameter]
        public bool Filter
        {
            get => _filter;
            set
            {
                if (_filter == value)
                    return;
                _filter = value;
                StateHasChanged();
                foreach (var chip in _chips)
                    chip.ForceRerender();
            }
        }

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
        public EventCallback<MudChip> OnClose { get; set; }

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
        private bool _filter;

        internal async Task OnChipClicked(MudChip chip)
        {
            var was_selected=chip.IsSelected;
            if (MultiSelection)
            {
                chip.IsSelected = !chip.IsSelected;
            }
            else
            {
                foreach (var ch in _chips)
                {
                     ch.IsSelected = (ch==chip); // <-- exclusively select the one chip only, thus all others must be deselected
                }
                if (!Mandatory)
                    chip.IsSelected = !was_selected;
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
            OnClose.InvokeAsync(chip);
        }
    }
}
