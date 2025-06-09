namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// Represents the theme settings for the MudBlazor user interface.
    /// </summary>
    public class MudTheme
    {
        /// <summary>
        /// Palette for the light theme.
        /// </summary>
        /// <remarks>Renamed from <c>Palette</c> to <c>PaletteLight</c> in v7.</remarks>
        public PaletteLight PaletteLight { get; set; }

        /// <summary>
        /// Palette for the dark theme.
        /// </summary>
        public PaletteDark PaletteDark { get; set; }

        /// <summary>
        /// Shadow settings.
        /// </summary>
        public Shadow Shadows { get; set; }

        /// <summary>
        /// Typography settings.
        /// </summary>
        public Typography Typography { get; set; }

        /// <summary>
        /// Layout properties.
        /// </summary>
        public LayoutProperties LayoutProperties { get; set; }

        /// <summary>
        /// Z-index values.
        /// </summary>
        public ZIndex ZIndex { get; set; }

        /// <summary>
        /// Pseudo CSS styles.
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
