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
            static string? FindRepoSrcDir(string startPath)
            {
                if (string.IsNullOrWhiteSpace(startPath))
                {
                    return null;
                }

                var current = new DirectoryInfo(startPath);
                while (current is not null)
                {
                    if (string.Equals(current.Name, "src", StringComparison.OrdinalIgnoreCase))
                    {
                        var docsCandidate = Path.Combine(current.FullName, DocsDirectory);
                        var testsCandidate = Path.Combine(current.FullName, TestDirectory);
                        if (Directory.Exists(docsCandidate) && Directory.Exists(testsCandidate))
                        {
                            return current.FullName;
                        }
                    }

                    current = current.Parent;
                }

                return null;
            }

            return FindRepoSrcDir(Directory.GetCurrentDirectory())
                ?? FindRepoSrcDir(AppContext.BaseDirectory)
                ?? string.Empty;
        }
    }

    public static string DocsDirPath
    {
        get
        {
            var srcDirPath = SrcDirPath;
            if (string.IsNullOrWhiteSpace(srcDirPath))
            {
                return string.Empty;
            }

            var docsDirPath = Path.Combine(srcDirPath, DocsDirectory);
            return Directory.Exists(docsDirPath) ? docsDirPath : string.Empty;
        }
    }

    public static string TestDirPath
    {
        get
        {
            var srcDirPath = SrcDirPath;
            if (string.IsNullOrWhiteSpace(srcDirPath))
            {
                return string.Empty;
            }

            var testDirPath = Path.Combine(srcDirPath, TestDirectory, "Generated");
            return Directory.Exists(testDirPath) ? testDirPath : string.Empty;
        }
    }

    public static string ComponentTestsFilePath => Path.Join(TestDirPath, ComponentTestsFile);

    public static string ApiPageTestsFilePath => Path.Join(TestDirPath, ApiPageTestsFile);
}
