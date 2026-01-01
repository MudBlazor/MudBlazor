using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

#nullable enable
namespace MudBlazor.Utilities
{
    [ExcludeFromCodeCoverage]
    public class RuntimeLocation
    {
        public static bool IsClientSide => RuntimeInformation.OSDescription == "Browser"; // WASM
        public static bool IsServerSide => RuntimeInformation.OSDescription != "Browser";
    }

}
