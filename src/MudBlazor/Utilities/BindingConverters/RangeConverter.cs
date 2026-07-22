using System.Linq;

namespace MudBlazor
{
    public class RangeConverter<T> : Converter<Range<T>>
    {
        readonly DefaultConverter<T> _converter;

        public RangeConverter()
        {
            _converter = new DefaultConverter<T>();

            SetFunc = OnSet;
            GetFunc = OnGet;
        }

        private Range<T> OnGet(string value)
        {
            if (!Split(value, out var valueStart, out var valueEnd))
                return null;

            return new Range<T>(_converter.Get(valueStart), _converter.Get(valueEnd));
        }

        private string OnSet(Range<T> arg)
        {
            if (arg == null)
                return string.Empty;

            return Join(_converter.Set(arg.Start), _converter.Set(arg.End));
        }

        public static string Join(string valueStart, string valueEnd)
        {
            if (string.IsNullOrEmpty(valueStart) && string.IsNullOrEmpty(valueEnd))
                return string.Empty;

            return $"[{valueStart};{valueEnd}]";
        }

        public static bool Split(string value, out string valueStart, out string valueEnd)
        {
            valueStart = valueEnd = string.Empty;

            if (string.IsNullOrEmpty(value) || value[0] != '[' || value[^1] != ']')
            {
                return false;
            }

            var idx = value.IndexOf(';');

            if (idx < 1)
            {
                return false;
            }

            valueStart = value[1..idx];
            valueEnd = value[(idx + 1)..^1];

            return true;
        }
    }
}
