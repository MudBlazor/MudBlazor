namespace MudBlazor.Services;

/// <summary>
/// Configuration knobs for resize observation, including debounce timing and optional logging.
/// </summary>
/// <remarks>
/// Consumers pass these options when registering or creating an <see cref="IResizeObserver"/> to tune how often callbacks fire and to enable diagnostics while troubleshooting layout behavior.
/// </remarks>
public class ResizeObserverOptions
{
    /// <summary>
    /// Timespan in milliseconds after the browser detects the last change and notifies the interop service.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>200</c>.  Setting this value too low can cause poor application performance.
    /// </remarks>
    public int ReportRate { get; set; } = 200;

    /// <summary>
    /// Report resize events in the browser's console.
    /// </summary>
    public bool EnableLogging { get; set; }
}
