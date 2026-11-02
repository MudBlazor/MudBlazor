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
        private readonly ParameterState<bool> _expandedState;

        private string GetCssClasses() => new CssBuilder()
            .AddClass("mud-select-item-nested", ParentItem is not null)
            .AddClass("mud-select-item-tree", IsTree)
            .AddClass(Class)
            .Build();

        /// <summary>
        /// Whether the expand and collapse icon comes before the text.
        /// </summary>
        private bool ExpandIconAtStart => MudSelect?.ExpandIconPlacement != ExpandIconPlacement.End;

        private string ToggleCssClass => new CssBuilder("mud-select-item-toggle")
            .AddClass("mud-select-item-toggle-start", ExpandIconAtStart)
            .Build();

        /// <summary>
        /// Indents nested items by one step per level, on the inline-start side so right-to-left layouts mirror.
        /// </summary>
        /// <remarks>
        /// With the icon at the start, every row in a tree also reserves a column for it so leaves line up with their siblings.
        /// </remarks>
        private string? GetStyle() => new StyleBuilder()
            .AddStyle("--mud-select-tree-level", Level.ToString(System.Globalization.CultureInfo.InvariantCulture), IsTree)
            .AddStyle("padding-inline-start", ExpandIconAtStart ? $"calc(2.5rem + {Level} * 1.5rem)" : $"calc(1rem + {Level} * 1.5rem)", IsTree)
            .AddStyle(Style)
            .Build();

        /// <summary>
        /// Describes the connector segments drawn in the indent of this row, one per ancestor level.
        /// </summary>
        /// <remarks>
        /// The last column joins this row to its parent.
        /// Earlier columns carry an ancestor's line past this row, which only continues while that ancestor has later siblings.
        /// </remarks>
        private IEnumerable<(int Column, string CssClass)> GetGuides()
        {
            if (MudSelect?.TreeLines != true || _context is null || Level == 0)
            {
                yield break;
            }

            var through = new bool[Level - 1];
            var ancestor = ParentItem;
            for (var column = Level - 2; column >= 0 && ancestor is not null; column--)
            {
                through[column] = !_context.IsLastChild(ancestor);
                ancestor = ancestor.ParentItem;
            }

            for (var column = 0; column < through.Length; column++)
            {
                if (through[column])
                {
                    yield return (column, "mud-select-item-guide");
                }
            }

            yield return (Level - 1, _context.IsLastChild(this)
                ? "mud-select-item-guide mud-select-item-guide-elbow mud-select-item-guide-last"
                : "mud-select-item-guide mud-select-item-guide-elbow");
        }

        internal string ItemId { get; } = Identifier.Create();

        /// <summary>
        /// Builds fallback accessibility attributes for the rendered option element.
        /// </summary>
        /// <remarks>
        /// Option semantics come from this item's selection state rather than the popup's temporary active item.
        /// </remarks>
        private Dictionary<string, object?> GetUserAttributes()
        {
            var attributes = new Dictionary<string, object?>(UserAttributes, StringComparer.OrdinalIgnoreCase);
            attributes.TryAdd("aria-selected", Selected ? "true" : "false");

            if (IsTree)
            {
                attributes.TryAdd("role", "treeitem");
                attributes.TryAdd("aria-level", (Level + 1).ToString(System.Globalization.CultureInfo.InvariantCulture));
                if (HasNestedItems)
                {
                    attributes.TryAdd("aria-expanded", _expandedState.Value ? "true" : "false");
                }
            }

            if (Disabled)
            {
                attributes.TryAdd("aria-disabled", "true");
            }

            return attributes;
        }

        public MudSelectItem()
        {
            using var registerScope = CreateRegisterScope();
            registerScope.RegisterParameter<IMudSelect?>(nameof(IMudSelect))
                .WithParameter(() => IMudSelect)
                .WithChangeHandler(OnMudSelectChanged);
            registerScope.RegisterParameter<IMudShadowSelect?>(nameof(IMudShadowSelect))
                .WithParameter(() => IMudShadowSelect)
                .WithChangeHandler(OnMudShadowSelectChanged);
            registerScope.RegisterParameter<T?>(nameof(Value))
                .WithParameter(() => Value)
                .WithChangeHandler(OnValueChanged)
                .WithComparer(() => (_context ?? _shadowContext)?.Comparer ?? EqualityComparer<T?>.Default);
            _expandedState = registerScope.RegisterParameter<bool>(nameof(Expanded))
                .WithParameter(() => Expanded)
                .WithEventCallback(() => ExpandedChanged)
                .WithChangeHandler(OnExpandedChangedAsync);
        }

        /// <summary>
        /// The item that contains this one in its <see cref="NestedList"/>, if any.
        /// </summary>
        [CascadingParameter(Name = "ParentSelectItem")]
        internal MudSelectItem<T>? ParentItem { get; set; }

        /// <summary>
        /// The depth of this item, where top-level items are <c>0</c>.
        /// </summary>
        internal int Level => ParentItem is null ? 0 : ParentItem.Level + 1;

        /// <summary>
        /// Whether this item contains nested items.
        /// </summary>
        internal bool HasNestedItems => NestedList is not null;

        /// <summary>
        /// Whether this item or the select it belongs to is a tree.
        /// </summary>
        private bool IsTree => ParentItem is not null || HasNestedItems || _context?.HasNestedItems == true;

        /// <summary>
        /// Whether the nested items of this item are currently shown.
        /// </summary>
        internal bool IsExpanded => HasNestedItems && _expandedState.Value;

        /// <summary>
        /// Items shown under this one when it is expanded.
        /// </summary>
        /// <remarks>
        /// Selecting this item does not select its nested items.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.General.Behavior)]
        public RenderFragment? NestedList { get; set; }

        /// <summary>
        /// Shows the items in <see cref="NestedList"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// Has no effect unless <see cref="NestedList"/> is set.
        /// </remarks>
        [Parameter, ParameterState]
        [Category(CategoryTypes.General.Appearance)]
        public bool Expanded { get; set; }

        /// <summary>
        /// Occurs when the user expands or collapses this item.
        /// </summary>
        [Parameter]
        public EventCallback<bool> ExpandedChanged { get; set; }

        /// <summary>
        /// The icon shown while this item is expanded.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.ExpandLess"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.General.Appearance)]
        public string ExpandLessIcon { get; set; } = Icons.Material.Filled.ExpandLess;

        /// <summary>
        /// The icon shown while this item is collapsed.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.ExpandMore"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.General.Appearance)]
        public string ExpandMoreIcon { get; set; } = Icons.Material.Filled.ExpandMore;

        /// <summary>
        /// The color of the expand and collapse icon.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Color.Default"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.General.Appearance)]
        public Color ExpandIconColor { get; set; } = Color.Default;

        /// <summary>
        /// Expands or collapses this item and remembers the choice for when the popup is reopened.
        /// </summary>
        internal async Task SetExpandedAsync(bool expanded)
        {
            if (!HasNestedItems || _expandedState.Value == expanded)
            {
                return;
            }

            _context?.SetExpansion(Value, expanded);
            await _expandedState.SetValueAsync(expanded);

            if (!expanded && MudSelect is not null)
            {
                await MudSelect.OnItemCollapsedAsync(this);
            }

            StateHasChanged();
        }

        private Task ToggleExpandedAsync() => SetExpandedAsync(!_expandedState.Value);

        /// <summary>
        /// Handles the parent changing <see cref="Expanded"/> from outside.
        /// </summary>
        private async Task OnExpandedChangedAsync(ParameterChangedEventArgs<bool> args)
        {
            _context?.SetExpansion(Value, args.Value);
            if (!args.Value && MudSelect is not null)
            {
                await MudSelect.OnItemCollapsedAsync(this);
            }
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
        /// Whether some, but not all, of the items below this one are checked, when parents cascade their selection.
        /// </summary>
        internal bool Indeterminate { get; set; }

        /// <summary>
        /// The icon to display whether this item is selected.
        /// </summary>
        /// <remarks>
        /// When <see cref="Selected"/> is <c>true</c>, the parent's <see cref="MudSelect{T}.CheckedIcon"/> is returned.  Otherwise, its <see cref="MudSelect{T}.UncheckedIcon"/>.
        /// </remarks>
        protected string? CheckBoxIcon
        {
            get
            {
                if (_context is not { MultiSelection: true } context)
                    return null;
                if (Indeterminate && MudSelect is { } select)
                {
                    return select.IndeterminateIcon;
                }

                return Selected ? context.CheckedIcon : context.UncheckedIcon;
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
        private async Task OnMudSelectChanged(ParameterChangedEventArgs<IMudSelect?> args)
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
                Indeterminate = _context.IsItemIndeterminate(this);

                _selectionSubscription = _context.SubscribeToSelectionChanges(OnSelectionChangedAsync);

                // The popup rebuilds its items every time it opens, so the expansion is restored from what the select remembered.
                if (HasNestedItems)
                {
                    var expanded = _context.TryGetExpansion(Value, out var remembered) ? remembered : _expandedState.Value || MudSelect?.InitiallyExpanded == true;
                    await SetExpandedAsync(expanded);
                }
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
        private Task OnSelectionChangedAsync(IReadOnlyCollection<T?> selectedValues)
        {
            var oldSelected = Selected;
            var oldIndeterminate = Indeterminate;
            if (_context?.MultiSelection == true && MudSelect?.CascadesSelection == true)
            {
                // A parent is checked by its descendants, so membership in the selected values is not enough.
                Selected = _context.IsItemSelected(this);
                Indeterminate = _context.IsItemIndeterminate(this);
            }
            else
            {
                Selected = selectedValues.Contains(Value);
                Indeterminate = false;
            }

            if (oldSelected != Selected || oldIndeterminate != Indeterminate)
            {
                // Avoid await InvokeAsync(StateHasChanged) due to a MAUI dispatcher bug:
                // https://github.com/MudBlazor/MudBlazor/issues/13009
                // This issue does not occur on other platforms (e.g., WASM, WinForms Hybrid, etc).
                StateHasChanged();
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Handles changes to the <see cref="Value"/> parameter.
        /// </summary>
        /// <remarks>
        /// When the parent swaps the underlying data collection while Blazor reuses the same component instances
        /// (e.g. a keyless <c>@foreach</c> binding new items to existing <see cref="MudSelectItem{T}"/> positions),
        /// the value-keyed lookups in <see cref="MudSelectContext{T}"/> must be updated to avoid stale keys.
        /// </remarks>
        private void OnValueChanged(ParameterChangedEventArgs<T?> args)
        {
            _context?.OnItemValueChanged(this, args.LastValue, args.Value);
            _shadowContext?.OnShadowItemValueChanged(this, args.LastValue, args.Value);

            if (_context is not null)
            {
                //Update selected value when updated without new item creation
                Selected = _context.IsItemSelected(this);
                Indeterminate = _context.IsItemIndeterminate(this);
            }
        }

        /// <summary>
        /// Handles click events on the item.
        /// </summary>
        private async Task OnClickHandleAsync()
        {
            if (MudSelect is not null)
            {
                await MudSelect.ActivateItemAsync(this);
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
