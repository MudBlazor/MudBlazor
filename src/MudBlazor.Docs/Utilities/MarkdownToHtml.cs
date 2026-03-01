// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text;
using System.Text.RegularExpressions;
using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Renderers.Html.Inlines;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace MudBlazor.Docs.Utilities;

#nullable enable
public partial class MarkdownToHtml
{
    private const string GitHubBlobDevBaseUrl = "https://github.com/MudBlazor/MudBlazor/blob/dev/";
    private const string GitHubRawDevBaseUrl = "https://raw.githubusercontent.com/MudBlazor/MudBlazor/dev/";

    public enum RenderMode
    {
        Default = 0,
        ReleasePageRender = 1,
        ContributionPageRender = 2,
    }

    public static string Parse(string markdownBody, Uri? baseUrl = null, RenderMode renderMode = RenderMode.Default)
    {
        ArgumentNullException.ThrowIfNull(markdownBody);

        var body = renderMode switch
        {
            RenderMode.ReleasePageRender => PreprocessReleaseMarkdown(markdownBody),
            RenderMode.ContributionPageRender => PreprocessContributionMarkdown(markdownBody),
            _ => markdownBody
        };

        var pipeline = new MarkdownPipelineBuilder()
            .UseAutoIdentifiers()
            .Build();
        var builder = new StringBuilder();
        using var textWriter = new StringWriter(builder);
        var renderer = new HtmlRenderer(textWriter) { BaseUrl = baseUrl };
        renderer.ObjectRenderers.ReplaceOrAdd<HtmlObjectRenderer<HeadingBlock>>(new MudHeadingRenderer(renderMode));
        renderer.ObjectRenderers.ReplaceOrAdd<HtmlObjectRenderer<LinkInline>>(new MudLinkRenderer());
        renderer.ObjectRenderers.ReplaceOrAdd<HtmlObjectRenderer<ListBlock>>(new MudListRenderer(renderMode));

        var document = Markdown.Parse(body, pipeline);
        renderer.Render(document);

        var html = builder.ToString();
        return renderMode == RenderMode.ReleasePageRender
            ? PostProcessReleaseHtml(html)
            : html;
    }

    private static string PreprocessReleaseMarkdown(string markdownBody)
    {
        var body = LeadingReleaseCommentRegex().Replace(markdownBody, string.Empty);
        body = PullRequestUrlRegex().Replace(body, "[#$1]($0)");
        body = CompareUrlRegex().Replace(body, "[${range}]($0)");
        body = GitHubMentionRegex().Replace(body, "[@$1](https://github.com/$1)");

        return body;
    }

    private static string PostProcessReleaseHtml(string html)
    {
        return FullChangelogParagraphRegex().Replace(
            html,
            "<p class=\"release-full-changelog\"><strong>Full Changelog</strong>:");
    }

    private static string PreprocessContributionMarkdown(string markdownBody)
    {
        var body = ContributionImageSourceRegex().Replace(markdownBody, "$1=\"" + GitHubRawDevBaseUrl + "$2\"");
        body = ContributionImageSourceSetRegex().Replace(body, "$1=\"" + GitHubRawDevBaseUrl + "$2\"");
        body = ContributionTocBlockRegex().Replace(body, match =>
        {
            var tocContent = match.Groups["toc"].Value.Trim();
            return $"## Table of Contents{Environment.NewLine}{Environment.NewLine}{tocContent}{Environment.NewLine}{Environment.NewLine}";
        });

        return MarkdownLinkRegex().Replace(body, RewriteContributionMarkdownLink);
    }

    private static string RewriteContributionMarkdownLink(Match match)
    {
        var text = match.Groups["text"].Value;
        var url = match.Groups["url"].Value;
        var suffix = match.Groups["suffix"].Value;

        if (url.StartsWith('#')
            || url.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
            || url.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
            || url.StartsWith("mailto:", StringComparison.OrdinalIgnoreCase))
        {
            return match.Value;
        }

        if (url.StartsWith('/'))
        {
            var normalizedPath = url.TrimStart('/');
            return $"[{text}]({GitHubBlobDevBaseUrl}{normalizedPath}{suffix})";
        }

        if (!IsMarkdownFileLink(url))
        {
            return match.Value;
        }

        var relativePath = url.StartsWith("./", StringComparison.Ordinal)
            ? url[2..]
            : url;

        return $"[{text}]({GitHubBlobDevBaseUrl}{relativePath}{suffix})";
    }

    private static bool IsMarkdownFileLink(string url)
    {
        var markdownExtensionIndex = url.IndexOf(".md", StringComparison.OrdinalIgnoreCase);
        if (markdownExtensionIndex < 0)
        {
            return false;
        }

        var extensionEndIndex = markdownExtensionIndex + 3;
        return extensionEndIndex == url.Length || url[extensionEndIndex] == '#';
    }

    public class MudListRenderer : HtmlObjectRenderer<ListBlock>
    {
        private readonly RenderMode _renderMode;

        public MudListRenderer(RenderMode renderMode)
        {
            _renderMode = renderMode;
        }

        protected override void Write(HtmlRenderer renderer, ListBlock obj)
        {
            var listRenderer = new ListRenderer();
            if (_renderMode == RenderMode.ReleasePageRender)
            {
                var attributes = obj.GetAttributes();
                attributes.AddClass("mt-3 mb-6 px-6");
            }

            listRenderer.Write(renderer, obj);
        }
    }

    public class MudHeadingRenderer : HtmlObjectRenderer<HeadingBlock>
    {
        private readonly RenderMode _renderMode;
        private readonly Dictionary<int, string> _heading = new()
        {
            { 1, "h4" },
            { 2, "h5" },
            { 3, "h6" },
            { 4, "h6" },
            { 5, "h6" },
            { 6, "h6" }
        };

        public MudHeadingRenderer(RenderMode renderMode)
        {
            _renderMode = renderMode;
        }

        protected override void Write(HtmlRenderer renderer, HeadingBlock obj)
        {
            renderer.EnsureLine();
            var heading = _heading[obj.Level];
            var headingId = obj.GetAttributes().Id;

            if (_renderMode == RenderMode.ReleasePageRender)
            {
                renderer.Write($"<{heading} class=\"mud-typography mud-typography-{heading}\">");
            }
            else
            {
                renderer.Write($"<{heading} id=\"{headingId}\" class=\"mud-typography mud-typography-{heading} mt-3\">");
                renderer.Write("<b>");
            }

            renderer.WriteLeafInline(obj);
            if (_renderMode != RenderMode.ReleasePageRender)
            {
                renderer.Write("</b>");
            }

            renderer.Write($"</{heading}>");
            if (obj.Level < 3 && _renderMode != RenderMode.ReleasePageRender)
            {
                renderer.Write("<hr class=\"mud-divider mud-divider-fullwidth\">");
            }

            renderer.EnsureLine();
        }
    }

    public class MudLinkRenderer : HtmlObjectRenderer<LinkInline>
    {
        protected override void Write(HtmlRenderer renderer, LinkInline obj)
        {
            if (obj.IsImage)
            {
                // Ignore images
                return;
            }

            var defaultRenderer = new LinkInlineRenderer();
            var attributes = obj.GetAttributes();
            if (IsGitHubUserLink(obj))
            {
                attributes.AddClass("mud-link mud-default-text mud-link-underline-hover github-user");
            }
            else
            {
                attributes.AddClass("mud-link mud-primary-text mud-link-underline-hover");
            }

            if (IsCompareLink(obj.Url))
            {
                attributes.AddClass("docs-code docs-code-primary");
            }

            if (obj.Url is not null)
            {
                if (obj.Url.StartsWith("http://") || obj.Url.StartsWith("https://"))
                {
                    // External url
                    attributes.AddProperty("target", "_blank");
                    attributes.AddProperty("rel", "noopener noreferrer");
                }
                else
                {
                    // Internal url
                    attributes.AddProperty("target", "_self");
                }
            }

            defaultRenderer.Write(renderer, obj);
        }

        private static bool IsCompareLink(string? url)
        {
            return url is not null
                && url.Contains("/compare/", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsGitHubUserLink(LinkInline obj)
        {
            if (obj.Url is null
                || !GitHubUserUrlRegex().IsMatch(obj.Url))
            {
                return false;
            }

            return GetInlineText(obj).StartsWith("@", StringComparison.Ordinal);
        }

        private static string GetInlineText(LinkInline obj)
        {
            var text = new StringBuilder();
            var child = obj.FirstChild;

            while (child is not null)
            {
                if (child is LiteralInline literal)
                {
                    text.Append(literal.Content.ToString());
                }

                child = child.NextSibling;
            }

            return text.ToString();
        }
    }

    [GeneratedRegex(@"^\s*<!--.*?-->\s*", RegexOptions.Singleline)]
    private static partial Regex LeadingReleaseCommentRegex();

    [GeneratedRegex(@"https://github\.com/MudBlazor/MudBlazor/pull/(?<id>\d{3,6})")]
    private static partial Regex PullRequestUrlRegex();

    [GeneratedRegex(@"https://github\.com/MudBlazor/MudBlazor/compare/(?<range>[^\s)]+)")]
    private static partial Regex CompareUrlRegex();

    [GeneratedRegex(@"(?<![\w/\[(`])@(?<user>[A-Za-z0-9](?:[A-Za-z0-9-]{0,38}))\b")]
    private static partial Regex GitHubMentionRegex();

    [GeneratedRegex(@"<p>\s*<strong>Full Changelog</strong>\s*:", RegexOptions.CultureInvariant)]
    private static partial Regex FullChangelogParagraphRegex();

    [GeneratedRegex(@"^https://github\.com/[A-Za-z0-9-]+/?$", RegexOptions.CultureInvariant)]
    private static partial Regex GitHubUserUrlRegex();

    [GeneratedRegex(@"\[(?<text>[^\]]+)\]\((?<url>[^)\s]+)(?<suffix>\s+""[^""]*"")?\)", RegexOptions.CultureInvariant)]
    private static partial Regex MarkdownLinkRegex();

    [GeneratedRegex(@"<!--\s*TOC start.*?-->\s*(?<toc>[\s\S]*?)\s*<!--\s*TOC end\s*-->", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex ContributionTocBlockRegex();

    [GeneratedRegex(@"(src)\s*=\s*""(content/[^""]+)""", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex ContributionImageSourceRegex();

    [GeneratedRegex(@"(srcset)\s*=\s*""(content/[^""]+)""", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex ContributionImageSourceSetRegex();
}
