using System;
using System.Threading.Tasks;

namespace MudBlazor
{
#nullable enable
    [Obsolete("This will be removed in v7.")]
    public enum TaskOption
    {
        None,
        Safe
    }

    public static class TaskExtensions
    {
        [Obsolete("Use the bool parameter version. This will be removed in v7.")]
        public static void AndForget(this Task task, TaskOption option) => AndForget(task);

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
                    MudGlobal.UnhandledExceptionHandler?.Invoke(ex);
            }
        }

        [Obsolete("Use the bool parameter version. This will be removed in v7.")]
        public static async void AndForget(this ValueTask task, TaskOption option) => AndForget(task);

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
                    MudGlobal.UnhandledExceptionHandler?.Invoke(ex);
            }
        }

        [Obsolete("Use the bool parameter version. This will be removed in v7.")]
        public static async void AndForget<T>(this ValueTask<T> task, TaskOption option) => AndForget(task, option);

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
                    MudGlobal.UnhandledExceptionHandler?.Invoke(ex);
            }
        }
    }
}
