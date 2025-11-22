using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// A selectable option displayed within a <see cref="MudSelect{T}"/> component.
    /// </summary>
    /// <typeparam name="T">The type of value linked to this item.  Must be the same type as the parent <see cref="MudSelect{T}"/>.</typeparam>
    /// <seealso cref="MudSelect{T}"/>
    public partial class MudSelectItem<T> : MudComponentBase, IDisposable
    {
        private IMudSelect? _parent;
        private IMudShadowSelect? _shadowParent;

        private string GetCssClasses() => new CssBuilder()
            .AddClass(Class)
            .Build();

        internal string ItemId { get; } = Identifier.Create();

        /// <summary>
        /// The <see cref="MudSelect{T}"/> hosting this item.
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
        /// The custom value associated with this item.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public T? Value { get; set; }

        /// <summary>
        /// Prevents the user from interacting with this item.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.General.Behavior)]
        public bool Disabled { get; set; }

        /// <summary>
        /// The custom content within this item.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.General.Behavior)]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// Whether multi-selection is enabled in the parent <see cref="MudSelect{T}"/>.
        /// </summary>
        protected bool MultiSelection => MudSelect is { MultiSelection: true };

        /// <summary>
        /// Whether this item is selected.
        /// </summary>
        /// <remarks>
        /// Only applies when <see cref="MultiSelection"/> is <c>true</c>.
        /// </remarks>
        internal bool Selected { get; set; }

        /// <summary>
        /// The icon to display whether this item is selected.
        /// </summary>
        /// <remarks>
        /// When <see cref="Selected"/> is <c>true</c>, <see cref="Icons.Material.Filled.CheckBox"/> is returned.  Otherwise, <see cref="Icons.Material.Filled.CheckBoxOutlineBlank"/>.
        /// </remarks>
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

        /// <summary>
        /// Releases resources used by this component.
        /// </summary>
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
