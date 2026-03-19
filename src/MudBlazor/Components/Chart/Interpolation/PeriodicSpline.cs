/*
 *  Work in this file is derived from code originally written by Hans-Peter Moser:
 *  http://www.mosismath.com/PeriodicSplines/PeriodicSplines.html
 */

using System.Diagnostics;

namespace MudBlazor.Interpolation
{
    internal class PeriodicSpline : SplineInterpolator
    {
        public PeriodicSpline(double[] xs, double[] ys, int resolution = 10) : base(xs, ys, resolution)
        {
            a = new double[n + 1];
            b = new double[n + 1];
            c = new double[n + 1];
            d = new double[n + 1];
            h = new double[n];

            CalcParameters();
            Interpolate();
        }

        private void CalcParameters()
        {
            Debug.Assert(a != null);
            Debug.Assert(b != null);
            Debug.Assert(c != null);
            Debug.Assert(d != null);
            Debug.Assert(h != null);

            for (var i = 0; i < n; i++)
            {
                a[i] = GivenYs[i];
            }

            for (var i = 0; i < n - 1; i++)
            {
                h[i] = GivenXs[i + 1] - GivenXs[i];
            }

            if (n == 1)
            {
                return;
            }

            a[n] = GivenYs[1];
            h[n - 1] = h[0];

            var size = n - 1;
            var sub = new double[size];
            var diag = new double[size];
            var sup = new double[size];
            var rhs = new double[size];

            for (var i = 0; i < size; i++)
            {
                diag[i] = 2.0 * (h[i] + h[i + 1]);
                if (i > 0)
                {
                    sub[i] = h[i];
                }

                if (i < size - 1)
                {
                    sup[i] = h[i + 1];
                }

                if ((h[i] != 0.0) && (h[i + 1] != 0.0))
                {
                    rhs[i] = (((a[i + 2] - a[i + 1]) / h[i + 1]) - ((a[i + 1] - a[i]) / h[i])) * 3.0;
                }
                else
                {
                    rhs[i] = 0.0;
                }
            }

            sub[0] = h[0];
            sup[size - 1] = h[0];

            var xValues = TridiagonalSolver.SolveCyclic(sub, diag, sup, rhs);

            for (var i = 1; i < n; i++)
            {
                c[i] = xValues[i - 1];
            }

            c[0] = c[n - 1];

            for (var i = 0; i < n; i++)
            {
                if (h[i] != 0.0)
                {
                    d[i] = 1.0 / 3.0 / h[i] * (c[i + 1] - c[i]);
                    b[i] = (1.0 / h[i] * (a[i + 1] - a[i])) - (h[i] / 3.0 * (c[i + 1] + (2 * c[i])));
                }
            }
        }
    }
}
