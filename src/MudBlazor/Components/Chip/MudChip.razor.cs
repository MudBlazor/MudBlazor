using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Services;
using MudBlazor.State;
using MudBlazor.Utilities;

namespace MudBlazor;

#nullable enable

/// <summary>
/// Represents a compact element used to enter information, select a choice, filter content, or trigger an action.
/// </summary>
/// <typeparam name="T">The type of item managed by this component.</typeparam>
/// <seealso cref="MudChipSet{T}"/>
public partial class MudChip<T> : MudComponentBase, IAsyncDisposable
{
    [Inject]
    private IKeyInterceptorService KeyInterceptorService { get; set; } = null!;

    private string _chipContainerId = $"chip-container-{Guid.NewGuid()}";

    internal readonly ParameterState<bool> SelectedState;

    public MudChip()
    {
        using var registerScope = CreateRegisterScope();
        SelectedState = registerScope.RegisterParameter<bool>(nameof(Selected))
            .WithParameter(() => Selected)
            .WithEventCallback(() => SelectedChanged)
            .WithChangeHandler(OnSelectedChangedAsync);
    }

    private Task OnSelectedChangedAsync(ParameterChangedEventArgs<bool> args)
    {
        if (ChipSet == null)
            return Task.CompletedTask;
        return ChipSet.OnChipSelectedChangedAsync(this, args.Value);
    }

    internal async Task UpdateSelectionStateAsync(bool selected)
    {
        await SelectedState.SetValueAsync(selected);
        StateHasChanged();
    }

    /// <summary>
    /// The service used to navigate the browser to another URL.
    /// </summary>
    [Inject]
    public NavigationManager? UriHelper { get; set; }

    /// <summary>
    /// The service used to perform browser actions such as navigation.
    /// </summary>
    [Inject]
    public IJsApiService? JsApiService { get; set; }

    protected string Classname => new CssBuilder("mud-chip")
        .AddClass($"mud-chip-{GetVariant().ToDescriptionString()}")
        .AddClass($"mud-chip-size-{GetSize().ToDescriptionString()}")
        .AddClass($"mud-chip-color-{GetColor().ToDescriptionString()}")
        .AddClass("mud-clickable", IsButton || IsAnchor)
        .AddClass("mud-ripple", IsButton && GetRipple())
        .AddClass("mud-chip-label", GetLabel())
        .AddClass("mud-disabled", GetDisabled())
        .AddClass("mud-chip-selected", SelectedState.Value)
        .AddClass(Class)
        .Build();

    private bool IsAnchor => !string.IsNullOrWhiteSpace(Href);

    private bool IsButton => GetDisabled() is false
                             && GetReadOnly() is false
                             && (ChipSet is not null || OnClick.HasDelegate);

    private bool IsClosable => (OnClose.HasDelegate || ChipSet?.AllClosable == true) && !IsAnchor;

    protected string GetHtmlTag()
    {
        if (IsButton)
        {
            return "button";
        }

        if (IsAnchor)
        {
            return "a";
        }

        return "div";
    }

    protected Dictionary<string, object?> GetAttributes()
    {
        var attributes = new Dictionary<string, object?>();

        if (IsButton)
        {
            attributes.Add("tabindex", 0);
            attributes.Add("type", "button");
        }
        else if (IsAnchor)
        {
            attributes.Add("tabindex", 0);

            attributes.Add("href", Href);
            attributes.Add("target", Target);

            if (Rel is null && Target == "_blank")
            {
                attributes.Add("rel", "noopener");
            }
            else
            {
                attributes.Add("rel", Rel);
            }
        }
        else
        {
            attributes.Add("tabindex", -1);
        }

        // User-defined attributes always take priority.
        foreach (var attribute in UserAttributes)
        {
            attributes[attribute.Key] = attribute.Value;
        }

        return attributes;
    }

    internal Variant GetVariant()
    {
        var chipSetVariant = ChipSet?.Variant ?? MudBlazor.Variant.Filled;
        var variant = Variant ?? chipSetVariant;
        return variant switch
        {
            MudBlazor.Variant.Text => SelectedState.Value ? MudBlazor.Variant.Filled : MudBlazor.Variant.Text,
            MudBlazor.Variant.Filled => SelectedState.Value ? MudBlazor.Variant.Text : MudBlazor.Variant.Filled,
            MudBlazor.Variant.Outlined => MudBlazor.Variant.Outlined,
            _ => MudBlazor.Variant.Outlined
        };
    }

    private Color GetColor()
    {
        var selectedColor = GetSelectedColor();
        if (SelectedState.Value && selectedColor != MudBlazor.Color.Inherit)
        {
            return selectedColor;
        }
        return Color ?? ChipSet?.Color ?? MudBlazor.Color.Default;
    }

    private Color GetSelectedColor() => SelectedColor ?? ChipSet?.SelectedColor ?? MudBlazor.Color.Inherit;

    private Color GetIconColor() => IconColor ?? ChipSet?.IconColor ?? MudBlazor.Color.Inherit;

    private Size GetSize() => Size ?? ChipSet?.Size ?? MudBlazor.Size.Medium;

    private bool GetDisabled() => Disabled || (ChipSet?.Disabled ?? false);

    private bool GetReadOnly() => ChipSet?.ReadOnly ?? false;

    private bool GetRipple() => Ripple ?? ChipSet?.Ripple ?? true;

    private bool GetLabel() => Label ?? ChipSet?.Label ?? false;

    private string GetCheckedIcon() => CheckedIcon ?? ChipSet?.CheckedIcon ?? Icons.Material.Filled.Check;

    private string GetCloseIcon() => CloseIcon ?? ChipSet?.CloseIcon ?? Icons.Material.Filled.Cancel;

    internal bool ShowCheckMark => SelectedState.Value && ChipSet?.CheckMark == true;

    [CascadingParameter]
    private MudChipSet<T>? ChipSet { get; set; }

    /// <summary>
    /// The color of this chip.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.  When <see cref="SelectedColor"/> is set, this color is used when the chip is unselected.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Chip.Appearance)]
    public Color? Color { get; set; }

    /// <summary>
    /// The color of the chip when it is selected.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.  When set, this color is used for a selected chip, otherwise <see cref="Color"/> is used.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Chip.Appearance)]
    public Color? SelectedColor { get; set; }

    /// <summary>
    /// The size of the chip.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Chip.Appearance)]
    public Size? Size { get; set; }

    /// <summary>
    /// The display variation to use.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Chip.Appearance)]
    public Variant? Variant { get; set; }

    /// <summary>
    /// The avatar content to display inside the chip.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chip.Appearance)]
    public RenderFragment? AvatarContent { get; set; }

    /// <summary>
    /// Uses the theme border radius for chip edges.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.  When <c>true</c>, the <see cref="LayoutProperties.DefaultBorderRadius"/> is used for chip edges.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Chip.Appearance)]
    public bool? Label { get; set; }

    /// <summary>
    /// Prevents the user from interacting with this chip.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.  When <c>true</c>, the chip is visibly disabled and interaction is not allowed.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Chip.Behavior)]
    public bool Disabled { get; set; }

    /// <summary>
    /// The icon to display within the chip.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.  Use the <see cref="IconColor"/> to control the color of this icon.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Chip.Behavior)]
    public string? Icon { get; set; }

    /// <summary>
    /// The icon to display when <see cref="Selected"/> is <c>true</c>.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Chip.Appearance)]
    public string? CheckedIcon { get; set; }

    /// <summary>
    /// The color of the <see cref="Icon"/>.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Chip.Appearance)]
    public Color? IconColor { get; set; }

    /// <summary>
    /// The close icon to display when <see cref="OnClose"/> is set.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chip.Appearance)]
    public string? CloseIcon { get; set; }

    /// <summary>
    /// Displays a ripple effect when this chip is clicked.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Chip.Appearance)]
    public bool? Ripple { get; set; }

    /// <summary>
    /// The content within this chip.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chip.Behavior)]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// The URL to navigate to when the chip is clicked.
    /// </summary>
    /// <remarks>
    /// <para>Defaults to <c>null</c>.  Use <see cref="Target"/> to control where the URL is opened.</para>
    /// <para>Note: The close button cannot be enabled if this is set because <see href="https://developer.mozilla.org/en-US/docs/Web/HTML/Element/a#technical_summary">interactive content violates the HTML spec</see>.</para>
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Chip.ClickAction)]
    public string? Href { get; set; }

    /// <summary>
    /// The target to open URLs if <see cref="Href"/> is set.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.  This value is typically <c>_blank</c>, <c>_self</c>, <c>_parent</c>, <c>_top</c>, or the name of an <c>iframe</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Chip.ClickAction)]
    public string? Target { get; set; }

    /// <summary>
    /// The relationship between the current document and the linked document when <see cref="Href"/> is set.
    /// </summary>
    /// <remarks>
    /// This property is typically used by web crawlers to get more information about a link.  Common values can be found here: <see href="https://www.w3schools.com/tags/att_a_rel.asp" />
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Chip.ClickAction)]
    public string? Rel { get; set; }

    /// <summary>
    /// The text label for the chip.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.  This will be shown so long as <see cref="ChildContent"/> is not set.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Chip.Behavior)]
    public string? Text { get; set; }

    /// <summary>
    /// The value applied when the chip is selected.
    /// </summary>
    /// <remarks>
    /// When part of a <see cref="MudChipSet{T}"/>, the <see cref="MudChipSet{T}.SelectedValue"/> is set to this value when the chip is selected.  Once set, the value should not change.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Chip.Behavior)]
    public T? Value { get; set; }

    /// <summary>
    /// Selects this chip by default when part of a <see cref="MudChipSet{T}"/>.
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
    /// <remarks>
    /// If an <see cref="Href"/> is set, this callback will not be triggered and the browser will handle the click.
    /// </remarks>
    [Parameter]
    public EventCallback<MouseEventArgs> OnClick { get; set; }

    /// <summary>
    /// Occurs when this chip has been closed.
    /// </summary>
    /// <remarks>
    /// Subscribing to this event enables the close button, unless <see cref="Href"/> is also set.
    /// </remarks>
    [Parameter]
    public EventCallback<MudChip<T>> OnClose { get; set; }

    /// <summary>
    /// Selects this chip.
    /// </summary>
    /// <remarks>
    /// When <c>true</c>, the chip is displayed in a selected state.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Chip.Behavior)]
    public bool Selected { get; set; }

    /// <summary>
    /// Occurs when the <see cref="Selected"/> property has changed.
    /// </summary>
    [Parameter]
    public EventCallback<bool> SelectedChanged { get; set; }

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

        if (ChipSet is not null)
        {
            await ChipSet.AddAsync(this);
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            var options = new KeyInterceptorOptions(
                "mud-chip",
                [
                    new(" ", preventDown: "key+none", preventUp: "key+none"),
                    new("Backspace", preventDown: "key+none"),
                    new("Delete", preventDown: "key+none")
                ]);

            await KeyInterceptorService.SubscribeAsync(_chipContainerId, options, keyDown: HandleKeyDownAsync);
        }
    }

    protected internal async Task OnClickAsync(MouseEventArgs ev)
    {
        if (ChipSet?.ReadOnly == true || IsAnchor)
        {
            return;
        }
        if (ChipSet != null)
        {
            await SelectedState.SetValueAsync(!SelectedState.Value);
            await ChipSet.OnChipSelectedChangedAsync(this, SelectedState.Value);
        }

        await OnClick.InvokeAsync(ev);
    }

    protected async Task OnCloseAsync(MouseEventArgs ev)
    {
        if (GetReadOnly() || IsClosable is false)
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

    private async Task HandleKeyDownAsync(KeyboardEventArgs args)
    {
        if (GetDisabled() || GetReadOnly())
        {
            return;
        }

        switch (args.Key)
        {
            case " ":
                await OnClickAsync(new MouseEventArgs());
                break;
            case "Backspace" or "Delete":
                await OnCloseAsync(new MouseEventArgs());
                break;
        }
    }

    /// <summary>
    /// Releases unused resources.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        try
        {
            if (ChipSet is not null)
            {
                await ChipSet.RemoveAsync(this);
            }

            if (IsJSRuntimeAvailable)
            {
                await KeyInterceptorService.UnsubscribeAsync(_chipContainerId);
            }
        }
        catch (Exception)
        {
            /* ignore! */
        }
    }
}
