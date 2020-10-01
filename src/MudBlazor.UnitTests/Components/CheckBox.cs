using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bunit;
using FluentAssertions;
using NUnit.Framework;


namespace MudBlazor.UnitTests
{

    [TestFixture]
    public class CheckBoxTests
    {
        [Test]
        public void CheckBoxTest1() {
            // single checkbox, check -  uncheck
            // setup
            using var ctx = new Bunit.TestContext();
            var comp = ctx.RenderComponent<CheckBoxTest1>();
            // print the generated html
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var box = comp.FindComponent<MudCheckBox>();
            var input =comp.Find("input");
            // check initial state
            box.Instance.Checked.Should().Be(false);
            // click and check if it has toggled
            input.Change(true);
            box.Instance.Checked.Should().Be(true);
            input.Change(false);
            box.Instance.Checked.Should().Be(false);
        }

        [Test]
        public void CheckBoxTest2()
        {
            // single checkbox, initialized true, check -  uncheck
            // setup
            using var ctx = new Bunit.TestContext();
            var comp = ctx.RenderComponent<CheckBoxTest2>();
            // print the generated html
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var box = comp.FindComponent<MudCheckBox>();
            var input = comp.Find("input");
            // check initial state
            box.Instance.Checked.Should().Be(true);
            // click and check if it has toggled
            input.Change(false);
            box.Instance.Checked.Should().Be(false);
            input.Change(true);
            box.Instance.Checked.Should().Be(true);
        }

        [Test]
        public void CheckBoxTest3()
        {
            // there are two checkboxes synced via a bound variable, so checking one also check the other and vice versa.
            // setup
            using var ctx = new Bunit.TestContext();
            var comp = ctx.RenderComponent<CheckBoxTest3>();
            // print the generated html
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var boxes = comp.FindComponents<MudCheckBox>();
            var inputs = comp.FindAll("input");
            // check initial state
            boxes[0].Instance.Checked.Should().Be(true);
            boxes[1].Instance.Checked.Should().Be(true);
            // click and check if it has toggled
            inputs[0].Change(false);
            boxes[0].Instance.Checked.Should().Be(false);
            boxes[1].Instance.Checked.Should().Be(false);
            inputs = comp.FindAll("input");
            inputs[0].Change(true);
            boxes[0].Instance.Checked.Should().Be(true);
            boxes[1].Instance.Checked.Should().Be(true);
            inputs = comp.FindAll("input");
            inputs[1].Change(false);
            boxes[0].Instance.Checked.Should().Be(false);
            boxes[1].Instance.Checked.Should().Be(false);
            inputs = comp.FindAll("input");
            inputs[1].Change(true);
            boxes[0].Instance.Checked.Should().Be(true);
            boxes[1].Instance.Checked.Should().Be(true);
        }

    }
}
