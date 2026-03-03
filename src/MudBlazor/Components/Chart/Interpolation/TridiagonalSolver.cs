// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace MudBlazor.Interpolation
{
    internal static class TridiagonalSolver
    {
        /// <summary>
        /// Solves a tridiagonal system of equations using the Thomas algorithm.
        /// a[i] * x[i-1] + b[i] * x[i] + c[i] * x[i+1] = d[i]
        /// </summary>
        /// <param name="a">Lower diagonal (size N, a[0] is ignored)</param>
        /// <param name="b">Main diagonal (size N)</param>
        /// <param name="c">Upper diagonal (size N, c[N-1] is ignored)</param>
        /// <param name="d">Right-hand side (size N)</param>
        /// <returns>Solution vector x (size N)</returns>
        public static double[] Solve(double[] a, double[] b, double[] c, double[] d)
        {
            int n = d.Length;
            if (n == 0) return Array.Empty<double>();

            double[] x = new double[n];
            if (n == 1)
            {
                x[0] = d[0] / b[0];
                return x;
            }

            double[] cPrime = new double[n];
            double[] dPrime = new double[n];

            cPrime[0] = c[0] / b[0];
            dPrime[0] = d[0] / b[0];

            for (int i = 1; i < n; i++)
            {
                double m = 1.0 / (b[i] - a[i] * cPrime[i - 1]);
                if (i < n - 1)
                    cPrime[i] = c[i] * m;
                dPrime[i] = (d[i] - a[i] * dPrime[i - 1]) * m;
            }

            x[n - 1] = dPrime[n - 1];
            for (int i = n - 2; i >= 0; i--)
            {
                x[i] = dPrime[i] - cPrime[i] * x[i + 1];
            }

            return x;
        }

        /// <summary>
        /// Solves a cyclic tridiagonal system of equations using the Sherman-Morrison formula.
        /// </summary>
        public static double[] SolveCyclic(double[] a, double[] b, double[] c, double[] d)
        {
            int n = d.Length;
            if (n <= 2) return Solve(a, b, c, d);

            // The cyclic tridiagonal system is A x = d
            // where A is tridiagonal plus elements A[0, n-1] = alpha and A[n-1, 0] = beta.
            // In MudBlazor's PeriodicSpline, alpha = a[0] and beta = c[n-1] (conceptually)
            // but let's look at PeriodicSpline.cs:
            // _matrix.a[0, n - 2] = h[0];
            // _matrix.a[n - 2, 0] = h[0];
            // Here matrix size is N = n-1. So alpha = A[0, N-1] = h[0], beta = A[N-1, 0] = h[0].

            double alpha = a[0];
            double beta = c[n - 1];

            // Modified system A' x = d where A' is tridiagonal
            double[] bPrime = (double[])b.Clone();
            double gamma = -b[0]; // To avoid small denominators
            bPrime[0] -= gamma;
            bPrime[n - 1] -= alpha * beta / gamma;

            double[] x = Solve(a, bPrime, c, d);

            // Solve A' u = v where v = [gamma, 0, ..., beta]^T
            double[] v = new double[n];
            v[0] = gamma;
            v[n - 1] = beta;
            double[] u = Solve(a, bPrime, c, v);

            double factor = (x[0] + alpha * x[n - 1] / gamma) / (1.0 + u[0] + alpha * u[n - 1] / gamma);

            for (int i = 0; i < n; i++)
            {
                x[i] -= factor * u[i];
            }

            return x;
        }
    }
}
