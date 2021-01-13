namespace MudBlazor.Docs.Compiler
{
    public class Program
    {
        public static int Main()
        {
            var success =
                new CodeSnippets().Execute()
                && new DocStrings().Execute()
                && new ExamplesMarkup().Execute()
                && new TestsForExamples().Execute()
                && new TestsForApiPages().Execute();
            return success ? 0 : 1;
        }
    }
}
