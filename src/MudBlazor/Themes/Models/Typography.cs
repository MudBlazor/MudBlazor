
namespace MudBlazor
{
    public class Typography
    {
        public Default Default { get; set; } = new Default
        {
            FontFamily = new[] { "Roboto", "Helvetica", "Arial", "sans-serif" },
            FontSize = ".875rem",
            FontWeight = 400,
            LineHeight = 1.43,
            LetterSpacing = ".01071em"
        };
        public H1 H1 { get; set; } = new H1
        {
            FontFamily = new[] { "Roboto", "Helvetica", "Arial", "sans-serif" },
            FontSize = "6rem",
            FontWeight = 300,
            LineHeight = 1.167,
            LetterSpacing = "-.01562em"
        };
        public H2 H2 { get; set; } = new H2
        {
            FontFamily = new[] { "Roboto", "Helvetica", "Arial", "sans-serif" },
            FontSize = "3.75rem",
            FontWeight = 300,
            LineHeight = 1.2,
            LetterSpacing = "-.00833em"
        };
        public H3 H3 { get; set; } = new H3
        {
            FontFamily = new[] { "Roboto", "Helvetica", "Arial", "sans-serif" },
            FontSize = "3rem",
            FontWeight = 400,
            LineHeight = 1.167,
            LetterSpacing = "0"
        };
        public H4 H4 { get; set; } = new H4
        {
            FontFamily = new[] { "Roboto", "Helvetica", "Arial", "sans-serif" },
            FontSize = "2.125rem",
            FontWeight = 400,
            LineHeight = 1.235,
            LetterSpacing = ".00735em"
        };
        public H5 H5 { get; set; } = new H5
        {
            FontFamily = new[] { "Roboto", "Helvetica", "Arial", "sans-serif" },
            FontSize = "1.5rem",
            FontWeight = 400,
            LineHeight = 1.334,
            LetterSpacing = "0"
        };
        public H6 H6 { get; set; } = new H6
        {
            FontFamily = new[] { "Roboto", "Helvetica", "Arial", "sans-serif" },
            FontSize = "1.25rem",
            FontWeight = 500,
            LineHeight = 1.6,
            LetterSpacing = ".0075em"
        };
        public Subtitle1 Subtitle1 { get; set; } = new Subtitle1
        {
            FontFamily = new[] { "Roboto", "Helvetica", "Arial", "sans-serif" },
            FontSize = "1rem",
            FontWeight = 400,
            LineHeight = 1.75,
            LetterSpacing = ".00938em"
        };
        public Subtitle2 Subtitle2 { get; set; } = new Subtitle2
        {
            FontFamily = new[] { "Roboto", "Helvetica", "Arial", "sans-serif" },
            FontSize = ".875rem",
            FontWeight = 500,
            LineHeight = 1.57,
            LetterSpacing = ".00714em"
        };
        public Body1 Body1 { get; set; } = new Body1
        {
            FontFamily = new[] { "Roboto", "Helvetica", "Arial", "sans-serif" },
            FontSize = "1rem",
            FontWeight = 400,
            LineHeight = 1.5,
            LetterSpacing = ".00938em"
        };
        public Body2 Body2 { get; set; } = new Body2
        {
            FontFamily = new[] { "Roboto", "Helvetica", "Arial", "sans-serif" },
            FontSize = ".875rem",
            FontWeight = 400,
            LineHeight = 1.43,
            LetterSpacing = ".01071em"
        };
        public Button Button { get; set; } = new Button
        {
            FontFamily = new[] { "Roboto", "Helvetica", "Arial", "sans-serif" },
            FontSize = ".875rem",
            FontWeight = 500,
            LineHeight = 1.75,
            LetterSpacing = ".02857em"
        };
        public Caption Caption { get; set; } = new Caption
        {
            FontFamily = new[] { "Roboto", "Helvetica", "Arial", "sans-serif" },
            FontSize = ".75rem",
            FontWeight = 400,
            LineHeight = 1.66,
            LetterSpacing = ".03333em"
        };
        public Overline Overline { get; set; } = new Overline
        {
            FontFamily = new[] { "Roboto", "Helvetica", "Arial", "sans-serif" },
            FontSize = ".75rem",
            FontWeight = 400,
            LineHeight = 2.66,
            LetterSpacing = ".08333em"
        };
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
