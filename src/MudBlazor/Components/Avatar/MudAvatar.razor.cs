﻿using System;
using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
    partial class MudAvatar : MudComponentBase, IDisposable
    {
        [CascadingParameter] protected MudAvatarGroup AvatarGroup { get; set; }
        protected string Classname =>
        new CssBuilder("mud-avatar")
          .AddClass($"mud-avatar-{Size.ToDescriptionString()}")
          .AddClass($"mud-avatar-rounded", Rounded)
          .AddClass($"mud-avatar-square", Square)
          .AddClass($"mud-avatar-{Variant.ToDescriptionString()}")
          .AddClass($"mud-avatar-{Variant.ToDescriptionString()}-{Color.ToDescriptionString()}")
          .AddClass($"mud-elevation-{Elevation.ToString()}")
          .AddClass(AvatarGroup?.GetAvatarSpacing() ?? new CssBuilder(), AvatarGroup != null)
          .AddClass(Class)
        .Build();

        protected string Stylesname =>
            new StyleBuilder()
            .AddStyle(AvatarGroup?.GetAvatarZindex(this) ?? new StyleBuilder(), AvatarGroup != null)
            .AddStyle(Style)
            .Build();

        /// <summary>
        /// The higher the number, the heavier the drop-shadow.
        /// </summary>
        [Parameter] public int Elevation { set; get; } = 0;

        /// <summary>
        /// If true, border-radius is set to 0.
        /// </summary>
        [Parameter] public bool Square { get; set; }

        /// <summary>
        /// If true, border-radius is set to the themes default value.
        /// </summary>
        [Parameter] public bool Rounded { get; set; }

        /// <summary>
        /// Link to image, if set a image will be displayed instead of text.
        /// </summary>
        [Parameter] public string Image { get; set; }

        /// <summary>
        /// If set (and Image is also set), will add an alt property to the img element
        /// </summary>
        [Parameter] public string Alt { get; set; }

        /// <summary>
        /// The color of the component. It supports the theme colors.
        /// </summary>
        [Parameter] public Color Color { get; set; } = Color.Default;

        /// <summary>
        /// The Size of the MudAvatar.
        /// </summary>
        [Parameter] public Size Size { get; set; } = Size.Medium;

        /// <summary>
        /// The variant to use.
        /// </summary>
        [Parameter] public Variant Variant { get; set; } = Variant.Filled;

        /// <summary>
        /// Child content of the component.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            AvatarGroup?.AddAvatar(this);
        }

        public void Dispose()
        {
            AvatarGroup?.RemoveAvatar(this);
        }

        internal void ForceRedraw()
        {
            StateHasChanged();
        }
    }
}
