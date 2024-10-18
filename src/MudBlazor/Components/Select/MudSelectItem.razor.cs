using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// Represents an option of a select or multi-select. To be used inside MudSelect.
    /// </summary>
    public partial class MudSelectItem<T> : MudComponentBase, IDisposable
    {
        private IMudSelect? _parent;
        private IMudShadowSelect? _shadowParent;

        private string GetCssClasses() => new CssBuilder()
            .AddClass(Class)
            .Build();

        internal string ItemId { get; } = Identifier.Create();

        /// <summary>
        /// The parent select component
        /// </summary>
        [CascadingParameter]
        internal IMudSelect? IMudSelect
        {
            get => _parent;
            set
            {
                _parent = value;
                if (_parent == null)
                    return;
                _parent.CheckGenericTypeMatch(this);
                if (MudSelect == null)
                    return;
                var selected = MudSelect.Add(this);
                if (_parent.MultiSelection)
                {
                    MudSelect.SelectionChangedFromOutside += OnUpdateSelectionStateFromOutside;
                    InvokeAsync(() => OnUpdateSelectionStateFromOutside(MudSelect.SelectedValues));
                }
                else
                {
                    Selected = selected;
                }
            }
        }

        [CascadingParameter]
        internal IMudShadowSelect? IMudShadowSelect
        {
            get => _shadowParent;
            set
            {
                _shadowParent = value;
                ((MudSelect<T>?)_shadowParent)?.RegisterShadowItem(this);
            }
        }

        /// <summary>
        /// Select items with HideContent==true are only there to register their RenderFragment with the select but
        /// wont render and have no other purpose!
        /// </summary>
        [CascadingParameter(Name = "HideContent")]
        internal bool HideContent { get; set; }

        internal MudSelect<T>? MudSelect => (MudSelect<T>?)IMudSelect;

        private void OnUpdateSelectionStateFromOutside(IEnumerable<T?>? selection)
        {
            if (selection == null)
                return;
            var oldSelected = Selected;
            Selected = selection.Contains(Value);
            if (oldSelected != Selected)
                InvokeAsync(StateHasChanged);
        }

        /// <summary>
        /// A user-defined option that can be selected
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public T? Value { get; set; }

        /// <summary>
        /// Prevents the user from interacting with this item.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.General.Behavior)]
        public bool Disabled { get; set; }

        /// <summary>
        /// The content within this item.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.General.Behavior)]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// Mirrors the MultiSelection status of the parent select
        /// </summary>
        protected bool MultiSelection => MudSelect is { MultiSelection: true };

        /// <summary>
        /// Selected state of the option. Only works if the parent is a mulit-select
        /// </summary>
        internal bool Selected { get; set; }

        /// <summary>
        /// The checkbox icon reflects the multi-select option's state
        /// </summary>
        protected string? CheckBoxIcon
        {
            get
            {
                if (!MultiSelection)
                    return null;
                return Selected ? Icons.Material.Filled.CheckBox : Icons.Material.Filled.CheckBoxOutlineBlank;
            }
        }

        protected string? DisplayString
        {
            get
            {
                var converter = MudSelect?.Converter;
                if (converter == null)
                    return $"{Value}";
                return converter.Set(Value);
            }
        }

        private Task OnClickHandleAsync()
        {
            if (MultiSelection)
            {
                Selected = !Selected;
            }

            MudSelect?.SelectOption(Value);

            return InvokeAsync(StateHasChanged);
        }

        public void Dispose()
        {
            try
            {
                MudSelect?.Remove(this);
                ((MudSelect<T>?)_shadowParent)?.UnregisterShadowItem(this);
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}
