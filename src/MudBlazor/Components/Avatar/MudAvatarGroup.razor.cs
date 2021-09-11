﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
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

        private int _spacing = 3;

        /// <summary>
        /// Spacing between avatars where 0 is none and 16 max.
        /// </summary>
        [Parameter]
        public int Spacing
        {
            get => _spacing;
            set
            {
                if (value != _spacing)
                {
                    _spacing = value;
                    _childrenNeedUpdates = true;
                }
            }
        }

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

        private int _max = 3;

        /// <summary>
        /// Max avatars to show before showing +x avatar, default value 0 has no max.
        /// </summary>
        [Parameter]
        public int Max
        {
            get => _max;
            set
            {
                if (value != _max)
                {
                    _max = value;
                    _childrenNeedUpdates = true;
                }
            }
        }

        /// <summary>
        /// Custom class/classes for MaxAvatar
        /// </summary>
        [Parameter] public string MaxAvatarClass { get; set; }

        /// <summary>
        /// Child content of the component.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        internal List<MudAvatar> _avatars = new();

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
            if (_avatars.IndexOf(avatar) < Max)
                return true;
            else
                return false;
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            if (_childrenNeedUpdates == true)
            {
                foreach (var avatar in _avatars)
                {
                    avatar.ForceRedraw();
                }

                _childrenNeedUpdates = false;
            }
        }
    }
}
