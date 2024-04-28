﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.State;
using MudBlazor.Utilities;

namespace MudBlazor;

#nullable enable

/// <summary>
/// Represents a compact element used to enter information, select a choice, filter content, or trigger an action.
/// </summary>
/// <typeparam name="T">The type of item managed by this component.</typeparam>
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

    /// <summary>
    /// Gets or sets the service used to navigate the browser to another URL.
    /// </summary>
    [Inject]
    public NavigationManager? UriHelper { get; set; }

    /// <summary>
    /// Gets or sets the service used to perform browser actions such as navigation.
    /// </summary>
    [Inject]
    public IJsApiService? JsApiService { get; set; }

    protected string Classname => new CssBuilder("mud-chip")
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
    /// Gets or sets the color of this chip.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.  When <see cref="SelectedColor"/> is set, this color is used when the chip is unselected.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Chip.Appearance)]
    public Color? Color { get; set; }

    /// <summary>
    /// The chip color to use when selected, only works together with ChipSet, Color.Inherit for default value.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.  When set, this color is used for a selected chip, otherwise <see cref="Color"/> is used.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Chip.Appearance)]
    public Color? SelectedColor { get; set; }

    /// <summary>
    /// Gets or sets the size of the chip.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Chip.Appearance)]
    public Size? Size { get; set; }

    /// <summary>
    /// Gets or sets the display variation to use.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Chip.Appearance)]
    public Variant? Variant { get; set; }

    /// <summary>
    /// Gets or sets any avatar content to display inside the chip.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chip.Appearance)]
    public RenderFragment? AvatarContent { get; set; }

    /// <summary>
    /// Gets or sets whether the theme border radius is used for the chip edges.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.  When <c>true</c>, the <see cref="LayoutProperties.DefaultBorderRadius"/> is used for chip edges.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Chip.Appearance)]
    public bool? Label { get; set; }

    /// <summary>
    /// Gets or sets whether the user cannot interact with this chip.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.  When <c>true</c>, the chip is visibly disabled and interaction is not allowed.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Chip.Behavior)]
    public bool Disabled { get; set; }

    /// <summary>
    /// Gets or sets the icon to display within the chip.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.  Use the <see cref="IconColor"/> to control the color of this icon.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Chip.Behavior)]
    public string? Icon { get; set; }

    /// <summary>
    /// Gets or sets the icon to display when <see cref="IsSelected"/> is <c>true</c>.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Chip.Appearance)]
    public string? CheckedIcon { get; set; }

    /// <summary>
    /// Gets or sets the color of the <see cref="Icon"/>.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Chip.Appearance)]
    public Color? IconColor { get; set; }

    /// <summary>
    /// Gets or sets the close icon to display when <see cref="OnClose"/> is set.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chip.Appearance)]
    public string? CloseIcon { get; set; }

    /// <summary>
    /// Gets or sets whether a ripple effect is show when the chip is clicked.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Chip.Appearance)]
    public bool? Ripple { get; set; }

    /// <summary>
    /// Gets or sets any custom content within this chip.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chip.Behavior)]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets the URL to navigate to when the chip is clicked.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.  Use <see cref="Target"/> to control where the URL is opened.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Chip.ClickAction)]
    public string? Href { get; set; }

    /// <summary>
    /// Gets or sets the target to open URLs if <see cref="Href"/> is set.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.  This value is typically <c>_blank</c>, <c>_self</c>, <c>_parent</c>, <c>_top</c>, or the name of an <c>iframe</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Chip.ClickAction)]
    public string? Target { get; set; }

    /// <summary>
    /// Gets or sets the text label for the chip.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.  This will be shown so long as <see cref="ChildContent"/> is not set.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Chip.Behavior)]
    public string? Text { get; set; }

    /// <summary>
    /// Gets or sets the value applied when the chip is selected.
    /// </summary>
    /// <remarks>
    /// When part of a <see cref="MudChipSet{T}"/>, the <see cref="MudChipSet{T}.SelectedValue"/> is set to this value when the chip is selected.  Once set, the value should not change.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Chip.Behavior)]
    public T? Value { get; set; }

    /// <summary>
    /// Gets or sets whether a full page refresh is performed when navigating to the URL in <see cref="Href"/>.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.  When <c>true</c>, client-side routing is bypassed and a full page reload occurs.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Chip.ClickAction)]
    public bool ForceLoad { get; set; }

    /// <summary>
    /// Gets or sets whether this chip is selected by default when part of a <see cref="MudChipSet{T}"/>.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Chip.Behavior)]
    public bool? Default { get; set; }

    /// <summary>
    /// Occurs when this chip is clicked.
    /// </summary>
    [Parameter]
    public EventCallback<MouseEventArgs> OnClick { get; set; }

    /// <summary>
    /// Occurs when this chip has been closed.
    /// </summary>
    /// <remarks>
    /// When set, the close icon can be controlled via the <see cref="CloseIcon"/> property.
    /// </remarks>
    [Parameter]
    public EventCallback<MudChip<T>> OnClose { get; set; }

    internal bool ShowCheckMark => IsSelectedState.Value && ChipSet?.CheckMark == true;

    /// <summary>
    /// Gets or sets whether this chip is selected.
    /// </summary>
    /// <remarks>
    /// When <c>true</c>, the chip is displayed in a selected state.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Chip.Behavior)]
    public bool IsSelected { get; set; }

    /// <summary>
    /// Occurs when the <see cref="IsSelected"/> property has changed.
    /// </summary>
    [Parameter]
    public EventCallback<bool> IsSelectedChanged { get; set; }

    internal T? GetValue()
    {
        if (typeof(T) == typeof(string) && Value is null && Text is not null)
            return (T)(object)Text;
        return Value;
    }

    /// <inheritdoc />
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

    /// <summary>
    /// Releases unused resources.
    /// </summary>
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
