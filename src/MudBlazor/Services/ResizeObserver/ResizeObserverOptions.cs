namespace MudBlazor.Services
{
    public class ResizeObserverOptions
    {
        /// <summary>
        /// Timepsan in milliseconds after the browser detect the last chance and notify the interop service.
        /// Setting this value too low can cause poor application performance.
        /// </summary>
        public int ReportRate { get; set; } = 200;

        /// <summary>
        /// Report resize events in the browser's console.
        /// </summary>
        public bool EnableLogging { get; set; } = false;
    }
}
