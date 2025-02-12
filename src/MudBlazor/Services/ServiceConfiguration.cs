namespace MudBlazor.Services
{
    // Add additional configuration objects here when adding new services

    /// <summary>
    /// Common services configuration required by MudBlazor components
    /// </summary>
    public class MudServicesConfiguration
    {
        public SnackbarConfiguration SnackbarConfiguration { get; set; } = new SnackbarConfiguration();

        public ResizeOptions ResizeOptions { get; set; } = new ResizeOptions();

        public ResizeObserverOptions ResizeObserverOptions { get; set; } = new ResizeObserverOptions();

        public PopoverOptions PopoverOptions { get; set; } = new PopoverOptions();
    }
}
