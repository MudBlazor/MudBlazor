using System.ComponentModel;

namespace MudBlazor
{
    public enum CharacterType
    {
        [Description("letter")]
        Letter,
        [Description("digit")]
        Digit,
        [Description("letterordigit")]
        LetterOrDigit,
        [Description("other")]
        Other,
        [Description("custom")]
        Custom,
        [Description("none")]
        None,
    }
}
