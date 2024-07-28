using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using MudBlazor.Interfaces;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// Represents a grouping of multiple <see cref="MudAvatar"/> components.
    /// </summary>
    partial class MudAvatarGroup : MudComponentBase
    {
        private bool _childrenNeedUpdates = false;

        protected string Classname => new CssBuilder("mud-avatar-group")
            .AddClass($"mud-avatar-group-outlined", Outlined)
            .AddClass($"mud-avatar-group-outlined-{OutlineColor.ToDescriptionString()}", Outlined)
            .AddClass(Class)
            .Build();

        protected string MaxAvatarClassname => new CssBuilder("mud-avatar-group-max-avatar")
            .AddClass($"ms-n{Spacing}")
            .AddClass(MaxAvatarClass)
            .Build();

        /// <summary>
        /// The amount of space between avatars, between <c>0</c> and <c>16</c>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>3</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.AvatarGroup.Behavior)]
        public int Spacing { get; set; } = 3;

        /// <summary>
        /// Displays an outline for the group.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.  This property is useful to differentiate avatars which are the same color or use images.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.AvatarGroup.Appearance)]
        public bool Outlined { get; set; } = true;

        /// <summary>
        /// The color of the outline when <see cref="Outlined"/> is <c>true</c>.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.AvatarGroup.Appearance)]
        public Color OutlineColor { get; set; } = Color.Surface;

        /// <summary>
        /// The size of the drop shadow when the number of avatars exceeds <see cref="Max"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>0</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.AvatarGroup.Appearance)]
        public int MaxElevation { set; get; } = 0;

        /// <summary>
        /// Disables rounded corners when the number of avatars exceeds <see cref="Max"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  When <c>true</c>, the <c>border-radius</c> CSS style is set to <c>0</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.AvatarGroup.Appearance)]
        public bool MaxSquare { get; set; }

        /// <summary>
        /// Shows rounded corners when the number of avatars exceeds <see cref="Max"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  When <c>true</c>, the <c>border-radius</c> style is set to the theme's default value.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.AvatarGroup.Appearance)]
        public bool MaxRounded { get; set; }

        /// <summary>
        /// The color of the avatar when the number of avatars exceeds <see cref="Max"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Color.Default"/>.  Theme colors are supported.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.AvatarGroup.Appearance)]
        public Color MaxColor { get; set; } = Color.Default;

        /// <summary>
        /// The size of the avatar when the number of avatars exceeds <see cref="Max"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Size.Medium"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.AvatarGroup.Appearance)]
        public Size MaxSize { get; set; } = Size.Medium;

        /// <summary>
        /// The display variant to use when the number of avatars exceeds <see cref="Max"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Variant.Filled" />. The variant changes the appearance of the avatar, such as <c>Text</c>, <c>Outlined</c>, or <c>Filled</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.AvatarGroup.Appearance)]
        public Variant MaxVariant { get; set; } = Variant.Filled;

        /// <summary>
        /// The maximum allowed avatars to display.
        /// </summary>
        /// <remarks>
        /// Avatars above this limit are hidden, and a "+#" is shown for the number of avatars in excess. Defaults to <see cref="int.MaxValue" />.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.AvatarGroup.Behavior)]
        public int Max { get; set; } = int.MaxValue;

        /// <summary>
        /// The CSS class applied when the number of avatars exceeds <see cref="Max"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.AvatarGroup.Appearance)]
        public string? MaxAvatarClass { get; set; }

        /// <summary>
        /// The template used to render avatars when the number of avatars exceeds <see cref="Max"/>.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.AvatarGroup.Appearance)]
        public RenderFragment<int>? MaxAvatarsTemplate { get; set; }

        /// <summary>
        /// The content within this component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.AvatarGroup.Behavior)]
        public RenderFragment? ChildContent { get; set; }

        internal List<MudAvatar> _avatars = new();

        public MudAvatarGroup()
        {
            using var registerScope = CreateRegisterScope();
            registerScope.RegisterParameter<int>(nameof(Spacing))
                .WithParameter(() => Spacing)
                .WithChangeHandler(() => _childrenNeedUpdates = true)
                .Attach();
            registerScope.RegisterParameter<int>(nameof(Max))
                .WithParameter(() => Max)
                .WithChangeHandler(() => _childrenNeedUpdates = true)
                .Attach();
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
            return _avatars.IndexOf(avatar) >= Max;
        }

        /// <inheritdoc />
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
