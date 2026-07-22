using System.Diagnostics;

namespace MudBlazor.Docs.Compiler;

public class Program
{
    public static int Main(string[] args)
    {
        var stopWatch = Stopwatch.StartNew();
        // Optional arg 0: path to MudBlazor's reference assembly (stable unless the public API changes).
        var referenceAssemblyPath = args.Length > 0 ? args[0] : null;
        var success =
            new CodeSnippets().Execute()
            && new ApiDocumentationBuilder { ReferenceAssemblyPath = referenceAssemblyPath }.Execute()
            && new ExamplesMarkup().Execute();

        Console.WriteLine(@$"Docs.Compiler completed in {stopWatch.ElapsedMilliseconds} milliseconds.");
        return success ? 0 : 1;
    }
}
