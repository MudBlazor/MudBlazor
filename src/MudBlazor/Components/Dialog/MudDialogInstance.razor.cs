﻿// Copyright (c) 2019 Blazored
// Copyright (c) 2020 Adapted by Jonny Larsson, Meinrad Recheis and Contributors

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Extensions;
using MudBlazor.Services;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudDialogInstance : MudComponentBase
    {
        private DialogOptions _options = new();
        private string _elementId = "dialog_" + Guid.NewGuid().ToString().Substring(0, 8);

        [Inject] private IKeyInterceptor _keyInterceptor { get; set; }
        [CascadingParameter] public bool RightToLeft { get; set; }
        [CascadingParameter] private MudDialogProvider Parent { get; set; }
        [CascadingParameter] private DialogOptions GlobalDialogOptions { get; set; } = new DialogOptions();

        [Parameter]
        public DialogOptions Options
        {
            get
            {
                if (_options == null)
                    _options = new DialogOptions();
                return _options;
            }
            set => _options = value;
        }

        [Parameter] public string Title { get; set; }
        [Parameter] public RenderFragment TitleContent { get; set; }
        [Parameter] public RenderFragment Content { get; set; }
        [Parameter] public Guid Id { get; set; }

        /// <summary>
        /// Custom close icon.
        /// </summary>
        [Parameter] public string CloseIcon { get; set; } = Icons.Material.Filled.Close;

        private string Position { get; set; }
        private string DialogMaxWidth { get; set; }
        private bool DisableBackdropClick { get; set; }
        private bool CloseOnEscapeKey { get; set; }
        private bool NoHeader { get; set; }
        private bool CloseButton { get; set; }
        private bool FullScreen { get; set; }
        private bool FullWidth { get; set; }


        protected override void OnInitialized()
        {
            ConfigureInstance();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                //Since CloseOnEscapeKey is the only thing to be handled, turn interceptor off
                if (CloseOnEscapeKey)
                {
                    await _keyInterceptor.Connect(_elementId, new KeyInterceptorOptions()
                    {
                        TargetClass = "mud-dialog",
                        Keys = {
                            new KeyOptions { Key="Escape", SubscribeDown = true },
                        },
                    });
                    _keyInterceptor.KeyDown += HandleKeyDown;
                }
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        internal void HandleKeyDown(KeyboardEventArgs args)
        {
             switch (args.Key)
            {
                case "Escape":
                    if (CloseOnEscapeKey)
                    {
                        Cancel();
                    }
                    break;
            }
        }

        public void SetOptions(DialogOptions options)
        {
            Options = options;
            ConfigureInstance();
            StateHasChanged();
        }

        public void SetTitle(string title)
        {
            Title = title;
            StateHasChanged();
        }

        /// <summary>
        /// Close and return null. 
        /// 
        /// This is a shorthand of Close(DialogResult.Ok((object)null));
        /// </summary>
        public void Close()
        {
            Close(DialogResult.Ok<object>(null));
        }

        /// <summary>
        /// Close with dialog result.
        /// 
        /// Usage: Close(DialogResult.Ok(returnValue))
        /// </summary>
        public void Close(DialogResult dialogResult)
        {
            Parent.DismissInstance(Id, dialogResult);
        }

        /// <summary>
        /// Close and directly pass a return value. 
        /// 
        /// This is a shorthand for Close(DialogResult.Ok(returnValue))
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="returnValue"></param>
        public void Close<T>(T returnValue)
        {
            var dialogResult = DialogResult.Ok<T>(returnValue);
            Parent.DismissInstance(Id, dialogResult);
        }

        /// <summary>
        /// Cancel the dialog. DialogResult.Cancelled will be set to true
        /// </summary>
        public void Cancel()
        {
            Close(DialogResult.Cancel());
        }

        private void ConfigureInstance()
        {
            Position = SetPosition();
            DialogMaxWidth = SetMaxWidth();
            Class = Classname;
            NoHeader = SetHideHeader();
            CloseButton = SetCloseButton();
            FullWidth = SetFullWidth();
            FullScreen = SetFulScreen();
            DisableBackdropClick = SetDisableBackdropClick();
            CloseOnEscapeKey = SetCloseOnEscapeKey();
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

        private bool SetFulScreen()
        {
            if (Options.FullScreen.HasValue)
                return Options.FullScreen.Value;

            if (GlobalDialogOptions.FullScreen.HasValue)
                return GlobalDialogOptions.FullScreen.Value;

            return false;
        }

        protected string Classname =>
            new CssBuilder("mud-dialog")
                .AddClass(DialogMaxWidth, !FullScreen)
                .AddClass("mud-dialog-width-full", FullWidth && !FullScreen)
                .AddClass("mud-dialog-fullscreen", FullScreen)
                .AddClass("mud-dialog-rtl", RightToLeft)
                .AddClass(Class)
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

        private bool SetCloseOnEscapeKey()
        {
            if (Options.CloseOnEscapeKey.HasValue)
                return Options.CloseOnEscapeKey.Value;

            if (GlobalDialogOptions.CloseOnEscapeKey.HasValue)
                return GlobalDialogOptions.CloseOnEscapeKey.Value;

            return false;
        }

        private void HandleBackgroundClick()
        {
            if (DisableBackdropClick)
                return;
            Cancel();
        }

        private MudDialog _dialog;
        public void Register(MudDialog dialog)
        {
            if (dialog == null)
                return;
            _dialog = dialog;
            Class = dialog.Class;
            Style = dialog.Style;
            TitleContent = dialog.TitleContent;
            StateHasChanged();
        }

        public void ForceRender()
        {
            StateHasChanged();
        }
    }
}
