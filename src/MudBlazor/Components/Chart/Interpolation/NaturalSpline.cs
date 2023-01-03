/*
 *  Work in this file is derived from code originally written by Hans-Peter Moser:
 *  http://www.mosismath.com/NaturalSplines/NaturalSplines.html
 */
using System;

namespace MudBlazor.Components.Chart.Interpolation
{
    public class NaturalSpline : SplineInterpolator
    {
        /// <summary>
        /// Natural Spline data interpolator
        /// </summary>  
        public NaturalSpline(double[] xs, double[] ys, int resolution = 10) : base(xs, ys, resolution)
        {
            m = new Matrix(n - 2);
            gauss = new MatrixSolver(n - 2, m);

            a = new double[n];
            b = new double[n];
            c = new double[n];
            d = new double[n];
            h = new double[n - 1];

            CalcParameters();
            Integrate();
            Interpolate();
        }
        public void CalcParameters()
        {
            for (int i = 0; i < n; i++)
                a[i] = GivenYs[i];

            for (int i = 0; i < n - 1; i++)
                h[i] = GivenXs[i + 1] - GivenXs[i];

            for (int i = 0; i < n - 2; i++)
            {
                for (int k = 0; k < n - 2; k++)
                {
                    m.a[i, k] = 0.0;
                    m.y[i] = 0.0;
                    m.x[i] = 0.0;
                }
            }

            for (int i = 0; i < n - 2; i++)
            {
                if (i == 0)
                {
                    m.a[i, 0] = 2.0 * (h[0] + h[1]);
                    m.a[i, 1] = h[1];
                }
                else
                {
                    m.a[i, i - 1] = h[i];
                    m.a[i, i] = 2.0 * (h[i] + h[i + 1]);
                    if (i < n - 3)
                        m.a[i, i + 1] = h[i + 1];
                }

                if ((h[i] != 0.0) && (h[i + 1] != 0.0))
                    m.y[i] = ((a[i + 2] - a[i + 1]) / h[i + 1] - (a[i + 1] - a[i]) / h[i]) * 3.0;
                else
                    m.y[i] = 0.0;
            }

            if (gauss.Eliminate() == false)
                throw new InvalidOperationException("error in matrix calculation");

            gauss.Solve();

            c[0] = 0.0;
            c[n - 1] = 0.0;

            for (int i = 1; i < n - 1; i++)
                c[i] = m.x[i - 1];

            for (int i = 0; i < n - 1; i++)
            {
                if (h[i] != 0.0)
                {
                    d[i] = 1.0 / 3.0 / h[i] * (c[i + 1] - c[i]);
                    b[i] = 1.0 / h[i] * (a[i + 1] - a[i]) - h[i] / 3.0 * (c[i + 1] + 2 * c[i]);
                }
            }
        }
    }
}
