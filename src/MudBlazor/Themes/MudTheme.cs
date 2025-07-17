namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// Represents the theme settings for the MudBlazor user interface.
    /// </summary>
    public class MudTheme
    {
        /// <summary>
        /// The palette for the light theme.
        /// </summary>
        /// <remarks>Renamed from <c>Palette</c> to <c>PaletteLight</c> in v7.</remarks>
        public PaletteLight PaletteLight { get; set; }

        /// <summary>
        /// The palette for the dark theme.
        /// </summary>
        public PaletteDark PaletteDark { get; set; }

        /// <summary>
        /// The shadow settings.
        /// </summary>
        public Shadow Shadows { get; set; }

        /// <summary>
        /// The typography settings.
        /// </summary>
        public Typography Typography { get; set; }

        /// <summary>
        /// The layout properties.
        /// </summary>
        public LayoutProperties LayoutProperties { get; set; }

        /// <summary>
        /// The z-index values.
        /// </summary>
        public ZIndex ZIndex { get; set; }

        /// <summary>
        /// The pseudo CSS styles.
        /// </summary>
        public PseudoCss PseudoCss { get; set; }

        /// <summary>
        /// Initializes the <see cref="MudTheme"/> class.
        /// </summary>
        public MudTheme()
        {
            PaletteLight = new PaletteLight();
            PaletteDark = new PaletteDark();
            Shadows = new Shadow();
            Typography = new Typography();
            LayoutProperties = new LayoutProperties();
            ZIndex = new ZIndex();
            PseudoCss = new PseudoCss();
        }
    }
}
