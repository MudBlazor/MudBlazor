namespace MudBlazor.Docs.Compiler;

public static class Paths
{
    private const string DocsDirectory = "MudBlazor.Docs";
    private const string SnippetsFile = "Snippets.generated.cs";
    private const string ApiDocumentationFile = "ApiDocumentation.generated.cs";
    private const string NewFilesToBuild = "NewFilesToBuild.txt";

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
                        if (Directory.Exists(docsCandidate))
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

    public static string DocsStringSnippetsDirPath => Path.Join(DocsDirPath, "Models");

    public static string SnippetsFilePath => Path.Join(DocsStringSnippetsDirPath, SnippetsFile);

    public static string NewFilesToBuildPath => Path.Join(DocsDirPath, NewFilesToBuild);

    public static string ApiDocumentationPath => Path.Join(DocsDirPath, "Models", "Generated");

    public static string ApiDocumentationFilePath => Path.Join(ApiDocumentationPath, ApiDocumentationFile);
}
