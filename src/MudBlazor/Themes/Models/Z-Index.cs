namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// Represents the Z-index values for different components.
    /// </summary>
    public class ZIndex
    {
        /// <summary>
        /// Gets or sets the Z-index value for the Drawer component.
        /// Default value is "1100"".
        /// </summary>
        public int Drawer { get; set; } = 1100;

        /// <summary>
        /// Gets or sets the Z-index value for the Popover component.
        /// Default value is "1200".
        /// </summary>
        public int Popover { get; set; } = 1200;

        /// <summary>
        /// Gets or sets the Z-index value for the AppBar component.
        /// Default value is "1300".
        /// </summary>
        public int AppBar { get; set; } = 1300;

        /// <summary>
        /// Gets or sets the Z-index value for the Dialog component.
        /// Default value is "1400".
        /// </summary>
        public int Dialog { get; set; } = 1400;

        /// <summary>
        /// Gets or sets the Z-index value for the SnackBar component.
        /// Default value is "1500".
        /// </summary>
        public int Snackbar { get; set; } = 1500;

        /// <summary>
        /// Gets or sets the Z-index value for the Tooltip component.
        /// Default value is "1600".
        /// </summary>
        public int Tooltip { get; set; } = 1600;
    }
}
