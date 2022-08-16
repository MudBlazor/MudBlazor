using System.ComponentModel;

namespace MudBlazor
{
    public enum ValuePresenter
    {
        [Description("none")]
        None,
        [Description("text")]
        Text,
        [Description("chip")]
        Chip,
        [Description("template")]
        Template,
    }
}
