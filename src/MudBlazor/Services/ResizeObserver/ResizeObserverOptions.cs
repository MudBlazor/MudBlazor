﻿namespace MudBlazor.Services;

#nullable enable
/// <summary>
/// Options for configuring the <see cref="IResizeObserver"/>.
/// </summary>
public class ResizeObserverOptions
{
    /// <summary>
    /// Timespan in milliseconds after the browser detects the last change and notifies the interop service.
    /// Setting this value too low can cause poor application performance.
    /// </summary>
    public int ReportRate { get; set; } = 200;

    /// <summary>
    /// Report resize events in the browser's console.
    /// </summary>
    public bool EnableLogging { get; set; }
}
