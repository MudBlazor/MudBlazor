using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor;

/// <summary>
/// A floating action button shown as one of the selectable options within a <see cref="MudFabMenu"/>.
/// </summary>
/// <seealso cref="MudFab" />
/// <seealso cref="MudFabMenu" />
public partial class MudFabMenuItem : MudFab
{
    /// <summary>
    /// Indicates whether the <see cref="Variant"/> property was explicitly set by the user.
    /// </summary>
    private bool _variantExplicitlySet;

    /// <summary>
    /// CSS class names for the component, including base classes and conditional classes based on properties.
    /// </summary>
    private new string Classname => new CssBuilder("mud-fab-menu-item")
            .AddClass(Class)
            .Build();

    /// <summary>
    /// The parent <see cref="MudFabMenu"/> component, used to inherit <see cref="Variant"/> when not explicitly set.
    /// </summary>
    [CascadingParameter]
    public MudFabMenu? MudFabMenu { get; set; }

    /// <summary>
    /// The display variation to use.
    /// </summary>
    private Variant EffectiveVariant => _variantExplicitlySet ? Variant : MudFabMenu?.Variant ?? Variant;

    /// <summary>
    /// The size of the menu item.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="Size.Medium"/>.
    /// </remarks>
    [Parameter, Category(CategoryTypes.Button.Appearance)]
    public override Size Size { get; set; } = Size.Medium;

    /// <summary>
    /// Sets the parameters for the component and determines if the Variant was explicitly set.
    /// </summary>
    /// <param name="parameters">The parameters to set.</param>
    public override Task SetParametersAsync(ParameterView parameters)
    {
        _variantExplicitlySet = parameters.TryGetValue<Variant>(nameof(Variant), out _);
        return base.SetParametersAsync(parameters);
    }
}
