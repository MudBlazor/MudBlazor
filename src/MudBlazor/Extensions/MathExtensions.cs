using System.Numerics;

namespace MudBlazor.Extensions
{
    public static class MathExtensions
    {
        public static double Map(double sourceMin, double sourceMax, double targetMin, double targetMax, double value) =>
            (value / (sourceMax - sourceMin)) * (targetMax - targetMin);

        /// <summary>
        /// Calculates the sum of a generic list of numbers.
        /// </summary>
        /// <typeparam name="T">The numeric data type</typeparam>
        /// <param name="list">The list of numbers to be summed</param>
        public static T SumGeneric<T>(this IReadOnlyList<T>? list) where T : INumber<T>
        {
            if (list == null || list.Count == 0)
                return T.Zero;

            var sum = T.Zero;

            foreach (var item in list)
                sum += item;

            return sum;
        }
    }
}
