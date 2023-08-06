// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudBlazor.Extensions;

namespace MudBlazor
{
    public static class MudGlobal
    {
        /// <summary>
        /// Global unhandled exception handler for such exceptions which can not be bubbled up. Note: this is not a global catch-all.
        /// It just allows the user to handle such exceptions which were suppressed inside MudBlazor using Task.AndForget() in places
        /// where it is impossible to await the task. Exceptions in user code or in razor files will still crash your app if you are not carefully
        /// handling everything with <ErrorBoundary></ErrorBoundary>.
        /// </summary>
        public static Action<Exception> UnhandledExceptionHandler { get; set; } = OnDefaultExceptionHandler;

        /// <summary>
        /// Note: the user can overwrite this default handler with their own implementation. The default implementation
        /// makes sure that the unhandled exceptions don't go unnoticed
        /// </summary>
        private static void OnDefaultExceptionHandler(Exception ex)
        {
            Console.Write(ex);
        }
    }
}
