using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.UnitTests.TestComponents.Popover;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class PopoverTests : BunitTest
    {
        [Test]
        public void PopoverOptions_Defaults()
        {
            var options = new PopoverOptions();

            options.ContainerClass.Should().Be("mudblazor-main-content");
            options.FlipMargin.Should().Be(0);
            options.ThrowOnDuplicateProvider.Should().Be(true);
        }

        //[Test]
        //[Obsolete]
        //public async Task MudPopoverHandler_DetachAndUpdateFragmentConcurrent_UpdateFragmentDoesNotRunInTheSameTimeAsDetach()
        //{
        //    var connectTcs = new TaskCompletionSource<IJSVoidResult>();

        //    var mock = new Mock<IJSRuntime>();
        //    var handler = new MudPopoverHandler((tree) => { }, mock.Object, () => { });

        //    mock.Setup(x => x.InvokeAsync<IJSVoidResult>("mudPopover.connect", It.IsAny<CancellationToken>(), It.Is<object[]>(y => y.Length == 1 && (Guid)y[0] == handler.Id)))
        //        .ReturnsAsync(Mock.Of<IJSVoidResult>())
        //        .Verifiable();

        //    mock.Setup(x => x.InvokeAsync<IJSVoidResult>("mudPopover.disconnect", It.IsAny<CancellationToken>(), It.Is<object[]>(y => y.Length == 1 && (Guid)y[0] == handler.Id)))
        //        .Returns(new ValueTask<IJSVoidResult>(connectTcs.Task))
        //        .Verifiable();


        //    var comp = Context.RenderComponent<MudBadge>(p =>
        //    {
        //        p.Add(x => x.UserAttributes, new Dictionary<string, object> { { "myprop1", "myValue1" } });
        //        p.Add(x => x.Tag, "my tag");

        //    });

        //    RenderFragment newRenderFragement = (tree) => { };
        //    await handler.Initialize();

        //    _ = handler.Detach();
        //    var task2 = handler.UpdateFragmentAsync(newRenderFragement, comp.Instance, "my-new-extra-class", "my-new-extra-style:2px", true);

        //    var completedTask = await Task.WhenAny(Task.Delay(50), task2);

        //    completedTask.Should().NotBe(task2);

        //    mock.Verify();
        //    mock.VerifyNoOtherCalls();
        //}

        //[Test]
        //[Obsolete]
        //public async Task MudPopoverHandler_DetachAndUpdateFragmentConcurrent_UpdateFragmentAsyncShouldRunAfterDetach()
        //{
        //    var connectTcs = new TaskCompletionSource<IJSVoidResult>();

        //    var mock = new Mock<IJSRuntime>();
        //    var handler = new MudPopoverHandler(_ => { }, mock.Object, () => { });

        //    mock.Setup(x => x.InvokeAsync<IJSVoidResult>("mudPopover.connect", It.IsAny<CancellationToken>(), It.Is<object[]>(y => y.Length == 1 && (Guid)y[0] == handler.Id)))
        //        .ReturnsAsync(Mock.Of<IJSVoidResult>())
        //        .Verifiable();

        //    mock.Setup(x => x.InvokeAsync<IJSVoidResult>("mudPopover.disconnect", It.IsAny<CancellationToken>(), It.Is<object[]>(y => y.Length == 1 && (Guid)y[0] == handler.Id)))
        //        .Returns(new ValueTask<IJSVoidResult>(connectTcs.Task))
        //        .Verifiable();


        //    var comp = Context.RenderComponent<MudBadge>(p =>
        //    {
        //        p.Add(x => x.UserAttributes, new Dictionary<string, object> { { "myprop1", "myValue1" } });
        //        p.Add(x => x.Tag, "my tag");

        //    });

        //    RenderFragment newRenderFragement = (tree) => { };
        //    await handler.Initialize();

        //    var task1 = handler.Detach();
        //    var task2 = handler.UpdateFragmentAsync(newRenderFragement, comp.Instance, "my-new-extra-class", "my-new-extra-style:2px", true);
        //    connectTcs.SetResult(Mock.Of<IJSVoidResult>());

        //    await Task.WhenAll(task1, task2);

        //    mock.Verify();
        //    mock.VerifyNoOtherCalls();
        //}

        [Test]
        public void MudPopover_DefaultValues()
        {
            var popover = new MudPopover();

            popover.MaxHeight.Should().BeNull();
            popover.Paper.Should().BeTrue();
            popover.DropShadow.Should().BeTrue();
            popover.Elevation.Should().Be(8);
            popover.Square.Should().BeFalse();
            popover.Open.Should().BeFalse();
            popover.Fixed.Should().BeFalse();
            popover.AnchorOrigin.Should().Be(Origin.TopLeft);
            popover.TransformOrigin.Should().Be(Origin.TopLeft);
            popover.RelativeWidth.Should().BeNull();
            popover.OverflowBehavior.Should().Be(OverflowBehavior.FlipOnOpen);
            popover.Duration.Should().Be(251);
        }

        [Test]
        public async Task MudPopover_OpenAndClose()
        {
            var comp = Context.RenderComponent<PopoverTest>();

            //popup is close, so only the popover-content should be there
            var provider = comp.Find(".mud-popover-provider");
            provider.Children.Should().ContainSingle();

            var popoverNode = comp.Find(".popoverparent").Children[1];
            popoverNode.Id.Should().StartWith("popover-");

            var handlerId = Guid.Parse(popoverNode.Id.Substring(8));
            handlerId.Should().NotBe(Guid.Empty);

            provider.Children.Should().ContainSingle();
            provider.FirstElementChild.Id.Should().Be($"popovercontent-{handlerId}");
            provider.FirstElementChild.Children.Should().BeEmpty();

            //open the popover, content should be visible
            await comp.Instance.Open();

            provider.FirstElementChild.Children.Should().ContainSingle();
            provider.FirstElementChild.Children[0].TextContent.Should().Be("Popover content");

            //close the popover, not content should be visible again
            await comp.Instance.Close();
            provider.FirstElementChild.Children.Should().BeEmpty();
        }

        [Test]
        public void MudPopover_Property_MaxHeight()
        {
            var comp = Context.RenderComponent<PopoverPropertyTest>(p => p.Add(x => x.MaxHeight, 100));

            var popoverElement = comp.Find(".test-popover-content").ParentElement;

            popoverElement.GetAttribute("style").Split(';', StringSplitOptions.RemoveEmptyEntries).Should().Contain(new[] { "max-height:100px", "my-custom-style:3px" });
        }

        [Test]
        public void MudPopover_Property_TransitionDuration()
        {
            var comp = Context.RenderComponent<PopoverPropertyTest>(p => p.Add(x => x.Duration, 100));

            var popoverElement = comp.Find(".test-popover-content").ParentElement;

            popoverElement.GetAttribute("style").Split(';', StringSplitOptions.RemoveEmptyEntries).Should().Contain(new[] { "transition-duration:100ms" });
        }

        [Test]
        public void MudPopover_Property_Fixed()
        {
            var comp = Context.RenderComponent<PopoverPropertyTest>(p => p.Add(
                x => x.Fixed, true));

            var popoverElement = comp.Find(".test-popover-content").ParentElement;

            popoverElement.ClassList.Should().Contain(new[] { "mud-popover-open", "mud-popover-fixed", "my-custom-class" });
        }

        [Test]
        public void MudPopover_Property_RelativeWidth()
        {
            var comp = Context.RenderComponent<PopoverPropertyTest>(p => p.Add(
                x => x.RelativeWidth, true));

            var popoverElement = comp.Find(".test-popover-content").ParentElement;

            popoverElement.ClassList.Should().Contain(new[] { "mud-popover-open", "mud-popover-relative-width", "my-custom-class" });
        }

        [Test]
        public void MudPopover_Property_Paper()
        {
            var comp = Context.RenderComponent<PopoverPropertyTest>(p => p.Add(
                x => x.Paper, true));

            var popoverElement = comp.Find(".test-popover-content").ParentElement;

            popoverElement.ClassList.Should().Contain(new[] { "mud-popover-open", "mud-paper", "mud-elevation-8", "my-custom-class" });
        }

        [Test]
        public void MudPopover_Property_PaperAndSqaure()
        {
            var comp = Context.RenderComponent<PopoverPropertyTest>(p =>
            {
                p.Add(x => x.Paper, true);
                p.Add(x => x.Square, true);

            });

            var popoverElement = comp.Find(".test-popover-content").ParentElement;

            popoverElement.ClassList.Should().Contain(new[] { "mud-popover-open", "mud-paper-square", "mud-elevation-8", "my-custom-class" });
        }

        [Test]
        public void MudPopover_Property_Elevation()
        {
            var comp = Context.RenderComponent<PopoverPropertyTest>(p =>
            {
                p.Add(x => x.Paper, true);
                p.Add(x => x.Elevation, 10);

            });

            var popoverElement = comp.Find(".test-popover-content").ParentElement;
            popoverElement.ClassList.Should().Contain(new[] { "mud-popover-open", "mud-paper", "mud-elevation-10", "my-custom-class" });
        }

        [Test]
        [TestCase(Origin.BottomCenter, "bottom-center")]
        [TestCase(Origin.BottomLeft, "bottom-left")]
        [TestCase(Origin.BottomRight, "bottom-right")]
        [TestCase(Origin.CenterCenter, "center-center")]
        [TestCase(Origin.CenterLeft, "center-left")]
        [TestCase(Origin.CenterRight, "center-right")]
        [TestCase(Origin.TopCenter, "top-center")]
        [TestCase(Origin.TopLeft, "top-left")]
        [TestCase(Origin.TopRight, "top-right")]
        public void MudPopover_Property_TransformOrigin(Origin transformOrigin, string expectedClass)
        {
            var comp = Context.RenderComponent<PopoverPropertyTest>(p => p.Add(
                x => x.TransformOrigin, transformOrigin));

            var popoverElement = comp.Find(".test-popover-content").ParentElement;

            popoverElement.ClassList.Should().Contain(new[] { "mud-popover-open", $"mud-popover-{expectedClass}", "my-custom-class" });
        }

        [Test]
        [TestCase(Origin.BottomCenter, "bottom-center")]
        [TestCase(Origin.BottomLeft, "bottom-left")]
        [TestCase(Origin.BottomRight, "bottom-right")]
        [TestCase(Origin.CenterCenter, "center-center")]
        [TestCase(Origin.CenterLeft, "center-left")]
        [TestCase(Origin.CenterRight, "center-right")]
        [TestCase(Origin.TopCenter, "top-center")]
        [TestCase(Origin.TopLeft, "top-left")]
        [TestCase(Origin.TopRight, "top-right")]
        public void MudPopover_Property_AnchorOrigin(Origin anchorOrigin, string expectedClass)
        {
            var comp = Context.RenderComponent<PopoverPropertyTest>(p => p.Add(
                x => x.AnchorOrigin, anchorOrigin));

            var popoverElement = comp.Find(".test-popover-content").ParentElement;

            popoverElement.ClassList.Should().Contain(new[] { "mud-popover-open", $"mud-popover-anchor-{expectedClass}", "my-custom-class" });
        }

        [Test]
        [TestCase(OverflowBehavior.FlipNever, "flip-never")]
        [TestCase(OverflowBehavior.FlipOnOpen, "flip-onopen")]
        [TestCase(OverflowBehavior.FlipAlways, "flip-always")]
        public void MudPopover_Property_OverflowBehavior(OverflowBehavior overflowBehavior, string expectedClass)
        {
            var comp = Context.RenderComponent<PopoverPropertyTest>(p => p.Add(
                x => x.OverflowBehavior, overflowBehavior));

            var popoverElement = comp.Find(".test-popover-content").ParentElement;

            popoverElement.ClassList.Should().Contain(new[] { "mud-popover-open", $"mud-popover-overflow-{expectedClass}", "my-custom-class" });
        }

        [Test]
        public void MudPopover_Property_DropShadow_False_NoElevation()
        {
            var comp = Context.RenderComponent<PopoverPropertyTest>(p => p.Add(
                x => x.DropShadow, false));

            var popoverElement = comp.Find(".test-popover-content").ParentElement;

            popoverElement.ClassList.Should().NotContainMatch("mud-elevation-*");
        }

        [Test]
        public async Task MudPopover_WithDynamicContent()
        {
            var comp = Context.RenderComponent<PopoverComplexContent>();

            var dynamicContentElement = comp.Find(".dynamic-content");
            dynamicContentElement.ChildNodes.Should().BeEmpty();

            await comp.Instance.AddRow();

            dynamicContentElement.ChildNodes.Should().ContainSingle();
            dynamicContentElement.ChildNodes[0].TextContent.Should().Be("Popover content 0");

            await comp.Instance.AddRow();
            dynamicContentElement.ChildNodes.Should().HaveCount(2);
            dynamicContentElement.ChildNodes[0].TextContent.Should().Be("Popover content 0");
            dynamicContentElement.ChildNodes[1].TextContent.Should().Be("Popover content 1");

            await comp.Instance.AddRow();
            dynamicContentElement.ChildNodes.Should().HaveCount(3);
            dynamicContentElement.ChildNodes[0].TextContent.Should().Be("Popover content 0");
            dynamicContentElement.ChildNodes[1].TextContent.Should().Be("Popover content 1");
            dynamicContentElement.ChildNodes[2].TextContent.Should().Be("Popover content 2");
        }

        [Test]
        public void MudPopoverProvider_DefaultValue()
        {
            var provider = new MudPopoverProvider();
            provider.Enabled.Should().BeTrue();
        }

        [Test]
        public void MudPopoverProvider_RenderElementsBasedOnEnableState()
        {
            var comp = Context.RenderComponent<PopoverProviderTest>(p => p.Add(x => x.ProviderEnabled, true));
            comp.Find("#my-content").TextContent.Should().Be("Popover content");

            for (var i = 0; i < 3; i++)
            {
                comp.SetParametersAndRender(p => p.Add(x => x.ProviderEnabled, false));
                Assert.Throws<ElementNotFoundException>(() => comp.Find("#my-content"));

                comp.SetParametersAndRender(p => p.Add(x => x.ProviderEnabled, true));
                comp.Find("#my-content").TextContent.Should().Be("Popover content");
            }
        }

        [Test]
        public void MudPopoverProvider_NoRenderWhenEnabledIsFalse()
        {
            var comp = Context.RenderComponent<PopoverProviderTest>(p => p.Add(x => x.ProviderEnabled, false));
            Assert.Throws<ElementNotFoundException>(() => comp.Find("#my-content"));
        }

        //[TestCase(false)] always blocks duplicate provider with latest change
        [TestCase(true)]
        public async Task MudPopoverProvider_ThrowOnDuplicate(bool throwOnDuplicateProvider)
        {
            var options = new PopoverOptions
            {
                ThrowOnDuplicateProvider = throwOnDuplicateProvider
            };

            Context.Services.Configure<PopoverOptions>(x => x.ThrowOnDuplicateProvider = throwOnDuplicateProvider);
            Context.JSInterop.Setup<int>("mudpopoverHelper.countProviders").SetResult(throwOnDuplicateProvider ? 2 : 1);

            if (throwOnDuplicateProvider)
            {
                var ex = Assert.Throws<InvalidOperationException>(() => Context.RenderComponent<PopoverDuplicationTest>());
                ex.Message.Should().StartWith("Duplicate MudPopoverProvider detected");
            }
            else
            {
                var comp = Context.RenderComponent<PopoverDuplicationTest>();
                await comp.Instance.Open();
                await comp.Instance.Close();
            }
        }

        [Test]
        public void MudPopoverProvider_DropdownSettings_SetsDefaultValues()
        {
            var settings = new DropdownSettings();

            settings.Fixed.Should().BeFalse();
            settings.OverflowBehavior.Should().Be(OverflowBehavior.FlipOnOpen);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void MudPopoverProvider_DropdownSettings_Fixed_CanBeSetCorrectly(bool fixedValue)
        {
            var settings = new DropdownSettings { Fixed = fixedValue };

            settings.Fixed.Should().Be(fixedValue);
        }

        [Test]
        public void MudPopoverProvider_DropdownSettings_OverflowBehavior_CanBeSetCorrectly()
        {
            var settings = new DropdownSettings { OverflowBehavior = OverflowBehavior.FlipAlways };

            settings.OverflowBehavior.Should().Be(OverflowBehavior.FlipAlways);
        }
    }
}
