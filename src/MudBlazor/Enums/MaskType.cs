using System.ComponentModel;

namespace MudBlazor
{
    public enum MaskType
    {
        [Description("default")]
        Default,
        [Description("telephone")]
        Telephone,
        [Description("mac")]
        Mac,
    }
}
