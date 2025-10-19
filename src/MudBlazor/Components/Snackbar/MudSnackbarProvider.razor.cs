// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// Changes and improvements Copyright (c) The MudBlazor Team

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using MudBlazor.Utilities;

#nullable enable

namespace MudBlazor
{
    public partial class MudSnackbarProvider : MudComponentBase, IDisposable
    {
        [Inject]
        private ISnackbar Snackbars { get; set; } = null!;

        [Inject]
        private NavigationManager NavigationManager { get; set; } = null!;

        [CascadingParameter(Name = "RightToLeft")]
        public bool RightToLeft { get; set; }

        protected IEnumerable<Snackbar> Snackbar => Snackbars.Configuration.NewestOnTop
                ? Snackbars.ShownSnackbars.Reverse()
                : Snackbars.ShownSnackbars;

        protected string Classname =>
            new CssBuilder(Class)
                .AddClass(GetPositionClass())
                .Build();

        private string GetPositionClass()
        {
            var positionClass = Snackbars.Configuration.PositionClass;
            return positionClass switch
            {
                Defaults.Classes.Position.BottomStart => RightToLeft ? Defaults.Classes.Position.BottomRight : Defaults.Classes.Position.BottomLeft,
                Defaults.Classes.Position.BottomEnd => RightToLeft ? Defaults.Classes.Position.BottomLeft : Defaults.Classes.Position.BottomRight,
                Defaults.Classes.Position.TopStart => RightToLeft ? Defaults.Classes.Position.TopRight : Defaults.Classes.Position.TopLeft,
                Defaults.Classes.Position.TopEnd => RightToLeft ? Defaults.Classes.Position.TopLeft : Defaults.Classes.Position.TopRight,
                _ => positionClass
            };
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            Snackbars.OnSnackbarsUpdated += OnSnackbarsUpdated;
            NavigationManager.LocationChanged += NavigationManager_LocationChanged;
        }

        private void OnSnackbarsUpdated()
        {
            InvokeAsync(StateHasChanged);
        }

        private void NavigationManager_LocationChanged(object? sender, LocationChangedEventArgs e)
        {
            if (Snackbars.Configuration.ClearAfterNavigation)
            {
                Snackbars.Clear();
            }
            else
            {
                var snackbarsToRemove = Snackbars.ShownSnackbars.Where(s => s.State.Options.CloseAfterNavigation).ToArray();
                foreach (var snackbar in snackbarsToRemove)
                {
                    Snackbars.Remove(snackbar);
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Snackbars.OnSnackbarsUpdated -= OnSnackbarsUpdated;
                NavigationManager.LocationChanged -= NavigationManager_LocationChanged;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
