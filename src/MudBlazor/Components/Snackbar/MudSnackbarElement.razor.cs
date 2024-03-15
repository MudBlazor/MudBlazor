// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// Changes and improvements Copyright (c) The MudBlazor Team

using System;
using Microsoft.AspNetCore.Components;
using MudBlazor.Components.Snackbar;
using static System.String;

namespace MudBlazor
{
    public partial class MudSnackbarElement : MudComponentBase, IDisposable
    {
        [Parameter]
        public Snackbar Snackbar { get; set; }

        /// <summary>
        /// Custom close icon.
        /// </summary>
        [Parameter] public string CloseIcon { get; set; } = Icons.Material.Filled.Close;

        // appearance
        private string Action => Snackbar?.State.Options.Action;
        private Color ActionColor => Snackbar?.State.Options.ActionColor ?? Color.Default;
        private Variant ActionVariant => Snackbar?.State.Options.ActionVariant ?? Snackbar?.State.Options.SnackbarVariant ?? Variant.Text;
        private string AnimationStyle => Snackbar?.State.AnimationStyle + Style;
        private string SnackbarClass => Snackbar?.State.SnackbarClass;
        private RenderFragment Css;
        private bool ShowActionButton => Snackbar?.State.ShowActionButton == true;
        private bool ShowCloseIcon => Snackbar?.State.ShowCloseIcon == true;

        // icon
        private bool HideIcon => Snackbar?.State.HideIcon == true;
        private string Icon => Snackbar?.State.Icon;
        private Color IconColor => Snackbar?.State.Options.IconColor ?? Color.Inherit;
        private Size IconSize => Snackbar?.State.Options.IconSize ?? Size.Medium;

        // behavior
        private void ActionClicked() => Snackbar?.Clicked(false);
        private void CloseIconClicked() => Snackbar?.Clicked(true);
        private SnackbarMessage Message => Snackbar?.SnackbarMessage;

        private void SnackbarClicked()
        {
            if (!ShowActionButton)
                Snackbar?.Clicked(false);
        }

        private void SnackbarUpdated()
        {
            InvokeAsync(StateHasChanged);
        }

        protected override void OnInitialized()
        {
            if (Snackbar != null)
            {
                Snackbar.OnUpdate += SnackbarUpdated;
                Snackbar.Init();

                Css = builder =>
                {
                    var transitionClass = Snackbar.State.TransitionClass;

                    if (!IsNullOrWhiteSpace(transitionClass))
                    {
                        builder.OpenElement(1, "style");
                        builder.AddContent(2, transitionClass);
                        builder.CloseElement();
                    }
                };
            }
        }

        protected void InteractionStartHandler()
        {
            // Pause snackbar transitions while the user is interacting through hover or touch.
            Snackbar.PauseTransitions(true);
        }

        protected void InteractionEndHandler()
        {
            // The user is done and we can now resume transitions.
            Snackbar.PauseTransitions(false);
        }

        public void Dispose()
        {
            if (Snackbar != null)
                Snackbar.OnUpdate -= SnackbarUpdated;
        }
    }
}
