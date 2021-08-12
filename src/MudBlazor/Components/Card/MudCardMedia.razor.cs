using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudCardMedia : MudComponentBase
    {
        protected string StyleString =>
            StyleBuilder.Default($"background-image:url(\"{Image}\");height: {Height}px;")
                .AddStyle(this.Style)
                .Build();

        protected string Classname =>
            new CssBuilder("mud-card-media")
                .AddClass(Class)
                .Build();

        [Parameter] public string Title { get; set; }
        [Parameter] public string Image { get; set; }
        [Parameter] public int Height { get; set; } = 300;
    }
}
