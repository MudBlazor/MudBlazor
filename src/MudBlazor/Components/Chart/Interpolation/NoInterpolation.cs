using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudBlazor.Components.Chart.Interpolation
{
    public class NoInterpolation : ILineInterpolator
    {    
        private double[] _xValues;
        private double[] _yValues;

        public NoInterpolation(double[] xValues, double[] yValues)
        {
            _xValues = xValues;
            _yValues = yValues;
        }
        public double[] givenYs { get; set; }
        public double[] givenXs { get; set; }
        public double[] interpolatedXs { get; set; }
        public double[] interpolatedYs { get; set; }
        public bool InterpolationRequired { get; set; } = false;
    }
}
