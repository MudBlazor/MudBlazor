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
        /// Executes the <see cref="Task"/> asynchronously as a fire-and-forget operation and forwards any exceptions to <see cref="MudGlobal.UnhandledExceptionHandler"/>.
        /// </summary>
        /// <param name="task">The task to be executed.</param>
        /// <param name="ignoreExceptions">If set to true, exceptions are ignored; otherwise, exceptions are forwarded to the global exception handler.</param>
        public static async void CatchAndLog(this Task task, bool ignoreExceptions = false)
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
        /// Executes the <see cref="ValueTask"/> asynchronously as a fire-and-forget operation and forwards any exceptions to <see cref="MudGlobal.UnhandledExceptionHandler"/>.
        /// </summary>
        /// <param name="task">The task to be executed.</param>
        /// <param name="ignoreExceptions">If set to true, exceptions are ignored; otherwise, exceptions are forwarded to the global exception handler.</param>
        public static async void CatchAndLog(this ValueTask task, bool ignoreExceptions = false)
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
        /// Executes the <see cref="ValueTask{T}"/> asynchronously as a fire-and-forget operation and forwards any exceptions to <see cref="MudGlobal.UnhandledExceptionHandler"/>.
        /// </summary>
        /// <param name="task">The task to be executed.</param>
        /// <param name="ignoreExceptions">If set to true, exceptions are ignored; otherwise, exceptions are forwarded to the global exception handler.</param>
        public static async void CatchAndLog<T>(this ValueTask<T> task, bool ignoreExceptions = false)
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
