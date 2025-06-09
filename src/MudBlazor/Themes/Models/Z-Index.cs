namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// Represents the Z-index values for different components.
    /// </summary>
    public class ZIndex
    {
        /// <summary>
        /// The Z-index value for the Drawer component.
        /// Defaults to <c>1100</c>..
        /// </summary>
        public int Drawer { get; set; } = 1100;

        /// <summary>
        /// The Z-index value for the Popover component.
        /// Defaults to <c>1200</c>.
        /// </summary>
        public int Popover { get; set; } = 1200;

        /// <summary>
        /// The Z-index value for the AppBar component.
        /// Defaults to <c>1300</c>.
        /// </summary>
        public int AppBar { get; set; } = 1300;

        /// <summary>
        /// The Z-index value for the Dialog component.
        /// Defaults to <c>1400</c>.
        /// </summary>
        public int Dialog { get; set; } = 1400;

        /// <summary>
        /// The Z-index value for the SnackBar component.
        /// Defaults to <c>1500</c>.
        /// </summary>
        public int Snackbar { get; set; } = 1500;

        /// <summary>
        /// The Z-index value for the Tooltip component.
        /// Defaults to <c>1600</c>.
        /// </summary>
        public int Tooltip { get; set; } = 1600;
    }
}
