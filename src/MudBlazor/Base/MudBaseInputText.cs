using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public abstract class MudBaseInputText : MudBaseInput<string>
    {
        // empty for a reason. MudBaseInputText is needed for backwards compatibiltiy only

    }
}
