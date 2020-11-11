using Microsoft.AspNetCore.Components;
using System.Text;

namespace MudBlazor
{
    public class OldComponentBaseMudThemeProvider : ComponentBase
    {
        [Parameter] public OldMudTheme Theme { get; set; }

        protected override void OnInitialized()
        {
            if (Theme == null)
            {
                OldMudTheme _theme = new OldMudTheme();
                Theme = _theme;
            }
        }
    }
}
