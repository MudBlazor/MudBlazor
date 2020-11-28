
namespace MudBlazor
{
    public class Typography
    {
        public Default Default { get; set; }
        public H1 H1 { get; set; }
        public H2 H2 { get; set; }
        public H3 H3 { get; set; }
        public H4 H4 { get; set; }
        public H5 H5 { get; set; }
        public H6 H6 { get; set; }
        public Subtitle1 Subtitle1 { get; set; }
        public Subtitle2 Subtitle2 { get; set; }
        public Body1 Body1 { get; set; }
        public Body2 Body2 { get; set; }
        public Button Button { get; set; }
        public Caption Caption { get; set; }
        public Overline Overline { get; set; }
    }

    public class Default : BaseTypography { }
    public class H1 : BaseTypography { }
    public class H2 : BaseTypography { }
    public class H3 : BaseTypography { }
    public class H4 : BaseTypography { }
    public class H5 : BaseTypography { }
    public class H6 : BaseTypography { }
    public class Subtitle1 : BaseTypography { }
    public class Subtitle2 : BaseTypography { }
    public class Body1 : BaseTypography { }
    public class Body2 : BaseTypography { }
    public class Button : BaseTypography { }
    public class Caption : BaseTypography { }
    public class Overline : BaseTypography { }

    public class BaseTypography
    {
        public string[] FontFamily { get; set; }
        public int FontWeight { get; set; }
        public string FontSize { get; set; }
        public double LineHeight { get; set; }
        public string LetterSpacing { get; set; }
    }
}
