namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// Represents the theme settings for the MudBlazor user interface.
    /// </summary>
    public class MudTheme
    {
        /// <summary>
        /// Gets or sets the palette for the light theme.
        /// </summary>
        public PaletteLight PaletteLight { get; set; }

        /// <summary>
        /// Gets or sets the palette for the dark theme.
        /// </summary>
        public PaletteDark PaletteDark { get; set; }

        /// <summary>
        /// Gets or sets the shadow settings.
        /// </summary>
        public Shadow Shadows { get; set; }

        /// <summary>
        /// Gets or sets the typography settings.
        /// </summary>
        public Typography Typography { get; set; }

        /// <summary>
        /// Gets or sets the layout properties.
        /// </summary>
        public LayoutProperties LayoutProperties { get; set; }

        /// <summary>
        /// Gets or sets the z-index values.
        /// </summary>
        public ZIndex ZIndex { get; set; }

        /// <summary>
        /// Gets or sets the pseudo CSS styles.
        /// </summary>
        public PseudoCss PseudoCss { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MudTheme"/> class.
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
