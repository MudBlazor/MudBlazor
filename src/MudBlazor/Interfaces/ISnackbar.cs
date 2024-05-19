//Copyright(c) Alessandro Ghidini.All rights reserved.
//Changes and improvements Copyright (c) The MudBlazor Team.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;

namespace MudBlazor
{
    public interface ISnackbar : IDisposable
    {
        IEnumerable<Snackbar> ShownSnackbars { get; }

        SnackbarConfiguration Configuration { get; }

        event Action OnSnackbarsUpdated;

        Snackbar Add(string message, Severity severity = Severity.Normal, Action<SnackbarOptions> configure = null, string key = "");
        Snackbar Add(MarkupString message, Severity severity = Severity.Normal, Action<SnackbarOptions> configure = null, string key = "");
        Snackbar Add(RenderFragment message, Severity severity = Severity.Normal, Action<SnackbarOptions> configure = null, string key = "");
        Snackbar Add<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(Dictionary<string, object> componentParameters = null, Severity severity = Severity.Normal, Action<SnackbarOptions> configure = null, string key = "") where T : IComponent;

        void Clear();

        void Remove(Snackbar snackbar);

        void RemoveByKey(string key);
    }
}
