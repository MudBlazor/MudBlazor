using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
    /// <summary>
    /// Represents an option of a select or multi-select. To be used inside MudSelect.
    /// </summary>
    public partial class MudSelectItem<T> : MudBaseSelectItem, IDisposable
    {
        private string GetCssClasses() => new CssBuilder()
            .AddClass(Class)
            .Build();

        private IMudSelect _parent;
        internal string ItemId { get; } = Identifier.Create();

        /// <summary>
        /// The parent select component
        /// </summary>
        [CascadingParameter]
        internal IMudSelect IMudSelect
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

        private IMudShadowSelect _shadowParent;
        private bool _selected;

        [CascadingParameter]
        internal IMudShadowSelect IMudShadowSelect
        {
            get => _shadowParent;
            set
            {
                _shadowParent = value;
                ((MudSelect<T>)_shadowParent)?.RegisterShadowItem(this);
            }
        }

        /// <summary>
        /// Select items with HideContent==true are only there to register their RenderFragment with the select but
        /// wont render and have no other purpose!
        /// </summary>
        [CascadingParameter(Name = "HideContent")]
        internal bool HideContent { get; set; }

        internal MudSelect<T> MudSelect => (MudSelect<T>)IMudSelect;

        private void OnUpdateSelectionStateFromOutside(IEnumerable<T> selection)
        {
            if (selection == null)
                return;
            var old_selected = Selected;
            Selected = selection.Contains(Value);
            if (old_selected != Selected)
                InvokeAsync(StateHasChanged);
        }

        /// <summary>
        /// A user-defined option that can be selected
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public T Value { get; set; }

        /// <summary>
        /// Mirrors the MultiSelection status of the parent select
        /// </summary>
        protected bool MultiSelection
        {
            get
            {
                if (MudSelect == null)
                    return false;
                return MudSelect.MultiSelection;
            }
        }

        /// <summary>
        /// Selected state of the option. Only works if the parent is a mulit-select
        /// </summary>
        internal bool Selected
        {
            get => _selected;
            set
            {
                _selected = value;
            }
        }

        /// <summary>
        /// The checkbox icon reflects the multi-select option's state
        /// </summary>
        protected string CheckBoxIcon
        {
            get
            {
                if (!MultiSelection)
                    return null;
                return Selected ? Icons.Material.Filled.CheckBox : Icons.Material.Filled.CheckBoxOutlineBlank;
            }
        }

        protected string DisplayString
        {
            get
            {
                var converter = MudSelect?.Converter;
                if (converter == null)
                    return $"{Value}";
                return converter.Set(Value);
            }
        }

        private void OnClicked()
        {
            if (MultiSelection)
                Selected = !Selected;

            MudSelect?.SelectOption(Value);
            InvokeAsync(StateHasChanged);
        }

        public void Dispose()
        {
            try
            {
                MudSelect?.Remove(this);
                ((MudSelect<T>)_shadowParent)?.UnregisterShadowItem(this);
            }
            catch (Exception) { }
        }
    }
}
