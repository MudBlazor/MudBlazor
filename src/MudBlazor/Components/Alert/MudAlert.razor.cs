using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MudBlazor
{
    public partial class MudAlert : ComponentBase
    {
        protected string Classname =>
        new CssBuilder("mud-alert")
          .AddClass($"mud-alert-{Variant.ToDescriptionString()}-{Severity.ToDescriptionString()}")
          .AddClass($"mud-dense", Dense)
          .AddClass($"mud-square", Square)
          .AddClass($"mud-elevation-{Elevation.ToString()}")
          .AddClass(Class)
        .Build();

        [Parameter] public int Elevation { set; get; } = 0;
        [Parameter] public bool Square { get; set; }
        [Parameter] public bool Dense { get; set; }
        [Parameter] public bool NoIcon { get; set; }
        [Parameter] public Severity Severity { get; set; } = Severity.Normal;
        [Parameter] public Variant Variant { get; set; } = Variant.Text;
        [Parameter] public string Class { set; get; }
        [Parameter] public RenderFragment ChildContent { get; set; }

        public string Icon;
        public string variant;
        protected override void OnInitialized()
        {
            switch(Severity)
            {
                case Severity.Normal:
                    Icon = "M19 3h-1V1h-2v2H8V1H6v2H5c-1.1 0-2 .9-2 2v14c0 1.1.9 2 2 2h14c1.1 0 2-.9 2-2V5c0-1.1-.9-2-2-2zm0 16H5V9h14v10zM5 7V5h14v2H5zm2 4h10v2H7zm0 4h7v2H7z";
                    break;
                case Severity.Info:
                    Icon = "M11 7h2v2h-2zm0 4h2v6h-2zm1-9C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm0 18c-4.41 0-8-3.59-8-8s3.59-8 8-8 8 3.59 8 8-3.59 8-8 8z";
                    break;
                case Severity.Success:
                    Icon = "M20,12A8,8 0 0,1 12,20A8,8 0 0,1 4,12A8,8 0 0,1 12,4C12.76,4 13.5,4.11 14.2, 4.31L15.77,2.74C14.61,2.26 13.34,2 12,2A10,10 0 0,0 2,12A10,10 0 0,0 12,22A10,10 0 0, 0 22,12M7.91,10.08L6.5,11.5L11,16L21,6L19.59,4.58L11,13.17L7.91,10.08Z";
                    break;
                case Severity.Warning:
                    Icon = "M12 5.99L19.53 19H4.47L12 5.99M12 2L1 21h22L12 2zm1 14h-2v2h2v-2zm0-6h-2v4h2v-4z";
                    break;
                case Severity.Error:
                    Icon = "M11 15h2v2h-2zm0-8h2v6h-2zm.99-5C6.47 2 2 6.48 2 12s4.47 10 9.99 10C17.52 22 22 17.52 22 12S17.52 2 11.99 2zM12 20c-4.42 0-8-3.58-8-8s3.58-8 8-8 8 3.58 8 8-3.58 8-8 8z";
                    break;
            }
        }
    }
}
   

