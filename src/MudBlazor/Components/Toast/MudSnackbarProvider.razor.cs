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
    public partial class MudToastProvider : MudComponentBase, IDisposable
    {
        [Inject] private ISnackbar Toaster { get; set; }

        protected IEnumerable<Snackbar> Toasts => Toaster.Configuration.NewestOnTop
                ? Toaster.ShownToasts.Reverse()
                : Toaster.ShownToasts;

        protected string Classname => 
            new CssBuilder(Class)
            .AddClass( Toaster.Configuration.PositionClass)
        .Build();

        protected override void OnInitialized()
        {
            base.OnInitialized();
            Toaster.OnToastsUpdated += OnToastsUpdated;
        }

        private void OnToastsUpdated()
        {
            InvokeAsync(StateHasChanged);
        }

        public void Dispose()
        {
            Toaster.OnToastsUpdated -= OnToastsUpdated;
        }
    }
}
