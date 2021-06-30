// Not Used

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudBlazor.Extensions
{
    public static class DoubleExtentions
    {
        public static Double ToRad(this Double input) => (Math.PI / 180) * input;
    }
}
