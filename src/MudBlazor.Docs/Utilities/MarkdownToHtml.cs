// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Renderers.Html.Inlines;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace MudBlazor.Docs.Utilities;

#nullable enable
public class MarkdownToHtml
{
    public enum RenderMode
    {
        Default = 0,
        ReleasePageRender = 1,
    }

    public static string Parse(string markdownBody, Uri? baseUrl = null, RenderMode renderMode = RenderMode.Default)
    {
        ArgumentNullException.ThrowIfNull(markdownBody);

        var pipeline = new MarkdownPipelineBuilder()
            .UseAutoIdentifiers()
            .Build();
        var builder = new StringBuilder();
        using var textWriter = new StringWriter(builder);
        var renderer = new HtmlRenderer(textWriter) { BaseUrl = baseUrl };
        renderer.ObjectRenderers.ReplaceOrAdd<HtmlObjectRenderer<HeadingBlock>>(new MudHeadingRenderer(renderMode));
        renderer.ObjectRenderers.ReplaceOrAdd<HtmlObjectRenderer<LinkInline>>(new MudLinkRenderer());
        renderer.ObjectRenderers.ReplaceOrAdd<HtmlObjectRenderer<ListBlock>>(new MudListRenderer(renderMode));
        renderer.ObjectRenderers.ReplaceOrAdd<HtmlObjectRenderer<ListItemBlock>>(new B());
        var document = Markdown.Parse(markdownBody, pipeline);
        renderer.Render(document);

        return builder.ToString();

    }

    public class B : HtmlObjectRenderer<ListItemBlock>
    {
        protected override void Write(HtmlRenderer renderer, ListItemBlock obj)
        {
            throw new NotImplementedException();
        }
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

        public MudHeadingRenderer(RenderMode renderMode) {
            _renderMode = renderMode;
        }

        protected override void Write(HtmlRenderer renderer, HeadingBlock obj)
        {
            renderer.EnsureLine();
            var heading = _heading[obj.Level];
            
            if (_renderMode == RenderMode.Default)
            {
                renderer.Write($"<{heading} id=\"{obj.GetAttributes().Id}\" class=\"mud-typography mud-typography-{heading} mt-3\">");
                renderer.Write("<b>");
            }
            else
            {
                renderer.Write($"<{heading} class=\"mud-typography mud-typography-{heading}\">");
                //renderer.Write("<ul class=\"mt-3 mb-6 px-6\">");
            }

            renderer.WriteLeafInline(obj);
            if (_renderMode == RenderMode.Default)
            {
                renderer.Write("</b>");
            }
            else
            {
                //renderer.Write("</ul>");
            }

            renderer.Write($"</{heading}>");
            if (obj.Level < 3)
            {
                if (_renderMode == RenderMode.Default)
                {
                    renderer.Write("<hr class=\"mud-divider mud-divider-fullwidth\">");
                }
            }

            renderer.EnsureLine();
        }
    }

    public class MudLinkRenderer : HtmlObjectRenderer<LinkInline>
    {
        protected override void Write(HtmlRenderer renderer, LinkInline obj)
        {
            var defaultRenderer = new LinkInlineRenderer();
            if (obj.IsImage)
            {
                // Ignore images
                return;
            }
           
            var attributes = obj.GetAttributes();
            attributes.AddClass("mud-link mud-primary-text mud-link-underline-hover");
            if (obj.Url is not null)
            {
                if (obj.Url.StartsWith("http://") || obj.Url.StartsWith("https://"))
                {
                    // External url
                    attributes.AddProperty("target", "_blank");
                }
                else
                {
                    // Internal url
                    attributes.AddProperty("target", "_self");
                }
            }

            defaultRenderer.Write(renderer, obj);
        }
    }
}
