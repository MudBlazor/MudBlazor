using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace MudBlazor
{
    [ExcludeFromCodeCoverage]
    internal static class Num
    {
        // From .NET 11 on, converting a double or float to decimal gives the exact binary value, so 0.1 becomes 0.1000000000000000055511151231.
        // Earlier runtimes kept 15 significant digits of a double and 7 of a float, and going through text keeps that on every runtime.
        // TODO(net11): Once every target runs on .NET 11 or later, decide whether the DataGrid and the slider should take the exact value instead.
        public static decimal ToDecimal(double d)
        {
            if (!double.IsFinite(d))
                throw new OverflowException();
            return decimal.Parse(d.ToString("G15", CultureInfo.InvariantCulture), NumberStyles.Float, CultureInfo.InvariantCulture);
        }

        public static decimal ToDecimal(float f)
        {
            if (!float.IsFinite(f))
                throw new OverflowException();
            return decimal.Parse(f.ToString("G7", CultureInfo.InvariantCulture), NumberStyles.Float, CultureInfo.InvariantCulture);
        }

        public static decimal ToDecimal(object value) => value switch
        {
            double d => ToDecimal(d),
            float f => ToDecimal(f),
            _ => Convert.ToDecimal(value)
        };
    }
}
