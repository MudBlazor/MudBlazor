namespace MudBlazor.Services;

#nullable enable
/// <summary>
/// Represents the size of a browser window.
/// </summary>
public class BrowserWindowSize : EventArgs
{
    /// <summary>
    /// Height of the browser window.
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    /// Width of the browser window.
    /// </summary>
    public int Width { get; set; }
}
