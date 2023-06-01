using System.ComponentModel;

namespace MudBlazor
{
    public enum DirectiveAttribute
    {
        [Description("preventDefault")]
        preventDefault,
        [Description("stopPropagation")]
        stopPropagation,
    }
}
