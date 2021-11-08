
namespace MudBlazor
{
    public class Typography
    {
        public Default Default { get; set; } = new Default();
        public H1 H1 { get; set; } = new H1();
        public H2 H2 { get; set; } = new H2();
        public H3 H3 { get; set; } = new H3();
        public H4 H4 { get; set; } = new H4();
        public H5 H5 { get; set; } = new H5();
        public H6 H6 { get; set; } = new H6();
        public Subtitle1 Subtitle1 { get; set; } = new Subtitle1();
        public Subtitle2 Subtitle2 { get; set; } = new Subtitle2();
        public Body1 Body1 { get; set; } = new Body1();
        public Body2 Body2 { get; set; } = new Body2();
        public Button Button { get; set; } = new Button();
        public Caption Caption { get; set; } = new Caption();
        public Overline Overline { get; set; } = new Overline();
    }

    public class Default : BaseTypography
    { 
        public Default()
        {
            FontFamily = new[] { "Roboto", "Helvetica", "Arial", "sans-serif" };
            FontSize = ".875rem";
            FontWeight = 400;
            LineHeight = 1.43;
            LetterSpacing = ".01071em";
        }
    }
    public class H1 : BaseTypography
    {
        public H1()
        {
            FontSize = "6rem";
            FontWeight = 300;
            LineHeight = 1.167;
            LetterSpacing = "-.01562em";
        }
    }
    public class H2 : BaseTypography
    {
        public H2()
        {
            FontSize = "3.75rem";
            FontWeight = 300;
            LineHeight = 1.2;
            LetterSpacing = "-.00833em";
        }
    }
    public class H3 : BaseTypography
    {
        public H3()
        {
            FontSize = "3rem";
            FontWeight = 400;
            LineHeight = 1.167;
            LetterSpacing = "0";
        }
    }
    public class H4 : BaseTypography
    {
        public H4()
        {
            FontSize = "2.125rem";
            FontWeight = 400;
            LineHeight = 1.235;
            LetterSpacing = ".00735em";
        }
    }
    public class H5 : BaseTypography
    {
        public H5()
        {
            FontSize = "1.5rem";
            FontWeight = 400;
            LineHeight = 1.334;
            LetterSpacing = "0";
        }
    }
    public class H6 : BaseTypography
    {
        public H6()
        {
            FontSize = "1.25rem";
            FontWeight = 500;
            LineHeight = 1.6;
            LetterSpacing = ".0075em";
        }
    }
    public class Subtitle1 : BaseTypography
    {
        public Subtitle1()
        {
            FontSize = "1rem";
            FontWeight = 400;
            LineHeight = 1.75;
            LetterSpacing = ".00938em";
        }
    }
    public class Subtitle2 : BaseTypography
    {
        public Subtitle2()
        {
            FontSize = ".875rem";
            FontWeight = 500;
            LineHeight = 1.57;
            LetterSpacing = ".00714em";
        }
    }
    public class Body1 : BaseTypography
    {
        public Body1()
        {
            FontSize = "1rem";
            FontWeight = 400;
            LineHeight = 1.5;
            LetterSpacing = ".00938em";
        }
    }
    public class Body2 : BaseTypography
    {
        public Body2()
        {
            FontSize = ".875rem";
            FontWeight = 400;
            LineHeight = 1.43;
            LetterSpacing = ".01071em";
        }
    }
    public class Button : BaseTypography
    {
        public Button()
        {
            FontSize = ".875rem";
            FontWeight = 500;
            LineHeight = 1.75;
            LetterSpacing = ".02857em";
            TextTransform = "uppercase";
        }
    }
    public class Caption : BaseTypography
    {
        public Caption()
        {
            FontSize = ".75rem";
            FontWeight = 400;
            LineHeight = 1.66;
            LetterSpacing = ".03333em";
        }
    }
    public class Overline : BaseTypography
    {
        public Overline()
        {
            FontSize = ".75rem";
            FontWeight = 400;
            LineHeight = 2.66;
            LetterSpacing = ".08333em";
        }
    }

    public class BaseTypography
    {
        public string[] FontFamily { get; set; }
        public int FontWeight { get; set; }
        public string FontSize { get; set; }
        public double LineHeight { get; set; }
        public string LetterSpacing { get; set; }
        public string TextTransform { get; set; } = "none";
    }
}
