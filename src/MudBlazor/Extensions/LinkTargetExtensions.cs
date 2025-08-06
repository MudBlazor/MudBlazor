// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor
{
#nullable enable
    public static class LinkTargetExtensions
    {
        public static string? GetDescriptionStringOrFrameTarget(this LinkTarget target, string? iframe) =>
            target == LinkTarget.Iframe
                ? iframe
                : target.ToDescriptionString();
    }
}
