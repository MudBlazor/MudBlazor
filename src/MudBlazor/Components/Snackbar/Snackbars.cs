// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MudBlazor
{
    /// <inheritdoc />
    internal class Snackbars : ISnackbar
    {
        public SnackbarConfiguration Configuration { get; }
        public event Action OnSnackbarsUpdated;

        private ReaderWriterLockSlim SnackBarLock { get; }
        private IList<Snackbar> SnackBarList { get; }

        public Snackbars(SnackbarConfiguration configuration)
        {
            Configuration = configuration;
            Configuration.OnUpdate += ConfigurationUpdated;

            SnackBarLock = new ReaderWriterLockSlim();
            SnackBarList = new List<Snackbar>();
        }

        public void Default(string message, Action<SnackbarOptions> configure = null)
        {
            Add(SnackbarType.Default, message, configure);
        }

        public void Info(string message, Action<SnackbarOptions> configure = null)
        {
            Add(SnackbarType.Info, message, configure);
        }

        public void Success(string message, Action<SnackbarOptions> configure = null)
        {
            Add(SnackbarType.Success, message, configure);
        }

        public void Warning(string message, Action<SnackbarOptions> configure = null)
        {
            Add(SnackbarType.Warning, message, configure);
        }

        public void Error(string message, Action<SnackbarOptions> configure = null)
        {
            Add(SnackbarType.Error, message, configure);
        }

        public IEnumerable<Snackbar> ShownSnackbars
        {
            get
            {
                SnackBarLock.EnterReadLock();
                try
                {
                    return SnackBarList.Take(Configuration.MaxDisplayedSnackbars);
                }
                finally
                {
                    SnackBarLock.ExitReadLock();
                }
            }
        }

        public void Add(SnackbarType type, string message, Action<SnackbarOptions> configure)
        {
            if (message.IsEmpty()) return;

            message = message.Trimmed();

            var options = new SnackbarOptions(type, Configuration);
            configure?.Invoke(options);

            var toast = new Snackbar(message, options);

            SnackBarLock.EnterWriteLock();
            try
            {
                if (Configuration.PreventDuplicates && ToastAlreadyPresent(toast)) return;
                toast.OnClose += Remove;
                SnackBarList.Add(toast);
            }
            finally
            {
                SnackBarLock.ExitWriteLock();
            }

            OnSnackbarsUpdated?.Invoke();
        }

        public void Clear()
        {
            SnackBarLock.EnterWriteLock();
            try
            {
                RemoveAllToasts(SnackBarList);
            }
            finally
            {
                SnackBarLock.ExitWriteLock();
            }

            OnSnackbarsUpdated?.Invoke();
        }

        public void Remove(Snackbar toast)
        {
            toast.Dispose();
            toast.OnClose -= Remove;

            SnackBarLock.EnterWriteLock();
            try
            {
                var index = SnackBarList.IndexOf(toast);
                if (index < 0) return;
                SnackBarList.RemoveAt(index);
            }
            finally
            {
                SnackBarLock.ExitWriteLock();
            }

            OnSnackbarsUpdated?.Invoke();
        }

        private bool ToastAlreadyPresent(Snackbar newToast)
        {
            return SnackBarList.Any(toast =>
                newToast.Message.Equals(toast.Message) &&
                newToast.Type.Equals(toast.Type)
            );
        }

        private void ConfigurationUpdated()
        {
            OnSnackbarsUpdated?.Invoke();
        }

        public void Dispose()
        {
            Configuration.OnUpdate -= ConfigurationUpdated;
            RemoveAllToasts(SnackBarList);
        }

        private void RemoveAllToasts(IEnumerable<Snackbar> toasts)
        {
            if (SnackBarList.Count == 0) return;

            foreach (var toast in toasts)
            {
                toast.OnClose -= Remove;
                toast.Dispose();
            }

            SnackBarList.Clear();
        }
    }
}