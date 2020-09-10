using Microsoft.AspNetCore.Components;
using System.Text;

namespace MudBlazor
{
    public class ComponentBaseThemeProvider : ComponentBase
    {
        [Parameter] public MudTheme Theme { get; set; }

        protected override void OnInitialized()
        {
            if (Theme == null)
            {
                MudTheme _theme = new MudTheme();
                Theme = _theme;
            }
        }
    }
}
