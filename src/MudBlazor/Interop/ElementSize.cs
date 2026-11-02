namespace MudBlazor.Interop;

/// <summary>
/// Width and height of an HTML element, with a timestamp used to discard out-of-order resize events in Blazor Server.
/// </summary>
public class ElementSize
{
    /// <summary>
    /// The height of the Element.
    /// </summary>
    public required double Height { get; init; }

    /// <summary>
    /// The width of the Element.
    /// </summary>
    public required double Width { get; init; }

    /// <summary>
    /// The timestamp of the size change, used to help filter out of order events for server mode.
    /// </summary>
    public long Timestamp { get; set; }
}
