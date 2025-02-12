using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.State;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable

    /// <summary>
    /// An item within a <see cref="MudList{T}"/> component.
    /// </summary>
    /// <typeparam name="T">The type of item being listed.</typeparam>
    /// <seealso cref="MudList{T}"/>
    /// <seealso cref="MudListSubheader"/>
    public partial class MudListItem<T> : MudComponentBase, IDisposable
    {
        private bool _selected;
        private bool MultiSelection => MudList?.SelectionMode == SelectionMode.MultiSelection;

        private ParameterState<bool> _expandedState;

        public MudListItem()
        {
            using var registerScope = CreateRegisterScope();
            _expandedState = registerScope.RegisterParameter<bool>(nameof(Expanded))
                .WithParameter(() => Expanded)
                .WithEventCallback(() => ExpandedChanged);
        }

        protected string Classname =>
            new CssBuilder("mud-list-item")
                .AddClass("mud-list-item-dense", GetDense())
                .AddClass("mud-list-item-gutters", GetGutters())
                .AddClass("mud-list-item-clickable", GetClickable())
                .AddClass("mud-ripple", Ripple && GetClickable())
                .AddClass($"mud-selected-item mud-{MudList?.Color.ToDescriptionString()}-text", !MultiSelection && _selected && !GetDisabled())
                .AddClass($"mud-{MudList?.Color.ToDescriptionString()}-hover", !MultiSelection && _selected && !GetDisabled())
                .AddClass("mud-list-item-disabled", GetDisabled())
                .AddClass(Class)
                .Build();

        [Inject]
        protected NavigationManager UriHelper { get; set; } = null!;

        [CascadingParameter]
        protected MudList<T>? MudList { get; set; }

        private MudList<T>? TopLevelList => MudList?.TopLevelList;

        /// <summary>
        /// The text to display.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public string? Text { get; set; }

        /// <summary>
        /// The secondary text displayed.
        /// </summary>
        /// <remarks>
        /// This text is displayed under <see cref="Text"/>, in a smaller size.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public string? SecondaryText { get; set; }

        /// <summary>
        /// The value associated with this item.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Selecting)]
        public T? Value { get; set; }

        /// <summary>
        /// The custom <see cref="MudAvatar" /> to display to the left of <see cref="Text"/>.
        /// </summary>
        /// <remarks>
        /// When a value is set, <see cref="Icon"/> is ignored.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Chip.Appearance)]
        public RenderFragment? AvatarContent { get; set; }

        /// <summary>
        /// The URL to navigate to upon click.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.ClickAction)]
        public string? Href { get; set; }

        /// <summary>
        /// The browser frame to open this link when <see cref="Href"/> is specified.
        /// </summary>
        /// <remarks>
        /// Possible values include <c>_blank</c>, <c>_self</c>, <c>_parent</c>, <c>_top</c>, or a <i>frame name</i>. <br/>
        /// Defaults to <c>null</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Button.ClickAction)]
        public string? Target { get; set; }

        /// <summary>
        /// Causes a full page refresh when this list item is clicked and <see cref="Href"/> is set.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  When <c>true</c>, bypasses client-side routing and forces the browser to load the 
        /// new page from the server, whether or not the URI would normally be handled by the client-side router.
        /// See: <see cref="NavigationManager.NavigateTo(string, bool, bool)"/>
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.List.ClickAction)]
        public bool ForceLoad { get; set; }

        /// <summary>
        /// Prevents this list item from being clicked.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  This value can be overridden by <see cref="MudList{T}.Disabled"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public bool Disabled { get; set; }

        /// <summary>
        /// Shows a ripple effect when this item is clicked.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool Ripple { get; set; } = true;

        /// <summary>
        /// The icon to display for this list item.
        /// </summary>
        /// <remarks>
        /// When <see cref="AvatarContent"/> is set, this property is ignored.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public string? Icon { get; set; }

        /// <summary>
        /// The color of the <see cref="Icon"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Color.Inherit"/>.  When <see cref="AvatarContent"/> is set, this property is ignored.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public Color IconColor { get; set; } = Color.Inherit;

        /// <summary>
        /// The size of the <see cref="Icon"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Size.Medium"/>.  When <see cref="AvatarContent"/> is set, this property is ignored.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public Size IconSize { get; set; } = Size.Medium;

        /// <summary>
        /// The color of the <see cref="ExpandLessIcon"/> and <see cref="ExpandMoreIcon"/> icons.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Color.Default"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.List.Expanding)]
        public Color ExpandIconColor { get; set; } = Color.Default;

        /// <summary>
        /// The icon displayed when <see cref="Expanded"/> is <c>true</c>.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.ExpandLess"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.List.Expanding)]
        public string ExpandLessIcon { get; set; } = Icons.Material.Filled.ExpandLess;

        /// <summary>
        /// The icon displayed when <see cref="Expanded"/> is <c>false</c>.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.ExpandMore"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.List.Expanding)]
        public string ExpandMoreIcon { get; set; } = Icons.Material.Filled.ExpandMore;

        /// <summary>
        /// Applies an indent to this list item.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool Inset { get; set; }

        /// <summary>
        /// Uses less vertical padding between items.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool? Dense { get; set; }

        /// <summary>
        /// Applies left and right padding to this list items.
        /// </summary>
        /// <remarks>
        /// Defaults to the value of the parent <see cref="MudList{T}.Gutters"/>. When set, it overrides the list's setting.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool? Gutters { get; set; }

        /// <summary>
        /// Expand or collapse nested list. Two-way bindable. Note: if you directly set this to
        /// true or false (instead of using two-way binding) it will initialize the nested list's expansion state.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Expanding)]
        public bool Expanded { get; set; }

        [Parameter]
        public EventCallback<bool> ExpandedChanged { get; set; }

        /// <summary>
        /// Display content of this list item. If set, this overrides Text
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// If set, clicking the list item will only execute the OnClick handler and prevent all other
        /// functionality such as following Href or selection.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public bool OnClickPreventDefault { get; set; }

        /// <summary>
        /// Add child list items here to create a nested list.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public RenderFragment? NestedList { get; set; }

        /// <summary>
        /// List click event.
        /// Also called when <see cref="Href"/> is set
        /// </summary>
        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        private IEqualityComparer<T?> Comparer => MudList?.Comparer ?? EqualityComparer<T?>.Default;

        private bool ReadOnly => TopLevelList is not null && TopLevelList.ReadOnly;

        private SelectionMode SelectionMode => TopLevelList?.SelectionMode ?? SelectionMode.SingleSelection;

        private Typo TextTypo => GetDense() ? Typo.body2 : Typo.body1;

        private bool GetClickable()
        {
            if (Disabled)
            {
                return false;
            }
            if (NestedList != null)
            {
                return true;
            }
            return !GetReadOnly();
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            if (MudList is not null)
            {
                await MudList.RegisterAsync(this);
            }
        }

        protected async Task OnClickHandlerAsync(MouseEventArgs eventArgs)
        {
            if (GetDisabled())
            {
                return;
            }
            if (OnClickPreventDefault)
            {
                await OnClick.InvokeAsync(eventArgs);
                return;
            }
            if (NestedList != null)
            {
                await _expandedState.SetValueAsync(!_expandedState.Value);
                return;
            }
            if (TopLevelList is not null && !GetReadOnly())
            {
                var value = GetValue();
                if (MultiSelection)
                {
                    await OnCheckboxChangedAsync();
                }
                else
                {
                    if (SelectionMode == SelectionMode.SingleSelection)
                    {
                        await TopLevelList.SetSelectedValueAsync(value);
                    }
                    else // ToggleSelection
                    {
                        await TopLevelList.SetSelectedValueAsync(_selected ? default : value);
                    }
                }
            }
            await OnClick.InvokeAsync(eventArgs);
            // the only case a manual Navigition is required, is when
            // the target is empty, but a force reload is desired, all other cases are handled
            // by the html anchor
            if (ForceLoad && string.IsNullOrEmpty(Href) == false && string.IsNullOrEmpty(Target))
            {
                UriHelper.NavigateTo(Href, forceLoad: ForceLoad);
            }
        }

        internal void SetSelected(bool selected)
        {
            if (GetDisabled() || _selected == selected)
            {
                return;
            }
            _selected = selected;
            StateHasChanged();
        }

        internal T? GetValue()
        {
            if (typeof(T) == typeof(string) && Value is null && Text is not null)
            {
                return (T)(object)Text;
            }
            return Value;
        }

        private bool GetDisabled() => Disabled || MudList?.GetDisabled() == true || TopLevelList?.GetDisabled() == true;

        private bool GetReadOnly() => MudList?.ReadOnly == true || TopLevelList?.GetReadOnly() == true;

        private bool GetDense() => Dense ?? MudList?.Dense == true;

        private bool GetGutters() => Gutters ?? MudList?.Gutters ?? true;

        private bool? GetCheckBoxState() => _selected;

        private async Task OnCheckboxChangedAsync()
        {
            if (TopLevelList is null || ReadOnly || GetDisabled())
            {
                return;
            }
            if (_selected)
            {
                await TopLevelList.DeselectValueAsync(GetValue());
            }
            else
            {
                await TopLevelList.SelectValueAsync(GetValue());
            }
        }

        private string GetIndeterminateIcon()
        {
            // Note: this will only become important should we ever want to add checkboxes for nesting list items similar to treeview
            // in non-tristate mode we need to fake the checked status. the actual status of the checkbox is irrelevant,
            // only _selected matters!
            return _selected ? GetCheckedIcon() : GetUncheckedIcon();
        }

        private Color GetCheckBoxColor() => TopLevelList?.CheckBoxColor ?? default;

        private string GetCheckedIcon() => TopLevelList?.CheckedIcon ?? Icons.Material.Filled.CheckBox;

        private string GetUncheckedIcon() => TopLevelList?.UncheckedIcon ?? Icons.Material.Filled.CheckBoxOutlineBlank;

        /// <summary>
        /// returns the kind of element the list item should render to
        /// When <see cref="OnClickPreventDefault"/> is set the link should not be followed thus it is rendered as div
        /// </summary>        
        private string HtmlTag => string.IsNullOrEmpty(Href) || OnClickPreventDefault ? "div" : "a";

        private bool GetPreventDefault() => GetDisabled();

        private bool GetClickPropagation() => false;

        public void Dispose()
        {
            if (MudList is null)
            {
                return;
            }
            try
            {
                MudList.Unregister(this);
            }
            catch (Exception) { /*ignore*/ }
        }
    }
}
