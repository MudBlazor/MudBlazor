//Copyright (c) 2019 Alessandro Ghidini.All rights reserved.
//Copyright (c) 2020 Jonny Larson and Meinrad Recheis

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace MudBlazor
{
    /// <inheritdoc />
    public class SnackbarService : ISnackbar, IDisposable
    {
        public SnackbarConfiguration Configuration { get; }
        public event Action OnSnackbarsUpdated;

        private NavigationManager _navigationManager;
        private ReaderWriterLockSlim SnackBarLock { get; }
        private IList<Snackbar> SnackBarList { get; }

        public SnackbarService(NavigationManager navigationManager, SnackbarConfiguration configuration = null)
        {
            _navigationManager = navigationManager;
            configuration ??= new SnackbarConfiguration();

            Configuration = configuration;
            Configuration.OnUpdate += ConfigurationUpdated;
            navigationManager.LocationChanged += NavigationManager_LocationChanged;

            SnackBarLock = new ReaderWriterLockSlim();
            SnackBarList = new List<Snackbar>();
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

        [Obsolete("Use Add instead.", true)]
        public Snackbar AddNew(Severity severity, string message, Action<SnackbarOptions> configure)
        {
            return Add(message, severity, configure);
        }

        public Snackbar Add(string message, Severity severity = Severity.Normal, Action<SnackbarOptions> configure = null)
        {
            if (message.IsEmpty()) return null;

            message = message.Trimmed();

            var options = new SnackbarOptions(severity, Configuration);
            configure?.Invoke(options);

            var snackbar = new Snackbar(message, options);

            SnackBarLock.EnterWriteLock();
            try
            {
                if (Configuration.PreventDuplicates && SnackbarAlreadyPresent(snackbar)) return null;
                snackbar.OnClose += Remove;
                SnackBarList.Add(snackbar);
            }
            finally
            {
                SnackBarLock.ExitWriteLock();
            }

            OnSnackbarsUpdated?.Invoke();

            return snackbar;
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

        private void NavigationManager_LocationChanged(object sender, LocationChangedEventArgs e)
        {
            if (Configuration.ClearAfterNavigation)
            {
                Clear();
            }
            else
            {
                ShownSnackbars.Where(s => s.State.Options.CloseAfterNavigation).ToList().ForEach(s => Remove(s));
            }
        }

        public void Dispose()
        {
            Configuration.OnUpdate -= ConfigurationUpdated;
            _navigationManager.LocationChanged -= NavigationManager_LocationChanged;
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
