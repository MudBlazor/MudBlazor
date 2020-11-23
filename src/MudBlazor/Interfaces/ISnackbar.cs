//Copyright(c) Alessandro Ghidini.All rights reserved.
//Changes and improvements Copyright (c) The MudBlazor Team.

using System;
using System.Collections.Generic;

namespace MudBlazor
{
    public interface ISnackbar : IDisposable
    {
        IEnumerable<Snackbar> ShownSnackbars { get; }

        SnackbarConfiguration Configuration { get; }
        
        event Action OnSnackbarsUpdated;

        void Add(string message, Severity severity = Severity.Normal, Action<SnackbarOptions> configure = null);

        void AddNew(Severity severity, string message, Action<SnackbarOptions> configure);

        void Clear();

        void Remove(Snackbar snackbar);
    }
}