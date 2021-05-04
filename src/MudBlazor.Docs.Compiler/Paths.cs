using System.IO;
using System.Linq;

namespace MudBlazor.Docs.Compiler
{
    public class Paths
    {
        private const string DocsDirectory = "MudBlazor.Docs";
        private const string TestDirectory = "MudBlazor.UnitTests";
        private const string SnippetsFile = "Snippets.generated.cs";
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

        public string DocsDirPath
        {
            get
            {
                return Directory.EnumerateDirectories(SrcDirPath, DocsDirectory).FirstOrDefault();
            }
        }

        public string TestDirPath
        {
            get
            {
                return Path.Join(Directory.EnumerateDirectories(SrcDirPath, TestDirectory).FirstOrDefault(), "Generated");
            }
        }

        public string DocsStringSnippetsDirPath
        {
            get
            {
                return Path.Join(DocsDirPath, "Models");
            }
        }

        public string DocStringsFilePath
        {
            get
            {
                return Path.Join(DocsStringSnippetsDirPath, DocStringsFile);
            }
        }

        public string SnippetsFilePath
        {
            get
            {
                return Path.Join(DocsStringSnippetsDirPath, SnippetsFile);
            }
        }

        public string ComponentTestsFilePath
        {
            get
            {
                return Path.Join(TestDirPath, ComponentTestsFile);
            }
        }

        public string ApiPageTestsFilePath
        {
            get
            {
                return Path.Join(TestDirPath, ApiPageTestsFile);
            }
        }

        public string NewFilesToBuildPath
        {
            get
            {
                return Path.Join(DocsDirPath, NewFilesToBuild);
            }
        }
    }
}
