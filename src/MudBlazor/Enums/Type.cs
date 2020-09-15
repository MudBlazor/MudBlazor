using System.ComponentModel;

namespace MudBlazor
{
    public enum Type
    {
        [Description("text")]
        Text,
        [Description("password")]
        Password,
        [Description("email")]
        Email
    }
}
