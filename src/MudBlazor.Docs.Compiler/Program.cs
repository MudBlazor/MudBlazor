using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MudBlazor.Docs.Compiler
{
    public class Program
    {
        public static int Main()
        {
            var stopWatch = Stopwatch.StartNew();
            var success =
                new CodeSnippets().Execute()
                && new ApiDocumentationBuilder().Execute()
                && new ExamplesMarkup().Execute()
                && new TestsForExamples().Execute()
                && new TestsForApiPages().Execute();

            Console.WriteLine($"Docs.Compiler completed in {stopWatch.ElapsedMilliseconds} msecs");
            return success ? 0 : 1;
        }
    }
}
