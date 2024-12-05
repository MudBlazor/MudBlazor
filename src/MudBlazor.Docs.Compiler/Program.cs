using System.Diagnostics;

namespace MudBlazor.Docs.Compiler;

public class Program
{
    public static int Main(string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("Please supply the path to MudBlazor.dll for documentation");
            return 1;
        }

        var mudBlazorDllPath = args[0];
        var stopWatch = Stopwatch.StartNew();
        var success =
            new CodeSnippets().Execute()
            && new ApiDocumentationBuilder().Execute(mudBlazorDllPath)
            && new ExamplesMarkup().Execute();

        Console.WriteLine(@$"Docs.Compiler completed in {stopWatch.ElapsedMilliseconds} milliseconds.");
        return success ? 0 : 1;
    }
}
