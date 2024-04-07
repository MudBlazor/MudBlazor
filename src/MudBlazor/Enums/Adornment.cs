using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MudBlazor
{
    public enum Adornment
    {
        [Description("none")]
        None,
        [Display(Name = "start")]
        [Description("start")]
        Start,
        [Display(Name = "end")]
        [Description("end")]
        End
    }
}
