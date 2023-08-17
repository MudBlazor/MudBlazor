using System;
using MudBlazor.Components.Chart.Interpolation;

namespace MudBlazor.Components.Chart
{
    public abstract class SplineInterpolator : ILineInterpolator
    {
        protected Matrix m;
        protected MatrixSolver gauss;

        protected readonly int n;
        protected double[] a, b, c, d, h;

        public double[] GivenYs { get; set; }
        public double[] GivenXs { get; set; }
        public double[] InterpolatedXs { get; set; }
        public double[] InterpolatedYs { get; set; }
        public bool InterpolationRequired { get; set; } = true;

        public SplineInterpolator(double[] xs, double[] ys, int resolution = 10)
        {
            if (xs is null || ys is null)
                throw new ArgumentException("xs and ys cannot be null");

            if (xs.Length != ys.Length)
                throw new ArgumentException("xs and ys must have the same length");

            if (xs.Length < 4)
                throw new ArgumentException("xs and ys must have a length of 4 or greater");

            if (resolution < 1)
                throw new ArgumentException("resolution must be 1 or greater");

            GivenXs = xs;
            GivenYs = ys;
            n = xs.Length;

            InterpolatedXs = new double[n * resolution];
            InterpolatedYs = new double[n * resolution];
        }
        public void Interpolate()
        {
            int resolution = InterpolatedXs.Length / n;
            for (int i = 0; i < h.Length; i++)
            {
                for (int k = 0; k < resolution; k++)
                {
                    double deltaX = (double)k / resolution * h[i];
                    double termA = a[i];
                    double termB = b[i] * deltaX;
                    double termC = c[i] * deltaX * deltaX;
                    double termD = d[i] * deltaX * deltaX * deltaX;
                    int interpolatedIndex = i * resolution + k;
                    InterpolatedXs[interpolatedIndex] = deltaX + GivenXs[i];
                    InterpolatedYs[interpolatedIndex] = termA + termB + termC + termD;
                }
            }

            // After interpolation the last several values of the interpolated arrays
            // contain uninitialized data. This section identifies the values which are
            // populated with values and copies just the useful data into new arrays.
            int pointsToKeep = resolution * (n - 1) + 1;
            double[] interpolatedXsCopy = new double[pointsToKeep];
            double[] interpolatedYsCopy = new double[pointsToKeep];
            Array.Copy(InterpolatedXs, 0, interpolatedXsCopy, 0, pointsToKeep - 1);
            Array.Copy(InterpolatedYs, 0, interpolatedYsCopy, 0, pointsToKeep - 1);
            InterpolatedXs = interpolatedXsCopy;
            InterpolatedYs = interpolatedYsCopy;
            InterpolatedXs[pointsToKeep - 1] = GivenXs[n - 1];
            InterpolatedYs[pointsToKeep - 1] = GivenYs[n - 1];
        }

        public double Integrate()
        {
            double integral = 0;
            for (int i = 0; i < h.Length; i++)
            {
                double termA = a[i] * h[i];
                double termB = b[i] * Math.Pow(h[i], 2) / 2.0;
                double termC = c[i] * Math.Pow(h[i], 3) / 3.0;
                double termD = d[i] * Math.Pow(h[i], 4) / 4.0;
                integral += termA + termB + termC + termD;
            }
            return integral;
        }
    }
}
