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
