using System.ComponentModel;

namespace MudBlazor
{
    public enum ButtonType
    {
        [Description("button")]
        Button,
        [Description("submit")]
        Submit,
        [Description("reset")]
        Reset
    }
}
