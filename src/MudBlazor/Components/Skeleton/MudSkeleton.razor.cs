using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudSkeleton : MudComponentBase
    {
        protected string Classname =>
            new CssBuilder("mud-skeleton")
                .AddClass($"mud-skeleton-{SkeletonType.ToDescriptionString()}")
                .AddClass($"mud-skeleton-{Animation.ToDescriptionString()}")
                .AddClass(Class)
                .Build();

        /// <summary>
        /// With defined in string, needs px or % or equal prefix.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Skeleton.Appearance)]
        public string Width { set; get; }

        /// <summary>
        /// Height defined in string, needs px or % or equal prefix.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Skeleton.Appearance)]
        public string Height { set; get; }

        /// <summary>
        /// Shape of the skeleton that will be rendered.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Skeleton.Appearance)]
        public SkeletonType SkeletonType { set; get; } = SkeletonType.Text;

        /// <summary>
        /// Animation style, if false it will be disabled.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Skeleton.Appearance)]
        public Animation Animation { set; get; } = Animation.Pulse;

        private string _width;
        private string _height;
        private string _styleString;

        protected override void OnInitialized()
        {
            if (!string.IsNullOrEmpty(Width))
            {
                _width = $"width:{Width};";
            }

            if (!string.IsNullOrEmpty(Height))
            {
                _height = $"height:{Height};";
            }

            _styleString = $"{_width}{_height}{Style}";
        }
    }
}
