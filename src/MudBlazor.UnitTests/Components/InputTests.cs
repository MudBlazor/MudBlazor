
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using MudBlazor.UnitTests.TestComponents;
using NUnit.Framework;
using static Bunit.ComponentParameterFactory;
namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class InputTests : BunitTest
    {
        [Test]
        public void Input_Test()
        {
            var comp = Context.RenderComponent<MudInput<string>>();

            comp.Instance.OnInput(new ChangeEventArgs());
            comp.Find("input").PasteAsync(new Microsoft.AspNetCore.Components.Web.ClipboardEventArgs());
        }

        [Test]
        public void Input_TestHidden()
        {
            var comp = Context.RenderComponent<InputTestHidden>();
            var inputHidden = comp.FindComponent<MudInput<string>>();
            _ = inputHidden.Instance.FocusAsync();
        }

        [Test]
        public void RangeInput_Test()
        {
            var comp = Context.RenderComponent<RangeInputTestHidden>();
            var inputHidden = comp.FindComponent<MudRangeInput<string>>();

            inputHidden.Instance.FocusStartAsync();
            inputHidden.Instance.SelectStartAsync();
            inputHidden.Instance.SelectRangeStartAsync(0, 0);
            inputHidden.Instance.FocusEndAsync();
            inputHidden.Instance.SelectEndAsync();
            inputHidden.Instance.SelectRangeEndAsync(0, 0);

            inputHidden.Instance.TextStart = "";
            inputHidden.Instance.TextEnd.Should().Be("");
            inputHidden.Instance.TextEnd = "";
            inputHidden.Instance.TextEnd = null;
            inputHidden.Instance.TextEnd.Should().BeNull();

            //inputHidden.Instance.GetHashCode().Should().Be(12279601);
            Range<string> r = new();
            r.GetHashCode();
        }
    }
}
