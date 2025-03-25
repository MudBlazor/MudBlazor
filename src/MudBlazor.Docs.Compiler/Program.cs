using System.Diagnostics;

namespace MudBlazor.Docs.Compiler;

public class Program
{
    public static int Main()
    {
        var stopWatch = Stopwatch.StartNew();
        var success =
            new CodeSnippets().Execute()
            && new ApiDocumentationBuilder().Execute()
            && new ExamplesMarkup().Execute();

        Console.WriteLine(@$"Docs.Compiler completed in {stopWatch.ElapsedMilliseconds} milliseconds.");
        return success ? 0 : 1;
    }
}
