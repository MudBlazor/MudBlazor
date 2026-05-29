namespace MudBlazor.Services
{
    // Add additional configuration objects here when adding new services

    /// <summary>
    /// Shared configuration container for MudBlazor services registered in DI.
    /// </summary>
    /// <remarks>
    /// This is the aggregation point used by the MudBlazor service registration helpers so callers can configure related services in one place.
    /// </remarks>
    public class MudServicesConfiguration
    {
        public SnackbarConfiguration SnackbarConfiguration { get; set; } = new SnackbarConfiguration();

        public ResizeOptions ResizeOptions { get; set; } = new ResizeOptions();

        public ResizeObserverOptions ResizeObserverOptions { get; set; } = new ResizeObserverOptions();

        public PopoverOptions PopoverOptions { get; set; } = new PopoverOptions();
    }
}
