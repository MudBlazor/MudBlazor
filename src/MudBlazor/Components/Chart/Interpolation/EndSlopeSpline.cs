/*
 *  Work in this file is derived from code originally written by Hans-Peter Moser:
 *  http://www.mosismath.com/AngleSplines/EndSlopeSplines.html
 */

using System.Diagnostics;

#nullable enable
namespace MudBlazor.Interpolation
{
    internal class EndSlopeSpline : SplineInterpolator
    {
        public EndSlopeSpline(double[] xs, double[] ys,
           int resolution = 10, double firstSlopeDegrees = 0, double lastSlopeDegrees = 0) :
           base(xs, ys, resolution)
        {
            _matrix = new Matrix(n);
            _gauss = new MatrixSolver(n, _matrix);

            a = new double[n];
            b = new double[n];
            c = new double[n];
            d = new double[n];
            h = new double[n];

            CalcParameters(firstSlopeDegrees, lastSlopeDegrees);
            Interpolate();
        }

        private void CalcParameters(double alpha, double beta)
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

            _matrix.a[0, 0] = 2.0 * h[0];
            _matrix.a[0, 1] = h[0];
            _matrix.y[0] = 3 * (((a[1] - a[0]) / h[0]) - Math.Tan(alpha * Math.PI / 180));

            for (var i = 0; i < n - 2; i++)
            {
                _matrix.a[i + 1, i] = h[i];
                _matrix.a[i + 1, i + 1] = 2.0 * (h[i] + h[i + 1]);
                if (i < n - 2)
                    _matrix.a[i + 1, i + 2] = h[i + 1];

                if ((h[i] != 0.0) && (h[i + 1] != 0.0))
                    _matrix.y[i + 1] = (((a[i + 2] - a[i + 1]) / h[i + 1]) - ((a[i + 1] - a[i]) / h[i])) * 3.0;
                else
                    _matrix.y[i + 1] = 0.0;
            }

            _matrix.a[n - 1, n - 2] = h[n - 2];
            _matrix.a[n - 1, n - 1] = 2.0 * h[n - 2];
            _matrix.y[n - 1] = 3.0 * (Math.Tan(beta * Math.PI / 180) - ((a[n - 1] - a[n - 2]) / h[n - 2]));

            if (_gauss.Eliminate() == false)
                throw new InvalidOperationException();

            _gauss.Solve();

            for (var i = 0; i < n; i++)
            {
                c[i] = _matrix.x[i];
            }
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
