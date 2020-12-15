using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ColorCode;

namespace MudBlazor.Docs.Compiler
{
    public class ExamplesMarkup
    {
        public bool Execute()
        {
            var paths = new Paths();
            bool success = true;
            try
            {
                var formatter = new HtmlClassFormatter();

                foreach (var entry in Directory.EnumerateFiles(paths.DocsDirPath, "*.razor", SearchOption.AllDirectories)
                    .OrderBy(e => e.Replace("\\","/"), StringComparer.Ordinal))
                {
                    if (entry.EndsWith("Code.razor"))
                        continue;
                    var filename = Path.GetFileName(entry);
                    if (!filename.Contains(Paths.ExampleDiscriminator))
                        continue;
                    //var component_name = Path.GetFileNameWithoutExtension(filename);
                    var markupPath = entry.Replace("Examples", "Code").Replace(".razor", "Code.razor");
                    var markupDir = Path.GetDirectoryName(markupPath);
                    if (!Directory.Exists(markupDir))
                        Directory.CreateDirectory(markupDir);
                    //Console.WriteLine("Found code snippet: " + component_name);
                    var src = StripComponentSource(entry);
                    var blocks = src.Split("@code");
                    var blocks0 = Regex.Replace(blocks[0], @"</?DocsFrame>", "")
                        .Replace("@", "PlaceholdeR")
                        .Trim();
                    // Note: the @ creates problems and thus we replace it with an unlikely placeholder and in the markup replace back.
                    var html = formatter.GetHtmlString(blocks0, Languages.Html).Replace("PlaceholdeR", "@");
                    html = AttributePostprocessing(html).Replace("@", "<span class=\"atSign\">&#64;</span>");
                    using (var f = File.Create(markupPath))
                    using (var w = new StreamWriter(f) { NewLine = "\n" })
                    {
                        w.WriteLine("@* Auto-generated markup. Any changes will be overwritten *@");
                        w.WriteLine("@namespace MudBlazor.Docs.Examples.Markup");
                        w.WriteLine("<div class=\"mud-codeblock\">");
                        w.WriteLine(html.ToLfLineEndings());
                        if (blocks.Length == 2)
                        {
                            w.WriteLine(
                                formatter.GetHtmlString("@code" + blocks[1], Languages.CSharp)
                                    .Replace("@", "<span class=\"atSign\">&#64;</span>")
                                    .ToLfLineEndings()
                            );
                        }

                        w.WriteLine("</div>");
                        w.Flush();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error generating examples markup : {e.Message}");
                success = false;
            }

            return success;
        }

        private static string StripComponentSource(string path)
        {
            var source = File.ReadAllText(path, Encoding.UTF8);
            //source = Regex.Replace(source, "@using .+?\n", "");
            source = Regex.Replace(source, "@(namespace|layout|page) .+?\n", "");
            return source.Trim();
        }

        public static string AttributePostprocessing(string html)
        {
            return Regex.Replace(html, @"<span class=""htmlAttributeValue"">&quot;(?'value'.*?)&quot;</span>",
                new MatchEvaluator(
                    m =>
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
            if (value == "true" || value == "false")
                return $"<span class=\"keyword\">{value}</span>";
            if (Regex.IsMatch(value, "^[A-Z][A-Za-z0-9]+[.][A-Za-z][A-Za-z0-9]+$"))
            {
                var tokens = value.Split('.');
                return $"<span class=\"enum\">{tokens[0]}</span><span class=\"enumValue\">.{tokens[1]}</span>";
            }

            if (Regex.IsMatch(value, "^@[A-Za-z0-9]+$"))
            {
                return $"<span class=\"sharpVariable\">{value}</span>";
            }

            return $"<span class=\"htmlAttributeValue\">{value}</span>";
        }

    }
}