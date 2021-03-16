using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudBlazor.Components.Chart.Interpolation
{
    public interface ILineInterpolator
    {
        public double[] givenYs { get; set; }
        public double[] givenXs { get; set; }
        public double[] interpolatedXs { get; set; }
        public double[] interpolatedYs { get; set; }
        public bool InterpolationRequired { get; set; }

    }
}
