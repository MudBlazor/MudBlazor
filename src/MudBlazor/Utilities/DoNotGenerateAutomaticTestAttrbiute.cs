// Not Used

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudBlazor.Utilities
{
    [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
    public sealed class DoNotGenerateAutomaticTestAttribute : Attribute
    {
        public DoNotGenerateAutomaticTestAttribute()
        {
        }
    }
}
