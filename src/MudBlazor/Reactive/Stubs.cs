// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.

// https://github.com/dotnet/reactive/blob/main/Rx.NET/Source/src/System.Reactive/Internal/Stubs.cs
// https://github.com/dotnet/reactive/blob/main/LICENSE

using System.Runtime.ExceptionServices;

// ReSharper disable CheckNamespace
namespace System.Reactive
// ReSharper restore CheckNamespace
{
    internal static class Stubs<T>
    {
        public static readonly Action<T> Ignore = static _ => { };
        public static readonly Func<T, T> I = static _ => _;
    }

    internal static class Stubs
    {
        public static readonly Action Nop = static () => { };
        public static readonly Action<Exception> Throw = e => ExceptionDispatchInfo.Capture(e).Throw();
    }
}
