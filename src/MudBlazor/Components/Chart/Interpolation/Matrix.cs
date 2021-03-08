// Not Used

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudBlazor.Components.Chart
{
    public class Matrix
    {
        public double[,] a;
        public double[] y;
        public double[] x;

        public Matrix(int size)
        {
            a = new double[size, size];
            y = new double[size];
            x = new double[size];
        }
    }
}
