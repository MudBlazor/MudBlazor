using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorRepl.Client.Components
{
    public partial class ReplLoader : ComponentBase
    {
        [Parameter]
        public bool Show { get; set; }

        [Parameter]
        public string Message { get; set; }
    }
}
