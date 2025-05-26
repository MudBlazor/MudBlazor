using Microsoft.AspNetCore.Components;
using MudBlazor.Docs.Services;

namespace MudBlazor.Docs.Shared
{
    public partial class MainLayout : LayoutComponentBase, IDisposable
    {
        private MudThemeProvider _mudThemeProvider;

        [Inject]
        private LayoutService LayoutService { get; set; }

        protected override void OnInitialized()
        {
            LayoutService.MajorUpdateOccurred += OnMajorUpdateOccured;
            base.OnInitialized();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var dark = await _mudThemeProvider.GetSystemDarkModeAsync();

                LayoutService.UpdateDarkModeState(dark);

                await LayoutService.ApplyUserPreferencesAsync();

                await _mudThemeProvider.WatchSystemDarkModeAsync(LayoutService.OnSystemModeChangedAsync);

                StateHasChanged();
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        public void Dispose()
        {
            LayoutService.MajorUpdateOccurred -= OnMajorUpdateOccured;
        }

        private void OnMajorUpdateOccured(object sender, EventArgs e) => StateHasChanged();
    }
}
