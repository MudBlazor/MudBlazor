﻿//Copyright (c) 2019 Alessandro Ghidini.All rights reserved.
//Copyright (c) 2020 Jonny Larson and Meinrad Recheis

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
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
        public SnackbarConfiguration Configuration { get; }
        public event Action? OnSnackbarsUpdated;

        private NavigationManager _navigationManager;
        private ReaderWriterLockSlim SnackBarLock { get; }
        private List<Snackbar> SnackBarList { get; }

        public SnackbarService(NavigationManager navigationManager, IOptions<SnackbarConfiguration>? configuration = null)
        {
            _navigationManager = navigationManager;
            Configuration = configuration?.Value ?? new SnackbarConfiguration();
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

        private Snackbar? Add(SnackbarMessage message, Severity severity = Severity.Normal, Action<SnackbarOptions>? configure = null)
        {
            var options = new SnackbarOptions(severity, Configuration);
            configure?.Invoke(options);

            var snackbar = new Snackbar(message, options);

            SnackBarLock.EnterWriteLock();
            try
            {
                if (ResolvePreventDuplicates(options) && SnackbarAlreadyPresent(snackbar)) return null;
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
        
        /// <inheritdoc />
        public Snackbar? Add<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(Dictionary<string, object>? componentParameters = null, Severity severity = Severity.Normal, Action<SnackbarOptions>? configure = null, string? key = null) where T : IComponent
        {
            var type = typeof(T);
            var message = new SnackbarMessage(type, componentParameters, key);

            return Add(message, severity, configure);
        }

        /// <inheritdoc />
        public Snackbar? Add(RenderFragment message, Severity severity = Severity.Normal, Action<SnackbarOptions>? configure = null, string? key = null)
        {

            var componentParams = new Dictionary<string, object>()
            {
                { "Message", message as object }
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

        /// <inheritdoc />
        public void Remove(Snackbar snackbar)
        {
            snackbar.OnClose -= Remove;
            snackbar.Dispose();

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

        /// <inheritdoc />
        public void RemoveByKey(string key)
        {
            SnackBarLock.EnterWriteLock();
            try
            {
                var snackbars = SnackBarList.Where(snackbar => snackbar.SnackbarMessage.Key == key).ToArray();
                foreach (var snackbar in snackbars)
                {
                    snackbar.OnClose -= Remove;
                    snackbar.Dispose();
                    SnackBarList.Remove(snackbar);
                }
            }
            finally
            {
                SnackBarLock.ExitWriteLock();
            }

            OnSnackbarsUpdated?.Invoke();
        }

        private Snackbar? AddCore<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(string text, Dictionary<string, object>? componentParameters = null, Severity severity = Severity.Normal, Action<SnackbarOptions>? configure = null, string key = "") where T : IComponent
        {
            var type = typeof(T);
            var message = new SnackbarMessage(type, componentParameters, key) { Text = text };

            return Add(message, severity, configure);
        }

        private bool ResolvePreventDuplicates(SnackbarOptions options)
        {
            return options.DuplicatesBehavior == SnackbarDuplicatesBehavior.Prevent
                    || (options.DuplicatesBehavior == SnackbarDuplicatesBehavior.GlobalDefault && Configuration.PreventDuplicates);
        }

        private bool SnackbarAlreadyPresent(Snackbar newSnackbar)
        {
            return !string.IsNullOrEmpty(newSnackbar.SnackbarMessage.Key) && SnackBarList.Any(snackbar => newSnackbar.SnackbarMessage.Key == snackbar.SnackbarMessage.Key);
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
                ShownSnackbars.Where(s => s.State.Options.CloseAfterNavigation).ToList().ForEach(s => Remove(s));
            }
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

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                SnackBarLock.Dispose();
                Configuration.OnUpdate -= ConfigurationUpdated;
                _navigationManager.LocationChanged -= NavigationManager_LocationChanged;
                RemoveAllSnackbars(SnackBarList);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
