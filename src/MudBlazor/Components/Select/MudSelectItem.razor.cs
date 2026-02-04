using Microsoft.AspNetCore.Components;
using MudBlazor.State;
using MudBlazor.Utilities;

namespace MudBlazor
{
    /// <summary>
    /// A selectable option displayed within a <see cref="MudSelect{T}"/> component.
    /// </summary>
    /// <typeparam name="T">The type of value linked to this item.  Must be the same type as the parent <see cref="MudSelect{T}"/>.</typeparam>
    /// <seealso cref="MudSelect{T}"/>
    public partial class MudSelectItem<T> : MudComponentBase, IDisposable
    {
        private MudSelectContext<T>? _context;
        private MudSelectContext<T>? _shadowContext;
        private IDisposable? _selectionSubscription;

        private string GetCssClasses() => new CssBuilder()
            .AddClass(Class)
            .Build();

        internal string ItemId { get; } = Identifier.Create();

        public MudSelectItem()
        {
            using var registerScope = CreateRegisterScope();
            registerScope.RegisterParameter<IMudSelect?>(nameof(IMudSelect))
                .WithParameter(() => IMudSelect)
                .WithChangeHandler(OnMudSelectChanged);
            registerScope.RegisterParameter<IMudShadowSelect?>(nameof(IMudShadowSelect))
                .WithParameter(() => IMudShadowSelect)
                .WithChangeHandler(OnMudShadowSelectChanged);
        }

        /// <summary>
        /// The <see cref="MudSelect{T}"/> hosting this item.
        /// </summary>
        /// <remarks>
        /// This cascading parameter is used to obtain the context for registration.
        /// When this parameter changes, OnMudSelectChanged is invoked to handle
        /// registration and unregistration with the appropriate parent.
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
        /// Items subscribe to selection changes and update this state accordingly.
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

        protected string DisplayString
        {
            get
            {
                // Use the parent's ConvertValueToString which delegates to ConvertSet (handles ToStringFunc)
                return MudSelect?.ConvertValueToString(Value) ?? $"{Value}";
            }
        }

        /// <summary>
        /// Handles changes to the IMudShadowSelect cascading parameter.
        /// </summary>
        /// <remarks>
        /// This is invoked when the shadow select parent changes (e.g., when moving between different selects).
        /// It unregisters from the old parent and registers with the new one.
        /// </remarks>
        private void OnMudShadowSelectChanged(ParameterChangedEventArgs<IMudShadowSelect?> args)
        {
            if (args.LastValue?.SelectContext is MudSelectContext<T> oldContext)
            {
                oldContext.UnregisterShadowItem(this);
            }

            if (args.Value?.SelectContext is MudSelectContext<T> newContext)
            {
                _shadowContext = newContext;
                _shadowContext.RegisterShadowItem(this);
            }
        }

        /// <summary>
        /// Handles changes to the IMudSelect cascading parameter.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is invoked when the select parent changes (e.g., when moving between different selects).
        /// It handles the complete lifecycle:
        /// </para>
        /// <list type="number">
        /// <item><description>Unsubscribes from the old parent's selection changes</description></item>
        /// <item><description>Unregisters from the old parent's context</description></item>
        /// <item><description>Registers with the new parent's context</description></item>
        /// <item><description>Subscribes to the new parent's selection changes</description></item>
        /// <item><description>Updates the initial Selected state</description></item>
        /// </list>
        /// </remarks>
        private void OnMudSelectChanged(ParameterChangedEventArgs<IMudSelect?> args)
        {
            if (args.LastValue?.SelectContext is MudSelectContext<T> oldContext)
            {
                _selectionSubscription?.Dispose();
                _selectionSubscription = null;
                oldContext.UnregisterItem(this);
            }

            if (args.Value?.SelectContext is MudSelectContext<T> newContext)
            {
                _context = newContext;

                // Register as a visible item (adds to _items, _valueLookup, and _shadowLookup)
                var isSelected = _context.RegisterItem(this);
                Selected = isSelected;

                _selectionSubscription = _context.SubscribeToSelectionChanges(OnSelectionChangedAsync);
            }
        }

        /// <summary>
        /// Handles selection changes from the parent select.
        /// </summary>
        /// <remarks>
        /// This callback is invoked when the parent's SelectedValues changes.
        /// It updates the local Selected state and triggers a re-render if needed.
        /// This replaces the OnUpdateSelectionStateFromOutside method.
        /// </remarks>
        private async Task OnSelectionChangedAsync(IReadOnlyCollection<T?> selectedValues)
        {
            var oldSelected = Selected;
            Selected = selectedValues.Contains(Value);

            if (oldSelected != Selected)
            {
                await InvokeAsync(StateHasChanged);
            }
        }

        /// <summary>
        /// Handles click events on the item.
        /// </summary>
        private async Task OnClickHandleAsync()
        {
            if (MultiSelection)
            {
                Selected = !Selected;
            }

            if (MudSelect is not null)
            {
                await MudSelect.SelectOption(Value);
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases resources used by this component.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            var selection = _selectionSubscription;
            var context = _context;
            var shadow = _shadowContext;

            _selectionSubscription = null;
            _context = null;
            _shadowContext = null;

            selection?.Dispose();
            context?.UnregisterItem(this);
            shadow?.UnregisterShadowItem(this);
        }
    }
}
