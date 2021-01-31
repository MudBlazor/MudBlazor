using System;
using System.Collections.Generic;
using System.Text;
using AngleSharp.Dom;

namespace MudBlazor.UnitTests
{
    public static class AngleSharpExtensions
    {
        public static string TrimmedText(this IElement self)
        {
            return self.TextContent?.Trim();
        }
    }
}
