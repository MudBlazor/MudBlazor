using System.ComponentModel;

namespace MudBlazor
{
    public enum InputType
    {
        [Description("text")]
        Text,
        [Description("password")]
        Password,
        [Description("email")]
        Email,
        [Description("hidden")]
        Hidden
    }
}
