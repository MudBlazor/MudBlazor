using System;
using System.Collections.Generic;
using System.Text;

namespace MudBlazor.Extensions
{
    public static class ObjectExtensions
    {

        public static T As<T>(this object self)
        {
            if (self == null || !(self is T))
                return default(T);
            return (T) self;
        }
    }
}
