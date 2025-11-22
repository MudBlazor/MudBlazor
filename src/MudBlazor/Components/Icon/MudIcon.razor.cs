using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable

    /// <summary>
    /// A picture displayed via an SVG path or font.
    /// </summary>
    /// <remarks>
    /// You can use the <see cref="Icons"/> class and <see href="https://mudblazor.com/features/icons#icons">Icons Reference</see> for SVG paths, or a <see href="https://fontawesome.com/icons">Font Awesome CSS Class</see>.
    /// </remarks>
    /// <seealso cref="MudIconButton"/>
    public partial class MudIcon : MudComponentBase
    {
        protected string Classname =>
            new CssBuilder("mud-icon-root")
                .AddClass("mud-icon-default", Color == Color.Default && !Disabled)
                .AddClass("mud-svg-icon", !string.IsNullOrEmpty(Icon) && Icon.Trim().StartsWith("<"))
                .AddClass($"mud-{Color.ToDescriptionString()}-text", Color != Color.Default && Color != Color.Inherit && !Disabled)
                .AddClass($"mud-icon-size-{Size.ToDescriptionString()}")
                .AddClass(Class)
                .Build();

        /// <summary>
        /// The SVG path or Font Awesome font icon to display.
        /// </summary>
        /// <remarks>
        /// You can use the <see cref="Icons"/> class and <see href="https://mudblazor.com/features/icons#icons">Icons Reference</see> for SVG paths, or a <see href="https://fontawesome.com/icons">Font Awesome CSS Class</see>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Icon.Behavior)]
        public string? Icon { get; set; }

        /// <summary>
        /// The text to display for the tooltip.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  Sets the <c>title</c> HTML attribute.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Icon.Behavior)]
        public string? Title { get; set; }

        /// <summary>
        /// The size of this icon.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Size.Medium"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Icon.Appearance)]
        public Size Size { get; set; } = Size.Medium;

        /// <summary>
        /// Ignores any custom color.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  When <c>true</c>, a disabled color will be used instead of the <see cref="Color"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public bool Disabled { get; set; }

        /// <summary>
        /// The color of this icon.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Color.Inherit"/>.  When <see cref="Disabled"/> is <c>true</c>, this value is ignored.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Icon.Appearance)]
        public Color Color { get; set; } = Color.Inherit;

        /// <summary>
        /// For SVG icons, the size of the SVG viewbox.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>"0 0 24 24"</c>.  Applies when using the <see cref="Icons"/> class to set the icon.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Icon.Behavior)]
        public string ViewBox { get; set; } = "0 0 24 24";

        /// <summary>
        /// The content within this icon.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Icon.Behavior)]
        public RenderFragment? ChildContent { get; set; }

        [MemberNotNullWhen(true, nameof(Icon))]
        private bool IsAngleBracket => !string.IsNullOrEmpty(Icon) && Icon.Trim().StartsWith('<');

        private partial class IconSyntax
        {
            public string FontIconClass { get; }

            public string IconName { get; }

            public IconSyntax(string fontIconClass, string iconName)
            {
                FontIconClass = fontIconClass;
                IconName = iconName;
            }

            public static bool TryParseFontIconSyntax([NotNullWhen(true)] string? input, [MaybeNullWhen(false)] out IconSyntax syntax)
            {
                if (input is null)
                {
                    syntax = null;

                    return false;
                }

                var match = SlashContentRegex().Match(input);
                if (match.Success)
                {
                    var fontIconClass = match.Groups[1].Value;
                    var iconName = match.Groups[2].Value;
                    syntax = new IconSyntax(fontIconClass, iconName);

                    return true;
                }

                syntax = null;

                return false;
            }

            [GeneratedRegex(@"^(.+?)/(.+)$")]
            private static partial Regex SlashContentRegex();
        }
    }
}
