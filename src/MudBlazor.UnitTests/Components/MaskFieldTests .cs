﻿#pragma warning disable CS1998 // async without await
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
            var comp = Context.RenderComponent<MaskFieldStringTest>();
            var maskField = comp.FindComponent<MudMaskField<string>>();

            await comp.InvokeAsync(() => maskField.Instance.SetCaretPosition(6));
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "1" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(___) 1__-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("1"));

            await comp.InvokeAsync(() => maskField.Instance.SetCaretPosition(10));
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "a" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(___) 1__-a_"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("1a"));

            await comp.InvokeAsync(() => maskField.Instance.SetCaretPosition(1));
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "a" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(a__) 1__-a_"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("a1a"));

            await comp.InvokeAsync(() => maskField.Instance.SetCaretPosition(1));
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "b" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(ba_) 1__-a_"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("ba1a"));
        }

        [Test]
        public async Task MaskFieldTest_ChangeMaskCharacters()
        {
            //var comp = Context.RenderComponent<MaskFieldStringTest>();
            //var maskField = comp.FindComponent<MudMaskField<string>>();

            //await comp.InvokeAsync(() => maskField.Instance.SetCaretPosition(1));
            //await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "a" }));
            //comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(a__) ___-__"));
            //comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("a"));

            //await comp.InvokeAsync(() => maskField.Instance.MaskCharacters = new Dictionary<char, CharacterType>()
            //{
            //    ['b'] = CharacterType.Letter,
            //    ['9'] = CharacterType.Digit,
            //    ['+'] = CharacterType.LetterOrDigit,
            //});
            //await comp.InvokeAsync(() => maskField.Instance.Mask = "(bbb) 999-bb");

            //await comp.InvokeAsync(() => maskField.Instance.SetCaretPosition(2));
            //await comp.InvokeAsync(() => maskField.Instance.ImplementMask(null, maskField.Instance.Mask));
            //await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "b" }));
            //comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(ab_) ___-__"));
            //comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("ab"));
        }

        [Test]
        public async Task MaskFieldTest_RegexMasking()
        {
            //var comp = Context.RenderComponent<MaskFieldStringTest>();
            //var maskField = comp.FindComponent<MudMaskField<string>>();

            //await maskField.InvokeAsync(() => maskField.SetParam("Mask", "(cc) ee"));
            //await comp.InvokeAsync(() => maskField.Instance.SetCaretPosition(1));
            //await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "a" }));
            //comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(a_) __"));
            //comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("a"));
        }

        [Test]
        public async Task MaskFieldTest_KeepCharacterPositions()
        {
            var comp = Context.RenderComponent<MaskFieldStringTest>();
            var maskField = comp.FindComponent<MudMaskField<string>>();

            await maskField.InvokeAsync(() => maskField.SetParam("KeepCharacterPositions", true));

            await comp.InvokeAsync(() => maskField.Instance.SetCaretPosition(1));
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "a" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(a__) ___-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("a"));

            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "b" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(ab_) ___-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("ab"));

            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "c" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(abc) ___-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("abc"));

            await comp.InvokeAsync(() => maskField.Instance.SetCaretPosition(3));
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(a_c) ___-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("ac"));

            await comp.InvokeAsync(() => maskField.Instance.SetCaretPosition(6));
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "1" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(a_c) 1__-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("ac1"));

            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "0" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(a_c) 10_-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("ac10"));

            await comp.InvokeAsync(() => maskField.Instance.SetCaretPosition(1));
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Delete" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(__c) 10_-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("c10"));

            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Delete" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(__c) 10_-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("c10"));
        }

        [Test]
        public async Task MaskFieldTest_CopyandPaste()
        {
            var comp = Context.RenderComponent<MaskFieldStringTest>();
            var maskField = comp.FindComponent<MudMaskField<string>>();

            await comp.InvokeAsync(() => maskField.Instance.SetCaretPosition(1));
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "a" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(a__) ___-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("a"));

            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "b" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(ab_) ___-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("ab"));

            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "c" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(abc) ___-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("abc"));

            //await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "a", CtrlKey = true }));
            //await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "c", CtrlKey = true }));
            //await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "v", CtrlKey = true }));
            await comp.InvokeAsync(() => maskField.Instance.SetCaretPosition(10));
            await comp.InvokeAsync(() => maskField.Instance.OnPaste("zxc"));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(___) ___-zx"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("zx"));

            await comp.InvokeAsync(() => maskField.Instance.SetCaretPosition(1));
            await comp.InvokeAsync(() => maskField.Instance.OnPaste("zxc"));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(zxc) ___-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("zxc"));
        }

        [Test]
        public async Task MaskFieldTest_CaretPosition()
        {

        }

    }
}
