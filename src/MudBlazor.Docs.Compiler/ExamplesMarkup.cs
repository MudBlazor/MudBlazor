using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using ColorCode;

namespace MudBlazor.Docs.Compiler
{
    public partial class ExamplesMarkup
    {
        public bool Execute()
        {
            var newFiles = new StringBuilder();
            var success = true;
            var noOfFilesUpdated = 0;
            var noOfFilesCreated = 0;

            try
            {
                var formatter = new HtmlClassFormatter();
                var lastCheckedTime = new DateTime();
                if (File.Exists(Paths.NewFilesToBuildPath))
                {
                    var lastNewFilesToBuild = new FileInfo(Paths.NewFilesToBuildPath);
                    lastCheckedTime = lastNewFilesToBuild.LastWriteTime;
                }

                var directoryInfo = new DirectoryInfo(Paths.DocsDirPath);

                foreach (var entry in directoryInfo.GetFiles("*.razor", SearchOption.AllDirectories))
                {
                    if (entry.Name.EndsWith("Code.razor"))
                    {
                        continue;
                    }
                    if (!entry.Name.Contains(Paths.ExampleDiscriminator))
                        continue;
                    var markupPath = entry.FullName.Replace("Examples", "Code").Replace(".razor", "Code.html");
                    if (entry.LastWriteTime < lastCheckedTime && File.Exists(markupPath))
                    {
                        continue;
                    }

                    var markupDir = Path.GetDirectoryName(markupPath);
                    if (!Directory.Exists(markupDir))
                    {
                        Directory.CreateDirectory(markupDir);
                    }

                    var src = StripComponentSource(entry.FullName);
                    var blocks = src.Split("@code");
                    var blocks0 = DocsFrameEndTagRegularExpression().Replace(blocks[0], string.Empty)
                        .Replace("@", "PlaceholdeR")
                        .Trim();

                    // Note: the @ creates problems and thus we replace it with an unlikely placeholder and in the markup replace back.
                    var html = formatter.GetHtmlString(blocks0, Languages.Html).Replace("PlaceholdeR", "@");
                    html = AttributePostprocessing(html).Replace("@", "<span class=\"atSign\">&#64;</span>");

                    var currentCode = string.Empty;
                    if (File.Exists(markupPath))
                    {
                        currentCode = File.ReadAllText(markupPath);
                    }

                    var cb = new CodeBuilder();
                    // cb.AddLine("@* Auto-generated markup. Any changes will be overwritten *@");
                    // cb.AddLine("@namespace MudBlazor.Docs.Examples.Markup");
                    cb.AddLine("<div class=\"mud-codeblock\">");
                    cb.AddLine(html.ToLfLineEndings());
                    if (blocks.Length == 2)
                    {
                        cb.AddLine(
                            formatter.GetHtmlString("@code" + blocks[1], Languages.CSharp)
                                .Replace("@", "<span class=\"atSign\">&#64;</span>")
                                .ToLfLineEndings());
                    }

                    cb.AddLine("</div>");

                    if (currentCode != cb.ToString())
                    {
                        File.WriteAllText(markupPath, cb.ToString());
                        if (currentCode == string.Empty)
                        {
                            newFiles.AppendLine(markupPath);
                            noOfFilesCreated++;
                        }
                        else
                        {
                            noOfFilesUpdated++;
                        }
                    }
                }

                File.WriteAllText(Paths.NewFilesToBuildPath, newFiles.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error generating examples markup : {e.Message}");
                success = false;
            }

            Console.WriteLine($"Docs.Compiler updated {noOfFilesUpdated} generated files");
            Console.WriteLine($"Docs.Compiler generated {noOfFilesCreated} new files");
            return success;
        }

        private static string StripComponentSource(string path)
        {
            var source = File.ReadAllText(path, Encoding.UTF8);
            source = NamespaceLayoutOrPageRegularExpression().Replace(source, string.Empty);
            return source.Trim();
        }

        public static string AttributePostprocessing(string html)
        {
            return HtmlAttributeValueSpanRegularExpression().Replace(html, new MatchEvaluator(m =>
                    {
                        var value = m.Groups["value"].Value;
                        return
                            $@"<span class=""quot"">&quot;</span>{AttributeValuePostprocessing(value)}<span class=""quot"">&quot;</span>";
                    }));
        }

        private static string AttributeValuePostprocessing(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;
            if (value is "true" or "false")
                return $"<span class=\"keyword\">{value}</span>";
            if (AlphanumericDotAlphanumericRegularExpression().IsMatch(value))
            {
                var tokens = value.Split('.');
                return $"<span class=\"enum\">{tokens[0]}</span><span class=\"enumValue\">.{tokens[1]}</span>";
            }

            if (AlphanumericRegularExpression().IsMatch(value))
            {
                return $"<span class=\"sharpVariable\">{value}</span>";
            }

            return $"<span class=\"htmlAttributeValue\">{value}</span>";
        }

        [GeneratedRegex(@"</?DocsFrame>")]
        private static partial Regex DocsFrameEndTagRegularExpression();

        [GeneratedRegex("@(namespace|layout|page) .+?\n")]
        private static partial Regex NamespaceLayoutOrPageRegularExpression();

        [GeneratedRegex(@"<span class=""htmlAttributeValue"">&quot;(?'value'.*?)&quot;</span>")]
        private static partial Regex HtmlAttributeValueSpanRegularExpression();

        [GeneratedRegex("^[A-Z][A-Za-z0-9]+[.][A-Za-z][A-Za-z0-9]+$")]
        private static partial Regex AlphanumericDotAlphanumericRegularExpression();

        [GeneratedRegex("^@[A-Za-z0-9]+$")]
        private static partial Regex AlphanumericRegularExpression();
    }
}
