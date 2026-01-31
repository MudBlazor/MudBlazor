using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// A selectable option displayed within a <see cref="MudSelect{T}"/> component.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This component uses an explicit registration model for communication with its parent:
    /// </para>
    /// <list type="bullet">
    /// <item><description>Registers itself with the parent's context during initialization</description></item>
    /// <item><description>Subscribes to selection changes via an observable pattern</description></item>
    /// <item><description>Unregisters and unsubscribes during disposal</description></item>
    /// </list>
    /// <para>
    /// This design ensures:
    /// </para>
    /// <list type="bullet">
    /// <item><description>Clear ownership and lifecycle management</description></item>
    /// <item><description>No hidden side effects in parameter setters</description></item>
    /// <item><description>Automatic cleanup via IDisposable</description></item>
    /// </list>
    /// </remarks>
    /// <typeparam name="T">The type of value linked to this item.  Must be the same type as the parent <see cref="MudSelect{T}"/>.</typeparam>
    /// <seealso cref="MudSelect{T}"/>
    public partial class MudSelectItem<T> : MudComponentBase, IDisposable
    {
        private MudSelectContext<T>? _context;
        private MudSelectContext<T>? _shadowContext;
        private IDisposable? _selectionSubscription;
        private bool _isInitialized;

        private string GetCssClasses() => new CssBuilder()
            .AddClass(Class)
            .Build();

        internal string ItemId { get; } = Identifier.Create();

        /// <summary>
        /// The <see cref="MudSelect{T}"/> hosting this item.
        /// </summary>
        /// <remarks>
        /// This cascading parameter is used to obtain the context for registration.
        /// Registration itself happens explicitly in <see cref="OnInitialized"/> rather than
        /// implicitly in the parameter setter, making the lifecycle predictable.
        /// </remarks>
        [CascadingParameter]
        internal IMudSelect? IMudSelect { get; set; }

        /// <summary>
        /// The shadow select used for items that only provide RenderFragments.
        /// </summary>
        /// <remarks>
        /// Shadow items (HideContent=true) are registered in a separate lookup
        /// for value-to-RenderFragment resolution when the dropdown is closed.
        /// </remarks>
        [CascadingParameter]
        internal IMudShadowSelect? IMudShadowSelect { get; set; }

        /// <summary>
        /// Select items with HideContent==true are only there to register their RenderFragment with the select but
        /// won't render and have no other purpose!
        /// </summary>
        [CascadingParameter(Name = "HideContent")]
        internal bool HideContent { get; set; }

        /// <summary>
        /// Gets the parent MudSelect component.
        /// </summary>
        internal MudSelect<T>? MudSelect => (MudSelect<T>?)IMudSelect;

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
        protected bool MultiSelection => _context?.MultiSelection == true;

        /// <summary>
        /// Whether this item is selected.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This state is updated by observing the parent's selection via the context.
        /// In multi-selection mode, items subscribe to selection changes.
        /// In single-selection mode, selection is determined during registration.
        /// </para>
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

        /// <summary>
        /// Initializes the item and registers it with the parent select's context.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This explicit registration happens once during initialization, making the lifecycle clear:
        /// </para>
        /// <list type="number">
        /// <item><description>Get the context from the parent cascading parameter</description></item>
        /// <item><description>Register as a visible item OR shadow item based on HideContent</description></item>
        /// <item><description>For multi-selection, subscribe to selection changes</description></item>
        /// <item><description>Set initial Selected state</description></item>
        /// </list>
        /// </remarks>
        protected override void OnInitialized()
        {
            base.OnInitialized();

            // Explicit registration with the parent's context
            // This replaces the implicit registration that used to happen in cascading parameter setters
            if (IMudSelect?.SelectContext is MudSelectContext<T> context)
            {
                _context = context;

                // Register as a visible item (adds to _items, _valueLookup, and _shadowLookup)
                var isSelected = _context.RegisterItem(this);
                Selected = isSelected;

                // Subscribe to selection changes to keep Selected state in sync
                // This replaces the SelectionChangedFromOutside event subscription
                _selectionSubscription = _context.SubscribeToSelectionChanges(OnSelectionChanged);
            }
            else if (IMudShadowSelect?.SelectContext is MudSelectContext<T> shadowContext)
            {
                // Shadow items only register for value-to-RenderFragment lookup
                _shadowContext = shadowContext;
                _shadowContext.RegisterShadowItem(this);
            }

            _isInitialized = true;
        }

        /// <summary>
        /// Handles selection changes from the parent select.
        /// </summary>
        /// <remarks>
        /// This callback is invoked when the parent's SelectedValues changes.
        /// It updates the local Selected state and triggers a re-render if needed.
        /// This replaces the OnUpdateSelectionStateFromOutside method.
        /// </remarks>
        private void OnSelectionChanged(IReadOnlyCollection<T?> selectedValues)
        {
            if (!_isInitialized)
                return;

            var oldSelected = Selected;
            Selected = selectedValues.Contains(Value);

            if (oldSelected != Selected)
            {
                InvokeAsync(StateHasChanged);
            }
        }

        /// <summary>
        /// Handles click events on the item.
        /// </summary>
        private async Task OnClickHandleAsync()
        {
            if (MultiSelection)
            {
                // Toggle selection state optimistically
                Selected = !Selected;
            }

            // Notify parent to update selection
            if (MudSelect != null)
                await MudSelect.SelectOption(Value);

            await InvokeAsync(StateHasChanged);
        }

        /// <summary>
        /// Releases resources used by this component.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Cleanup is explicit and happens in a clear order:
        /// </para>
        /// <list type="number">
        /// <item><description>Unsubscribe from selection changes (disposes the subscription)</description></item>
        /// <item><description>Unregister from the context (removes from lookups)</description></item>
        /// </list>
        /// <para>
        /// This replaces manual event unsubscription and Remove() calls, providing
        /// automatic cleanup via the IDisposable pattern.
        /// </para>
        /// </remarks>
        public void Dispose()
        {
            try
            {
                // Unsubscribe from selection changes
                _selectionSubscription?.Dispose();
                _selectionSubscription = null;

                // Unregister from context
                if (_context != null)
                {
                    _context.UnregisterItem(this);
                    _context = null;
                }

                if (_shadowContext != null)
                {
                    _shadowContext.UnregisterShadowItem(this);
                    _shadowContext = null;
                }
            }
            catch (Exception)
            {
                // Ignore disposal errors
            }
        }
    }
}
