// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.JSInterop;

namespace MudBlazor.Docs.Services
{
    /// <summary>
    /// Detects whether ad-related cosmetic filters are hiding content on the docs page.
    /// </summary>
    public interface IAdBlockDetectionService
    {
        /// <summary>
        /// Returns <c>true</c> when ad-related cosmetic filters are likely active.
        /// </summary>
        /// <param name="waitMilliseconds">
        /// How long to wait for cosmetic filters.
        /// </param>
        ValueTask<bool> IsAdBlockedAsync(int waitMilliseconds = 2000);
    }

    /// <inheritdoc cref="IAdBlockDetectionService"/>
    public class AdBlockDetectionService : IAdBlockDetectionService
    {
        private readonly IJSRuntime _jsRuntime;

        public AdBlockDetectionService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async ValueTask<bool> IsAdBlockedAsync(int waitMilliseconds = 2000)
        {
            try
            {
                return await _jsRuntime.InvokeAsync<bool>("mudBlazorDocs.detectAdBlock", waitMilliseconds);
            }
            catch (JSException)
            {
                // Interop failures do not prove user-side blocking, so stay quiet instead of showing a misleading support message.
                return false;
            }
            catch (JSDisconnectedException)
            {
                // Circuit gone (Blazor Server) -- nothing meaningful to report.
                return false;
            }
            catch (TaskCanceledException)
            {
                return false;
            }
        }
    }
}
