using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudBlazor.Components.Chart.Interpolation
{
    public class NoInterpolation : ILineInterpolator
    {
        public double[] GivenYs { get; set; }
        public double[] GivenXs { get; set; }
        public double[] InterpolatedXs { get; set; }
        public double[] InterpolatedYs { get; set; }
        public bool InterpolationRequired { get; set; } = false;
    }
}
