namespace MudBlazor.Services
{
    // Add additional configuration objects here when adding new services

    /// <summary>
    /// Common services configuration required by MudBlazor components
    /// </summary>
    public class MudServicesConfiguration
    {
        public SnackbarConfiguration SnackbarConfiguration;
        public ResizeOptions ResizeOptions;

        public MudServicesConfiguration()
        {
            SnackbarConfiguration  = new SnackbarConfiguration();
            ResizeOptions = new ResizeOptions();
        }
    }
}