namespace MudBlazor
{
    public class MudTheme
    {
        //public Breakpoints Breakpoints { get; set; }
        public Palette Palette { get; set; }
        public Palette PaletteDark { get; set; }
        public Shadow Shadows { get; set; }
        public Typography Typography { get; set; }
        public LayoutProperties LayoutProperties { get; set; }
        public ZIndex ZIndex { get; set; }
        public PseudoCss PseudoCss { get; set; }

        public MudTheme()
        {
            Palette = new PaletteLight();
            PaletteDark = new PaletteDark();
            Shadows = new Shadow();
            Typography = new Typography();
            LayoutProperties = new LayoutProperties();
            ZIndex = new ZIndex();
            PseudoCss = new PseudoCss();
        }
    }
}
