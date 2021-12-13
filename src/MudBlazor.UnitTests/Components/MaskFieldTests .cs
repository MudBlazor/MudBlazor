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
        /// <summary>
        /// Test all IsMatch variants: letter, digit and symbols.
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task MaskFieldTest_Fundamentals()
        {
            var comp = Context.RenderComponent<MaskFieldStringTest>();

            var maskField = comp.FindComponent<MudMaskField<string>>();

            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be(""));
            //Unmatched keys should have no effect
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "1" }));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be(""));

            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "a" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(a__) ___-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("a"));

            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "b" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(ab_) ___-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("ab"));
            //Symbols should have no effect in letter
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "+" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(ab_) ___-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("ab"));
            //Symbol as a mask character should have no effect in letter
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "*" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(ab_) ___-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("ab"));
            //Check uppercase character
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "C" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(abC) ___-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("abC"));

            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "d" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(abC) ___-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("abC"));

            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "1" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(abC) 1__-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("abC1"));
            //Symbols should have no effect in digit
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "+" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(abC) 1__-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("abC1"));
            //Symbol as a mask character should have no effect in letter
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "*" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(abC) 1__-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("abC1"));

            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "2" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(abC) 12_-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("abC12"));

            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "0" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(abC) 120-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("abC120"));

            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "A" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(abC) 120-A_"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("abC120A"));
            //Check culture character
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "ı" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(abC) 120-Aı"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("abC120Aı"));
            //Keys should have no effect if the mask completed
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Z" }));
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "0" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(abC) 120-Aı"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("abC120Aı"));

            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(abC) 120-A_"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("abC120A"));

            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(abC) 120-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("abC120"));

            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(abC) 12_-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("abC12"));

            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(abC) 1__-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("abC1"));

            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(abC) ___-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("abC"));

            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(ab_) ___-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("ab"));

            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(a__) ___-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("a"));

            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(___) ___-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be(""));
            //Backspace should have no effect on empty value
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(___) ___-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be(""));
        }

        [Test]
        public async Task MaskFieldTest_Int()
        {
            var comp = Context.RenderComponent<MaskFieldIntTest>();

            var maskField = comp.FindComponent<MudMaskField<int>>();

            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be(0));
            //Unmatched keys should have no effect
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "a" }));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be(0));

            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "1" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(1)_-_"));
            comp.WaitForAssertion(() => maskField.Instance.GetRawValue().Should().Be("1"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be(1));

            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "2" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(1)2-_"));
            comp.WaitForAssertion(() => maskField.Instance.GetRawValue().Should().Be("12"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be(12));

            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "3" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(1)2-3"));
            comp.WaitForAssertion(() => maskField.Instance.GetRawValue().Should().Be("123"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be(123));

            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(1)2-_"));
            comp.WaitForAssertion(() => maskField.Instance.GetRawValue().Should().Be("12"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be(12));

            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(1)_-_"));
            comp.WaitForAssertion(() => maskField.Instance.GetRawValue().Should().Be("1"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be(1));

            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(_)_-_"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be(0));
        }

        [Test]
        public async Task MaskFieldTest_PlaceholderCheck()
        {

        }

        [Test]
        public async Task MaskFieldTest_InsertCharactersIntoMiddle()
        {

        }

        [Test]
        public async Task MaskFieldTest_ChangeMaskCharacters()
        {

        }

        [Test]
        public async Task MaskFieldTest_RegexMasking()
        {

        }

        [Test]
        public async Task MaskFieldTest_KeepCharacterPositions()
        {

        }

        [Test]
        public async Task MaskFieldTest_CopyandPaste()
        {

        }

        [Test]
        public async Task MaskFieldTest_CaretPosition()
        {

        }

    }
}
