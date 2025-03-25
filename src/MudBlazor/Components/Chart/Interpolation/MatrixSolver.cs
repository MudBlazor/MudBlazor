/*
 *  Work in this file is derived from code originally written by Hans-Peter Moser:
 *  http://www.mosismath.com/Basics/Basics.html
 *  http://www.mosismath.com/Matrix_Gauss/MatrixGauss.html
 */

#nullable enable
namespace MudBlazor.Interpolation
{
    // Matrix equation solver using the Gaussian elimination algorithm
    internal class MatrixSolver
    {
        public readonly Matrix _matrix;
        public readonly int _maxOrder;
        public bool _calcError;

        public MatrixSolver(int size, Matrix matrix)
        {
            _maxOrder = size;
            _matrix = matrix;
        }

        private void SwitchRows(int n)
        {
            double tempD;
            int i, j;
            for (i = n; i <= _maxOrder - 2; i++)
            {
                for (j = 0; j <= _maxOrder - 1; j++)
                {
                    tempD = _matrix.a[i, j];
                    _matrix.a[i, j] = _matrix.a[i + 1, j];
                    _matrix.a[i + 1, j] = tempD;
                }
                tempD = _matrix.y[i];
                _matrix.y[i] = _matrix.y[i + 1];
                _matrix.y[i + 1] = tempD;
            }
        }

        internal bool Eliminate()
        {
            int i, k, l;
            _calcError = false;
            for (k = 0; k <= _maxOrder - 2; k++)
            {
                for (i = k; i <= _maxOrder - 2; i++)
                {
                    if (Math.Abs(_matrix.a[i + 1, i]) < 1e-8)
                    {
                        SwitchRows(i + 1);
                    }
                    if (_matrix.a[i + 1, k] != 0.0)
                    {
                        for (l = k + 1; l <= _maxOrder - 1; l++)
                        {
                            if (!_calcError)
                            {
                                _matrix.a[i + 1, l] = (_matrix.a[i + 1, l] * _matrix.a[k, k]) - (_matrix.a[k, l] * _matrix.a[i + 1, k]);
                                if (_matrix.a[i + 1, l] > 10E260)
                                {
                                    _matrix.a[i + 1, k] = 0;
                                    _calcError = true;
                                }
                            }
                        }
                        _matrix.y[i + 1] = (_matrix.y[i + 1] * _matrix.a[k, k]) - (_matrix.y[k] * _matrix.a[i + 1, k]);
                        _matrix.a[i + 1, k] = 0;
                    }
                }
            }
            return !_calcError;
        }

        internal void Solve()
        {
            int k, l;
            for (k = _maxOrder - 1; k >= 0; k--)
            {
                for (l = _maxOrder - 1; l >= k; l--)
                {
                    _matrix.y[k] = _matrix.y[k] - (_matrix.x[l] * _matrix.a[k, l]);
                }
                if (_matrix.a[k, k] != 0)
                    _matrix.x[k] = _matrix.y[k] / _matrix.a[k, k];
                else
                    _matrix.x[k] = 0;
            }
        }
    }
}
