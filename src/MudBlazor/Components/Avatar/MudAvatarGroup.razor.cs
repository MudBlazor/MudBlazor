using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using MudBlazor.Interfaces;
using MudBlazor.State;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    partial class MudAvatarGroup : MudComponentBase
    {
        private bool _childrenNeedUpdates = false;

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
        [Parameter]
        [Category(CategoryTypes.AvatarGroup.Behavior)]
        public int Spacing { get; set; } = 3;

        /// <summary>
        /// Outlines the grouped avatars to distinguish them, useful when avatars are the same color or uses images.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.AvatarGroup.Appearance)]
        public bool Outlined { get; set; } = true;

        /// <summary>
        /// Sets the color of the outline if its used.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.AvatarGroup.Appearance)]
        public Color OutlineColor { get; set; } = Color.Surface;

        /// <summary>
        /// Elevation of the MaxAvatar the higher the number, the heavier the drop-shadow.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.AvatarGroup.Appearance)]
        public int MaxElevation { set; get; } = 0;

        /// <summary>
        /// If true, MaxAvatar border-radius is set to 0.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.AvatarGroup.Appearance)]
        public bool MaxSquare { get; set; }

        /// <summary>
        /// If true, MaxAvatar will be rounded.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.AvatarGroup.Appearance)]
        public bool MaxRounded { get; set; }

        /// <summary>
        /// Color for the MaxAvatar.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.AvatarGroup.Appearance)]
        public Color MaxColor { get; set; } = Color.Default;

        /// <summary>
        /// Size of the MaxAvatar.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.AvatarGroup.Appearance)]
        public Size MaxSize { get; set; } = Size.Medium;

        /// <summary>
        /// Variant of the MaxAvatar.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.AvatarGroup.Appearance)]
        public Variant MaxVariant { get; set; } = Variant.Filled;

        /// <summary>
        /// Max avatars to show before showing +x avatar, default value 0 has no max.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.AvatarGroup.Behavior)]
        public int Max { get; set; }

        /// <summary>
        /// Custom class/classes for MaxAvatar
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.AvatarGroup.Appearance)]
        public string? MaxAvatarClass { get; set; }

        /// <summary>
        /// Template that will be rendered when the number of avatars exceeds the maximum (parameter Max).
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.AvatarGroup.Appearance)]
        public RenderFragment<int>? MaxAvatarsTemplate { get; set; }

        /// <summary>
        /// Child content of the component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.AvatarGroup.Behavior)]
        public RenderFragment? ChildContent { get; set; }

        internal List<MudAvatar> _avatars = new();

        public MudAvatarGroup()
        {
            RegisterParameter(nameof(Spacing), () => Spacing, () => _childrenNeedUpdates = true);
            RegisterParameter(nameof(Max), () => Max, () => _childrenNeedUpdates = true);
        }

        internal void AddAvatar(MudAvatar avatar)
        {
            _avatars.Add(avatar);
            StateHasChanged();
        }

        internal void RemoveAvatar(MudAvatar avatar)
        {
            _avatars.Remove(avatar);
        }

        internal CssBuilder GetAvatarSpacing() => new CssBuilder()
            .AddClass($"ms-n{Spacing}");

        internal StyleBuilder GetAvatarZindex(MudAvatar avatar) => new StyleBuilder()
            .AddStyle("z-index", $"{_avatars.Count - _avatars.IndexOf(avatar)}");

        internal bool MaxGroupReached(MudAvatar avatar)
        {
            return _avatars.IndexOf(avatar) < Max;
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            if (_childrenNeedUpdates)
            {
                foreach (IMudStateHasChanged avatar in _avatars)
                {
                    avatar.StateHasChanged();
                }

                _childrenNeedUpdates = false;
            }
        }
    }
}
