namespace MudBlazor.Services
{
    // Add additional configuration objects here when adding new services

    /// <summary>
    /// Common services configuration required by MudBlazor components
    /// </summary>
    public class MudServicesConfiguration
    {
        public SnackbarConfiguration SnackbarConfiguration { get; set; } = new();
        public ResizeOptions ResizeOptions { get; set; } = new();
        public ResizeObserverOptions ResizeObserverOptions { get; set; } = new();
    }
}
