// Copyright (c) 2020 Jonny Larsson
// License: MIT
// See https://github.com/Garderoben/MudBlazor
// Modified version of Blazored Modal
// Copyright (c) 2019 Blazored
// License: MIT
// See https://github.com/Blazored


using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Utilities;
using System;
using System.Threading.Tasks;

namespace MudBlazor.Dialog
{
    public partial class MudDialogInstance
    {
        [CascadingParameter] private MudDialogProvider Parent { get; set; }
        [CascadingParameter] private DialogOptions GlobalDialogOptions { get; set; } = new DialogOptions();

        [Parameter] public DialogOptions Options { get; set; } = new DialogOptions();
        [Parameter] public string Title { get; set; }
        [Parameter] public RenderFragment Content { get; set; }
        [Parameter] public Guid Id { get; set; }

        private string Position { get; set; }
        private string DialogMaxWidth { get; set; }
        private string Class { get; set; }
        private bool DisableBackdropClick { get; set; }
        private bool NoHeader { get; set; }
        private bool CloseButton { get; set; }
        private bool FullScreen { get; set; }
        private bool FullWidth { get; set; }


        protected override void OnInitialized()
        {
            ConfigureInstance();
        }

        public void SetTitle(string title)
        {
            Title = title;
            StateHasChanged();
        }
        public async Task Close()
        {
            await Close(DialogResult.Ok<object>(null));
        }

        public async Task Close(DialogResult dialogResult)
        {
            await Parent.DismissInstance(Id, dialogResult);
        }

        public async Task Cancel()
        {
            await Close(DialogResult.Cancel());
        }

        private void ConfigureInstance()
        {
            Position = SetPosition();
            DialogMaxWidth = SetMaxWidth();
            Class = Classname;
            NoHeader = SetHideHeader();
            CloseButton = SetCloseButton();
            FullWidth = SetFullWidth();
            DisableBackdropClick = SetDisableBackdropClick();
        }

        private string SetPosition()
        {
            DialogPosition position;

            if (Options != null && Options.Position.HasValue)
            {
                position = Options.Position.Value;
            }
            else if (GlobalDialogOptions.Position.HasValue)
            {
                position = GlobalDialogOptions.Position.Value;
            }
            else
            {
                position = DialogPosition.Center;
            }
            return $"mud-dialog-{position.ToDescriptionString()}";
        }

        private string SetMaxWidth()
        {
            MaxWidth maxWidth;

            if (Options.MaxWidth.HasValue)
            {
                maxWidth = Options.MaxWidth.Value;
            }
            else if (GlobalDialogOptions.MaxWidth.HasValue)
            {
                maxWidth = GlobalDialogOptions.MaxWidth.Value;
            }
            else
            {
                maxWidth = MaxWidth.Small;
            }
            return $"mud-dialog-width-{maxWidth.ToDescriptionString()}";
        }

        private bool SetFullWidth()
        {
            if (Options.FullWidth.HasValue)
                return Options.FullWidth.Value;

            if (GlobalDialogOptions.FullWidth.HasValue)
                return GlobalDialogOptions.FullWidth.Value;

            return false;
        }

        protected string Classname =>
        new CssBuilder("mud-dialog")
            .AddClass(DialogMaxWidth)
            .AddClass("mud-dialog-width-full", FullWidth == true)
        .Build();

        private bool SetHideHeader()
        {
            if (Options.NoHeader.HasValue)
                return Options.NoHeader.Value;

            if (GlobalDialogOptions.NoHeader.HasValue)
                return GlobalDialogOptions.NoHeader.Value;

            return false;
        }

        private bool SetCloseButton()
        {
            if (Options.CloseButton.HasValue)
                return Options.CloseButton.Value;

            if (GlobalDialogOptions.CloseButton.HasValue)
                return GlobalDialogOptions.CloseButton.Value;

            return false;
        }

        private bool SetDisableBackdropClick()
        {
            if (Options.DisableBackdropClick.HasValue)
                return Options.DisableBackdropClick.Value;

            if (GlobalDialogOptions.DisableBackdropClick.HasValue)
                return GlobalDialogOptions.DisableBackdropClick.Value;

            return false;
        }

        private async Task HandleBackgroundClick()
        {
            if (DisableBackdropClick) return;

            await Cancel();
        }

    }
}
