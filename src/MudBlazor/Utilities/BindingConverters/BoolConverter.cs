using System;
using System.Globalization;

namespace MudBlazor
{
    
    /// <summary>
    /// A universal T to bool? binding converter
    /// </summary>
    public class BoolConverter<T> : Converter<T, bool?>
    {
       
        public BoolConverter()
        {
            SetFunc = OnSet;
            GetFunc = OnGet;
        }

        private T OnGet(bool? value)
        {
            try
            {
                if (typeof(T) == typeof(bool))
                    return (T)(object)(value == true);
                else if (typeof(T) == typeof(bool?))
                    return (T)(object)value;
                else if (typeof(T) == typeof(string))
                    return (T)(object)(value == true ? "true" : (value == false ? "false" : null));
                else if (typeof(T) == typeof(int))
                    return (T)(object)(value == true ? 1 : 0);
                else if (typeof(T) == typeof(int?))
                    return (T)(object)(value == true ? 1 : (value == false ? (int?)0 : null));
                else
                {
                    UpdateGetError($"Conversion to type {typeof(T)} not implemented");
                }
            }
            catch (Exception e)
            {
                UpdateGetError("Conversion error: "+e.Message);
                return default(T);
            }
            return default(T);
        }

        private bool? OnSet(T arg)
        {
            if (arg == null)
                return null; // <-- this catches all nullable values which are null. no nullchecks necessary below!
            try
            {
                if (arg is bool)
                    return (bool)(object)arg;
                else if (arg is bool?)
                    return (bool?)(object)arg;
                else if (arg is int)
                    return ((int)(object)arg) > 0;
                else if (arg is string)
                {
                    var s = (string)(object)arg;
                    if (string.IsNullOrWhiteSpace(s))
                        return null;
                    if (bool.TryParse(s, out var b))
                        return b;
                    if (s.ToLowerInvariant() == "on")
                        return true;
                    if (s.ToLowerInvariant() == "off")
                        return false;
                    return null;
                }
                else
                {
                    UpdateSetError("Unable to convert to bool? from type " + typeof(T).Name);
                    return null;
                }
            }
            catch (FormatException e)
            {
                UpdateSetError("Conversion error: "+e.Message);
                return null;
            }
        }


        #region --> Floating Point comparison

        const double MinNormal = 2.2250738585072014E-308d;

        public static bool AreEqual(double a, double b, double epsilon = MinNormal)
        {
            // Copyright (c) Michael Borgwardt
            double absA = Math.Abs(a);
            double absB = Math.Abs(b);
            double diff = Math.Abs(a - b);

            if (a.Equals(b))
            { // shortcut, handles infinities
                return true;
            }
            else if (a == 0 || b == 0 || absA + absB < MinNormal)
            {
                // a or b is zero or both are extremely close to it
                // relative error is less meaningful here
                return diff < (epsilon * MinNormal);
            }
            else
            { // use relative error
                return diff / (absA + absB) < epsilon;
            }
        }

        #endregion
    }
}