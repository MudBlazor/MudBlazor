// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Interpolation;

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
        var n = d.Length;
        if (n == 0)
        {
            return [];
        }

        if (a.Length != n || b.Length != n || c.Length != n)
        {
            throw new ArgumentException("All input arrays must have the same length.");
        }

        var x = new double[n];
        if (n == 1)
        {
            if (Math.Abs(b[0]) < 1e-12)
            {
                throw new InvalidOperationException("Diagonal element is zero or near-zero.");
            }

            x[0] = d[0] / b[0];
            return x;
        }

        var cPrime = new double[n];
        var dPrime = new double[n];

        if (Math.Abs(b[0]) < 1e-12)
        {
            throw new InvalidOperationException("First diagonal element is zero or near-zero.");
        }

        cPrime[0] = c[0] / b[0];
        dPrime[0] = d[0] / b[0];

        for (var i = 1; i < n; i++)
        {
            var denominator = b[i] - (a[i] * cPrime[i - 1]);
            if (Math.Abs(denominator) < 1e-12)
            {
                throw new InvalidOperationException($"Denominator at index {i} is zero or near-zero.");
            }

            var m = 1.0 / denominator;
            if (i < n - 1)
            {
                cPrime[i] = c[i] * m;
            }

            dPrime[i] = (d[i] - (a[i] * dPrime[i - 1])) * m;
        }

        x[n - 1] = dPrime[n - 1];
        for (var i = n - 2; i >= 0; i--)
        {
            x[i] = dPrime[i] - (cPrime[i] * x[i + 1]);
        }

        return x;
    }

    /// <summary>
    /// Solves a cyclic tridiagonal system of equations using the Sherman-Morrison formula.
    /// </summary>
    public static double[] SolveCyclic(double[] a, double[] b, double[] c, double[] d)
    {
        var n = d.Length;
        if (n == 0)
        {
            return [];
        }

        if (a.Length != n || b.Length != n || c.Length != n)
        {
            throw new ArgumentException("All input arrays must have the same length.");
        }

        if (n <= 2)
        {
            return Solve(a, b, c, d);
        }

        var alpha = a[0];
        var beta = c[n - 1];

        var bPrime = (double[])b.Clone();
        var gamma = -b[0];

        if (Math.Abs(gamma) < 1e-12)
        {
            gamma = 1.0;
        }

        bPrime[0] -= gamma;
        bPrime[n - 1] -= alpha * beta / gamma;

        var x = Solve(a, bPrime, c, d);

        var v = new double[n];
        v[0] = gamma;
        v[n - 1] = beta;
        var u = Solve(a, bPrime, c, v);

        var factor = (x[0] + (alpha * x[n - 1] / gamma)) / (1.0 + u[0] + (alpha * u[n - 1] / gamma));

        for (var i = 0; i < n; i++)
        {
            x[i] -= factor * u[i];
        }

        return x;
    }
}
