﻿using System;

namespace MudBlazor.Components.Chart.Interpolation
{
    public interface ILineInterpolator
    {
        public double[] GivenYs { get; set; }
        public double[] GivenXs { get; set; }
        public double[] InterpolatedXs { get; set; }
        public double[] InterpolatedYs { get; set; }
        [Obsolete("This will be removed in v7")]
        public bool InterpolationRequired { get; set; }
    }
}
