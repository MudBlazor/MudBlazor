/*
 *  Work in this file is derived from code originally written by Hans-Peter Moser:
 *  http://www.mosismath.com/NaturalSplines/NaturalSplines.html
 */

#nullable enable
using System.Diagnostics;

namespace MudBlazor.Interpolation
{
    internal class NaturalSpline : SplineInterpolator
    {
        /// <summary>
        /// Natural Spline data interpolator
        /// </summary>  
        public NaturalSpline(double[] xs, double[] ys, int resolution = 10) : base(xs, ys, resolution)
        {
            _matrix = new Matrix(n - 2);
            _gauss = new MatrixSolver(n - 2, _matrix);

            a = new double[n];
            b = new double[n];
            c = new double[n];
            d = new double[n];
            h = new double[n - 1];

            CalcParameters();
            Integrate();
            Interpolate();
        }
        private void CalcParameters()
        {
            Debug.Assert(_matrix != null);
            Debug.Assert(_gauss != null);
            Debug.Assert(a != null);
            Debug.Assert(b != null);
            Debug.Assert(c != null);
            Debug.Assert(d != null);
            Debug.Assert(h != null);

            for (var i = 0; i < n; i++)
                a[i] = GivenYs[i];

            for (var i = 0; i < n - 1; i++)
                h[i] = GivenXs[i + 1] - GivenXs[i];

            for (var i = 0; i < n - 2; i++)
            {
                for (var k = 0; k < n - 2; k++)
                {
                    _matrix.a[i, k] = 0.0;
                    _matrix.y[i] = 0.0;
                    _matrix.x[i] = 0.0;
                }
            }

            for (var i = 0; i < n - 2; i++)
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
                    if (i < n - 3)
                        _matrix.a[i, i + 1] = h[i + 1];
                }

                if ((h[i] != 0.0) && (h[i + 1] != 0.0))
                    _matrix.y[i] = (((a[i + 2] - a[i + 1]) / h[i + 1]) - ((a[i + 1] - a[i]) / h[i])) * 3.0;
                else
                    _matrix.y[i] = 0.0;
            }

            if (_gauss.Eliminate() == false)
                throw new InvalidOperationException("error in matrix calculation");

            _gauss.Solve();

            c[0] = 0.0;
            c[n - 1] = 0.0;

            for (var i = 1; i < n - 1; i++)
                c[i] = _matrix.x[i - 1];

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
