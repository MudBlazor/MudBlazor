
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace MudBlazor
{
    public class MudThemeService
    {
#nullable enable
        public BaseMudThemeProvider? Provider { get; set; }

        public void Attach(BaseMudThemeProvider provider)
        {
            Provider = provider;
        }

        public void Detach()
        {
            Provider = null;
        }
    }
}
