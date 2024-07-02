using System.IO;
using System.Linq;

namespace MudBlazor.Docs.Compiler
{
    public static class Paths
    {
        private const string DocsDirectory = "MudBlazor.Docs";
        private const string TestDirectory = "MudBlazor.UnitTests";
        private const string SnippetsFile = "Snippets.generated.cs";
        private const string ApiDocumentationFile = "ApiDocumentation.generated.cs";
        private const string DocStringsFile = "DocStrings.generated.cs";
        private const string ComponentTestsFile = "ExampleDocsTests.generated.cs";
        private const string ApiPageTestsFile = "ApiDocsTests.generated.cs";
        private const string NewFilesToBuild = "NewFilesToBuild.txt";

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

                return workingPath;
            }
        }

        public static string DocsDirPath => Directory.EnumerateDirectories(SrcDirPath, DocsDirectory).FirstOrDefault();

        public static string TestDirPath => Path.Join(Directory.EnumerateDirectories(SrcDirPath, TestDirectory).FirstOrDefault(), "Generated");

        public static string DocsStringSnippetsDirPath => Path.Join(DocsDirPath, "Models");

        public static string DocStringsFilePath => Path.Join(DocsStringSnippetsDirPath, DocStringsFile);

        public static string SnippetsFilePath => Path.Join(DocsStringSnippetsDirPath, SnippetsFile);

        public static string ComponentTestsFilePath => Path.Join(TestDirPath, ComponentTestsFile);

        public static string ApiPageTestsFilePath => Path.Join(TestDirPath, ApiPageTestsFile);

        public static string NewFilesToBuildPath => Path.Join(DocsDirPath, NewFilesToBuild);

        public static string ApiDocumentationPath => Path.Join(DocsDirPath, "Models", "Generated");

        public static string ApiDocumentationFilePath => Path.Join(ApiDocumentationPath, ApiDocumentationFile);
    }
}
