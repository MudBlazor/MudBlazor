// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// Changes and improvements Copyright (c) The MudBlazor Team

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudSnackbarProvider : MudComponentBase, IDisposable
    {
        [Inject] private ISnackbar Snackbars { get; set; }

        protected IEnumerable<Snackbar> Snackbar => Snackbars.Configuration.NewestOnTop
                ? Snackbars.ShownSnackbars.Reverse()
                : Snackbars.ShownSnackbars;

        protected string Classname => 
            new CssBuilder(Class)
            .AddClass(Snackbars.Configuration.PositionClass)
        .Build();

        protected override void OnInitialized()
        {
            base.OnInitialized();
            Snackbars.OnSnackbarsUpdated += OnSnackbarsUpdated;
        }

        private void OnSnackbarsUpdated()
        {
            InvokeAsync(StateHasChanged);
        }

        public void Dispose()
        {
            Snackbars.OnSnackbarsUpdated -= OnSnackbarsUpdated;
        }
    }
}
