using System;
using MudBlazor.Resources;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// A universal T to bool? binding converter
    /// </summary>
    public class BoolConverter<T> : Converter<T?, bool?>
    {
        public BoolConverter()
        {
            SetFunc = OnSet;
            GetFunc = OnGet;
        }

        private T? OnGet(bool? value)
        {
            try
            {
                var type = typeof(T);
                if (type == typeof(bool))
                {
                    object result = value == true;
                    return (T)result;
                }

                if (type == typeof(bool?))
                {
                    object? result = value;
                    return (T?)result;
                }

                if (type == typeof(string))
                {
                    object? result = value switch
                    {
                        true => "on",
                        false => "off",
                        _ => null
                    };
                    return (T?)result;
                }

                if (type == typeof(int))
                {
                    object result = value == true ? 1 : 0;
                    return (T)result;
                }

                if (type == typeof(int?))
                {
                    object? result = value switch
                    {
                        true => 1,
                        false => 0,
                        _ => null
                    };
                    return (T?)result;
                }

                UpdateGetError(LanguageResource.Converter_ConversionNotImplemented, [typeof(T)]);
                return default(T);
            }
            catch (Exception exception)
            {
                UpdateGetError(LanguageResource.Converter_ConversionError, [exception.Message]);
                return default(T);
            }
        }

        private bool? OnSet(T? arg)
        {
            // This catches all nullable values which are null. No further null checks are needed below.!
            if (arg is null)
                return null;
            try
            {
                switch (arg)
                {
                    case bool boolValue:
                        return boolValue;
                    case int intValue:
                        return intValue > 0;
                    case string stringValue when string.IsNullOrWhiteSpace(stringValue):
                        return null;
                    case string stringValue when bool.TryParse(stringValue, out var flag):
                        return flag;
                    case string stringValue when stringValue.ToLowerInvariant() == "on":
                        return true;
                    case string stringValue when stringValue.ToLowerInvariant() == "off":
                        return false;
                    case string:
                        return null;
                    default:
                        UpdateSetError(LanguageResource.Converter_UnableToConvert, ["bool?", typeof(T).Name]);
                        return null;
                }
            }
            catch (FormatException exception)
            {
                UpdateSetError(LanguageResource.Converter_ConversionError, [exception.Message]);
                return null;
            }
        }
    }
}
