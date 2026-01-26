using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
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
        private IMudSelect? _previousParent;
        private IMudShadowSelect? _previousShadowParent;
        private bool _parametersInitialized;

        private string GetCssClasses() => new CssBuilder()
            .AddClass(Class)
            .Build();

        internal string ItemId { get; } = Identifier.Create();

        /// <summary>
        /// The <see cref="MudSelect{T}"/> hosting this item.
        /// </summary>
        [CascadingParameter]
        internal IMudSelect? IMudSelect { get; set; }

        [CascadingParameter]
        internal IMudShadowSelect? IMudShadowSelect { get; set; }

        /// <inheritdoc />
        public override async Task SetParametersAsync(ParameterView parameters)
        {
            var oldParent = _previousParent;
            var oldShadowParent = _previousShadowParent;

            await base.SetParametersAsync(parameters);

            // Check if this is initial setup or if parents changed
            var parentsChanged = !ReferenceEquals(oldParent, IMudSelect) || !ReferenceEquals(oldShadowParent, IMudShadowSelect);

            if (!_parametersInitialized || parentsChanged)
            {
                if (parentsChanged && _parametersInitialized)
                {
                    // Unregister from old parents if this is a parent change (not initial setup)
                    UnregisterFromPreviousParents(oldParent, oldShadowParent);
                }

                // Register with new/current parents
                RegisterWithParents();
                _parametersInitialized = true;
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
                // Use the parent's ConvertValueToString which delegates to ConvertSet (handles ToStringFunc)
                return MudSelect?.ConvertValueToString(Value) ?? $"{Value}";
            }
        }

        private async Task OnClickHandleAsync()
        {
            if (MultiSelection)
            {
                Selected = !Selected;
            }

            if (MudSelect != null)
                await MudSelect.SelectOption(Value);

            await InvokeAsync(StateHasChanged);
        }

        private void RegisterWithParents()
        {
            // Register with IMudSelect
            if (IMudSelect != null)
            {
                IMudSelect.CheckGenericTypeMatch(this);
                if (MudSelect != null)
                {
                    var selected = MudSelect.Add(this);
                    if (IMudSelect.MultiSelection)
                    {
                        MudSelect.SelectionChangedFromOutside += OnUpdateSelectionStateFromOutside;
                        InvokeAsync(() => OnUpdateSelectionStateFromOutside(MudSelect.GetState(x => x.SelectedValues)));
                    }
                    else
                    {
                        Selected = selected;
                    }
                }
            }

            // Register with IMudShadowSelect
            ((MudSelect<T>?)IMudShadowSelect)?.RegisterShadowItem(this);

            // Track current parents
            _previousParent = IMudSelect;
            _previousShadowParent = IMudShadowSelect;
        }

        private void UnregisterFromPreviousParents(IMudSelect? oldParent, IMudShadowSelect? oldShadowParent)
        {
            // Unregister from previous IMudSelect
            if (oldParent != null)
            {
                var previousMudSelect = (MudSelect<T>?)oldParent;
                if (previousMudSelect != null && oldParent.MultiSelection)
                {
                    previousMudSelect.SelectionChangedFromOutside -= OnUpdateSelectionStateFromOutside;
                }
                previousMudSelect?.Remove(this);
            }

            // Note: We intentionally don't unregister shadow items here because when components re-render,
            // new instances are created before old ones are disposed. Since _shadowLookup is keyed by Value,
            // if we unregister during disposal, we would remove the new item's entry that was just added.
            // The parent MudSelect will clean up the _shadowLookup when it's disposed or when entries are replaced.
        }

        /// <summary>
        /// Releases resources used by this component.
        /// </summary>
        public void Dispose()
        {
            try
            {
                UnregisterFromPreviousParents(_previousParent, _previousShadowParent);
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}
