using System;

namespace MudBlazor.Utilities.Exceptions
{
    public class GenericTypeMismatchException : Exception
    {
        public GenericTypeMismatchException(string parent, string child, Type t1, Type t2) : base($"{parent}<{t1.Name}> has a child {child}<{t2}> with mismatching generic type.")
        {
        }
    }
}
