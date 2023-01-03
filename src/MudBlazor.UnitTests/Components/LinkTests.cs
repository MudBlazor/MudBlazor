
#pragma warning disable CS1998 // async without await

using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components.Web;
using NUnit.Framework;
using static Bunit.ComponentParameterFactory;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class LinkTests : BunitTest
    {
        [Test]
        public async Task NavLink_CheckDisabled()
        {
            var comp = Context.RenderComponent<MudLink>(
                Parameter(nameof(MudLink.Href), "#"),
                Parameter(nameof(MudLink.Disabled), true));
            comp.Find("a").GetAttribute("href").Should().BeNullOrEmpty();
        }

        /// <summary>
        /// Link should execute OnClick delegate when is not disabled
        /// </summary>
        [Test]
        public async Task LinkShouldExecuteOnClickDelegate()
        {
            var calls = 0;
            var onClickDelegate = EventCallback<MouseEventArgs>(nameof(MudLink.OnClick), e => calls++);

            var comp = Context.RenderComponent<MudLink>(onClickDelegate);
            await comp.Find("a").ClickAsync(new MouseEventArgs());
            
            calls.Should().Be(1);
        }

        /// <summary>
        /// Link should not execute OnClick delegate when is disabled
        /// </summary>
        [Test]
        public async Task LinkShouldNotExecuteOnClickDelegateWhenDisabled()
        {
            var calls = 0;
            var onClickDelegate = EventCallback<MouseEventArgs>(nameof(MudLink.OnClick), e => calls++);
            var isDisabled = Parameter(nameof(MudLink.Disabled), true);

            var comp = Context.RenderComponent<MudLink>(onClickDelegate, isDisabled);
            await comp.Find("a").ClickAsync(new MouseEventArgs());

            calls.Should().Be(0);
        }
    }
}
