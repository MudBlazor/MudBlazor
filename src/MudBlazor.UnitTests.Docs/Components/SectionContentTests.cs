using System.IO.Compression;
using System.Text;
using AwesomeAssertions;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MudBlazor.Docs.Components;
using MudBlazor.Docs.Examples;
using MudBlazor.Docs.Services;
using MudBlazor.UnitTests.Mocks;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Docs.Components;

[TestFixture]
public class SectionContentTests : BunitTest
{
    /// <summary>
    /// The TryMudBlazor snippet of an example using the Company model also carries the model, so the playground can compile it.
    /// </summary>
    [TestCase(nameof(DataGridGroupingMultiLevelExample))]
    [TestCase(nameof(DataGridGroupingCustomizationExample))]
    public async Task RunOnTryMudBlazor_IncludesCompanyModel(string example)
    {
        var url = await RunOnTryMudBlazorAsync(example);
        NUnit.Framework.TestContext.Out.WriteLine($"{example} TryMudBlazor URL length: {url.Length}");

        var files = DecodeSnippetFiles(url);

        files.Keys.Should().BeEquivalentTo("__Main.razor", "Company.cs");
        files["Company.cs"].Should().Contain("namespace MudBlazor.Examples.Data.Models").And.Contain("public class Company");
    }

    private async Task<string> RunOnTryMudBlazorAsync(string code)
    {
        string url = null;
        var jsApiService = new Mock<IJsApiService>();
        jsApiService.Setup(x => x.OpenInNewTabAsync(It.IsAny<string>()))
            .Callback<string>(x => url = x)
            .Returns(ValueTask.CompletedTask);
        Context.Services.AddSingleton(jsApiService.Object);
        Context.Services.AddSingleton<IDocsJsApiService, MockDocsJsApiService>();

        var comp = Context.Render<SectionContent>(parameters => parameters
            .Add(x => x.Code, code)
            .Add(x => x.ChildContent, (RenderFragment)(builder => builder.AddContent(0, "Example"))));
        await comp.FindAll("button").First(x => x.TextContent.Trim() == "Run").ClickAsync();

        url.Should().StartWith("https://try.mudblazor.com/snippet/");
        return url;
    }

    private static Dictionary<string, string> DecodeSnippetFiles(string url)
    {
        var bytes = WebEncoders.Base64UrlDecode(url[(url.LastIndexOf('/') + 1)..]);
        using var compressed = new MemoryStream(bytes);
        using var decompressor = new DeflateStream(compressed, CompressionMode.Decompress);
        using var reader = new StreamReader(decompressor, Encoding.UTF8);
        var parts = reader.ReadToEnd().Split((char)31);

        return Enumerable.Range(0, parts.Length / 2).ToDictionary(i => parts[i * 2], i => parts[(i * 2) + 1]);
    }
}
