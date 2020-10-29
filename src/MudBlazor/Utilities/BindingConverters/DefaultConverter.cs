using System;
using System.Globalization;

namespace MudBlazor
{
    
    /// <summary>
    /// A universal T to string binding converter
    /// </summary>
    public class DefaultConverter<T> : Converter<T>
    {
       
        public DefaultConverter()
        {
            SetFunc = OnSet;
            GetFunc = OnGet;
        }

        private T OnGet(string value)
        {
            try
            {
                if (typeof(T) == typeof(string))
                {
                    return (T)(object)value;
                }
                else if (typeof(T) == typeof(int) || typeof(T) == typeof(int?))
                {
                    if (int.TryParse(value, NumberStyles.Integer, Culture, out var parsedValue))
                        return (T)(object)parsedValue;
                    OnError?.Invoke("Not a valid number");
                }
                else if (typeof(T) == typeof(double) || typeof(T) == typeof(double?))
                {
                    if (double.TryParse(value, NumberStyles.Any, Culture, out var parsedValue))
                        return (T)(object)parsedValue;
                    OnError?.Invoke("Not a valid number");
                }
                else if (typeof(T) == typeof(Guid))
                {
                    if (Guid.TryParse(value, out var parsedValue))
                        return (T)(object)parsedValue;
                    OnError?.Invoke("Not a valid GUID");
                }
                else if (typeof(T).IsEnum)
                {
                    if (Enum.TryParse(typeof(T), value, out var parsedValue))
                        return (T)parsedValue;
                    OnError?.Invoke("Not a value of " + typeof(T).Name);
                }
                else if (typeof(T) == typeof(DateTime) || typeof(T) == typeof(DateTime?))
                {
                    if (DateTime.TryParse(value, Culture,  DateTimeStyles.None, out var parsedValue))
                        return (T)(object)parsedValue;
                    OnError?.Invoke("Not a valid date time");
                }
                else if (typeof(T) == typeof(TimeSpan) || typeof(T) == typeof(TimeSpan?))
                {
                    if (TimeSpan.TryParse(value, Culture, out var parsedValue))
                        return (T)(object)parsedValue;
                    OnError?.Invoke("Not a valid date time");
                }
            }
            catch (Exception e)
            {
                OnError?.Invoke("Conversion error: "+e.Message);
                return default(T);
            }
            OnError?.Invoke($"Conversion to type {typeof(T)} not implemented");
            return default(T);
        }

        private string OnSet(T arg)
        {
            if (arg == null)
                return null;
            try
            {
                if (typeof(T) == typeof(string))
                {
                    return (string)(object)arg;
                }
                else if (typeof(T) == typeof(int) )
                {
                    var value = (int) (object) arg;
                    return value.ToString(Culture);
                }
                else if (typeof(T) == typeof(int?))
                {
                    var value = (int?) (object) arg;
                    if (value == null)
                        return null;
                    return value.Value.ToString(Culture);
                }
                else if (typeof(T) == typeof(double))
                {
                    var value = (double) (object) arg;
                    return value.ToString(Culture);
                }
                else if (typeof(T) == typeof(double?))
                {
                    var value = (double?) (object) arg;
                    if (value == null)
                        return null;
                    return value.Value.ToString(Culture);
                }
                else if (typeof(T) == typeof(Guid))
                {
                    var value = (Guid) (object) arg;
                    return value.ToString();
                }
                else if (typeof(T).IsEnum)
                {
                    var value = (Enum) (object) arg;
                    return value.ToString();
                }
                else if (typeof(T) == typeof(DateTime))
                {
                    var value = (DateTime) (object) arg;
                    return value.ToString(Culture);
                }  
                else if (typeof(T) == typeof(DateTime?))
                {
                    var value = (DateTime?) (object) arg;
                    if (value == null)
                        return null;
                    return value.Value.ToString(Culture);
                }               
                else if (typeof(T) == typeof(TimeSpan))
                {
                    var value = (TimeSpan) (object) arg;
                    return value.ToString();
                }  
                else if (typeof(T) == typeof(TimeSpan?))
                {
                    var value = (TimeSpan?) (object) arg;
                    if (value == null)
                        return null;
                    return value.Value.ToString();
                }               
                return arg.ToString( );
            }
            catch (FormatException e)
            {
                OnError?.Invoke("Conversion error: "+e.Message);
                return null;
            }
        }
    }
}