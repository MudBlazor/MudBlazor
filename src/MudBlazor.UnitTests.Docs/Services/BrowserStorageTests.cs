using AwesomeAssertions;
using Bunit;
using MudBlazor.Docs.Enums;
using MudBlazor.Docs.Services;
using MudBlazor.Docs.Services.UserPreferences;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Docs.Services;

[TestFixture]
public class BrowserStorageTests
{
    private BunitContext _ctx;
    private BrowserStorage _storage;

    [SetUp]
    public void Setup()
    {
        _ctx = new BunitContext();
        _storage = new BrowserStorage(_ctx.JSInterop.JSRuntime);
    }

    [TearDown]
    public void TearDown() => _ctx.Dispose();

    /// <summary>
    /// Reads preferences in the format Blazored.LocalStorage saved and docs.js reads.
    /// </summary>
    [Test]
    public async Task ReadsSavedPreferences()
    {
        _ctx.JSInterop.Setup<string>("mudBlazorDocs.getStorageItem", "userPreferences")
            .SetResult("""{"RightToLeft":true,"DarkLightTheme":2}""");

        var preferences = await _storage.GetAsync<UserPreferences>("userPreferences");

        preferences.RightToLeft.Should().BeTrue();
        preferences.DarkLightTheme.Should().Be(DarkLightMode.Dark);
    }

    /// <summary>
    /// A missing key reads as the type's default.
    /// </summary>
    [Test]
    public async Task MissingKeyReadsAsDefault()
    {
        _ctx.JSInterop.Setup<string>("mudBlazorDocs.getStorageItem", _ => true).SetResult(null);

        (await _storage.GetAsync<UserPreferences>("userPreferences")).Should().BeNull();
        (await _storage.GetAsync<DateTime>("__notficationTimestamp")).Should().Be(DateTime.MinValue);
    }

    /// <summary>
    /// A stored value that isn't valid JSON reads as missing instead of throwing.
    /// </summary>
    [Test]
    public async Task InvalidJsonReadsAsDefault()
    {
        _ctx.JSInterop.Setup<string>("mudBlazorDocs.getStorageItem", "userPreferences").SetResult("{not json");

        (await _storage.GetAsync<UserPreferences>("userPreferences")).Should().BeNull();
    }

    /// <summary>
    /// Writes values as the JSON that Blazored.LocalStorage wrote.
    /// </summary>
    [Test]
    public async Task WritesJson()
    {
        _ctx.JSInterop.SetupVoid("mudBlazorDocs.setStorageItem", _ => true).SetVoidResult();

        await _storage.SetAsync("userPreferences", new UserPreferences { RightToLeft = true, DarkLightTheme = DarkLightMode.Light });
        await _storage.SetAsync("__notficationTimestamp", new DateTime(2026, 03, 01));

        var invocations = _ctx.JSInterop.VerifyInvoke("mudBlazorDocs.setStorageItem", 2);
        invocations[0].Arguments.Should().Equal("userPreferences", """{"RightToLeft":true,"DarkLightTheme":1}""");
        invocations[1].Arguments.Should().Equal("__notficationTimestamp", "\"2026-03-01T00:00:00\"");
    }
}
