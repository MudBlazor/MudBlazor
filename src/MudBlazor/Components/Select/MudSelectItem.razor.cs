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
        private String GetCssClasses() => new CssBuilder()
            .AddClass(Class)
            .Build();

        private IMudSelect _parent;
        internal string ItemId { get; } = "_"+Guid.NewGuid().ToString().Substring(0,8);

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
                bool isSelected = MudSelect.Add(this);
                if (_parent.MultiSelection)
                {
                    MudSelect.SelectionChangedFromOutside += OnUpdateSelectionStateFromOutside;
                    InvokeAsync(() => OnUpdateSelectionStateFromOutside(MudSelect.SelectedValues));
                }
                else
                {
                    IsSelected = isSelected;
                }
            }
        }

        /// <summary>
        /// Functional items does not hold values. If a value set on Functional item, it ignores by the MudSelect. They cannot be subject of keyboard navigation and selection.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public bool IsFunctional { get; set; }

        /// <summary>
        /// The text to display
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public string Text { get; set; }

        //private IMudShadowSelect  _shadowParent;
        private bool _isSelected;

        //[CascadingParameter]
        //internal IMudShadowSelect IMudShadowSelect
        //{
        //    get => _shadowParent;
        //    set
        //    {
        //        _shadowParent = value;
        //        ((MudSelect<T>)_shadowParent)?.RegisterShadowItem(this);
        //    }
        //}

        /// <summary>
        /// Select items with HideContent==true are only there to register their RenderFragment with the select but
        /// wont render and have no other purpose!
        /// </summary>
        //[CascadingParameter(Name = "HideContent")]
        //internal bool HideContent { get; set; }

        internal MudSelect<T> MudSelect => (MudSelect<T>)IMudSelect;

        private void OnUpdateSelectionStateFromOutside(IEnumerable<T> selection)
        {
            if (selection == null)
                return;
            var old_is_selected = IsSelected;
            IsSelected = selection.Contains(Value);
            if (old_is_selected != IsSelected)
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
        internal bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected == value)
                    return;
                _isSelected = value;
            }
        }

        /// <summary>
        /// The checkbox icon reflects the multi-select option's state
        /// </summary>
        //protected string CheckBoxIcon
        //{
        //    get
        //    {
        //        if (!MultiSelection)
        //            return null;
        //        return IsSelected ? Icons.Material.Filled.CheckBox : Icons.Material.Filled.CheckBoxOutlineBlank;
        //    }
        //}

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

        private void HandleOnClick()
        {
            //if (MultiSelection)
            //    IsSelected = !IsSelected;

            //MudSelect?.SelectOption(Value).AndForget();
            //InvokeAsync(StateHasChanged);
            if (MultiSelection == false)
            {
                MudSelect.CloseMenu().AndForget();
            }
            OnClick.InvokeAsync().AndForget();
        }

        public void Dispose()
        {
            try
            {
                MudSelect?.Remove(this);
                //((MudSelect<T>)_shadowParent)?.UnregisterShadowItem(this);
            }
            catch (Exception) { }
        }
    }
}
