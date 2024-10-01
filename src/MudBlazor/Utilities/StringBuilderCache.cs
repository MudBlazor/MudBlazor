// Copyright (c) Microsoft Corporation.  All rights reserved.
// License: MIT
// See https://github.com/microsoft/referencesource/blob/master/mscorlib/system/text/stringbuildercache.cs
/*============================================================
**
** Class:  StringBuilderCache
**
** Purpose: provide a cached reusable instance of StringBuilder
**          per thread  it's an optimisation that reduces the 
**          number of instances constructed and collected.
**
**  Acquire - is used to get a string builder to use of a 
**            particular size.  It can be called any number of 
**            times, if a StringBuilder is in the cache then
**            it will be returned and the cache emptied.
**            subsequent calls will return a new StringBuilder.
**
**            A StringBuilder instance is cached in 
**            Thread Local Storage and so there is one per thread
**
**  Release - Place the specified builder in the cache if it is 
**            not too big.
**            The StringBuilder should not be used after it has 
**            been released.
**            Unbalanced Releases are perfectly acceptable.  It
**            will merely cause the runtime to create a new 
**            StringBuilder next time Acquire is called.
**
**  GetStringAndRelease
**          - ToString() the StringBuilder, Release it to the 
**            cache and return the resulting string
**
===========================================================*/

namespace System.Text
{
#nullable enable
    internal static class StringBuilderCache
    {
        // The value 360 was chosen in discussion with performance experts as a compromise between using
        // as little memory (per thread) as possible and still covering a large part of short-lived
        // StringBuilder creations on the startup path of VS designers.
        private const int MaxBuilderSize = 360;

        [ThreadStatic]
        private static StringBuilder? _cachedInstance;

        public static StringBuilder Acquire(int capacity = MaxBuilderSize)
        {
            if (capacity <= MaxBuilderSize)
            {
                var sb = _cachedInstance;
                if (sb is not null)
                {
                    // Avoid StringBuilder block fragmentation by getting a new StringBuilder
                    // when the requested size is larger than the current capacity
                    if (capacity <= sb.Capacity)
                    {
                        _cachedInstance = null;
                        sb.Clear();
                        return sb;
                    }
                }
            }
            return new StringBuilder(capacity);
        }

        public static void Release(StringBuilder sb)
        {
            if (sb.Capacity <= MaxBuilderSize)
            {
                _cachedInstance = sb;
            }
        }

        public static string GetStringAndRelease(StringBuilder sb)
        {
            var result = sb.ToString();
            Release(sb);
            return result;
        }
    }
}
