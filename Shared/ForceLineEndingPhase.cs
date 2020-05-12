using Microsoft.AspNetCore.Razor.Language;
using System.Reflection;
using Microsoft.AspNetCore.Razor.Language.CodeGeneration;

namespace BlazorFiddlePoC.Shared
{
    public class ForceLineEndingPhase : RazorEnginePhaseBase
    {
        public ForceLineEndingPhase(string lineEnding)
        {
            LineEnding = lineEnding;
        }

        public string LineEnding { get; }

        protected override void ExecuteCore(RazorCodeDocument codeDocument)
        {
            var field = typeof(CodeRenderingContext).GetField("NewLineString", BindingFlags.Static | BindingFlags.NonPublic);
            var key = field.GetValue(null);
            codeDocument.Items[key] = LineEnding;
        }
    }
}
