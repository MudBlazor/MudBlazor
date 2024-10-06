namespace MudBlazor.Extensions
{
#nullable enable
    public static class MathExtensions
    {
        public static double Map(double sourceMin, double sourceMax, double targetMin, double targetMax, double value) =>
            (value / (sourceMax - sourceMin)) * (targetMax - targetMin);
    }
}
