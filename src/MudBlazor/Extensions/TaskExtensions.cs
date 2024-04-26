// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;

namespace MudBlazor
{
#nullable enable
#pragma warning disable CS1998
    public static class TaskExtensions
    {
        /// <summary>
        /// Task will be awaited and exceptions will be forwarded to MudBlazorGlobal.UnhandledExceptionHandler.
        /// </summary>
        public static async void AndForget(this Task task, bool ignoreExceptions = false)
        {
            try
            {
                await task;
            }
            catch (Exception ex)
            {
                if (!ignoreExceptions)
                {
                    MudGlobal.UnhandledExceptionHandler?.Invoke(ex);
                }
            }
        }

        /// <summary>
        /// ValueTask will be awaited and exceptions will be forwarded to MudBlazorGlobal.UnhandledExceptionHandler.
        /// </summary>
        public static async void AndForget(this ValueTask task, bool ignoreExceptions = false)
        {
            try
            {
                await task;
            }
            catch (Exception ex)
            {
                if (!ignoreExceptions)
                {
                    MudGlobal.UnhandledExceptionHandler?.Invoke(ex);
                }
            }
        }

        /// <summary>
        /// ValueTask(bool) will be awaited and exceptions will be forwarded to MudBlazorGlobal.UnhandledExceptionHandler.
        /// </summary>
        public static async void AndForget<T>(this ValueTask<T> task, bool ignoreExceptions = false)
        {
            try
            {
                await task;
            }
            catch (Exception ex)
            {
                if (!ignoreExceptions)
                {
                    MudGlobal.UnhandledExceptionHandler?.Invoke(ex);
                }
            }
        }
    }
}
