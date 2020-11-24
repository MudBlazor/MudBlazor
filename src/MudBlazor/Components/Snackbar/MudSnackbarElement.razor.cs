// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// Changes and improvements Copyright (c) The MudBlazor Team

using System;
using Microsoft.AspNetCore.Components;

namespace MudBlazor
{
    public partial class MudSnackbarElement : MudComponentBase, IDisposable
    {
        [Parameter]
        public Snackbar Snackbar { get; set; }
        protected RenderFragment Css;

        protected string Message => Snackbar?.Message;

        protected void Clicked() => Snackbar?.Clicked(false);
        protected void CloseIconClicked() => Snackbar?.Clicked(true);

        protected override void OnInitialized()
        {
            if (Snackbar == null)
                return;
            Snackbar.OnUpdate += SnackbarUpdated;
            Snackbar.Init();

            Css = builder =>
            {
                var transitionClass = Snackbar.State.TransitionClass;
                if (string.IsNullOrWhiteSpace(transitionClass)) 
                    return;
                builder.OpenElement(1, "style");
                builder.AddContent(2, transitionClass);
                builder.CloseElement();
            };
        }

        private void SnackbarUpdated()
        {
            InvokeAsync(StateHasChanged);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || Snackbar == null) 
                return;
            Snackbar.OnUpdate -= SnackbarUpdated;
        }
    }
}