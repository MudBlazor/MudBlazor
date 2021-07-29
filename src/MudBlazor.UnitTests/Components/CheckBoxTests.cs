﻿#pragma warning disable IDE1006 // leading underscore

using System;
using System.Linq;
using Bunit;
using FluentAssertions;
using MudBlazor.UnitTests.TestComponents;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{

    [TestFixture]
    public class CheckBoxTests
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
        /// single checkbox, initialized false, check -  uncheck
        /// </summary>
        [Test]
        public void CheckBoxTest1()
        {
            using var comp = ctx.RenderComponent<MudCheckBox<bool>>();
            // print the generated html
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var box = comp.Instance;
            var input = comp.Find("input");
            // check initial state
            box.Checked.Should().Be(false);
            // click and check if it has toggled
            input.Change(true);
            box.Checked.Should().Be(true);
            input.Change(false);
            box.Checked.Should().Be(false);
        }

        /// <summary>
        /// single checkbox, initialized true, check -  uncheck
        /// </summary>
        [Test]
        public void CheckBoxTest2()
        {
            using var comp = ctx.RenderComponent<MudCheckBox<bool>>(new[] { ComponentParameter.CreateParameter("Checked", true), });
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var box = comp.Instance;
            var input = comp.Find("input");
            // check initial state
            box.Checked.Should().Be(true);
            // click and check if it has toggled
            input.Change(false);
            box.Checked.Should().Be(false);
            input.Change(true);
            box.Checked.Should().Be(true);
        }

        /// <summary>
        /// there are two checkboxes synced via a bound variable, so checking one also check the other and vice versa.
        /// </summary>
        [Test]
        public void CheckBoxTest3()
        {
            using var comp = ctx.RenderComponent<CheckBoxTest3>();
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var boxes = comp.FindComponents<MudCheckBox<bool>>();
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

        /// <summary>
        /// Check the correct css classes are applied.
        /// </summary>

        [Test]
        public void CheckBoxTest4()
        {
            using var comp = ctx.RenderComponent<CheckBoxTest4>();
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var spans = comp.FindAll("span").ToArray();
            var svgs = comp.FindAll("svg").ToArray();
            // check dense
            spans[0].ClassList.Should().Contain("mud-checkbox-dense");
            spans[1].ClassList.Should().NotContain("mud-checkbox-dense");
            spans[2].ClassList.Should().NotContain("mud-checkbox-dense");
            spans[3].ClassList.Should().NotContain("mud-checkbox-dense");
            // check size
            svgs[0].ClassList.Should().Contain("mud-icon-size-medium");
            svgs[1].ClassList.Should().Contain("mud-icon-size-small");
            svgs[2].ClassList.Should().Contain("mud-icon-size-medium");
            svgs[3].ClassList.Should().Contain("mud-icon-size-large");
        }

        /// <summary>
        /// Check the implementation of the TriState parameter
        /// </summary>
        [Test]
        public void CheckBoxTriStateTest()
        {
            using var comp = ctx.RenderComponent<MudCheckBox<bool?>>(new[] { ComponentParameter.CreateParameter("TriState", true) });
            // print the generated html
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var box = comp.Instance;
            var input = comp.Find("input");
            // check initial state
            box.Checked.Should().Be(default);
            // click and check if it has toggled
            input.Change(true);
            box.Checked.Should().Be(true);
            Console.WriteLine(comp.Markup);
            input.Change(false);
            box.Checked.Should().Be(false);
            Console.WriteLine(comp.Markup);
            // click and check if this is the indeterminate value
            input.Change(false);
            box.Checked.Should().Be(default);
            Console.WriteLine(comp.Markup);
            // click and check if this is the true value
            input.Change(true);
            box.Checked.Should().Be(true);
            Console.WriteLine(comp.Markup);
        }

        /// <summary>
        /// Without clicking the required checkbox the form should not validate
        /// </summary>
        [Test]
        public void CheckBoxFormTest1()
        {
            using var comp = ctx.RenderComponent<CheckBoxFormTest1>();
            Console.WriteLine(comp.Markup);
            var form = comp.FindComponent<MudForm>().Instance;
            form.IsValid.Should().BeFalse();
            form.Errors.Length.Should().Be(0);
            var checkbox = comp.FindComponent<MudCheckBox<bool>>();
            // click the checkbox to make the form valid
            checkbox.Find("input").Change(true);
            form.IsValid.Should().BeTrue();
            // click the checkbox to make the form invalid again because the checkbox is required
            checkbox.Find("input").Change(false);
            checkbox.Instance.Error.Should().BeTrue();
            checkbox.Instance.ErrorText.Should().Be("You must agree");
            form.IsValid.Should().BeFalse();
            form.Errors.Length.Should().Be(1);
            form.Errors[0].Should().Be("You must agree");
            // click the checkbox to make the form valid again
            checkbox.Find("input").Change(true);
            form.IsValid.Should().BeTrue();
            checkbox.Instance.Error.Should().BeFalse();
            checkbox.Instance.ErrorText.Should().Be(null);
        }

        /// <summary>
        /// Binding checkboxes two-way against an array of bools
        /// </summary>
        [Test]
        public void CheckBoxesBindAgainstArrayTest()
        {
            using var comp = ctx.RenderComponent<CheckBoxesBindAgainstArrayTest>();
            Console.WriteLine(comp.Markup);
            comp.FindAll("p")[^1].TrimmedText().Should().Be("A=True, B=False, C=True, D=False, E=True");
            comp.FindAll("input")[0].Change(false);
            comp.FindAll("p")[^1].TrimmedText().Should().Be("A=False, B=False, C=True, D=False, E=True");
            comp.FindAll("input")[1].Change(true);
            comp.FindAll("p")[^1].TrimmedText().Should().Be("A=False, B=True, C=True, D=False, E=True");
        }
    }
}
