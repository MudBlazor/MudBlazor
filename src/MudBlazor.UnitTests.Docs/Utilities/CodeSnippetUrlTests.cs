using System.IO.Compression;
using System.Text;
using FluentAssertions;
using MudBlazor.Docs.Models;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Docs.Utilities;

[TestFixture]
public class CodeSnippetUrlTests
{
    [Test]
    public void CompressedUrlRoundtripTest()
    {
        var snippet = Snippets.GetCode("TableServerSidePaginateExample");
        string urlEncodedBase64CompressedCode;
        string base64CompressedCode;
        string snippet1;
        byte[] bytes;
        // compression
        using (var uncompressed = new MemoryStream(Encoding.UTF8.GetBytes(snippet)))
        using (var compressed = new MemoryStream())
        using (var compressor = new DeflateStream(compressed, CompressionMode.Compress))
        {
            uncompressed.CopyTo(compressor);
            compressor.Close();
            bytes = compressed.ToArray();
            base64CompressedCode = Convert.ToBase64String(bytes);
            urlEncodedBase64CompressedCode = Uri.EscapeDataString(base64CompressedCode);
        }
        // uncompress
        base64CompressedCode = Uri.UnescapeDataString(urlEncodedBase64CompressedCode);
        bytes = Convert.FromBase64String(base64CompressedCode);
        using (var uncompressed = new MemoryStream())
        using (var compressedStream = new MemoryStream(bytes))
        using (var uncompressor = new DeflateStream(compressedStream, CompressionMode.Decompress))
        {
            uncompressor.CopyTo(uncompressed);
            uncompressor.Close();
            //uncompressed.Position = 0;
            snippet1 = Encoding.UTF8.GetString(uncompressed.ToArray());
        }
        // compare
        snippet1.Should().Be(snippet);
    }
}
