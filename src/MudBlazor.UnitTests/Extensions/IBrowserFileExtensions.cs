﻿// Copyright (c) MudBlazor 2021
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
        /// <param name="file"></param>
        /// <returns></returns>
        public static async Task<string> GetFileContents(this IBrowserFile file)
        {
            using var fileStream = file.OpenReadStream();
            using var memoryStream = new MemoryStream();
            await fileStream.CopyToAsync(memoryStream);
            return Encoding.ASCII.GetString(memoryStream.ToArray());
        }
    }
}
