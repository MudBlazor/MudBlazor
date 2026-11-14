// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.JSInterop;

namespace MudBlazor.Docs.Services
{
    /// <summary>
    /// Detects whether an ad blocker (or network filter) is preventing
    /// the Carbon Ads slot from rendering on the docs page.
    /// </summary>
    public interface IAdBlockDetectionService
    {
        /// <summary>
        /// Returns <c>true</c> when the ad slot is likely being blocked,
        /// either by an element-hiding cosmetic filter or because the
        /// carbon.js script could not be fetched.
        /// </summary>
        /// <param name="waitMilliseconds">
        /// How long to wait before sampling the page state. Gives the
        /// ad blocker time to apply cosmetic filters and the carbon.js
        /// script time to load on slow connections.
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
                // If the JS layer itself fails (e.g. blocked file or interop error)
                // we conservatively assume the ad slot is not visible.
                return true;
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
