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

    private static readonly RenderProfile _defaultProfile = new(
        static value => value,
        null,
        new HeadingRenderOptions(true, true, true, true),
        new ListRenderOptions(null),
        new LinkRenderOptions(true, true));

    private static readonly RenderProfile _releaseProfile = new(
        PreprocessReleaseMarkdown,
        PostProcessReleaseHtml,
        new HeadingRenderOptions(false, false, false, false),
        new ListRenderOptions("mt-3 mb-6 px-6"),
        new LinkRenderOptions(true, true));

    private static readonly RenderProfile _contributionProfile = new(
        PreprocessContributionMarkdown,
        null,
        new HeadingRenderOptions(true, true, true, true),
        new ListRenderOptions(null),
        new LinkRenderOptions(true, true));

    public enum RenderMode
    {
        Default = 0,
        ReleasePageRender = 1,
        ContributionPageRender = 2,
    }

    public static string Parse(string markdownBody, Uri? baseUrl = null, RenderMode renderMode = RenderMode.Default)
    {
        ArgumentNullException.ThrowIfNull(markdownBody);

        var profile = GetProfile(renderMode);
        var body = profile.PreprocessMarkdown(markdownBody);

        var pipeline = new MarkdownPipelineBuilder()
            .UseAutoIdentifiers()
            .Build();
        var builder = new StringBuilder();
        using var textWriter = new StringWriter(builder);
        var renderer = new HtmlRenderer(textWriter) { BaseUrl = baseUrl };
        renderer.ObjectRenderers.ReplaceOrAdd<HtmlObjectRenderer<HeadingBlock>>(new MudHeadingRenderer(profile.HeadingOptions));
        renderer.ObjectRenderers.ReplaceOrAdd<HtmlObjectRenderer<LinkInline>>(new MudLinkRenderer(profile.LinkOptions));
        renderer.ObjectRenderers.ReplaceOrAdd<HtmlObjectRenderer<ListBlock>>(new MudListRenderer(profile.ListOptions));

        var document = Markdown.Parse(body, pipeline);
        renderer.Render(document);

        var html = builder.ToString();
        return profile.PostprocessHtml is null
            ? html
            : profile.PostprocessHtml(html);
    }

    private static RenderProfile GetProfile(RenderMode renderMode)
    {
        return renderMode switch
        {
            RenderMode.ReleasePageRender => _releaseProfile,
            RenderMode.ContributionPageRender => _contributionProfile,
            _ => _defaultProfile
        };
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
        body = ContributionTocBlockRegex().Replace(body, static match =>
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

    private class MudListRenderer : HtmlObjectRenderer<ListBlock>
    {
        private readonly ListRenderOptions _options;

        public MudListRenderer(ListRenderOptions options)
        {
            _options = options;
        }

        protected override void Write(HtmlRenderer renderer, ListBlock obj)
        {
            var listRenderer = new ListRenderer();
            if (!string.IsNullOrWhiteSpace(_options.AdditionalCssClass))
            {
                var attributes = obj.GetAttributes();
                attributes.AddClass(_options.AdditionalCssClass);
            }

            listRenderer.Write(renderer, obj);
        }
    }

    private class MudHeadingRenderer : HtmlObjectRenderer<HeadingBlock>
    {
        private readonly HeadingRenderOptions _options;
        private readonly Dictionary<int, string> _heading = new()
        {
            { 1, "h4" },
            { 2, "h5" },
            { 3, "h6" },
            { 4, "h6" },
            { 5, "h6" },
            { 6, "h6" }
        };

        public MudHeadingRenderer(HeadingRenderOptions options)
        {
            _options = options;
        }

        protected override void Write(HtmlRenderer renderer, HeadingBlock obj)
        {
            renderer.EnsureLine();

            var heading = _heading[obj.Level];
            var headingId = obj.GetAttributes().Id;
            var className = _options.IncludeTopMargin
                ? $"mud-typography mud-typography-{heading} mt-3"
                : $"mud-typography mud-typography-{heading}";

            if (_options.IncludeIds && !string.IsNullOrWhiteSpace(headingId))
            {
                renderer.Write($"<{heading} id=\"{headingId}\" class=\"{className}\">");
            }
            else
            {
                renderer.Write($"<{heading} class=\"{className}\">");
            }

            if (_options.BoldText)
            {
                renderer.Write("<b>");
            }

            renderer.WriteLeafInline(obj);

            if (_options.BoldText)
            {
                renderer.Write("</b>");
            }

            renderer.Write($"</{heading}>");
            if (_options.DividerForTopHeadings && obj.Level < 3)
            {
                renderer.Write("<hr class=\"mud-divider mud-divider-fullwidth\">");
            }

            renderer.EnsureLine();
        }
    }

    private class MudLinkRenderer : HtmlObjectRenderer<LinkInline>
    {
        private readonly LinkRenderOptions _options;

        public MudLinkRenderer(LinkRenderOptions options)
        {
            _options = options;
        }

        protected override void Write(HtmlRenderer renderer, LinkInline obj)
        {
            if (obj.IsImage)
            {
                // Ignore images
                return;
            }

            var defaultRenderer = new LinkInlineRenderer();
            var attributes = obj.GetAttributes();
            if (_options.HighlightGitHubMentions && IsGitHubUserLink(obj))
            {
                attributes.AddClass("mud-link mud-default-text mud-link-underline-hover github-user");
            }
            else
            {
                attributes.AddClass("mud-link mud-primary-text mud-link-underline-hover");
            }

            if (_options.HighlightCompareLinks && IsCompareLink(obj.Url))
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

    private sealed record RenderProfile(
        Func<string, string> PreprocessMarkdown,
        Func<string, string>? PostprocessHtml,
        HeadingRenderOptions HeadingOptions,
        ListRenderOptions ListOptions,
        LinkRenderOptions LinkOptions);

    private sealed record HeadingRenderOptions(bool IncludeIds, bool IncludeTopMargin, bool BoldText, bool DividerForTopHeadings);

    private sealed record ListRenderOptions(string? AdditionalCssClass);

    private sealed record LinkRenderOptions(bool HighlightGitHubMentions, bool HighlightCompareLinks);

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
