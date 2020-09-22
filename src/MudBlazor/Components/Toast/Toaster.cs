// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MudBlazor
{
    /// <inheritdoc />
    internal class Toaster : IToaster
    {
        public ToasterConfiguration Configuration { get; }
        public event Action OnToastsUpdated;
        //public string Version { get; }

        private ReaderWriterLockSlim ToastLock { get; }
        private IList<Toast> Toasts { get; }

        public Toaster(ToasterConfiguration configuration)
        {
            Configuration = configuration;
            Configuration.OnUpdate += ConfigurationUpdated;

            ToastLock = new ReaderWriterLockSlim();
            Toasts = new List<Toast>();
            //Version = GetType().InformationalVersion();
        }

        public void Info(string message, string title = null, Action<ToastOptions> configure = null)
        {
            Add(ToastType.Info, message, title, configure);
        }

        public void Success(string message, string title = null, Action<ToastOptions> configure = null)
        {
            Add(ToastType.Success, message, title, configure);
        }

        public void Warning(string message, string title = null, Action<ToastOptions> configure = null)
        {
            Add(ToastType.Warning, message, title, configure);
        }

        public void Error(string message, string title = null, Action<ToastOptions> configure = null)
        {
            Add(ToastType.Error, message, title, configure);
        }

        public IEnumerable<Toast> ShownToasts
        {
            get
            {
                ToastLock.EnterReadLock();
                try
                {
                    return Toasts.Take(Configuration.MaxDisplayedToasts);
                }
                finally
                {
                    ToastLock.ExitReadLock();
                }
            }
        }

        public void Add(ToastType type, string message, string title, Action<ToastOptions> configure)
        {
            if (message.IsEmpty()) return;

            message = message.Trimmed();
            title = title.Trimmed();

            var options = new ToastOptions(type, Configuration);
            configure?.Invoke(options);

            var toast = new Toast(title, message, options);

            ToastLock.EnterWriteLock();
            try
            {
                if (Configuration.PreventDuplicates && ToastAlreadyPresent(toast)) return;
                toast.OnClose += Remove;
                Toasts.Add(toast);
            }
            finally
            {
                ToastLock.ExitWriteLock();
            }

            OnToastsUpdated?.Invoke();
        }

        public void Clear()
        {
            ToastLock.EnterWriteLock();
            try
            {
                RemoveAllToasts(Toasts);
            }
            finally
            {
                ToastLock.ExitWriteLock();
            }

            OnToastsUpdated?.Invoke();
        }

        public void Remove(Toast toast)
        {
            toast.Dispose();
            toast.OnClose -= Remove;

            ToastLock.EnterWriteLock();
            try
            {
                var index = Toasts.IndexOf(toast);
                if (index < 0) return;
                Toasts.RemoveAt(index);
            }
            finally
            {
                ToastLock.ExitWriteLock();
            }

            OnToastsUpdated?.Invoke();
        }

        private bool ToastAlreadyPresent(Toast newToast)
        {
            return Toasts.Any(toast =>
                newToast.Message.Equals(toast.Message) &&
                newToast.Title.Equals(toast.Title) &&
                newToast.Type.Equals(toast.Type)
            );
        }

        private void ConfigurationUpdated()
        {
            OnToastsUpdated?.Invoke();
        }

        public void Dispose()
        {
            Configuration.OnUpdate -= ConfigurationUpdated;
            RemoveAllToasts(Toasts);
        }

        private void RemoveAllToasts(IEnumerable<Toast> toasts)
        {
            if (Toasts.Count == 0) return;

            foreach (var toast in toasts)
            {
                toast.OnClose -= Remove;
                toast.Dispose();
            }

            Toasts.Clear();
        }
    }
}