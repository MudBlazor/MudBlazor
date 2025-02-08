namespace MudBlazor.UnitTests.Docs.Generator;

public static class Paths
{
    private const string DocsDirectory = "MudBlazor.Docs";
    private const string TestDirectory = "MudBlazor.UnitTests.Docs";
    private const string ComponentTestsFile = "ExampleDocsTests.generated.cs";
    private const string ApiPageTestsFile = "ApiDocsTests.generated.cs";

    public const string ExampleDiscriminator = "Example"; // example components must contain this string

    public static string SrcDirPath
    {
        get
        {
            var workingPath = Path.GetFullPath(".");
            do
            {
                workingPath = Path.GetDirectoryName(workingPath);
            }
            while (Path.GetFileName(workingPath) != "src" && !string.IsNullOrWhiteSpace(workingPath));

            return workingPath!;
        }
    }

    public static string DocsDirPath => Directory.EnumerateDirectories(SrcDirPath, DocsDirectory).FirstOrDefault() ?? string.Empty;

    public static string TestDirPath => Path.Join(Directory.EnumerateDirectories(SrcDirPath, TestDirectory).FirstOrDefault(), "Generated");

    public static string ComponentTestsFilePath => Path.Join(TestDirPath, ComponentTestsFile);

    public static string ApiPageTestsFilePath => Path.Join(TestDirPath, ApiPageTestsFile);
}
