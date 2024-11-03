//Copyright (c) 2019 Alessandro Ghidini.All rights reserved.
//Copyright (c) 2020 Jonny Larson and Meinrad Recheis

using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.Extensions.Options;
using MudBlazor.Components.Snackbar;
using MudBlazor.Components.Snackbar.InternalComponents;

#nullable enable

namespace MudBlazor
{
    /// <inheritdoc />
    public class SnackbarService : ISnackbar
    {
        private readonly List<Snackbar> _snackBarList;
        private readonly ReaderWriterLockSlim _snackBarLock;
        private readonly NavigationManager _navigationManager;

        public SnackbarConfiguration Configuration { get; }

        public event Action? OnSnackbarsUpdated;

        public SnackbarService(NavigationManager navigationManager, IOptions<SnackbarConfiguration>? configuration = null)
        {
            _navigationManager = navigationManager;
            Configuration = configuration?.Value ?? new SnackbarConfiguration();
            Configuration.OnUpdate += ConfigurationUpdated;
            navigationManager.LocationChanged += NavigationManager_LocationChanged;

            _snackBarLock = new ReaderWriterLockSlim();
            _snackBarList = new List<Snackbar>();
        }

        public IEnumerable<Snackbar> ShownSnackbars
        {
            get
            {
                _snackBarLock.EnterReadLock();
                try
                {
                    return _snackBarList.Take(Configuration.MaxDisplayedSnackbars);
                }
                finally
                {
                    _snackBarLock.ExitReadLock();
                }
            }
        }

        /// <inheritdoc />
        public Snackbar? Add<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(Dictionary<string, object>? componentParameters = null, Severity severity = Severity.Normal, Action<SnackbarOptions>? configure = null, string? key = null) where T : IComponent
        {
            var type = typeof(T);
            var message = new SnackbarMessage(type, componentParameters, key);

            return AddCore(message, severity, configure);
        }

        /// <inheritdoc />
        public Snackbar? Add(RenderFragment message, Severity severity = Severity.Normal, Action<SnackbarOptions>? configure = null, string? key = null)
        {

            var componentParams = new Dictionary<string, object>()
            {
                { "Message", message }
            };

            return Add<SnackbarMessageRenderFragment>
            (
                componentParams,
                severity,
                configure,
                key
            );
        }

        /// <inheritdoc />
        public Snackbar? Add(MarkupString message, Severity severity = Severity.Normal, Action<SnackbarOptions>? configure = null, string? key = null)
        {
            if (message.ToString().IsEmpty()) return null;

            var componentParams = new Dictionary<string, object>() { { "Message", message } };
            var keyToUse = string.IsNullOrEmpty(key) ? message.ToString() : key;

            return Add<SnackbarMessageMarkupString>(componentParams, severity, configure, keyToUse);
        }

        /// <inheritdoc />
        public Snackbar? Add(string message, Severity severity = Severity.Normal, Action<SnackbarOptions>? configure = null, string? key = null)
        {
            if (message.IsEmpty()) return null;
            message = message.Trimmed();

            var componentParams = new Dictionary<string, object>() { { "Message", message } };

            return AddCore<SnackbarMessageText>(message, componentParams, severity, configure, string.IsNullOrEmpty(key) ? message : key);
        }

        /// <inheritdoc />
        public void Clear()
        {
            _snackBarLock.EnterWriteLock();
            try
            {
                RemoveAllSnackbars(_snackBarList);
            }
            finally
            {
                _snackBarLock.ExitWriteLock();
            }

            OnSnackbarsUpdated?.Invoke();
        }

        /// <inheritdoc />
        public void Remove(Snackbar snackbar)
        {
            snackbar.OnClose -= Remove;
            snackbar.Dispose();

            _snackBarLock.EnterWriteLock();
            try
            {
                var index = _snackBarList.IndexOf(snackbar);
                if (index < 0) return;
                _snackBarList.RemoveAt(index);
            }
            finally
            {
                _snackBarLock.ExitWriteLock();
            }

            OnSnackbarsUpdated?.Invoke();
        }

        /// <inheritdoc />
        public void RemoveByKey(string key)
        {
            _snackBarLock.EnterWriteLock();
            try
            {
                var snackbars = _snackBarList.Where(snackbar => snackbar.SnackbarMessage.Key == key).ToArray();
                foreach (var snackbar in snackbars)
                {
                    snackbar.OnClose -= Remove;
                    snackbar.Dispose();
                    _snackBarList.Remove(snackbar);
                }
            }
            finally
            {
                _snackBarLock.ExitWriteLock();
            }

            OnSnackbarsUpdated?.Invoke();
        }

        private Snackbar? AddCore<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(string text, Dictionary<string, object>? componentParameters = null, Severity severity = Severity.Normal, Action<SnackbarOptions>? configure = null, string key = "") where T : IComponent
        {
            var type = typeof(T);
            var message = new SnackbarMessage(type, componentParameters, key) { Text = text };

            return AddCore(message, severity, configure);
        }

        private Snackbar? AddCore(SnackbarMessage message, Severity severity = Severity.Normal, Action<SnackbarOptions>? configure = null)
        {
            var options = new SnackbarOptions(severity, Configuration);
            configure?.Invoke(options);

            var snackbar = new Snackbar(message, options);

            _snackBarLock.EnterWriteLock();
            try
            {
                if (ResolvePreventDuplicates(options) && SnackbarAlreadyPresent(snackbar)) return null;
                snackbar.OnClose += Remove;
                _snackBarList.Add(snackbar);
            }
            finally
            {
                _snackBarLock.ExitWriteLock();
            }

            OnSnackbarsUpdated?.Invoke();

            return snackbar;
        }

        private bool ResolvePreventDuplicates(SnackbarOptions options)
        {
            return options.DuplicatesBehavior == SnackbarDuplicatesBehavior.Prevent
                    || (options.DuplicatesBehavior == SnackbarDuplicatesBehavior.GlobalDefault && Configuration.PreventDuplicates);
        }

        private bool SnackbarAlreadyPresent(Snackbar newSnackbar)
        {
            return !string.IsNullOrEmpty(newSnackbar.SnackbarMessage.Key) && _snackBarList.Any(snackbar => newSnackbar.SnackbarMessage.Key == snackbar.SnackbarMessage.Key);
        }

        private void ConfigurationUpdated()
        {
            OnSnackbarsUpdated?.Invoke();
        }

        private void NavigationManager_LocationChanged(object? sender, LocationChangedEventArgs e)
        {
            if (Configuration.ClearAfterNavigation)
            {
                Clear();
            }
            else
            {
                var snackbarsToRemove = ShownSnackbars.Where(s => s.State.Options.CloseAfterNavigation).ToArray();
                foreach (var snackbar in snackbarsToRemove)
                {
                    Remove(snackbar);
                }
            }
        }


        private void RemoveAllSnackbars(IEnumerable<Snackbar> snackbars)
        {
            if (_snackBarList.Count == 0) return;

            foreach (var snackbar in snackbars)
            {
                snackbar.OnClose -= Remove;
                snackbar.Dispose();
            }

            _snackBarList.Clear();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Configuration.OnUpdate -= ConfigurationUpdated;
                _navigationManager.LocationChanged -= NavigationManager_LocationChanged;
                RemoveAllSnackbars(_snackBarList);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
