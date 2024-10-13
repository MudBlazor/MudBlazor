/*
 *  Work in this file is derived from code originally written by Hans-Peter Moser:
 *  http://www.mosismath.com/PeriodicSplines/PeriodicSplines.html
 */

#nullable enable
using System.Diagnostics;

namespace MudBlazor.Interpolation
{
    internal class PeriodicSpline : SplineInterpolator
    {
        public PeriodicSpline(double[] xs, double[] ys, int resolution = 10) : base(xs, ys, resolution)
        {
            _matrix = new Matrix(n - 1);
            _gauss = new MatrixSolver(n - 1, _matrix);

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
            Debug.Assert(_matrix != null);
            Debug.Assert(_gauss != null);

            for (var i = 0; i < n; i++)
                a[i] = GivenYs[i];

            for (var i = 0; i < n - 1; i++)
                h[i] = GivenXs[i + 1] - GivenXs[i];

            a[n] = GivenYs[1];
            h[n - 1] = h[0];

            for (var i = 0; i < n - 1; i++)
                for (var k = 0; k < n - 1; k++)
                {
                    _matrix.a[i, k] = 0.0;
                    _matrix.y[i] = 0.0;
                    _matrix.x[i] = 0.0;
                }

            for (var i = 0; i < n - 1; i++)
            {
                if (i == 0)
                {
                    _matrix.a[i, 0] = 2.0 * (h[0] + h[1]);
                    _matrix.a[i, 1] = h[1];
                }
                else
                {
                    _matrix.a[i, i - 1] = h[i];
                    _matrix.a[i, i] = 2.0 * (h[i] + h[i + 1]);
                    if (i < n - 2)
                        _matrix.a[i, i + 1] = h[i + 1];
                }
                if ((h[i] != 0.0) && (h[i + 1] != 0.0))
                    _matrix.y[i] = (((a[i + 2] - a[i + 1]) / h[i + 1]) - ((a[i + 1] - a[i]) / h[i])) * 3.0;
                else
                    _matrix.y[i] = 0.0;
            }

            _matrix.a[0, n - 2] = h[0];
            _matrix.a[n - 2, 0] = h[0];

            if (_gauss.Eliminate() == false)
                throw new InvalidOperationException();

            _gauss.Solve();

            for (var i = 1; i < n; i++)
                c[i] = _matrix.x[i - 1];
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
