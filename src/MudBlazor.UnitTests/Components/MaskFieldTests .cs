// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
            comp.WaitForAssertion(() => maskField.Instance._caretPosition.Should().Be(0));

            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "a" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(a__) ___-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("a"));
            comp.WaitForAssertion(() => maskField.Instance._caretPosition.Should().Be(2));

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
            comp.WaitForAssertion(() => maskField.Instance._caretPosition.Should().Be(6));

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
            comp.WaitForAssertion(() => maskField.Instance._caretPosition.Should().Be(10));

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
            //Middle input should move the after characters
            await comp.InvokeAsync(() => maskField.Instance.SetCaretPosition(9));
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "b" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(abC) 120-bA"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("abC120bA"));

            await comp.InvokeAsync(() => maskField.Instance.SetCaretPosition(11));
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "c" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(abC) 120-bc"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("abC120bc"));
            comp.WaitForAssertion(() => maskField.Instance._caretPosition.Should().Be(12));

            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(abC) 120-b_"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("abC120b"));
            comp.WaitForAssertion(() => maskField.Instance._caretPosition.Should().Be(11));

            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(abC) 120-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("abC120"));

            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(abC) 12_-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("abC12"));
            comp.WaitForAssertion(() => maskField.Instance._caretPosition.Should().Be(8));

            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(abC) 1__-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("abC1"));

            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(abC) ___-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("abC"));

            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(ab_) ___-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("ab"));
            comp.WaitForAssertion(() => maskField.Instance._caretPosition.Should().Be(3));

            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(a__) ___-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("a"));

            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(___) ___-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be(""));
            comp.WaitForAssertion(() => maskField.Instance._caretPosition.Should().Be(1));
            //Backspace should have no effect on empty value
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(___) ___-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be(""));
            comp.WaitForAssertion(() => maskField.Instance._caretPosition.Should().Be(1));

            await comp.InvokeAsync(() => maskField.Instance.Value = "abc120ac");
            await comp.InvokeAsync(() => maskField.Instance.SetRawValueDictionary("abc120ac"));
            await comp.InvokeAsync(() => maskField.Instance.ImplementMask(null, maskField.Instance.Mask));

            await comp.InvokeAsync(() => maskField.Instance.SetCaretPosition(1));
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Delete" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(bc_) 20_-c_"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("bc20c"));
            comp.WaitForAssertion(() => maskField.Instance._caretPosition.Should().Be(1));

            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(bc_) 20_-c_"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("bc20c"));

            await comp.InvokeAsync(() => maskField.Instance.SetCaretPosition(0));
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(bc_) 20_-c_"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("bc20c"));

            await comp.InvokeAsync(() => maskField.Instance.Clear());
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be(null));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be(null));

            await comp.InvokeAsync(() => maskField.Instance.OnFocused(new FocusEventArgs()));
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "a" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(a__) ___-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("a"));
            comp.WaitForAssertion(() => maskField.Instance._caretPosition.Should().Be(2));

            await comp.InvokeAsync(() => maskField.Instance.SetCaretPosition(maskField.Instance.FindLastCaretLocation()));
            comp.WaitForAssertion(() => maskField.Instance._caretPosition.Should().Be(12));
        }

        [Test]
        public async Task MaskFieldTest_Int()
        {
            var comp = Context.RenderComponent<MaskFieldIntTest>();

            var maskField = comp.FindComponent<MudMaskField<int>>();

            await comp.InvokeAsync(() => maskField.Instance.SetCaretPosition(1));
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
            var comp = Context.RenderComponent<MaskFieldStringPlaceholderTest>();
            var maskField = comp.FindComponent<MudMaskField<string>>();

            await comp.InvokeAsync(() => maskField.Instance.OnFocused(new FocusEventArgs()));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be(null));

            await comp.InvokeAsync(() => maskField.Instance.SetCaretPosition(1));
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "a" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(a__) ___-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("a"));

            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be(null));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be(""));
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
            var comp = Context.RenderComponent<MaskFieldStringTest>();
            var maskField = comp.FindComponent<MudMaskField<string>>();

            await comp.InvokeAsync(() => maskField.Instance.SetCaretPosition(1));
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "a" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(a__) ___-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("a"));

            await comp.InvokeAsync(() => maskField.Instance.MaskCharacters = new Dictionary<char, CharacterType>()
            {
                ['b'] = CharacterType.Letter,
                ['9'] = CharacterType.Digit,
                ['+'] = CharacterType.LetterOrDigit,
            });
            await comp.InvokeAsync(() => maskField.Instance.Mask = "(bbb) 999-bb");
            await comp.InvokeAsync(() => maskField.Instance.OnFocused(new FocusEventArgs()));

            await comp.InvokeAsync(() => maskField.Instance.SetCaretPosition(2));
            await comp.InvokeAsync(() => maskField.Instance.ImplementMask(null, maskField.Instance.Mask, true));
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "b" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(ab_) ___-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("ab"));
        }

        [Test]
        public async Task MaskFieldTest_RegexMasking()
        {
            var comp = Context.RenderComponent<MaskFieldStringTest>();
            var maskField = comp.FindComponent<MudMaskField<string>>();

            await comp.InvokeAsync(() => maskField.Instance.Mask = "(cc) ee");
            await comp.InvokeAsync(() => maskField.Instance.OnFocused(new FocusEventArgs()));

            await comp.InvokeAsync(() => maskField.Instance.SetCaretPosition(1));
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "a" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(a_) __"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("a"));
            //Need to set mask parameter for each condition, don't know why
            await comp.InvokeAsync(() => maskField.Instance.Mask = "(cc) ee");
            await comp.InvokeAsync(() => maskField.Instance.OnFocused(new FocusEventArgs()));
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "A" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(a_) __"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("a"));

            await comp.InvokeAsync(() => maskField.Instance.Mask = "(cc) ee");
            await comp.InvokeAsync(() => maskField.Instance.OnFocused(new FocusEventArgs()));
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "a" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(aa) __"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("aa"));

            await comp.InvokeAsync(() => maskField.Instance.Mask = "(cc) ee");
            await comp.InvokeAsync(() => maskField.Instance.OnFocused(new FocusEventArgs()));
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "A" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(aa) A_"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("aaA"));
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
        public async Task MaskFieldTest_Paste()
        {
            var comp = Context.RenderComponent<MaskFieldStringTest>();
            var maskField = comp.FindComponent<MudMaskField<string>>();

            await comp.InvokeAsync(() => maskField.Instance.Value = "abc");
            await comp.InvokeAsync(() => maskField.Instance.SetRawValueDictionary("abc"));
            await comp.InvokeAsync(() => maskField.Instance.ImplementMask(null, maskField.Instance.Mask));

            await comp.InvokeAsync(() => maskField.Instance.SetCaretPosition(10));
            await comp.InvokeAsync(() => maskField.Instance.OnPaste("zxc"));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(abc) ___-zx"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("abczx"));

            await comp.InvokeAsync(() => maskField.Instance.SetCaretPosition(2));
            await comp.InvokeAsync(() => maskField.Instance.OnPaste("defgh"));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(ade) ___-zx"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("adezx"));

            await comp.InvokeAsync(() => maskField.Instance.SetCaretPosition(7));
            await comp.InvokeAsync(() => maskField.Instance.OnPaste("120"));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(ade) _12-_x"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("ade12x"));
            //Symbols should not be paste but remove the related index
            await comp.InvokeAsync(() => maskField.Instance.SetCaretPosition(1));
            await comp.InvokeAsync(() => maskField.Instance.OnPaste("+-"));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(__e) _12-_x"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("e12x"));
        }

        [Test]
        public async Task MaskFieldTest_Selection()
        {
            var comp = Context.RenderComponent<MudMaskField<string>>();
            var maskField = comp.Instance;

            await comp.InvokeAsync(() => maskField.Mask = "0000 0000 000");
            await comp.InvokeAsync(() => maskField.Value = "1234567899");
            await comp.InvokeAsync(() => maskField.OnFocused(new FocusEventArgs()));
            await comp.InvokeAsync(() => maskField.SetRawValueDictionary("1234567899"));
            await comp.InvokeAsync(() => maskField.ImplementMask(null, maskField.Mask));
            //await comp.InvokeAsync(() => maskField.OnFocused(new FocusEventArgs()));

            await comp.InvokeAsync(() => maskField.SetCaretPosition(12));
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "9" }));
            comp.WaitForAssertion(() => maskField.Text.Should().Be("1234 5678 999"));
            //Select and delete
            await comp.InvokeAsync(() => maskField.OnSelect(10, 12));
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField.Text.Should().Be("1234 5678 9__"));
            comp.WaitForAssertion(() => maskField.Value.Should().Be("123456789"));
            //Select with a whitespace and test again
            await comp.InvokeAsync(() => maskField.OnSelect(4, 8));
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "Delete" }));
            comp.WaitForAssertion(() => maskField.Text.Should().Be("1234 89__ ___"));
            comp.WaitForAssertion(() => maskField.Value.Should().Be("123489"));

            await comp.InvokeAsync(() => maskField.SetCaretPosition(7));
            await comp.InvokeAsync(() => maskField.OnSelect(7, 11));
            await comp.InvokeAsync(() => maskField.OnPaste("567"));
            comp.WaitForAssertion(() => maskField.Text.Should().Be("1234 8956 7__"));
            comp.WaitForAssertion(() => maskField.Value.Should().Be("123489567"));

            await comp.InvokeAsync(() => maskField.SetCaretPosition(0));
            await comp.InvokeAsync(() => maskField.OnSelect(0, 1));
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "a" }));
            comp.WaitForAssertion(() => maskField.Text.Should().Be("_234 8956 7__"));
            comp.WaitForAssertion(() => maskField.Value.Should().Be("23489567"));

            await comp.InvokeAsync(() => maskField.SetCaretPosition(0));
            await comp.InvokeAsync(() => maskField.OnSelect(0, 1));
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "1" }));
            comp.WaitForAssertion(() => maskField.Text.Should().Be("1234 8956 7__"));
            comp.WaitForAssertion(() => maskField.Value.Should().Be("123489567"));

            await comp.InvokeAsync(() => maskField.SetCaretPosition(6));
            await comp.InvokeAsync(() => maskField.OnSelect(6, 11));
            await comp.InvokeAsync(() => maskField.OnPaste("1Mud9"));
            comp.WaitForAssertion(() => maskField.Text.Should().Be("1234 81__ _9_"));
            comp.WaitForAssertion(() => maskField.Value.Should().Be("1234819"));

            await comp.InvokeAsync(() => maskField.KeepCharacterPositions = true);
            await comp.InvokeAsync(() => maskField.SetCaretPosition(1));
            await comp.InvokeAsync(() => maskField.OnSelect(1, 3));
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField.Text.Should().Be("1__4 81__ _9_"));
            comp.WaitForAssertion(() => maskField.Value.Should().Be("14819"));

            await comp.InvokeAsync(() => maskField.SetCaretPosition(3));
            await comp.InvokeAsync(() => maskField.OnSelect(3, 7));
            await comp.InvokeAsync(() => maskField.OnPaste("a1a"));
            comp.WaitForAssertion(() => maskField.Text.Should().Be("1___ 1___ _9_"));
            comp.WaitForAssertion(() => maskField.Value.Should().Be("119"));
        }

    }
}
