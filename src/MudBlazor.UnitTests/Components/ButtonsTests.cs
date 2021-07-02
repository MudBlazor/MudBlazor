#pragma warning disable CS1998 // async without await
#pragma warning disable IDE1006 // leading underscore

using System;
using System.Windows.Input;
using Bunit;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using static Bunit.ComponentParameterFactory;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class ButtonsTests
    {
        private Bunit.TestContext ctx;

        [SetUp]
        public void Setup()
        {
            ctx = new Bunit.TestContext();
            ctx.AddTestServices();
        }

        [TearDown]
        public void TearDown() => ctx.Dispose();

        /// <summary>
        /// MudButton whithout specifying HtmlTag, renders a button
        /// </summary>
        [Test]
        public void MudButtonShouldRenderAButtonByDefault()
        {
            var comp = ctx.RenderComponent<MudButton>();
            //no HtmlTag nor Link properties are set, so HtmlTag is button by default
            comp.Instance
                .HtmlTag
                .Should()
                .Be("button");
            //it is a button, and has by default stopPropagation on onclick
            comp.Markup
                .Replace(" ", string.Empty)
                .Should()
                .StartWith("<button")
                .And
                .Contain("stopPropagation");
        }

        /// <summary>
        /// MudButton renders an anchor element when Link is set
        /// </summary>
        [Test]
        public void MudButtonShouldRenderAnAnchorIfLinkIsSetAndIsNotDisabled()
        {
            var link = Parameter(nameof(MudButton.Link), "https://www.google.com");
            var target = Parameter(nameof(MudButton.Target), "_blank");
            var disabled = Parameter(nameof(MudButton.Disabled), true);
            var comp = ctx.RenderComponent<MudButton>(link, target);
            //Link property is set, so it has to render an anchor element
            comp.Instance
                .HtmlTag
                .Should()
                .Be("a");
            //Target property is set, so it must have the rel attribute set to noopener
            comp.Markup
                .Should()
                .Contain("rel=\"noopener\"");
            //it is an anchor and not contains stopPropagation 
            comp.Markup
                .Replace(" ", string.Empty)
                .Should()
                .StartWith("<a")
                .And
                .NotContain("__internal_stopPropagation_onclick");

            comp = ctx.RenderComponent<MudButton>(link, target, disabled);
            comp.Instance.HtmlTag.Should().Be("button");

        }

        /// <summary>
        /// MudButton whithout specifying HtmlTag, renders a button
        /// </summary>
        [Test]
        public void MudIconButtonShouldRenderAButtonByDefault()
        {
            var comp = ctx.RenderComponent<MudIconButton>();
            //no HtmlTag nor Link properties are set, so HtmlTag is button by default
            comp.Instance
                .HtmlTag
                .Should()
                .Be("button");
            //it is a button
            comp.Markup
                .Replace(" ", string.Empty)
                .Should()
                .StartWith("<button");
        }

        /// <summary>
        /// MudButton renders an anchor element when Link is set
        /// </summary>
        [Test]
        public void MudIconButtonShouldRenderAnAnchorIfLinkIsSet()
        {
            using var ctx = new Bunit.TestContext();
            var link = Parameter(nameof(MudIconButton.Link), "https://www.google.com");
            var target = Parameter(nameof(MudIconButton.Target), "_blank");
            var comp = ctx.RenderComponent<MudIconButton>(link, target);
            //Link property is set, so it has to render an anchor element
            comp.Instance
                .HtmlTag
                .Should()
                .Be("a");
            //Target property is set, so it must have the rel attribute set to noopener
            comp.Markup
                .Should()
                .Contain("rel=\"noopener\"");
            //it is an anchor
            comp.Markup
                .Replace(" ", string.Empty)
                .Should()
                .StartWith("<a");
        }

        /// <summary>
        /// MudButton whithout specifying HtmlTag, renders a button
        /// </summary>
        [Test]
        public void MudFabShouldRenderAButtonByDefault()
        {
            var comp = ctx.RenderComponent<MudFab>();
            //no HtmlTag nor Link properties are set, so HtmlTag is button by default
            comp.Instance
                .HtmlTag
                .Should()
                .Be("button");
            //it is a button
            comp.Markup
                .Replace(" ", string.Empty)
                .Should()
                .StartWith("<button");
        }

        /// <summary>
        /// MudButton renders an anchor element when Link is set
        /// </summary>
        [Test]
        public void MudFabShouldRenderAnAnchorIfLinkIsSet()
        {
            var link = Parameter(nameof(MudFab.Link), "https://www.google.com");
            var target = Parameter(nameof(MudFab.Target), "_blank");
            var comp = ctx.RenderComponent<MudFab>(link, target);
            //Link property is set, so it has to render an anchor element
            comp.Instance
                .HtmlTag
                .Should()
                .Be("a");
            //Target property is set, so it must have the rel attribute set to noopener
            comp.Markup
                .Should()
                .Contain("rel=\"noopener\"");
            //it is an anchor
            comp.Markup
                .Replace(" ", string.Empty)
                .Should()
                .StartWith("<a");
        }

        /// <summary>
        /// MudFab should only render an icon if one is specified.
        /// </summary>
        [Test]
        public void MudFabShouldNotRenderIconIfNoneSpecified()
        {
            var comp = ctx.RenderComponent<MudFab>();
            comp.Markup
                .Should()
                .NotContainAny("mud-icon-root");
        }

        /// <summary>
        /// MudIconButton should have a title tag/attribute if specified
        /// </summary>
        [Test]
        public void ShouldRenderTitle()
        {
            var title = "Title and tooltip";
            var icon = Parameter(nameof(MudIconButton.Icon), Icons.Filled.Add);
            var titleParam = Parameter(nameof(MudIconButton.Title), title);
            var comp = ctx.RenderComponent<MudIconButton>(icon, titleParam);
            comp.Find("svg Title").TextContent.Should().Be(title);

            icon = Parameter(nameof(MudIconButton.Icon), "customicon");
            comp.SetParametersAndRender(icon, titleParam);
            comp.Find("button span.mud-icon-button-label").InnerHtml.Trim().Should().StartWith("<span")
                .And.Contain("customicon")
                .And.Contain($"title=\"{title}\"");
        }

        /// <summary>
        /// MudButtons should respect <see cref="ICommand.CanExecute(object?)"/> and set <see cref="MudBaseButton.Disabled"/> accordingly.
        /// </summary>
        [Test]
        [TestCase(true, false)]
        [TestCase(false, true)]
        public void DisabledShouldMatchCanExecuteState(bool canExecuteResult, bool isDisabled)
        {
            var command = new Mock<ICommand>();
            command.Setup(x => x.CanExecute(null)).Returns(canExecuteResult);

            var commandParam = Parameter(nameof(MudButton.Command), command.Object);
            var comp = ctx.RenderComponent<MudButton>(commandParam);
            comp.Instance.Disabled.Should().Be(isDisabled);
        }

        /// <summary>
        /// MudButtons should react to <see cref="ICommand.CanExecuteChanged"/> and update <see cref="MudBaseButton.Disabled"/>.
        /// </summary>
        [Test]
        [TestCase(false, true, false)]
        [TestCase(true, false, true)]
        public void DisabledShouldChangeIfCanExecuteChanged(bool initialCanExecute, bool changedCanExecute, bool isDisabled)
        {
            var command = new Mock<ICommand>();
            command.Setup(x => x.CanExecute(null)).Returns(initialCanExecute);

            var commandParam = Parameter(nameof(MudButton.Command), command.Object);
            var comp = ctx.RenderComponent<MudButton>(commandParam);
            comp.Instance.Disabled.Should().Be(!isDisabled);

            command.Setup(x => x.CanExecute(null)).Returns(changedCanExecute);
            command.Raise(m => m.CanExecuteChanged += null, command.Object, EventArgs.Empty);
            comp.Instance.Disabled.Should().Be(isDisabled);
        }

        /// <summary>
        /// MudButtons should unhook <see cref="ICommand.CanExecuteChanged"/> when getting disposed to avoid memory leaks.
        /// </summary>
        [Test]
        public void ShouldUnhookEventWhenDisposed()
        {
            var command = new Mock<ICommand>();
            //command.SetupRemove(m => m.CanExecuteChanged -= It.IsAny<EventHandler>());

            var commandParam = Parameter(nameof(MudButton.Command), command.Object);
            var comp = ctx.RenderComponent<MudButton>(commandParam);
            ctx.Dispose();

            GC.Collect();
            GC.Collect();

            command.VerifyRemove(m => m.CanExecuteChanged -= It.IsAny<EventHandler>());
        }

        /// <summary>
        /// MudButtons should unhook old <see cref="ICommand.CanExecuteChanged"/> when a new command is set to avoid memory leaks.
        /// </summary>
        [Test]
        public void ShouldUnhookEventWhenCommandChanged()
        {
            var command = new Mock<ICommand>();
            //command.SetupRemove(m => m.CanExecuteChanged -= It.IsAny<EventHandler>());

            var commandParam = Parameter(nameof(MudButton.Command), command.Object);
            var comp = ctx.RenderComponent<MudButton>(commandParam);
            var newCommand = new Mock<ICommand>();
            comp.SetParam(nameof(MudButton.Command), newCommand.Object);

            command.VerifyRemove(m => m.CanExecuteChanged -= It.IsAny<EventHandler>());
        }
    }
}



