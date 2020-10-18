using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ColorCode;

namespace MudBlazor.Docs.Compiler
{
    public class Program
    {
        const string DocDir = "MudBlazor.Docs";
        const string TestsFile = "_AllComponents.cs";
        const string ExampleDiscriminator = "Example";  // example components must contain this string

        static void Main(string[] args)
        {
            var path = Path.GetFullPath(".");
            var src_path = string.Join("/", path.Split('/', '\\').TakeWhile(x => x != "src").Concat(new[] { "src" }));
            var doc_path = Directory.EnumerateDirectories(src_path, DocDir).FirstOrDefault();
            if (doc_path == null)
                throw new InvalidOperationException("Directory not found: " + DocDir);

            CreateHilitedCode( doc_path);
            var test_path = Directory.EnumerateFiles(src_path, TestsFile, SearchOption.AllDirectories).FirstOrDefault();
            if (test_path == null)
                throw new InvalidOperationException("File not found: " + TestsFile);
            CreateTestsFromExamples(test_path, doc_path);
        }

        private static string StripComponentSource(string path)
        {
            var source = File.ReadAllText(path, Encoding.UTF8);
            source = Regex.Replace(source, "@using .+?\n", "");
            source = Regex.Replace(source, "@namespace .+?\n", "");
            return source.Trim();
        }

        private static void CreateHilitedCode(string doc_path)
        {
            var formatter = new HtmlClassFormatter();
            foreach (var entry in Directory.EnumerateFiles(doc_path, "*.razor", SearchOption.AllDirectories).ToArray())
            {
                if (entry.EndsWith("Code.razor"))
                    continue;
                var filename = Path.GetFileName(entry);
                if (!filename.Contains(ExampleDiscriminator))
                    continue;
                //var component_name = Path.GetFileNameWithoutExtension(filename);
                var markup_path = entry.Replace("Examples", "Code").Replace(".razor", "Code.razor");
                var markup_dir = Path.GetDirectoryName(markup_path);
                if (!Directory.Exists(markup_dir))
                    Directory.CreateDirectory(markup_dir);
                //Console.WriteLine("Found code snippet: " + component_name);
                var src = StripComponentSource(entry);
                var blocks=src.Split("@code");
                var blocks0 = Regex.Replace(blocks[0], @"</?DocsFrame>", "")
                    .Replace("@", "PlaceholdeR")
                    .Trim();
                // Note: the @ creates problems and thus we replace it with an unlikely placeholder and in the markup replace back.
                var html = formatter.GetHtmlString(blocks0, Languages.Html).Replace("PlaceholdeR", "@");
                html = AttributePostprocessing(html).Replace("@", "<span class=\"atSign\">&#64;</span>");
                using (var f = File.Open(markup_path, FileMode.Create))
                using (var w = new StreamWriter(f) { NewLine = "\r\n" })
                {
                    w.WriteLine("@* Auto-generated markup. Any changes will be overwritten *@");
                    w.WriteLine("@namespace MudBlazor.Docs.Examples.Markup");
                    w.WriteLine("<div class=\"mud-codeblock\">");
                    w.WriteLine(html.ToWindowsLineEndings());
                    if (blocks.Length == 2)
                    {
                        w.WriteLine(
                            formatter.GetHtmlString("@code" + blocks[1], Languages.CSharp)
                                .Replace("@", "<span class=\"atSign\">&#64;</span>")
                                .ToWindowsLineEndings()
                        );
                    }
                    w.WriteLine("</div>");
                    w.Flush();
                }
            }
        }

        public static string AttributePostprocessing(string html)
        {
            return Regex.Replace(html, @"<span class=""htmlAttributeValue"">&quot;(?'value'.*?)&quot;</span>", new MatchEvaluator(
                m =>
                {
                    var value = m.Groups["value"].Value;
                    return $@"<span class=""quot"">&quot;</span>{AttributeValuePostprocessing(value)}<span class=""quot"">&quot;</span>";
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


    private static void CreateTestsFromExamples(string testPath, string docPath)
        {
            using (var f = File.Open(testPath, FileMode.Create))
            using (var w = new StreamWriter(f))
            {
                w.WriteLine("// NOTE: this file is autogenerated. Any changes will be overwritten!");
                w.WriteLine(
@"using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using MudBlazor.UnitTests.Mocks;
using MudBlazor.Docs.Examples;
using MudBlazor.Dialog;
using MudBlazor.Services;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class _AllComponents
    {
        // These tests just check if all the examples from the doc page render without errors

");
                foreach (var entry in Directory.EnumerateFiles(docPath, "*.razor", SearchOption.AllDirectories))
                {
                    if (entry.EndsWith("Code.razor"))
                        continue;
                    var filename = Path.GetFileName(entry);
                    var component_name = Path.GetFileNameWithoutExtension(filename);
                    if (!filename.Contains(ExampleDiscriminator))
                        continue;
                    w.WriteLine(
@$"
        [Test]
        public void {component_name}_Test()
        {{
                using var ctx = new Bunit.TestContext();
                ctx.Services.AddSingleton<NavigationManager>(new MockNavigationManager());
                ctx.Services.AddSingleton<IDialogService>(new DialogService());
                ctx.Services.AddSingleton<IResizeListenerService>(new MockResizeListenerService());
                var comp = ctx.RenderComponent<{component_name}>();
        }}
");
                }

                w.WriteLine(
                    @"    }
}
");
                w.Flush();
            }
        }

    }

}
