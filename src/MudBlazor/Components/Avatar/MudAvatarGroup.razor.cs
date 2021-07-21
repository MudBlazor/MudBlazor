using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
    partial class MudAvatarGroup : MudComponentBase
    {
        protected string Classname =>
        new CssBuilder("mud-avatar-group")
            .AddClass($"mud-avatar-group-outlined", Outlined)
            .AddClass($"mud-avatar-group-outlined-{OutlineColor.ToDescriptionString()}", Outlined)
          .AddClass(Class)
        .Build();

        protected string MaxAvatarClassname =>
        new CssBuilder("mud-avatar-group-max-avatar")
            .AddClass($"ms-n{Spacing}")
          .AddClass(MaxAvatarClass)
        .Build();

        /// <summary>
        /// Spacing between avatars where 0 is none and 16 max.
        /// </summary>
        [Parameter] public int Spacing { get; set; } = 2;

        /// <summary>
        /// Outlines the grouped avatars to distinguish them, useful when avatars are the same color or uses images.
        /// </summary>
        [Parameter] public bool Outlined { get; set; } = true;

        /// <summary>
        /// Sets the color of the outline if its used.
        /// </summary>
        [Parameter] public Color OutlineColor { get; set; } = Color.Surface;

        /// <summary>
        /// Elevation of the MaxAvatar the higher the number, the heavier the drop-shadow.
        /// </summary>
        [Parameter] public int MaxElevation { set; get; } = 0;

        /// <summary>
        /// If true, MaxAvatar border-radius is set to 0.
        /// </summary>
        [Parameter] public bool MaxSquare { get; set; }

        /// <summary>
        /// If true, MaxAvatar will be rounded.
        /// </summary>
        [Parameter] public bool MaxRounded { get; set; }

        /// <summary>
        /// Color for the MaxAvatar.
        /// </summary>
        [Parameter] public Color MaxColor { get; set; } = Color.Default;

        /// <summary>
        /// Size of the MaxAvatar.
        /// </summary>
        [Parameter] public Size MaxSize { get; set; } = Size.Medium;

        /// <summary>
        /// Variant of the MaxAvatar.
        /// </summary>
        [Parameter] public Variant MaxVariant { get; set; } = Variant.Filled;


        /// <summary>
        /// Max avatars to show before showing +x avatar, default value 0 has no max.
        /// </summary>
        [Parameter] public int Max { get; set; }

        /// <summary>
        /// Custom class/classes for MaxAvatar
        /// </summary>
        [Parameter] public string MaxAvatarClass { get; set; }

        /// <summary>
        /// Child content of the component.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        internal List<MudAvatar> _avatars = new List<MudAvatar>();
        internal bool _groupMaxReached;

        internal void AddAvatar(MudAvatar avatar)
        {
            _avatars.Add(avatar);
            if(Max > 0 && _avatars.Count > Max)
            {
                _groupMaxReached = true;
                avatar.GroupMaxReached = true;
            }

            StateHasChanged();
        }

        internal void RemoveAvatar(MudAvatar avatar)
        {
            _avatars.Remove(avatar);
        }
    }
}
