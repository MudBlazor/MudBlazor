// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace MudBlazor.Docs.Services
{
    public interface IDocsJsApiService
    {
        /// <summary>
        /// Return the inner text of the HTML element referenced by given id
        /// </summary>
        ValueTask<string> GetInnerTextByIdAsync(string id);
    }

    public class DocsJsApiService : IDocsJsApiService
    {
        private readonly IJSRuntime _jsRuntime;

        public DocsJsApiService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        /// <inheritdoc cref="IDocsJsApiService"/>
        public ValueTask<string> GetInnerTextByIdAsync(string id)
        {
            return _jsRuntime.InvokeAsync<string>($"mudBlazorDocs.getInnerTextById", id);
        }
    }
}
