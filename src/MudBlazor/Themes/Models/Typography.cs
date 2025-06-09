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
        /// Defaults to the values from the <see cref="DefaultTypography"/> constructor.
        /// </remarks>
        public BaseTypography Default { get; set; } = new DefaultTypography();

        /// <summary>
        /// Gets or sets the typography settings for <see cref="Typo.h1"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to the values from the <see cref="H1Typography"/> constructor.
        /// </remarks>
        public BaseTypography H1 { get; set; } = new H1Typography();

        /// <summary>
        /// Gets or sets the typography settings for <see cref="Typo.h2"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to the values from the <see cref="H2Typography"/> constructor.
        /// </remarks>
        public BaseTypography H2 { get; set; } = new H2Typography();

        /// <summary>
        /// Gets or sets the typography settings for <see cref="Typo.h3"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to the values from the <see cref="H3Typography"/> constructor.
        /// </remarks>
        public BaseTypography H3 { get; set; } = new H3Typography();

        /// <summary>
        /// Gets or sets the typography settings for <see cref="Typo.h4"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to the values from the <see cref="H4Typography"/> constructor.
        /// </remarks>
        public BaseTypography H4 { get; set; } = new H4Typography();

        /// <summary>
        /// Gets or sets the typography settings for <see cref="Typo.h5"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to the values from the <see cref="H5Typography"/> constructor.
        /// </remarks>
        public BaseTypography H5 { get; set; } = new H5Typography();

        /// <summary>
        /// Gets or sets the typography settings for <see cref="Typo.h6"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to the values from the <see cref="H6Typography"/> constructor.
        /// </remarks>
        public BaseTypography H6 { get; set; } = new H6Typography();

        /// <summary>
        /// Gets or sets the typography settings for <see cref="Typo.subtitle1"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to the values from the <see cref="Subtitle1Typography"/> constructor.
        /// </remarks>
        public BaseTypography Subtitle1 { get; set; } = new Subtitle1Typography();

        /// <summary>
        /// Gets or sets the typography settings for <see cref="Typo.subtitle2"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to the values from the <see cref="Subtitle2Typography"/> constructor.
        /// </remarks>
        public BaseTypography Subtitle2 { get; set; } = new Subtitle2Typography();

        /// <summary>
        /// Gets or sets the typography settings for <see cref="Typo.body1"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to the values from the <see cref="Body1Typography"/> constructor.
        /// </remarks>
        public BaseTypography Body1 { get; set; } = new Body1Typography();

        /// <summary>
        /// Gets or sets the typography settings for <see cref="Typo.body2"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to the values from the <see cref="Body2Typography"/> constructor.
        /// </remarks>
        public BaseTypography Body2 { get; set; } = new Body2Typography();

        /// <summary>
        /// Gets or sets the typography settings for <see cref="Typo.button"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to the values from the <see cref="ButtonTypography"/> constructor.
        /// </remarks>
        public BaseTypography Button { get; set; } = new ButtonTypography();

        /// <summary>
        /// Gets or sets the typography settings for <see cref="Typo.caption"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to the values from the <see cref="CaptionTypography"/> constructor.
        /// </remarks>
        public BaseTypography Caption { get; set; } = new CaptionTypography();

        /// <summary>
        /// Gets or sets the typography settings for <see cref="Typo.overline"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to the values from the <see cref="OverlineTypography"/> constructor.
        /// </remarks>
        public BaseTypography Overline { get; set; } = new OverlineTypography();
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
    public class H1Typography : BaseTypography
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="H1Typography"/> class with default values.
        /// </summary>
        public H1Typography()
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
    public class H2Typography : BaseTypography
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="H2Typography"/> class with default values.
        /// </summary>
        public H2Typography()
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
    public class H3Typography : BaseTypography
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="H3Typography"/> class with default values.
        /// </summary>
        public H3Typography()
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
    public class H4Typography : BaseTypography
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="H4Typography"/> class with default values.
        /// </summary>
        public H4Typography()
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
    public class H5Typography : BaseTypography
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="H5Typography"/> class with default values.
        /// </summary>
        public H5Typography()
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
    public class H6Typography : BaseTypography
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="H6Typography"/> class with default values.
        /// </summary>
        public H6Typography()
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
    public class Subtitle1Typography : BaseTypography
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Subtitle1Typography"/> class with default values.
        /// </summary>
        public Subtitle1Typography()
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
    public class Subtitle2Typography : BaseTypography
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Subtitle2Typography"/> class with default values.
        /// </summary>
        public Subtitle2Typography()
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
    public class Body1Typography : BaseTypography
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Body1Typography"/> class with default values.
        /// </summary>
        public Body1Typography()
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
    public class Body2Typography : BaseTypography
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Body2Typography"/> class with default values.
        /// </summary>
        public Body2Typography()
        {
            FontSize = ".875rem";
            FontWeight = "400";
            LineHeight = "1.43";
            LetterSpacing = ".01071em";
        }
    }

    /// <summary>
    /// Represents the Button typography settings.
    /// </summary>
    public class ButtonTypography : BaseTypography
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ButtonTypography"/> class with default values.
        /// </summary>
        public ButtonTypography()
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
    public class CaptionTypography : BaseTypography
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CaptionTypography"/> class with default values.
        /// </summary>
        public CaptionTypography()
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
    public class OverlineTypography : BaseTypography
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OverlineTypography"/> class with default values.
        /// </summary>
        public OverlineTypography()
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
    [JsonDerivedType(typeof(H1Typography), nameof(H1Typography))]
    [JsonDerivedType(typeof(H2Typography), nameof(H2Typography))]
    [JsonDerivedType(typeof(H3Typography), nameof(H3Typography))]
    [JsonDerivedType(typeof(H4Typography), nameof(H4Typography))]
    [JsonDerivedType(typeof(H5Typography), nameof(H5Typography))]
    [JsonDerivedType(typeof(H6Typography), nameof(H6Typography))]
    [JsonDerivedType(typeof(Subtitle1Typography), nameof(Subtitle1Typography))]
    [JsonDerivedType(typeof(Subtitle2Typography), nameof(Subtitle2Typography))]
    [JsonDerivedType(typeof(Body1Typography), nameof(Body1Typography))]
    [JsonDerivedType(typeof(Body2Typography), nameof(Body2Typography))]
    [JsonDerivedType(typeof(ButtonTypography), nameof(ButtonTypography))]
    [JsonDerivedType(typeof(CaptionTypography), nameof(CaptionTypography))]
    [JsonDerivedType(typeof(OverlineTypography), nameof(OverlineTypography))]
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
