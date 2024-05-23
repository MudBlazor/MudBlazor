using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using FluentAssertions;
using MudBlazor.Docs.Models;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Utilities
{
    [TestFixture]
    public class CodeSnippetUrlTests
    {
        [Test]
        public void CompressedUrlRoundtripTest()
        {
            var snippet = Snippets.GetCode("TableServerSidePaginateExample");
            string urlEncodedBase64compressedCode, base64compressedCode, snippet1;
            byte[] bytes;
            // compression
            using (var uncompressed = new MemoryStream(Encoding.UTF8.GetBytes(snippet)))
            using (var compressed = new MemoryStream())
            using (var compressor = new DeflateStream(compressed, CompressionMode.Compress))
            {
                uncompressed.CopyTo(compressor);
                compressor.Close();
                bytes = compressed.ToArray();
                base64compressedCode = Convert.ToBase64String(bytes);
                urlEncodedBase64compressedCode = Uri.EscapeDataString(base64compressedCode);
            }
            // uncompress
            base64compressedCode = Uri.UnescapeDataString(urlEncodedBase64compressedCode);
            bytes = Convert.FromBase64String(base64compressedCode);
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
}
