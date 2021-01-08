using System;
using System.Threading.Tasks;

namespace MudBlazor
{
    internal static class TaskExtensions
    {
        public static void FireAndForget(this Task task)
        {
            task.FireAndForget(ex => Console.WriteLine(ex));
        }

        public static async void FireAndForget(this Task task, Action<Exception> handler)
        {
            try
            {
                await task;
            }
            catch(Exception ex)
            {
                handler(ex);
            }
        }

        public static void FireAndForget(this ValueTask task)
        {
            task.FireAndForget(ex => Console.WriteLine(ex));
        }

        public static async void FireAndForget(this ValueTask task, Action<Exception> handler)
        {
            try
            {
                await task;
            }
            catch (Exception ex)
            {
                handler(ex);
            }
        }
    }
}
