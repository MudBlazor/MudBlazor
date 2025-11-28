// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;

namespace MudBlazor.Utilities.Converter
{
#nullable enable
    public sealed class RangeConverter<T> : IReversibleConverter<Range<T>?, string?>, ICultureAwareConverter
    {
        private readonly IReversibleConverter<T?, string?> _inner;

        public Func<string?> Format { get; set; } = () => null;

        public Func<CultureInfo> Culture { get; set; } = () => CultureInfo.InvariantCulture;

        public RangeConverter()
        {
            _inner = new DefaultConverter<T>()
            {
                Culture = Culture,
                Format = Format
            };
        }

        public string Convert(Range<T>? input)
        {
            if (input is null)
            {
                return string.Empty;
            }

            return Join(_inner.Convert(input.Start), _inner.Convert(input.End));
        }

        public Range<T>? ConvertBack(string? input)
        {
            if (!Split(input, out var startString, out var endString))
            {
                return null;
            }

            var startRange = _inner.ConvertBack(startString);
            var endRange = _inner.ConvertBack(endString);

            return new Range<T>(startRange, endRange);
        }

        public static string Join(string? valueStart, string? valueEnd)
        {
            if (string.IsNullOrEmpty(valueStart) && string.IsNullOrEmpty(valueEnd))
            {
                return string.Empty;
            }

            return $"[{valueStart};{valueEnd}]";
        }

        public static bool Split(string? value, out string valueStart, out string valueEnd)
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
