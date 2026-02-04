namespace MudBlazor.Services;

/// <summary>
/// Simple size payload for browser viewport measurements.
/// </summary>
/// <remarks>
/// Used by viewport services to carry width/height measurements from JS to .NET.
/// </remarks>
public class BrowserWindowSize : EventArgs
{
    /// <summary>
    /// Gets or sets the height of the browser window.
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    /// Gets or sets the width of the browser window.
    /// </summary>
    public int Width { get; set; }
}
