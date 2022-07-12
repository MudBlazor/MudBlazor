// Copyright (c) 2020 Jonny Larsson
// License: MIT
// See https://github.com/MudBlazor/MudBlazor
// Modified version of Blazored Modal
// Copyright (c) 2019 Blazored
// License: MIT
// See https://github.com/Blazored

using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Services;

namespace MudBlazor
{
    public partial class MudDialogProvider : IDisposable, IAsyncDisposable
    {
        private string _elementId = "dialogs_" + Guid.NewGuid().ToString().Substring(0, 8);
        private IKeyInterceptor _keyInterceptor;
        [Inject] private IKeyInterceptorFactory _keyInterceptorFactory { get; set; }
        [Inject] private IDialogService DialogService { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }

        [Parameter][Category(CategoryTypes.Dialog.Behavior)] public bool? NoHeader { get; set; }
        [Parameter][Category(CategoryTypes.Dialog.Behavior)] public bool? CloseButton { get; set; }
        [Parameter][Category(CategoryTypes.Dialog.Behavior)] public bool? DisableBackdropClick { get; set; }
        [Parameter][Category(CategoryTypes.Dialog.Behavior)] public bool? CloseOnEscapeKey { get; set; }
        [Parameter][Category(CategoryTypes.Dialog.Appearance)] public bool? FullWidth { get; set; }
        [Parameter][Category(CategoryTypes.Dialog.Appearance)] public DialogPosition? Position { get; set; }
        [Parameter][Category(CategoryTypes.Dialog.Appearance)] public MaxWidth? MaxWidth { get; set; }

        private readonly Collection<IDialogReference> _dialogs = new();
        private readonly DialogOptions _globalDialogOptions = new();
        /// <summary>
        /// Mainly for testing
        /// </summary>
        public IEnumerable<IDialogReference> VisibleDialogs => _dialogs.Where(x => !x.Result.IsCompleted);
        public IDialogReference TopmostDialog => VisibleDialogs.LastOrDefault();
        protected override void OnInitialized()
        {
            DialogService.OnDialogInstanceAdded += AddInstance;
            DialogService.OnDialogCloseRequested += DismissInstance;
            NavigationManager.LocationChanged += LocationChanged;

            _globalDialogOptions.DisableBackdropClick = DisableBackdropClick;
            _globalDialogOptions.CloseOnEscapeKey = CloseOnEscapeKey;
            _globalDialogOptions.CloseButton = CloseButton;
            _globalDialogOptions.NoHeader = NoHeader;
            _globalDialogOptions.Position = Position;
            _globalDialogOptions.FullWidth = FullWidth;
            _globalDialogOptions.MaxWidth = MaxWidth;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _keyInterceptor = _keyInterceptorFactory.Create();
                await _keyInterceptor.Connect(_elementId, new KeyInterceptorOptions()
                {
                    TargetClass = "mud-dialog",
                    Keys = {
                            new KeyOptions { Key="Escape", SubscribeDown = true },
                        },
                });
                _keyInterceptor.KeyDown += HandleKeyDown;
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        internal void HandleKeyDown(KeyboardEventArgs args)
        {
            // get top level dialog instance
            var topLevelDialog = _dialogs.LastOrDefault()?.Instance;
            // check if the top level dialog handles keyboard events
            var dialogInstance = topLevelDialog as MudDialogInstance;
            // if yes, forward keyboard events
            dialogInstance?.HandleKeyDown(args);
        }

        internal void DismissInstance(Guid id, DialogResult result)
        {
            var reference = GetDialogReference(id);
            if (reference != null)
                DismissInstance(reference, result);
        }

        private void AddInstance(IDialogReference dialog)
        {
            _dialogs.Add(dialog);
            StateHasChanged();
        }

        public void DismissAll()
        {
            _dialogs.ToList().ForEach(r => DismissInstance(r, DialogResult.Cancel()));
            StateHasChanged();
        }

        private void DismissInstance(IDialogReference dialog, DialogResult result)
        {
            if (!dialog.Dismiss(result)) return;

            _dialogs.Remove(dialog);
            StateHasChanged();
        }

        private IDialogReference GetDialogReference(Guid id)
        {
            return _dialogs.SingleOrDefault(x => x.Id == id);
        }

        private void LocationChanged(object sender, LocationChangedEventArgs args)
        {
            DismissAll();
        }

        public void Dispose()
        {
            if (NavigationManager != null)
                NavigationManager.LocationChanged -= LocationChanged;

            if (DialogService != null)
            {
                DialogService.OnDialogInstanceAdded -= AddInstance;
                DialogService.OnDialogCloseRequested -= DismissInstance;
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_keyInterceptor != null)
            {
                _keyInterceptor.KeyDown -= HandleKeyDown;
                await _keyInterceptor.Disconnect();
                _keyInterceptor = null;
            }
        }
    }
}
