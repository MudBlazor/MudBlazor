using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudCardMedia : MudComponentBase
    {
        protected string StyleString => StyleBuilder.Default($"background-image:url({_imageUrl});{_height};")
                .AddStyle(this.Style)
                .Build();

        [Parameter] public string Title { get; set; }

        [Parameter] public string Image { get; set; }

        [Parameter] public int Height { get; set; } = 300;

        private string _height { get; set; }
        private string _imageUrl { get; set; }

        protected override void OnInitialized()
        {
            _height = $"height: {Height}px";
            _imageUrl = "\"" + Image + "\"";
        }
    }
}
