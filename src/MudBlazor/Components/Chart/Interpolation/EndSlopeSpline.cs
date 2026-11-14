/*
 *  Work in this file is derived from code originally written by Hans-Peter Moser:
 *  http://www.mosismath.com/AngleSplines/EndSlopeSplines.html
 */

using System.Diagnostics;

namespace MudBlazor.Interpolation
{
    internal class EndSlopeSpline : SplineInterpolator
    {
        public EndSlopeSpline(double[] xs, double[] ys,
           int resolution = 10, double firstSlopeDegrees = 0, double lastSlopeDegrees = 0) :
           base(xs, ys, resolution)
        {
            a = new double[n];
            b = new double[n];
            c = new double[n];
            d = new double[n];
            h = new double[n - 1];

            CalcParameters(firstSlopeDegrees, lastSlopeDegrees);
            Interpolate();
        }

        private void CalcParameters(double alpha, double beta)
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

            var sub = new double[n];
            var diag = new double[n];
            var sup = new double[n];
            var rhs = new double[n];

            diag[0] = 2.0 * h[0];
            sup[0] = h[0];
            rhs[0] = 3 * (((a[1] - a[0]) / h[0]) - Math.Tan(alpha * Math.PI / 180));

            for (var i = 0; i < n - 2; i++)
            {
                sub[i + 1] = h[i];
                diag[i + 1] = 2.0 * (h[i] + h[i + 1]);
                sup[i + 1] = h[i + 1];

                if ((h[i] != 0.0) && (h[i + 1] != 0.0))
                {
                    rhs[i + 1] = (((a[i + 2] - a[i + 1]) / h[i + 1]) - ((a[i + 1] - a[i]) / h[i])) * 3.0;
                }
                else
                {
                    rhs[i + 1] = 0.0;
                }
            }

            sub[n - 1] = h[n - 2];
            diag[n - 1] = 2.0 * h[n - 2];
            rhs[n - 1] = 3.0 * (Math.Tan(beta * Math.PI / 180) - ((a[n - 1] - a[n - 2]) / h[n - 2]));

            var xValues = TridiagonalSolver.Solve(sub, diag, sup, rhs);

            for (var i = 0; i < n; i++)
            {
                c[i] = xValues[i];
            }

            for (var i = 0; i < n - 1; i++)
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
