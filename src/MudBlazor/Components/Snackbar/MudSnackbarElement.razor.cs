// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// Changes and improvements Copyright (c) The MudBlazor Team

using System;
using Microsoft.AspNetCore.Components;
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

        protected RenderFragment Css;

        protected string AnimationStyle => Snackbar?.State.AnimationStyle + Style;
        protected string SnackbarClass => Snackbar?.State.SnackbarClass;

        protected string Message => Snackbar?.Message;

        protected string Action => Snackbar?.State.Options.Action;
        protected Color ActionColor => Snackbar?.State.Options.ActionColor ?? Color.Default;
        protected Variant ActionVariant => Snackbar?.State.Options.ActionVariant ?? Snackbar?.State.Options.SnackbarVariant ?? Variant.Text;

        protected bool ShowActionButton => Snackbar?.State.ShowActionButton == true;
        protected bool ShowCloseIcon => Snackbar?.State.ShowCloseIcon == true;

        protected bool HideIcon => Snackbar?.State.HideIcon == true;
        protected string Icon => Snackbar?.State.Icon;
        protected Color IconColor => Snackbar?.State.Options.IconColor ?? Color.Inherit;
        protected Size IconSize => Snackbar?.State.Options.IconSize ?? Size.Medium;

        protected void ActionClicked() => Snackbar?.Clicked(false);
        protected void CloseIconClicked() => Snackbar?.Clicked(true);

        protected void SnackbarClicked()
        {
            if (!ShowActionButton)
                Snackbar?.Clicked(false);
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

        private void SnackbarUpdated()
        {
            InvokeAsync(StateHasChanged);
        }

        public void Dispose()
        {
            if (Snackbar != null)
                Snackbar.OnUpdate -= SnackbarUpdated;
        }
    }
}
