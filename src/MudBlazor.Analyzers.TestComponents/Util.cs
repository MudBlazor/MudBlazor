// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;

namespace MudBlazor.Analyzers.TestComponents
{
    public static class Util
    {
        private static string ThisDirectory([CallerFilePath] string callerFilePath = "")
        {
            return System.IO.Path.GetDirectoryName(callerFilePath)!;
        }

        public static string ProjectPath([CallerFilePath] string? callerFilePath = null)
        {
            return System.IO.Path.Combine(ThisDirectory(), "MudBlazor.Analyzers.TestComponents.csproj");
        }
    }
}
