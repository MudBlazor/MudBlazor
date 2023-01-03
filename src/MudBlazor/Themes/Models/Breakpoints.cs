using System.Diagnostics.CodeAnalysis;

namespace MudBlazor
{
#pragma warning disable IDE1006 // must being with upper case
    [ExcludeFromCodeCoverage]
    public class Breakpoints
    {
        public string xs { get; set; } = "0px";
        public string sm { get; set; } = "600px";
        public string md { get; set; } = "960px";
        public string lg { get; set; } = "1280px";
        public string xl { get; set; } = "1920px";
        public string xxl { get; set; } = "2560px";
    }
}
