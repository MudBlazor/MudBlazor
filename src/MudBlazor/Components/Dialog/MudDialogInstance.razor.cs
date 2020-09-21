using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;
using System;
using System.Threading.Tasks;

namespace MudBlazor.Dialog
{
    public partial class MudDialogInstance
    {
        [CascadingParameter] private MudDialogProvider Parent { get; set; }
        [CascadingParameter] private DialogOptions GlobalDialogOptions { get; set; }

        [Parameter] public DialogOptions Options { get; set; }
        [Parameter] public string Title { get; set; }
        [Parameter] public RenderFragment Content { get; set; }
        [Parameter] public Guid Id { get; set; }

        private string Position { get; set; }
        private string Class { get; set; }
        private bool DisableBackdropClick { get; set; }
        private bool NoHeader { get; set; }
        private bool CloseButton { get; set; }

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
            Class = Classname;
            NoHeader = SetHideHeader();
            CloseButton = SetCloseButton();
            DisableBackdropClick = SetDisableBackdropClick();
        }

        private string SetPosition()
        {
            DialogPosition position;

            if (Options.Position.HasValue)
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

            switch (position)
            {
                case DialogPosition.Center:
                    return "mud-dialog-center";
                case DialogPosition.TopLeft:
                    return "mud-dialog-topleft";
                case DialogPosition.TopRight:
                    return "mud-dialog-topright";
                case DialogPosition.BottomLeft:
                    return "mud-dialog-bottomleft";
                case DialogPosition.BottomRight:
                    return "mud-dialog-bottomright";
                case DialogPosition.Custom:
                default:
                    return "mud-dialog-center";
            }
        }

        protected string Classname =>
        new CssBuilder("mud-dialog")
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
