using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.State;
using MudBlazor.Utilities;

namespace MudBlazor;

#nullable enable
public partial class MudChip<T> : MudComponentBase, IAsyncDisposable
{
    public MudChip()
    {
        using var registerScope = CreateRegisterScope();
        IsSelectedState = registerScope.RegisterParameter<bool>(nameof(IsSelected))
            .WithParameter(() => IsSelected)
            .WithEventCallback(() => IsSelectedChanged)
            .WithChangeHandler(OnIsSelectedChangedAsync);
    }

    private Task OnIsSelectedChangedAsync(ParameterChangedEventArgs<bool> args)
    {
        if (ChipSet == null)
            return Task.CompletedTask;
        return ChipSet.OnChipIsSelectedChangedAsync(this, args.Value);
    }

    internal async Task UpdateSelectionStateAsync(bool isSelected)
    {
        await IsSelectedState.SetValueAsync(isSelected);
        StateHasChanged();
    }

    internal readonly ParameterState<bool> IsSelectedState;

    [Inject]
    public NavigationManager? UriHelper { get; set; }

    [Inject]
    public IJsApiService? JsApiService { get; set; }

    protected string Classname =>
        new CssBuilder("mud-chip")
            .AddClass($"mud-chip-{GetVariant().ToDescriptionString()}")
            .AddClass($"mud-chip-size-{GetSize().ToDescriptionString()}")
            .AddClass($"mud-chip-color-{GetColor().ToDescriptionString()}")
            .AddClass("mud-clickable", IsClickable)
            .AddClass("mud-ripple", IsClickable && GetRipple())
            .AddClass("mud-chip-label", GetLabel())
            .AddClass("mud-disabled", GetDisabled())
            .AddClass("mud-chip-selected", IsSelectedState.Value)
            .AddClass(Class)
            .Build();

    private bool IsClickable => !ChipSet?.ReadOnly ?? (OnClick.HasDelegate || !string.IsNullOrEmpty(Href));

    internal Variant GetVariant()
    {
        var chipSetVariant = ChipSet?.Variant ?? MudBlazor.Variant.Filled;
        var variant = Variant ?? chipSetVariant;
        return variant switch
        {
            MudBlazor.Variant.Text => IsSelectedState.Value ? MudBlazor.Variant.Filled : MudBlazor.Variant.Text,
            MudBlazor.Variant.Filled => IsSelectedState.Value ? MudBlazor.Variant.Text : MudBlazor.Variant.Filled,
            MudBlazor.Variant.Outlined => MudBlazor.Variant.Outlined,
            _ => MudBlazor.Variant.Outlined
        };
    }

    private Color GetColor()
    {
        var selectedColor = GetSelectedColor();
        if (IsSelectedState.Value && selectedColor != MudBlazor.Color.Inherit)
        {
            return selectedColor;
        }
        return Color ?? ChipSet?.Color ?? MudBlazor.Color.Default;
    }

    private Color GetSelectedColor() => SelectedColor ?? ChipSet?.SelectedColor ?? MudBlazor.Color.Inherit;

    private Color GetIconColor() => IconColor ?? ChipSet?.IconColor ?? MudBlazor.Color.Inherit;

    private Size GetSize() => Size ?? ChipSet?.Size ?? MudBlazor.Size.Medium;

    private bool GetDisabled() => Disabled || (ChipSet?.Disabled ?? false);

    private bool GetRipple() => Ripple ?? ChipSet?.Ripple ?? true;

    private bool GetLabel() => Label ?? ChipSet?.Label ?? false;

    private string GetCheckedIcon() => CheckedIcon ?? ChipSet?.CheckedIcon ?? Icons.Material.Filled.Check;

    private string GetCloseIcon() => CloseIcon ?? ChipSet?.CloseIcon ?? Icons.Material.Filled.Cancel;

    [CascadingParameter]
    private MudChipSet<T>? ChipSet { get; set; }

    /// <summary>
    /// The chip color when not selected.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chip.Appearance)]
    public Color? Color { get; set; }

    /// <summary>
    /// The chip color to use when selected, only works together with ChipSet, Color.Inherit for default value.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chip.Appearance)]
    public Color? SelectedColor { get; set; }

    /// <summary>
    /// The chip size.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chip.Appearance)]
    public Size? Size { get; set; }

    /// <summary>
    /// The chip variant.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chip.Appearance)]
    public Variant? Variant { get; set; }

    [Parameter]
    [Category(CategoryTypes.Chip.Appearance)]
    public RenderFragment? AvatarContent { get; set; }

    /// <summary>
    /// Removes circle edges and applies theme default.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chip.Appearance)]
    public bool? Label { get; set; }

    /// <summary>
    /// If true, the chip will be visibly disabled and interaction is disabled as well.
    /// Note, if the ChipSet is disabled this setting is overruled.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chip.Behavior)]
    public bool Disabled { get; set; }

    /// <summary>
    /// Sets the Icon to use.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chip.Behavior)]
    public string? Icon { get; set; }

    /// <summary>
    /// Custom checked icon.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chip.Appearance)]
    public string? CheckedIcon { get; set; }

    /// <summary>
    /// The color of the icon.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chip.Appearance)]
    public Color? IconColor { get; set; }

    /// <summary>
    /// Overrides the default close icon, only shown if OnClose is set.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chip.Appearance)]
    public string? CloseIcon { get; set; }

    /// <summary>
    /// If true, a ripple effect is applied to clickable chips.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chip.Appearance)]
    public bool? Ripple { get; set; }

    /// <summary>
    /// Child content of component.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chip.Behavior)]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// If set to a URL, clicking the button will open the referenced document. Use Target to specify where
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chip.ClickAction)]
    public string? Href { get; set; }

    /// <summary>
    /// The target attribute specifies where to open the link, if Href is specified. Possible values: _blank | _self | _parent | _top | <i>framename</i>
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chip.ClickAction)]
    public string? Target { get; set; }

    /// <summary>
    /// A string you want to associate with the chip. If the ChildContent is not set this will be shown as chip text.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chip.Behavior)]
    public string? Text { get; set; }

    /// <summary>
    /// A value that should be managed in the SelectedValues collection.
    /// Note: do not change the value during the chip's lifetime
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chip.Behavior)]
    public T? Value { get; set; }

    /// <summary>
    /// This concerns only chips with Href set to a hyperlink. If ForceLoad is true, the browser will follow the link outside of Blazor routing.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chip.ClickAction)]
    public bool ForceLoad { get; set; }

    /// <summary>
    /// If true, this chip is selected by default if used in a ChipSet.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chip.Behavior)]
    public bool? Default { get; set; }

    /// <summary>
    /// Chip click event, if set the chip focus, hover and click effects are applied.
    /// </summary>
    [Parameter]
    public EventCallback<MouseEventArgs> OnClick { get; set; }

    /// <summary>
    /// Chip delete event, if set the delete icon will be visible.
    /// </summary>
    [Parameter]
    public EventCallback<MudChip<T>> OnClose { get; set; }

    internal bool ShowCheckMark => IsSelectedState.Value && ChipSet?.CheckMark == true;

    /// <summary>
    /// True if the chip is selected. Bind this to manipulate the chip's selection state.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chip.Behavior)]
    public bool IsSelected { get; set; }

    /// <summary>
    /// Raised when IsSelected changes
    /// </summary>
    [Parameter]
    public EventCallback<bool> IsSelectedChanged { get; set; }

    internal T? GetValue()
    {
        if (typeof(T) == typeof(string) && Value is null && Text is not null)
            return (T)(object)Text;
        return Value;
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        if (ChipSet is null)
            return;
        await ChipSet.AddAsync(this);
    }

    protected internal async Task OnClickAsync(MouseEventArgs ev)
    {
        if (ChipSet?.ReadOnly == true)
        {
            return;
        }
        if (ChipSet != null)
        {
            await IsSelectedState.SetValueAsync(!IsSelectedState.Value);
            await ChipSet.OnChipIsSelectedChangedAsync(this, IsSelectedState.Value);
        }
        if (Href != null)
        {
            // TODO: use MudElement to render <a> and this code can be removed. we know that it has potential problems on iOS
            if (string.IsNullOrWhiteSpace(Target))
                UriHelper?.NavigateTo(Href, ForceLoad);
            else if (JsApiService != null)
                await JsApiService.Open(Href, Target);
        }
        else
        {
            await OnClick.InvokeAsync(ev);
        }
    }

    protected async Task OnCloseAsync(MouseEventArgs ev)
    {
        if (ChipSet?.ReadOnly == true)
        {
            return;
        }
        await OnClose.InvokeAsync(this);
        if (ChipSet is not null)
        {
            await ChipSet.OnChipDeletedAsync(this);
        }

        StateHasChanged();
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            if (ChipSet is null)
                return;
            await ChipSet.RemoveAsync(this);
        }
        catch (Exception)
        {
            /* ignore! */
        }
    }
}
