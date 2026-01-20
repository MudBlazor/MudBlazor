using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html;

namespace MudBlazor.UnitTests;

/// <summary>
/// Helper methods for pretty printing markup from <see cref="INode"/> and <see cref="INodeList"/>.
/// </summary>
internal static class NodePrintExtensions
{
    /// <summary>
    /// Writes the serialization of the node guided by the formatter.
    /// </summary>
    /// <param name="nodes">The nodes to serialize.</param>
    /// <param name="writer">The output target of the serialization.</param>
    /// <param name="formatter">The formatter to use.</param>
    public static void ToHtml(this IEnumerable<INode> nodes, TextWriter writer, IMarkupFormatter formatter)
    {
        ArgumentNullException.ThrowIfNull(nodes);

        foreach (var node in nodes)
        {
            node.ToHtml(writer, formatter);
        }
    }

    /// <summary>
    /// Uses the <see cref="PrettyMarkupFormatter"/> to generate a HTML markup string
    /// from a <see cref="IEnumerable{INode}"/> <paramref name="nodes"/>.
    /// </summary>
    public static string ToMarkup(this IEnumerable<INode> nodes)
    {
        ArgumentNullException.ThrowIfNull(nodes);
        using var sw = new StringWriter();
        var formatter = new PrettyMarkupFormatter
        {
            NewLine = Environment.NewLine,

            Indentation = "  ",
        };

        nodes.ToHtml(sw, formatter);

        return sw.ToString();
    }

    /// <summary>
    /// Uses the <see cref="PrettyMarkupFormatter"/> to generate a HTML markup
    /// from a <see cref="IMarkupFormattable"/> <paramref name="markupFormattable"/>.
    /// </summary>
    public static string ToMarkup(this IMarkupFormattable markupFormattable)
    {
        ArgumentNullException.ThrowIfNull(markupFormattable);
        using var sw = new StringWriter();

        var formatter = new PrettyMarkupFormatter
        {
            NewLine = Environment.NewLine,
            Indentation = "  ",
        };

        markupFormattable.ToHtml(sw, formatter);

        return sw.ToString();
    }
}
