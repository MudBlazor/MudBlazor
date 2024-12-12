using System.Diagnostics;

namespace MudBlazor.UnitTests.Docs.Generator;

public class Program
{
    public static int Main()
    {
        var stopWatch = Stopwatch.StartNew();
        var success =
            new TestsForExamples().Execute()
            && new TestsForApiPages().Execute();

        Console.WriteLine(@$"MudBlazor.UnitTests.Docs.Generator completed in {stopWatch.ElapsedMilliseconds} milliseconds.");
        return success ? 0 : 1;
    }
}
