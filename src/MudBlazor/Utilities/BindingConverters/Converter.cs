using System;

namespace MudBlazor
{
    public class Converter<T, U>
    {
        public Func<T, U> SetFunc { get; set; } 
        public Func<U, T> GetFunc { get; set; }

        public U Set(T value)
        {
            if (SetFunc==null)
                return default(U);
            return SetFunc(value);
        }
        
        public T Get(U value)
        {
            if (GetFunc==null)
                return default(T);
            return GetFunc(value);
        }
    }

    public class Converter<T> : Converter<T, string>
    {
        
    }
}