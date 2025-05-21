using Microsoft.AspNetCore.Components;
using MudBlazor.Docs.Services;

namespace MudBlazor.Docs.Shared
{
    public partial class MainLayout : LayoutComponentBase, IDisposable
    {
        private MudThemeProvider _mudThemeProvider;

        [Inject]
        private LayoutService LayoutService { get; set; }

        static MainLayout()
        {
            MudGlobal.TooltipDefaults.Delay = TimeSpan.FromMilliseconds(500);
        }

        protected override void OnInitialized()
        {
            LayoutService.MajorUpdateOccurred += LayoutServiceOnMajorUpdateOccured;
            base.OnInitialized();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                await ApplyUserPreferences();
                await _mudThemeProvider.WatchSystemThemeAsync(OnSystemThemeChangedAsync);
                StateHasChanged();
            }
        }

        private async Task ApplyUserPreferences()
        {
            var darkMode = await _mudThemeProvider.GetSystemThemeAsync();
            await LayoutService.ApplyUserPreferences(darkMode);
        }

        private async Task OnSystemThemeChangedAsync(bool newValue)
        {
            await LayoutService.OnSystemPreferenceChanged(newValue);
        }

        public void Dispose()
        {
            LayoutService.MajorUpdateOccurred -= LayoutServiceOnMajorUpdateOccured;
        }

        private void LayoutServiceOnMajorUpdateOccured(object sender, EventArgs e) => StateHasChanged();
    }
}
