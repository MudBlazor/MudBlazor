namespace MudBlazor;

/// <summary>
/// The design language of the MudBlazor components.
/// </summary>
public enum DesignLanguage
{
    /// <summary>
    /// Displays the components based on the <a href="https://m2.material.io">Material Design 2 guidelines</a>.
    /// </summary>
    MaterialV2 = 0,

    /// <summary>
    /// Displays the components based on the <a href="https://m3.material.io">Material Design 3 guidelines</a>.
    /// </summary>
    /// <remarks>
    /// This design language is still under development, and not all components support it yet; These components will be displayed using <see cref="MaterialV2"/>.
    /// </remarks>
    MaterialV3 = 1
}
