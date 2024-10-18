using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Docs.Services;

namespace MudBlazor.Docs.Shared
{
    public partial class MainLayout : LayoutComponentBase, IDisposable
    {
        [Inject]
        private LayoutService LayoutService { get; set; }

        private MudThemeProvider _mudThemeProvider;

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
                await _mudThemeProvider.WatchSystemPreference(OnSystemPreferenceChanged);
                StateHasChanged();
            }
        }

        private async Task ApplyUserPreferences()
        {
            var defaultDarkMode = await _mudThemeProvider.GetSystemPreference();
            await LayoutService.ApplyUserPreferences(defaultDarkMode);
        }

        private async Task OnSystemPreferenceChanged(bool newValue)
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
