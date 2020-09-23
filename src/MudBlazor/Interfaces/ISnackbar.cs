// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace MudBlazor
{
    public interface ISnackbar : IDisposable
    {
        IEnumerable<Snackbar> ShownSnackbars { get; }

        SnackbarConfiguration Configuration { get; }
        
        event Action OnSnackbarsUpdated;
        
        void Info(string message, string title = null, Action<SnackbarOptions> configure = null);

        void Success(string message, string title = null, Action<SnackbarOptions> configure = null);
        
        void Warning(string message, string title = null, Action<SnackbarOptions> configure = null);

        void Error(string message, string title = null, Action<SnackbarOptions> configure = null);

        void Add(SnackbarType type, string message, string title, Action<SnackbarOptions> configure);

        void Clear();

        void Remove(Snackbar snackbar);
    }
}