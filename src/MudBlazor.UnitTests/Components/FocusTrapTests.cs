using System.Linq;
using AwesomeAssertions;
using Bunit;
using Microsoft.AspNetCore.Components.Web;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components;

[TestFixture]
public class FocusTrapTests : BunitTest
{
    [Test]
    public void FocusTrap_ShouldSaveFocus_AndFocusFirstChild_ByDefault()
    {
        Context.Render<MudFocusTrap>();

        GetInvocationCount("mudElementRef.saveFocus").Should().Be(1);
        Context.JSInterop.VerifyInvoke("mudElementRef.focusFirst", 1);
        Context.JSInterop.Invocations["mudElementRef.focusFirst"].Single()
            .Arguments
            .Should()
            .HaveCount(3)
            .And
            .HaveElementAt(1, 2)
            .And
            .HaveElementAt(2, 4);
        GetInvocationCount("mudElementRef.focusLast").Should().Be(0);
        GetInvocationCount("Blazor._internal.domWrapper.focus").Should().Be(0);
    }

    [Test]
    public void FocusTrap_DefaultFocusElement_ShouldFocusFallbackElement()
    {
        Context.Render<MudFocusTrap>(parameters => parameters.Add(x => x.DefaultFocus, DefaultFocus.Element));

        GetInvocationCount("mudElementRef.saveFocus").Should().Be(1);
        Context.JSInterop.VerifyInvoke("Blazor._internal.domWrapper.focus", 1);
        Context.JSInterop.Invocations["Blazor._internal.domWrapper.focus"].Single()
            .Arguments
            .Should()
            .HaveCount(2);
        GetInvocationCount("mudElementRef.focusFirst").Should().Be(0);
        GetInvocationCount("mudElementRef.focusLast").Should().Be(0);
    }

    [Test]
    public void FocusTrap_DefaultFocusLastChild_ShouldFocusLastChild()
    {
        Context.Render<MudFocusTrap>(parameters => parameters.Add(x => x.DefaultFocus, DefaultFocus.LastChild));

        GetInvocationCount("mudElementRef.saveFocus").Should().Be(1);
        Context.JSInterop.VerifyInvoke("mudElementRef.focusLast", 1);
        Context.JSInterop.Invocations["mudElementRef.focusLast"].Single()
            .Arguments
            .Should()
            .HaveCount(3)
            .And
            .HaveElementAt(1, 2)
            .And
            .HaveElementAt(2, 4);
        GetInvocationCount("mudElementRef.focusFirst").Should().Be(0);
    }

    [Test]
    public void FocusTrap_DefaultFocusNone_ShouldSkipInitialFocus()
    {
        Context.Render<MudFocusTrap>(parameters => parameters.Add(x => x.DefaultFocus, DefaultFocus.None));

        GetInvocationCount("mudElementRef.saveFocus").Should().Be(1);
        GetInvocationCount("mudElementRef.focusFirst").Should().Be(0);
        GetInvocationCount("mudElementRef.focusLast").Should().Be(0);
        GetInvocationCount("Blazor._internal.domWrapper.focus").Should().Be(0);
    }

    [Test]
    public async Task FocusTrap_FocusEvents_ShouldRedirectToExpectedTargets()
    {
        var comp = Context.Render<MudFocusTrap>(parameters => parameters.Add(x => x.DefaultFocus, DefaultFocus.None));
        var root = comp.Find("div.mud-focus-trap");
        var trapChildren = root.Children;

        await root.FocusAsync();
        await trapChildren[0].FocusAsync();
        await trapChildren[5].FocusAsync();

        await comp.WaitForAssertionAsync(() =>
        {
            Context.JSInterop.VerifyInvoke("Blazor._internal.domWrapper.focus", 1);
            Context.JSInterop.VerifyInvoke("mudElementRef.focusFirst", 1);
            Context.JSInterop.VerifyInvoke("mudElementRef.focusLast", 1);
        });
    }

    [Test]
    public async Task FocusTrap_TabHandling_ShouldSuppressRerender_AndSwitchBumperDirection()
    {
        var comp = Context.Render<MudFocusTrap>(parameters => parameters.Add(x => x.DefaultFocus, DefaultFocus.None));
        var root = comp.Find("div.mud-focus-trap");
        var trapChildren = root.Children;
        var renderCountBeforeKeyEvent = comp.RenderCount;

        await root.KeyDownAsync(new KeyboardEventArgs { Key = "Tab", ShiftKey = true });

        comp.RenderCount.Should().Be(renderCountBeforeKeyEvent);

        await trapChildren[1].FocusAsync();

        await comp.WaitForAssertionAsync(() => Context.JSInterop.VerifyInvoke("mudElementRef.focusLast", 1));

        renderCountBeforeKeyEvent = comp.RenderCount;

        await root.KeyUpAsync(new KeyboardEventArgs { Key = "Tab" });

        comp.RenderCount.Should().Be(renderCountBeforeKeyEvent);

        await trapChildren[4].FocusAsync();

        await comp.WaitForAssertionAsync(() => Context.JSInterop.VerifyInvoke("mudElementRef.focusFirst", 1));

        renderCountBeforeKeyEvent = comp.RenderCount;

        await root.KeyDownAsync(new KeyboardEventArgs { Key = "ArrowDown" });

        comp.RenderCount.Should().Be(renderCountBeforeKeyEvent);

        await trapChildren[1].FocusAsync();

        await comp.WaitForAssertionAsync(() => Context.JSInterop.VerifyInvoke("mudElementRef.focusFirst", 2));
    }

    [Test]
    public async Task FocusTrap_DisabledState_ShouldDisableTabStops_AndInitializeWhenReenabled()
    {
        var comp = Context.Render<MudFocusTrap>(parameters => parameters.Add(x => x.Disabled, true));
        var trapChildren = comp.Find("div.mud-focus-trap").Children;

        trapChildren[0].GetAttribute("tabindex").Should().Be("-1");
        trapChildren[1].GetAttribute("tabindex").Should().Be("-1");
        trapChildren[4].GetAttribute("tabindex").Should().Be("-1");
        trapChildren[5].GetAttribute("tabindex").Should().Be("-1");
        GetInvocationCount("mudElementRef.focusFirst").Should().Be(0);
        GetInvocationCount("mudElementRef.focusLast").Should().Be(0);
        GetInvocationCount("Blazor._internal.domWrapper.focus").Should().Be(0);

        await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Disabled, false));

        trapChildren = comp.Find("div.mud-focus-trap").Children;
        trapChildren[0].GetAttribute("tabindex").Should().Be("0");
        trapChildren[1].GetAttribute("tabindex").Should().Be("0");
        trapChildren[4].GetAttribute("tabindex").Should().Be("0");
        trapChildren[5].GetAttribute("tabindex").Should().Be("0");

        await comp.WaitForAssertionAsync(() => Context.JSInterop.VerifyInvoke("mudElementRef.focusFirst", 1));
    }

    [Test]
    public void FocusTrap_Dispose_ShouldRestoreFocus_WhenEnabled()
    {
        var comp = Context.Render<MudFocusTrap>(parameters => parameters.Add(x => x.DefaultFocus, DefaultFocus.None));

        comp.Instance.Dispose();

        Context.JSInterop.VerifyInvoke("mudElementRef.restoreFocus", 1);
    }

    [Test]
    public void FocusTrap_Dispose_ShouldNotRestoreFocus_WhenDisabled()
    {
        var comp = Context.Render<MudFocusTrap>(parameters => parameters.Add(x => x.Disabled, true));

        comp.Instance.Dispose();

        GetInvocationCount("mudElementRef.restoreFocus").Should().Be(0);
    }

    private int GetInvocationCount(string identifier)
    {
        return Context.JSInterop.Invocations.Count(x => x.Identifier == identifier);
    }
}
