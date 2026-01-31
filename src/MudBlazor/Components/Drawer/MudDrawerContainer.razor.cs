using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable

    /// <summary>
    /// A container for a <see cref="MudDrawer"/> component.
    /// </summary>
    /// <seealso cref="MudDrawer"/>
    /// <seealso cref="MudDrawerHeader"/>
    public partial class MudDrawerContainer : MudComponentBase
    {
        protected bool Fixed { get; set; } = false;
        private readonly List<MudDrawer> _drawers = new();

        protected virtual string Classname =>
            new CssBuilder()
                .AddClass(GetDrawerClass(FindLeftDrawer()))
                .AddClass(GetDrawerClass(FindRightDrawer()))
                .AddClass(Class)
                .Build();

        protected string Stylename =>
            new StyleBuilder()
                .AddStyle("--mud-drawer-width-left", GetDrawerWidth(FindLeftDrawer()), !string.IsNullOrEmpty(GetDrawerWidth(FindLeftDrawer())))
                .AddStyle("--mud-drawer-width-right", GetDrawerWidth(FindRightDrawer()), !string.IsNullOrEmpty(GetDrawerWidth(FindRightDrawer())))
                .AddStyle("--mud-drawer-height-top", GetDrawerHeight(FindTopDrawer()), !string.IsNullOrEmpty(GetDrawerHeight(FindTopDrawer())))
                .AddStyle("--mud-drawer-height-bottom", GetDrawerHeight(FindBottomDrawer()), !string.IsNullOrEmpty(GetDrawerHeight(FindBottomDrawer())))
                .AddStyle("--mud-drawer-width-mini-left", GetMiniDrawerWidth(FindLeftMiniDrawer()), !string.IsNullOrEmpty(GetMiniDrawerWidth(FindLeftMiniDrawer())))
                .AddStyle("--mud-drawer-width-mini-right", GetMiniDrawerWidth(FindRightMiniDrawer()), !string.IsNullOrEmpty(GetMiniDrawerWidth(FindRightMiniDrawer())))
                .AddStyle(Style)
                .Build();

        [CascadingParameter(Name = "RightToLeft")]
        public bool RightToLeft { get; set; }

        /// <summary>
        /// The custom content inside this drawer.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Drawer.Behavior)]
        public RenderFragment? ChildContent { get; set; }

        internal void Add(MudDrawer drawer)
        {
            if (Fixed && !drawer.IsFixed)
                return;

            _drawers.Add(drawer);
            StateHasChanged();
        }

        internal void Remove(MudDrawer drawer)
        {
            _drawers.Remove(drawer);
            StateHasChanged();
        }

        private static string GetDrawerClass(MudDrawer? drawer)
        {
            if (drawer is null)
            {
                return string.Empty;
            }

            var className = $"mud-drawer-{(drawer.GetState<bool>(nameof(MudDrawer.Open)) ? "open" : "close")}-{drawer.Variant.ToStringFast(true)}";
            if (drawer.Variant is DrawerVariant.Responsive or DrawerVariant.Mini)
            {
                className += $"-{drawer.Breakpoint.ToStringFast(true)}";
            }
            className += $"-{drawer.GetPosition()}";

            className += $" mud-drawer-{drawer.GetPosition()}-clipped-{drawer.ClipMode.ToStringFast(true)}";

            return className;
        }

        private static string? GetDrawerWidth(MudDrawer? drawer)
        {
            if (drawer is null)
            {
                return string.Empty;
            }

            return drawer.Width;
        }

        private static string? GetDrawerHeight(MudDrawer? drawer)
        {
            if (drawer is null)
            {
                return string.Empty;
            }

            return drawer.Height;
        }

        private static string? GetMiniDrawerWidth(MudDrawer? drawer)
        {
            if (drawer is null)
            {
                return string.Empty;
            }

            return drawer.MiniWidth;
        }

        private MudDrawer? FindLeftDrawer()
        {
            var anchor = RightToLeft ? Anchor.End : Anchor.Start;

            return _drawers.FirstOrDefault(d => d.Anchor == anchor || d.Anchor == Anchor.Left);
        }

        private MudDrawer? FindRightDrawer()
        {
            var anchor = RightToLeft ? Anchor.Start : Anchor.End;

            return _drawers.FirstOrDefault(d => d.Anchor == anchor || d.Anchor == Anchor.Right);
        }

        private MudDrawer? FindTopDrawer()
        {
            return _drawers.FirstOrDefault(d => d.Anchor == Anchor.Top);
        }

        private MudDrawer? FindBottomDrawer()
        {
            return _drawers.FirstOrDefault(d => d.Anchor == Anchor.Bottom);
        }

        private MudDrawer? FindLeftMiniDrawer()
        {
            var anchor = RightToLeft ? Anchor.End : Anchor.Start;

            return _drawers.FirstOrDefault(d => d.Variant == DrawerVariant.Mini && (d.Anchor == anchor || d.Anchor == Anchor.Left));
        }

        private MudDrawer? FindRightMiniDrawer()
        {
            var anchor = RightToLeft ? Anchor.Start : Anchor.End;

            return _drawers.FirstOrDefault(d => d.Variant == DrawerVariant.Mini && (d.Anchor == anchor || d.Anchor == Anchor.Right));
        }
    }
}
