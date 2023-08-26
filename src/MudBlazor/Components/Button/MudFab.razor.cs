using System;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    public partial class MudFab : MudBaseButton
    {
        protected string Classname =>
            new CssBuilder("mud-button-root mud-fab")
                .AddClass($"mud-fab-extended", !string.IsNullOrEmpty(Label))
                .AddClass($"mud-fab-{Color.ToDescriptionString()}")
                .AddClass($"mud-fab-size-{Size.ToDescriptionString()}")
                .AddClass($"mud-ripple", !DisableRipple && !GetDisabledState())
                .AddClass($"mud-fab-disable-elevation", DisableElevation)
                .AddClass(Class)
                .Build();

        /// <summary>
        /// The color of the component. It supports the theme colors.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Appearance)]
        public Color Color { get; set; } = Color.Default;

        /// <summary>
        /// The Size of the component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Appearance)]
        public Size Size { get; set; } = Size.Large;

        /// <summary>
        /// If applied Icon will be added at the start of the component.
        /// </summary>
        [Obsolete("This property is obsolete. Use StartIcon instead.")]
        [Parameter]
        public string? Icon { get => StartIcon; set => StartIcon = value; }

        /// <summary>
        /// If applied Icon will be added at the start of the component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Behavior)]
        public string? StartIcon { get; set; }

        /// <summary>
        /// If applied Icon will be added at the end of the component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Behavior)]
        public string? EndIcon { get; set; }

        /// <summary>
        /// The color of the icon. It supports the theme colors.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Appearance)]
        public Color IconColor { get; set; } = Color.Inherit;

        /// <summary>
        /// The size of the icon.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Appearance)]
        public Size IconSize { get; set; } = Size.Medium;

        /// <summary>
        /// If applied the text will be added to the component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Behavior)]
        public string? Label { get; set; }

        /// <summary>
        /// Title of the icon used for accessibility.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Behavior)]
        public string? Title { get; set; }
    }
}
