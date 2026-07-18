using System.Text;
using System.Text.RegularExpressions;

namespace MudBlazor.Docs.Compiler
{
    public partial class CodeSnippets
    {
        public bool Execute()
        {
            var success = true;
            try
            {
                var exampleFiles = Directory.EnumerateFiles(Paths.DocsDirPath, "*.razor", SearchOption.AllDirectories)
                    .Where(f => Path.GetFileNameWithoutExtension(f).Contains(Paths.ExampleDiscriminator))
                    .OrderBy(e => e.Replace("\\", "/"), StringComparer.Ordinal)
                    .ToList();

                // Early exit: if the output exists and our stamp is newer than all example files, skip generation.
                if (Directory.Exists(Paths.SnippetsOutputDirPath) && File.Exists(Paths.SnippetsStampFilePath))
                {
                    var stampLastWrite = File.GetLastWriteTimeUtc(Paths.SnippetsStampFilePath);
                    var newestExampleTime = exampleFiles
                        .Select(f => File.GetLastWriteTimeUtc(f))
                        .DefaultIfEmpty(DateTime.MinValue)
                        .Max();

                    if (stampLastWrite > newestExampleTime)
                    {
                        Console.WriteLine("CodeSnippets: snippet assets are up-to-date, skipping generation.");
                        return true;
                    }
                }

                // Rewrite the output directory from scratch so sources for removed examples don't linger.
                if (Directory.Exists(Paths.SnippetsOutputDirPath))
                {
                    Directory.Delete(Paths.SnippetsOutputDirPath, recursive: true);
                }
                Directory.CreateDirectory(Paths.SnippetsOutputDirPath);

                foreach (var entry in exampleFiles)
                {
                    var componentName = Path.GetFileNameWithoutExtension(entry);
                    var outputPath = Path.Join(Paths.SnippetsOutputDirPath, componentName + ".txt");
                    File.WriteAllText(outputPath, ReadStrippedSource(entry), Encoding.UTF8);
                }

                Console.WriteLine($"CodeSnippets: wrote {exampleFiles.Count} snippet assets to {Paths.SnippetsOutputDirPath}");

                // Keep the generated partial as an (empty) stub so the Snippets class and its build wiring stay intact.
                // The raw sources now live as static assets, keeping ~1 MB of strings out of the WASM assembly.
                WriteStubIfChanged();

                // Stamp (not the assets) so the next build's early-exit works without forcing a recompile.
                Paths.TouchStamp(Paths.SnippetsStampFilePath);
            }
            catch (Exception e)
            {
                Console.WriteLine(@$"Error generating snippet assets : {e.Message}");
                success = false;
            }

            return success;
        }

        private static void WriteStubIfChanged()
        {
            var cb = new CodeBuilder();
            cb.AddHeader();
            cb.AddLine("namespace MudBlazor.Docs.Models");
            cb.AddLine("{");
            cb.IndentLevel++;
            cb.AddLine("[System.CodeDom.Compiler.GeneratedCodeAttribute(\"MudBlazor.Docs.Compiler\", \"0.0.0.0\")]");
            cb.AddLine("public static partial class Snippets");
            cb.AddLine("{");
            cb.AddLine("}");
            cb.IndentLevel--;
            cb.AddLine("}");

            var currentCode = File.Exists(Paths.SnippetsFilePath) ? File.ReadAllText(Paths.SnippetsFilePath) : string.Empty;
            if (currentCode != cb.ToString())
            {
                File.WriteAllText(Paths.SnippetsFilePath, cb.ToString());
                Console.WriteLine("CodeSnippets: Updated Snippets.generated.cs stub");
            }
        }

        private static string ReadStrippedSource(string path)
        {
            var source = File.ReadAllText(path, Encoding.UTF8);
            source = NamespaceLayoutOrPageRegularExpression().Replace(source, string.Empty);
            return source.Trim();
        }

        [GeneratedRegex("@(namespace|layout|page) .+?\n")]
        private static partial Regex NamespaceLayoutOrPageRegularExpression();
    }
}
