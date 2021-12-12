#pragma warning disable CS1998 // async without await
#pragma warning disable BL0005 // Set parameter outside component

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.UnitTests.TestComponents;
using NUnit.Framework;
using static Bunit.ComponentParameterFactory;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class MaskFieldTests : BunitTest
    {
        [Test]
        public async Task MaskFieldTest_MaskandUnmask()
        {
            var comp = Context.RenderComponent<MaskFieldStringTest>();

            var maskField = comp.FindComponent<MudMaskField<string>>();

            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be(""));
            //Unmatched keys should have no effect
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "1" }));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be(""));

            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "a" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(a__) ___-____"));
            comp.WaitForAssertion(() => maskField.Instance.GetRawValue().Should().Be("a"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("a"));

            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "b" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(ab_) ___-____"));
            comp.WaitForAssertion(() => maskField.Instance.GetRawValue().Should().Be("ab"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("ab"));

            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "c" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(abc) ___-____"));
            comp.WaitForAssertion(() => maskField.Instance.GetRawValue().Should().Be("abc"));

            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "d" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(abc) ___-____"));
            comp.WaitForAssertion(() => maskField.Instance.GetRawValue().Should().Be("abc"));

            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "1" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(abc) 1__-____"));
            comp.WaitForAssertion(() => maskField.Instance.GetRawValue().Should().Be("abc1"));

            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(abc) ___-____"));
            comp.WaitForAssertion(() => maskField.Instance.GetRawValue().Should().Be("abc"));

            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(ab_) ___-____"));
            comp.WaitForAssertion(() => maskField.Instance.GetRawValue().Should().Be("ab"));
        }
        
    }
}
