using System.Text.Json.Serialization;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// Typography settings for <see cref="Typo"/> types used throughout the theme.
    /// </summary>
    public class Typography
    {
        /// <summary>
        /// Gets or sets the typography settings for the default typo.
        /// </summary>
        /// <remarks>
        /// Defaults to the values from the <see cref="MudBlazor.DefaultTypography"/> constructor.
        /// </remarks>
        public BaseTypography Default { get; set; } = new DefaultTypography();

        /// <summary>
        /// Gets or sets the typography settings for <see cref="Typo.h1"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to the values from the <see cref="MudBlazor.H1"/> constructor.
        /// </remarks>
        public BaseTypography H1 { get; set; } = new H1();

        /// <summary>
        /// Gets or sets the typography settings for <see cref="Typo.h2"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to the values from the <see cref="MudBlazor.H2"/> constructor.
        /// </remarks>
        public BaseTypography H2 { get; set; } = new H2();

        /// <summary>
        /// Gets or sets the typography settings for <see cref="Typo.h3"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to the values from the <see cref="MudBlazor.H3"/> constructor.
        /// </remarks>
        public BaseTypography H3 { get; set; } = new H3();

        /// <summary>
        /// Gets or sets the typography settings for <see cref="Typo.h4"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to the values from the <see cref="MudBlazor.H4"/> constructor.
        /// </remarks>
        public BaseTypography H4 { get; set; } = new H4();

        /// <summary>
        /// Gets or sets the typography settings for <see cref="Typo.h5"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to the values from the <see cref="MudBlazor.H5"/> constructor.
        /// </remarks>
        public BaseTypography H5 { get; set; } = new H5();

        /// <summary>
        /// Gets or sets the typography settings for <see cref="Typo.h6"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to the values from the <see cref="MudBlazor.H6"/> constructor.
        /// </remarks>
        public BaseTypography H6 { get; set; } = new H6();

        /// <summary>
        /// Gets or sets the typography settings for <see cref="Typo.subtitle1"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to the values from the <see cref="MudBlazor.Subtitle1"/> constructor.
        /// </remarks>
        public BaseTypography Subtitle1 { get; set; } = new Subtitle1();

        /// <summary>
        /// Gets or sets the typography settings for <see cref="Typo.subtitle2"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to the values from the <see cref="MudBlazor.Subtitle2"/> constructor.
        /// </remarks>
        public BaseTypography Subtitle2 { get; set; } = new Subtitle2();

        /// <summary>
        /// Gets or sets the typography settings for <see cref="Typo.body1"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to the values from the <see cref="MudBlazor.Body1"/> constructor.
        /// </remarks>
        public BaseTypography Body1 { get; set; } = new Body1();

        /// <summary>
        /// Gets or sets the typography settings for <see cref="Typo.body2"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to the values from the <see cref="MudBlazor.Body2"/> constructor.
        /// </remarks>
        public BaseTypography Body2 { get; set; } = new Body2();

        /// <summary>
        /// Gets or sets the typography settings for <see cref="Typo.input"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to the values from the <see cref="MudBlazor.Input"/> constructor.
        /// </remarks>
        public BaseTypography Input { get; set; } = new Input();

        /// <summary>
        /// Gets or sets the typography settings for <see cref="Typo.button"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to the values from the <see cref="MudBlazor.Button"/> constructor.
        /// </remarks>
        public BaseTypography Button { get; set; } = new Button();

        /// <summary>
        /// Gets or sets the typography settings for <see cref="Typo.caption"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to the values from the <see cref="MudBlazor.Caption"/> constructor.
        /// </remarks>
        public BaseTypography Caption { get; set; } = new Caption();

        /// <summary>
        /// Gets or sets the typography settings for <see cref="Typo.overline"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to the values from the <see cref="MudBlazor.Overline"/> constructor.
        /// </remarks>
        public BaseTypography Overline { get; set; } = new Overline();
    }

    /// <summary>
    /// Represents the default typography settings.
    /// </summary>
    public class DefaultTypography : BaseTypography
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultTypography"/> class with default values.
        /// </summary>
        public DefaultTypography()
        {
            FontFamily = ["Roboto", "Helvetica", "Arial", "sans-serif"];
            FontSize = ".875rem";
            FontWeight = "400";
            LineHeight = "1.43";
            LetterSpacing = ".01071em";
        }
    }

    /// <summary>
    /// Represents the H1 typography settings.
    /// </summary>
    public class H1 : BaseTypography
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="H1"/> class with default values.
        /// </summary>
        public H1()
        {
            FontSize = "6rem";
            FontWeight = "300";
            LineHeight = "1.167";
            LetterSpacing = "-.01562em";
        }
    }

    /// <summary>
    /// Represents the H2 typography settings.
    /// </summary>
    public class H2 : BaseTypography
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="H2"/> class with default values.
        /// </summary>
        public H2()
        {
            FontSize = "3.75rem";
            FontWeight = "300";
            LineHeight = "1.2";
            LetterSpacing = "-.00833em";
        }
    }

    /// <summary>
    /// Represents the H3 typography settings.
    /// </summary>
    public class H3 : BaseTypography
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="H3"/> class with default values.
        /// </summary>
        public H3()
        {
            FontSize = "3rem";
            FontWeight = "400";
            LineHeight = "1.167";
            LetterSpacing = "0";
        }
    }

    /// <summary>
    /// Represents the H4 typography settings.
    /// </summary>
    public class H4 : BaseTypography
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="H4"/> class with default values.
        /// </summary>
        public H4()
        {
            FontSize = "2.125rem";
            FontWeight = "400";
            LineHeight = "1.235";
            LetterSpacing = ".00735em";
        }
    }

    /// <summary>
    /// Represents the H5 typography settings.
    /// </summary>
    public class H5 : BaseTypography
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="H5"/> class with default values.
        /// </summary>
        public H5()
        {
            FontSize = "1.5rem";
            FontWeight = "400";
            LineHeight = "1.334";
            LetterSpacing = "0";
        }
    }

    /// <summary>
    /// Represents the H6 typography settings.
    /// </summary>
    public class H6 : BaseTypography
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="H6"/> class with default values.
        /// </summary>
        public H6()
        {
            FontSize = "1.25rem";
            FontWeight = "500";
            LineHeight = "1.6";
            LetterSpacing = ".0075em";
        }
    }

    /// <summary>
    /// Represents the Subtitle1 typography settings.
    /// </summary>
    public class Subtitle1 : BaseTypography
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Subtitle1"/> class with default values.
        /// </summary>
        public Subtitle1()
        {
            FontSize = "1rem";
            FontWeight = "400";
            LineHeight = "1.75";
            LetterSpacing = ".00938em";
        }
    }

    /// <summary>
    /// Represents the Subtitle2 typography settings.
    /// </summary>
    public class Subtitle2 : BaseTypography
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Subtitle2"/> class with default values.
        /// </summary>
        public Subtitle2()
        {
            FontSize = ".875rem";
            FontWeight = "500";
            LineHeight = "1.57";
            LetterSpacing = ".00714em";
        }
    }

    /// <summary>
    /// Represents the Body1 typography settings.
    /// </summary>
    public class Body1 : BaseTypography
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Body1"/> class with default values.
        /// </summary>
        public Body1()
        {
            FontSize = "1rem";
            FontWeight = "400";
            LineHeight = "1.5";
            LetterSpacing = ".00938em";
        }
    }

    /// <summary>
    /// Represents the Body2 typography settings.
    /// </summary>
    public class Body2 : BaseTypography
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Body2"/> class with default values.
        /// </summary>
        public Body2()
        {
            FontSize = ".875rem";
            FontWeight = "400";
            LineHeight = "1.43";
            LetterSpacing = ".01071em";
        }
    }

    /// <summary>
    /// Represents the Input typography settings.
    /// </summary>
    public class Input : BaseTypography
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Input"/> class with default values.
        /// </summary>
        public Input()
        {
            FontSize = "1rem";
            FontWeight = "400";
            LineHeight = "1.1876";
            LetterSpacing = ".00938em";
        }
    }

    /// <summary>
    /// Represents the Button typography settings.
    /// </summary>
    public class Button : BaseTypography
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Button"/> class with default values.
        /// </summary>
        public Button()
        {
            FontSize = ".875rem";
            FontWeight = "500";
            LineHeight = "1.75";
            LetterSpacing = ".02857em";
            TextTransform = "uppercase";
        }
    }

    /// <summary>
    /// Represents the Caption typography settings.
    /// </summary>
    public class Caption : BaseTypography
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Caption"/> class with default values.
        /// </summary>
        public Caption()
        {
            FontSize = ".75rem";
            FontWeight = "400";
            LineHeight = "1.66";
            LetterSpacing = ".03333em";
        }
    }

    /// <summary>
    /// Represents the Overline typography settings.
    /// </summary>
    public class Overline : BaseTypography
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Overline"/> class with default values.
        /// </summary>
        public Overline()
        {
            FontSize = ".75rem";
            FontWeight = "400";
            LineHeight = "2.66";
            LetterSpacing = ".08333em";
        }
    }

    /// <summary>
    /// Represents the base typography settings.
    /// </summary>
    [JsonDerivedType(typeof(DefaultTypography), nameof(DefaultTypography))]
    [JsonDerivedType(typeof(H1), nameof(H1))]
    [JsonDerivedType(typeof(H2), nameof(H2))]
    [JsonDerivedType(typeof(H3), nameof(H3))]
    [JsonDerivedType(typeof(H4), nameof(H4))]
    [JsonDerivedType(typeof(H5), nameof(H5))]
    [JsonDerivedType(typeof(H6), nameof(H6))]
    [JsonDerivedType(typeof(Subtitle1), nameof(Subtitle1))]
    [JsonDerivedType(typeof(Subtitle2), nameof(Subtitle2))]
    [JsonDerivedType(typeof(Body1), nameof(Body1))]
    [JsonDerivedType(typeof(Body2), nameof(Body2))]
    [JsonDerivedType(typeof(Input), nameof(Input))]
    [JsonDerivedType(typeof(Button), nameof(Button))]
    [JsonDerivedType(typeof(Caption), nameof(Caption))]
    [JsonDerivedType(typeof(Overline), nameof(Overline))]
    public abstract class BaseTypography
    {
        /// <summary>
        /// Gets or sets the font family.
        /// </summary>
        public string[]? FontFamily { get; set; }

        /// <summary>
        /// Gets or sets the font weight.
        /// </summary>
        public string? FontWeight { get; set; }

        /// <summary>
        /// Gets or sets the font size.
        /// </summary>
        public string? FontSize { get; set; }

        /// <summary>
        /// Gets or sets the line height.
        /// </summary>
        public string? LineHeight { get; set; }

        /// <summary>
        /// Gets or sets the letter spacing.
        /// </summary>
        public string? LetterSpacing { get; set; }

        /// <summary>
        /// Gets or sets the text transform.
        /// </summary>
        public string TextTransform { get; set; } = "none";
    }
}
