using System;

namespace MudBlazor.Components.Chart.Interpolation
{
    [Obsolete("This will be removed in v7")]
    public class NoInterpolation : ILineInterpolator
    {
        public double[] GivenYs { get; set; }
        public double[] GivenXs { get; set; }
        public double[] InterpolatedXs { get; set; }
        public double[] InterpolatedYs { get; set; }
        public bool InterpolationRequired { get; set; } = false;
    }
}
