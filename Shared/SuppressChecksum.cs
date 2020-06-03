using Microsoft.AspNetCore.Razor.Language;

namespace BlazorRepl.Shared
{
    public class SuppressChecksum : IConfigureRazorCodeGenerationOptionsFeature
    {
        public int Order => 0;

        public RazorEngine Engine { get; set; }

        public void Configure(RazorCodeGenerationOptionsBuilder options)
        {
            options.SuppressChecksum = true;
        }
    }
}
