using System.Diagnostics;

namespace MudBlazor.UnitTests.Docs.Generator;

public class Program
{
    public static int Main()
    {
        if (Paths.DocsDirPath == string.Empty)
        {
            Console.WriteLine("Tests Generator was unable to determine MudBlazor.Docs directory path.");
            return 1;
        }

        var stopWatch = Stopwatch.StartNew();
        var success =
            new TestsForExamples().Execute()
            && new TestsForApiPages().Execute();

        Console.WriteLine(@$"MudBlazor.UnitTests.Docs.Generator completed in {stopWatch.ElapsedMilliseconds} milliseconds.");
        return success ? 0 : 1;
    }
}
