namespace MudBlazor.Services
{
    public class ResizeOptions
    {
        /// <summary>
        /// Rate in milliseconds that the browsers `resize()` event should report a change.
        /// Setting this value too low can cause poor application performance.
        /// </summary>
        public int ReportRate { get; set; } = 100;

        /// <summary>
        /// Report resize events and media queries in the browser's console.
        /// </summary>
        public bool EnableLogging { get; set; } = false;

        /// <summary>
        /// Suppress the first OnResized that is invoked when a new event handler is added.
        /// </summary>
        public bool SuppressInitEvent { get; set; } = false;
    }
}
