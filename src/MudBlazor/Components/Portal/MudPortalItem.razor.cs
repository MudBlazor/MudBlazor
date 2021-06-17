using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Interop;
using MudBlazor.Services;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudPortalItem
    {
        private ElementReference _fragmentReference;
        private BoundingClientRect _fragmentRect;
        private string _right, _left;

       

        private string Style => new StyleBuilder()
            .AddStyle("left", _left, _left.IsNonEmpty())
            .AddStyle("right", _right, _right.IsNonEmpty())
            .AddStyle("bottom", (-(_fragmentRect?.Height + 10)).ToPixels())
            .AddStyle("height", "max-content")
            .AddStyle("width", "max-content")
            .AddStyle("position", "absolute", Item.PortalType == typeof(MudTooltip))
            .Build();

        [Parameter] public RenderFragment ChildContent { get; set; }

        [Parameter] public PortalItem Item { get; set; }


        [Inject] public IBrowserWindowSizeProvider ViewPort { get; set; }

        public void Update() => StateHasChanged();              

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {

            var size = await ViewPort.GetBrowserWindowSize();
            if (size.Width > 0) return;
            if (_fragmentRect == null)
            {
                _fragmentRect = await _fragmentReference.MudGetBoundingClientRectAsync();
                StateHasChanged();
            }

            if (Item.Placement == Placement.Bottom || Item.Placement == Placement.Top)
            {
                _left = (Item.ClientRect?.Width / 2 - _fragmentRect?.Width / 2).ToPixels();

            }

            if (_fragmentRect?.Right > size.Width)
            {
                _right = "0";
                _left = "unset";


            }
        }
    }
}
