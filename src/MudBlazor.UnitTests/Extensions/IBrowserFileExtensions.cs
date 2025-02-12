// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;

namespace MudBlazor.UnitTests
{
    public static class IBrowserFileExtensions
    {
        /// <summary>
        /// Returns the string contents of an IBrowserFile
        /// </summary>
        public static async Task<string> GetFileContents(this IBrowserFile file)
        {
            await using var fileStream = file.OpenReadStream();
            using var streamReader = new StreamReader(fileStream, Encoding.UTF8);
            return await streamReader.ReadToEndAsync();
        }
    }
}
