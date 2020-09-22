// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace MudBlazor
{
    /// <inheritdoc />
    /// <summary>
    /// Represents an instance of the Toaster engine
    /// </summary>
    public interface IToaster : IDisposable
    {
        /// <summary>
        /// The current list of displayed toasts
        /// </summary>
        IEnumerable<Toast> ShownToasts { get; }

        /// <summary>
        /// The global <see cref="ToasterConfiguration"/> 
        /// </summary>
        ToasterConfiguration Configuration { get; }
        
        /// <summary>
        /// An event raised when the list of toasts changes or a global configuration setting is modified
        /// </summary>
        event Action OnToastsUpdated;
        
        /// <summary>
        /// Displays an info toast
        /// </summary>
        /// <param name="message">The toast main message</param>
        /// <param name="title">The optional toast tile</param>
        /// <param name="configure">An action for configuring a <see cref="ToastOptions"/> instance already containing the globally configured settings</param>
        void Info(string message, string title = null, Action<ToastOptions> configure = null);

        /// <summary>
        /// Displays a success toast
        /// </summary>
        /// <param name="message">The toast main message</param>
        /// <param name="title">The optional toast tile</param>
        /// <param name="configure">An action for configuring a <see cref="ToastOptions"/> instance already containing the globally configured settings</param>
        void Success(string message, string title = null, Action<ToastOptions> configure = null);
        
        /// <summary>
        /// Displays a warning info toast
        /// </summary>
        /// <param name="message">The toast main message</param>
        /// <param name="title">The optional toast tile</param>
        /// <param name="configure">An action for configuring a <see cref="ToastOptions"/> instance already containing the globally configured settings</param>
        void Warning(string message, string title = null, Action<ToastOptions> configure = null);

        /// <summary>
        /// Displays an error info toast
        /// </summary>
        /// <param name="message">The toast main message</param>
        /// <param name="title">The optional toast tile</param>
        /// <param name="configure">An action for configuring a <see cref="ToastOptions"/> instance already containing the globally configured settings</param>
        void Error(string message, string title = null, Action<ToastOptions> configure = null);

        /// <summary>
        /// Displays a toast with the specified <see cref="ToastType" />
        /// </summary>
        /// <param name="type">The toast <see cref="ToastType"/></param>
        /// <param name="message">The toast main message</param>
        /// <param name="title">The optional toast tile</param>
        /// <param name="configure">An action for configuring a <see cref="ToastOptions"/> instance already containing the globally configured settings</param>
        void Add(ToastType type, string message, string title, Action<ToastOptions> configure);

        /// <summary>
        /// Hides all the toasts, including the ones waiting to be displayed
        /// </summary>
        void Clear();

        /// <summary>
        /// Hides the specified <see cref="Toast"/>
        /// </summary>
        /// <param name="toast">The <see cref="Toast"/> to be hidden</param>
        void Remove(Toast toast);
    }
}