/*
 *  Work in this file is derived from code originally written by Hans-Peter Moser:
 *  http://www.mosismath.com/Basics/Basics.html
 *  http://www.mosismath.com/Matrix_Gauss/MatrixGauss.html
 */

using System;

namespace MudBlazor.Components.Chart
{
    // Matrix equation solver using the Gaussian elimination algorithm
    public class MatrixSolver
    {
        public readonly Matrix m;
        public readonly int maxOrder;
        public bool calcError;

        public MatrixSolver(int size, Matrix mi)
        {
            maxOrder = size;
            m = mi;
        }

        void SwitchRows(int n)
        {
            double tempD;
            int i, j;
            for (i = n; i <= maxOrder - 2; i++)
            {
                for (j = 0; j <= maxOrder - 1; j++)
                {
                    tempD = m.a[i, j];
                    m.a[i, j] = m.a[i + 1, j];
                    m.a[i + 1, j] = tempD;
                }
                tempD = m.y[i];
                m.y[i] = m.y[i + 1];
                m.y[i + 1] = tempD;
            }
        }

        public bool Eliminate()
        {
            int i, k, l;
            calcError = false;
            for (k = 0; k <= maxOrder - 2; k++)
            {
                for (i = k; i <= maxOrder - 2; i++)
                {
                    if (Math.Abs(m.a[i + 1, i]) < 1e-8)
                    {
                        SwitchRows(i + 1);
                    }
                    if (m.a[i + 1, k] != 0.0)
                    {
                        for (l = k + 1; l <= maxOrder - 1; l++)
                        {
                            if (!calcError)
                            {
                                m.a[i + 1, l] = m.a[i + 1, l] * m.a[k, k] - m.a[k, l] * m.a[i + 1, k];
                                if (m.a[i + 1, l] > 10E260)
                                {
                                    m.a[i + 1, k] = 0;
                                    calcError = true;
                                }
                            }
                        }
                        m.y[i + 1] = m.y[i + 1] * m.a[k, k] - m.y[k] * m.a[i + 1, k];
                        m.a[i + 1, k] = 0;
                    }
                }
            }
            return !calcError;
        }

        public void Solve()
        {
            int k, l;
            for (k = maxOrder - 1; k >= 0; k--)
            {
                for (l = maxOrder - 1; l >= k; l--)
                {
                    m.y[k] = m.y[k] - m.x[l] * m.a[k, l];
                }
                if (m.a[k, k] != 0)
                    m.x[k] = m.y[k] / m.a[k, k];
                else
                    m.x[k] = 0;
            }
        }
    }
}
