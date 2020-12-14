using System.IO;
using System.Linq;

public class Paths
{
    private const string docsDirectory = "MudBlazor.Docs";
    private const string testDirectory = "MudBlazor.UnitTests";
    private const string snippetsFile = "Snippets.generated.cs";
    private const string docStringsFile = "DocStrings.generated.cs";
    private const string componentTestsFile = "_AllComponents.cs";
    private const string apiPageTestsFile = "_AllApiPages.cs";
    public const string ExampleDiscriminator = "Example"; // example components must contain this string
    public string SrcDirPath
    {
        get{
            var exePath = Path.GetFullPath(".");
            return string.Join("/", exePath.Split('/', '\\').TakeWhile(x => x != "src").Concat(new[] { "src" }));
        }
    }

    public string DocsDirPath
    {
        get
        {
            return Directory.EnumerateDirectories(SrcDirPath, docsDirectory).FirstOrDefault();
        }
    }

    public string TestDirPath
    {
        get
        {
            return Path.Join(Directory.EnumerateDirectories(SrcDirPath, testDirectory).FirstOrDefault(), "Generated");
        }
    }

    public string DocsStringSnippetsDirPath
    {
        get
        {
            return Path.Join(DocsDirPath, "Models");
        }
    }

    public string DocsStringsFilePath
    {
        get
        {
            return Path.Join(DocsStringSnippetsDirPath, docStringsFile);
        }
    }

    public string SnippetsFilePath
    {
        get
        {
            return Path.Join(DocsStringSnippetsDirPath, snippetsFile);
        }
    }

    public string ComponentTestsFilePath
    {
        get
        {
            return Path.Join(TestDirPath, componentTestsFile);
        }
    }

    public string ApiPageTestsFilePath
    {
        get
        {
            return Path.Join(TestDirPath, apiPageTestsFile);
        }
    }
}