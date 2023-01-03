using System;
using System.Diagnostics.CodeAnalysis;

namespace MudBlazor
{

    [ExcludeFromCodeCoverage]
    [Serializable]
    public class ScrollManagerException : Exception
    {
        public ScrollManagerException() : base() { }
        public ScrollManagerException(string message) : base(message) { }
        public ScrollManagerException(string message, Exception inner) : base(message, inner) { }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client.
        protected ScrollManagerException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
