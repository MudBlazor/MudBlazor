using System;
using System.Diagnostics.CodeAnalysis;

namespace MudBlazor
{

    [ExcludeFromCodeCoverage]
    public class ScrollManagerException : Exception
    {
        public ScrollManagerException() { }
        public ScrollManagerException(string message) : base(message) { }
        public ScrollManagerException(string message, Exception inner) : base(message, inner) { }
    }
}
