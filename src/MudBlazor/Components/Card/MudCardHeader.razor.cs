using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudCardHeader : MudComponentBase
    {
        protected string Classname =>
        new CssBuilder("mud-card-header")
          .AddClass(Class)
        .Build();

        /// <summary>
        /// If used renders child content of the CardHeaderAvatar.
        /// </summary>
        [Parameter] public RenderFragment CardHeaderAvatar { get; set; }

        /// <summary>
        /// If used renders child content of the CardHeaderContent.
        /// </summary>
        [Parameter] public RenderFragment CardHeaderContent { get; set; }

        /// <summary>
        /// If used renders child content of the CardHeaderActions.
        /// </summary>
        [Parameter] public RenderFragment CardHeaderActions { get; set; }

        /// <summary>
        /// Optional child content
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }
    }
}
