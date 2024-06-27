using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
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
        /// Icon to be used can either be svg paths for font icons.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Icon.Behavior)]
        public string? Icon { get; set; }

        /// <summary>
        /// Text for the <c>title</c> attribute which provides a basic tooltip.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Icon.Behavior)]
        public string? Title { get; set; }

        /// <summary>
        /// The Size of the icon.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Icon.Appearance)]
        public Size Size { get; set; } = Size.Medium;

        /// <summary>
        /// If true, will ignore custom color if set.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public bool Disabled { get; set; }

        /// <summary>
        /// The color of the component. It supports the theme colors.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Icon.Appearance)]
        public Color Color { get; set; } = Color.Inherit;

        /// <summary>
        /// The viewbox size of an svg element.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Icon.Behavior)]
        public string ViewBox { get; set; } = "0 0 24 24";

        /// <summary>
        /// Child content of component.
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
