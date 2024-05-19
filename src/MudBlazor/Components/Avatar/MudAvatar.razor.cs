using System;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// Represents a component which displays circular user profile pictures, icons or text.
    /// </summary>
    partial class MudAvatar : MudComponentBase, IDisposable
    {
        [CascadingParameter]
        protected MudAvatarGroup? AvatarGroup { get; set; }

        protected string Classname => new CssBuilder("mud-avatar")
            .AddClass($"mud-avatar-{Size.ToDescriptionString()}")
            .AddClass($"mud-avatar-rounded", Rounded)
            .AddClass($"mud-avatar-square", Square)
            .AddClass($"mud-avatar-{Variant.ToDescriptionString()}")
            .AddClass($"mud-avatar-{Variant.ToDescriptionString()}-{Color.ToDescriptionString()}")
            .AddClass($"mud-elevation-{Elevation.ToString()}")
            .AddClass(AvatarGroup?.GetAvatarSpacing() ?? new CssBuilder(), AvatarGroup != null)
            .AddClass(Class)
            .Build();

        protected string Stylesname => new StyleBuilder()
            .AddStyle(AvatarGroup?.GetAvatarZindex(this) ?? new StyleBuilder(), AvatarGroup != null)
            .AddStyle(Style)
            .Build();

        /// <summary>
        /// The size of the drop shadow.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>0</c>.  A higher number creates a heavier drop shadow.  Use a value of <c>0</c> for no shadow.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Avatar.Appearance)]
        public int Elevation { set; get; } = 0;

        /// <summary>
        /// Disables rounded corners.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Avatar.Appearance)]
        public bool Square { get; set; }

        /// <summary>
        /// Shows rounded corners.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  When <c>true</c>, the <c>border-radius</c> style is set to the theme's default value.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Avatar.Appearance)]
        public bool Rounded { get; set; }

        /// <summary>
        /// The color of the avatar.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Color.Default"/>.  Theme colors are supported.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Avatar.Appearance)]
        public Color Color { get; set; } = Color.Default;

        /// <summary>
        /// The size of the avatar.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Size.Medium"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Avatar.Appearance)]
        public Size Size { get; set; } = Size.Medium;

        /// <summary>
        /// The display variant to use.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Variant.Filled" />. The variant changes the appearance of the avatar, such as <c>Text</c>, <c>Outlined</c>, or <c>Filled</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Avatar.Appearance)]
        public Variant Variant { get; set; } = Variant.Filled;

        /// <summary>
        /// The content within the avatar.
        /// </summary>
        /// <remarks>
        /// This property allows for custom content to displayed inside of the avatar, but it is not required.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Avatar.Behavior)]
        public RenderFragment? ChildContent { get; set; }

        /// <inheritdoc />
        protected override void OnInitialized()
        {
            base.OnInitialized();

            AvatarGroup?.AddAvatar(this);
        }

        /// <summary>
        /// Releases resources used by this component.
        /// </summary>
        public void Dispose()
        {
            AvatarGroup?.RemoveAvatar(this);
        }
    }
}
