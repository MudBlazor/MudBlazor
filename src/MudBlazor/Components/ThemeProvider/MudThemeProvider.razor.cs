using Microsoft.AspNetCore.Components;
using System.Text;

namespace MudBlazor
{
    public class ComponentBaseMudThemeProvider : ComponentBase
    {
        [Parameter] public MudThemeOld Theme { get; set; }

        protected override void OnInitialized()
        {
            if (Theme == null)
            {
                MudThemeOld _theme = new MudThemeOld();
                Theme = _theme;
            }
        }
    }
}
