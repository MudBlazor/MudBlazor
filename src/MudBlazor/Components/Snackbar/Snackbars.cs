//Copyright(c) Alessandro Ghidini.All rights reserved.
//Changes and improvements Copyright (c) The MudBlazor Team.

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

        public void Add(string message, Severity severity = Severity.Normal, Action<SnackbarOptions> configure = null)
        {
            AddNew(severity, message, configure);
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

        public void AddNew(Severity severity, string message, Action<SnackbarOptions> configure)
        {
            if (message.IsEmpty()) return;

            message = message.Trimmed();

            var options = new SnackbarOptions(severity, Configuration);
            configure?.Invoke(options);

            var snackbar = new Snackbar(message, options);

            SnackBarLock.EnterWriteLock();
            try
            {
                if (Configuration.PreventDuplicates && SnackbarAlreadyPresent(snackbar)) return;
                snackbar.OnClose += Remove;
                SnackBarList.Add(snackbar);
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
                RemoveAllSnackbars(SnackBarList);
            }
            finally
            {
                SnackBarLock.ExitWriteLock();
            }

            OnSnackbarsUpdated?.Invoke();
        }

        public void Remove(Snackbar snackbar)
        {
            snackbar.Dispose();
            snackbar.OnClose -= Remove;

            SnackBarLock.EnterWriteLock();
            try
            {
                var index = SnackBarList.IndexOf(snackbar);
                if (index < 0) return;
                SnackBarList.RemoveAt(index);
            }
            finally
            {
                SnackBarLock.ExitWriteLock();
            }

            OnSnackbarsUpdated?.Invoke();
        }

        private bool SnackbarAlreadyPresent(Snackbar newSnackbar)
        {
            return SnackBarList.Any(snackbar =>
                newSnackbar.Message.Equals(snackbar.Message) &&
                newSnackbar.Severity.Equals(snackbar.Severity)
            );
        }

        private void ConfigurationUpdated()
        {
            OnSnackbarsUpdated?.Invoke();
        }

        public void Dispose()
        {
            Configuration.OnUpdate -= ConfigurationUpdated;
            RemoveAllSnackbars(SnackBarList);
        }

        private void RemoveAllSnackbars(IEnumerable<Snackbar> snackbars)
        {
            if (SnackBarList.Count == 0) return;

            foreach (var snackbar in snackbars)
            {
                snackbar.OnClose -= Remove;
                snackbar.Dispose();
            }

            SnackBarList.Clear();
        }
    }
}