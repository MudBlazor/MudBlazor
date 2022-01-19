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
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
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
        public async Task MaskFieldTest_Fundamentals1()
        {
            var comp = Context.RenderComponent<MudMaskField<string>>();
            comp.SetParam(x => x.Mask, new SimpleMask("(aaa) 000-aa") { Placeholder = '_', CleanDelimiters = true });
            var maskField = comp;
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().BeNullOrEmpty());
            //Unmatched keys should have no effect
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "1" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(___) ___-__"));
            maskField.Instance.Value.Should().BeNullOrEmpty();
            maskField.Instance.Mask.ToString().Should().Be("(|___) ___-__");

            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "a" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(a__) ___-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("a"));
            comp.WaitForAssertion(() => maskField.Instance.Mask.CaretPos.Should().Be(2));

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
            comp.WaitForAssertion(() => maskField.Instance.Mask.CaretPos.Should().Be(6));

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
            comp.WaitForAssertion(() => maskField.Instance.Mask.CaretPos.Should().Be(10));

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
            await comp.InvokeAsync(() => maskField.Instance.OnCaretPositionChanged(9));
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "b" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(abC) 120-bA"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("abC120bA"));

            await comp.InvokeAsync(() => maskField.Instance.OnCaretPositionChanged(11));
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "c" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(abC) 120-bc"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("abC120bc"));
            comp.WaitForAssertion(() => maskField.Instance.Mask.CaretPos.Should().Be(12));

            await comp.InvokeAsync(
                () => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(abC) 120-b_"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("abC120b"));
            comp.WaitForAssertion(() => maskField.Instance.Mask.CaretPos.Should().Be(11));

            await comp.InvokeAsync(
                () => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(abC) 120-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("abC120"));

            await comp.InvokeAsync(
                () => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(abC) 12_-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("abC12"));
            comp.WaitForAssertion(() => maskField.Instance.Mask.CaretPos.Should().Be(8));

            await comp.InvokeAsync(
                () => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(abC) 1__-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("abC1"));

            await comp.InvokeAsync(
                () => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(abC) ___-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("abC"));

            await comp.InvokeAsync(
                () => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(ab_) ___-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("ab"));
            comp.WaitForAssertion(() => maskField.Instance.Mask.CaretPos.Should().Be(3));

            await comp.InvokeAsync(
                () => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(a__) ___-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("a"));

            await comp.InvokeAsync(
                () => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(___) ___-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be(""));
            comp.WaitForAssertion(() => maskField.Instance.Mask.CaretPos.Should().Be(1));
            //Backspace should have no effect on empty value
            await comp.InvokeAsync(
                () => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(___) ___-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be(""));
            comp.WaitForAssertion(() => maskField.Instance.Mask.CaretPos.Should().Be(0));
        }

        [Test]
        public async Task MaskFieldTest_Fundamentals2()
        {
            var comp = Context.RenderComponent<MudMaskField<string>>();
            comp.SetParam(x => x.Mask, new SimpleMask("(aaa) 000-aa") { Placeholder = '_', CleanDelimiters = true });
            var maskField = comp;

            await comp.InvokeAsync(() => maskField.Instance.OnFocused(new FocusEventArgs()));
            await comp.InvokeAsync(() => maskField.Instance.OnPaste( "abc120ac"));

            await comp.InvokeAsync(() => maskField.Instance.OnCaretPositionChanged(1));
            await comp.InvokeAsync(() => maskField.Instance.Mask.ToString().Should().Be("(|abc) 120-ac"));
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Delete" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(bca) ___-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("bca"));
            comp.WaitForAssertion(() => maskField.Instance.Mask.CaretPos.Should().Be(1));

            await comp.InvokeAsync(() => maskField.Instance.Clear());
            await comp.InvokeAsync(() => maskField.Instance.OnPaste( "abc120ac"));
            await comp.InvokeAsync(() => maskField.Instance.OnCaretPositionChanged(6));
            await comp.InvokeAsync(() => maskField.Instance.Mask.ToString().Should().Be("(abc) |120-ac"));
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            await comp.InvokeAsync(() => maskField.Instance.Mask.ToString().Should().Be("(ab|a) ___-__"));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(aba) ___-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("aba"));

            await comp.InvokeAsync(() => maskField.Instance.Clear());
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be(""));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be(""));

            await comp.InvokeAsync(() => maskField.Instance.OnFocused(new FocusEventArgs()));
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "a" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(a__) ___-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("a"));
            comp.WaitForAssertion(() => maskField.Instance.Mask.CaretPos.Should().Be(2));
        }

        [Test]
        public async Task MaskFieldTest_Int()
        {
            var comp = Context.RenderComponent<MudMaskField<int>>();
            comp.SetParam(x => x.Mask, new SimpleMask("(0)0-0)") { Placeholder = '_', CleanDelimiters = true });
            var maskField = comp;
        
            await comp.InvokeAsync(() => maskField.Instance.OnCaretPositionChanged(1));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be(0));
            //Unmatched keys should have no effect
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "a" }));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be(0));
        
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "1" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(1)_-_)"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be(1));
        
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "2" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(1)2-_)"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be(12));
        
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "3" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(1)2-3)"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be(123));
        
            await comp.InvokeAsync(
                () => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(1)2-_)"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be(12));
        
            await comp.InvokeAsync(
                () => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(1)_-_)"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be(1));
        
            await comp.InvokeAsync(
                () => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(_)_-_)"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be(0));
        }
        
        
        [Test]
        public async Task MaskFieldTest_InsertCharactersIntoMiddle()
        {
            var comp = Context.RenderComponent<MudMaskField<string>>();
            comp.SetParam(x => x.Mask, new SimpleMask("(aaa) 000-aa") { Placeholder = '_', CleanDelimiters = true });
            var maskField = comp;

            await comp.InvokeAsync(() => maskField.Instance.Mask.ToString().Should().Be("|"));
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "1" }));
            // 1 is not accepted because first mask position wants a letter
            comp.WaitForAssertion(() => maskField.Instance.Mask.ToString().Should().Be("(|___) ___-__"));
            await comp.InvokeAsync(() => maskField.Instance.OnCaretPositionChanged(6));
            comp.WaitForAssertion(() => maskField.Instance.Mask.ToString().Should().Be("(___) |___-__"));
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "1" }));
            comp.WaitForAssertion(() => maskField.Instance.Mask.ToString().Should().Be("(___) 1|__-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("1"));
        
            await comp.InvokeAsync(() => maskField.Instance.OnCaretPositionChanged(10));
            comp.WaitForAssertion(() => maskField.Instance.Mask.ToString().Should().Be("(___) 1__-|__"));
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "a" }));
            comp.WaitForAssertion(() => maskField.Instance.Mask.ToString().Should().Be("(___) 1__-a|_"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("1a"));
        
            await comp.InvokeAsync(() => maskField.Instance.OnCaretPositionChanged(1));
            comp.WaitForAssertion(() => maskField.Instance.Mask.ToString().Should().Be("(|___) 1__-a_"));
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "a" }));
            // mask collapses because that is how it is implemented in SimpleMask. If we want a mask that preserves position
            // we will have to craft a more complex algorithm. But I can not think of a use-case that is worth the effort
            comp.WaitForAssertion(() => maskField.Instance.Mask.ToString().Should().Be("(a|a_) ___-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("aa"));
        
            await comp.InvokeAsync(() => maskField.Instance.OnCaretPositionChanged(1));
            comp.WaitForAssertion(() => maskField.Instance.Mask.ToString().Should().Be("(|aa_) ___-__"));
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "b" }));
            comp.WaitForAssertion(() => maskField.Instance.Mask.ToString().Should().Be("(b|aa) ___-__"));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(baa) ___-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("baa"));
        }
        
        [Test]
        public async Task MaskFieldTest_ChangeMask1()
        {
            var comp = Context.RenderComponent<MudMaskField<string>>();
            comp.SetParam(x => x.Mask, new SimpleMask("(aaa) 000-aa") { Placeholder = '_', CleanDelimiters = true });
            var maskField = comp;
        
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "a" }));
            comp.WaitForAssertion(() => maskField.Instance.Mask.ToString().Should().Be("(a|__) ___-__"));
            // change the mask
            comp.SetParam(x => x.Mask, new SimpleMask("(bb+) 999-bb")
            {
                MaskChars = new MaskChar[] { MaskChar.Letter('b'), MaskChar.Digit('9'), MaskChar.LetterOrDigit('+'), },
                Placeholder = '_', 
                CleanDelimiters = true
            });
            // internal state is preserved!
            comp.WaitForAssertion(() => maskField.Instance.Mask.ToString().Should().Be("(a|__) ___-__"));
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "b" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(ab_) ___-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("ab"));
        }
        
        [Test]
        public async Task MaskFieldTest_ChangeMask2()
        {
            var comp = Context.RenderComponent<MudMaskField<string>>();
            comp.SetParam(x => x.Mask, new SimpleMask("(LL) UU") { Placeholder = '_', CleanDelimiters = true });
            var maskField = comp.Instance;

            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "a" }));
            comp.WaitForAssertion(() => maskField.Text.Should().Be("(a_) __"));
            comp.WaitForAssertion(() => maskField.Value.Should().Be("a"));
        
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "A" }));
            comp.WaitForAssertion(() => maskField.Text.Should().Be("(a_) __"));
            comp.WaitForAssertion(() => maskField.Value.Should().Be("a"));
        
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "a" }));
            comp.WaitForAssertion(() => maskField.Text.Should().Be("(aa) __"));
            comp.WaitForAssertion(() => maskField.Value.Should().Be("aa"));
        
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "A" }));
            comp.WaitForAssertion(() => maskField.Text.Should().Be("(aa) A_"));
            comp.WaitForAssertion(() => maskField.Value.Should().Be("aaA"));
        }
        
        // NOTE: Keep position is no longer supported by SimpleMask
        // [Test]
        // public async Task MaskFieldTest_KeepCharacterPositions()
        // {
        //     var comp = Context.RenderComponent<MudMaskField<string>>();
        //     var maskField = comp.Instance;
        //
        //     await comp.InvokeAsync(() => comp.SetParam("Mask", "(aaa) 000-aa"));
        //     await comp.InvokeAsync(() => comp.SetParam("KeepCharacterPositions", true));
        //
        //     await comp.InvokeAsync(() => maskField.OnCaretPositionChanged(1));
        //     await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "a" }));
        //     comp.WaitForAssertion(() => maskField.Text.Should().Be("(a__) ___-__"));
        //     comp.WaitForAssertion(() => maskField.Value.Should().Be("a"));
        //
        //     await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "b" }));
        //     comp.WaitForAssertion(() => maskField.Text.Should().Be("(ab_) ___-__"));
        //     comp.WaitForAssertion(() => maskField.Value.Should().Be("ab"));
        //
        //     await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "c" }));
        //     comp.WaitForAssertion(() => maskField.Text.Should().Be("(abc) ___-__"));
        //     comp.WaitForAssertion(() => maskField.Value.Should().Be("abc"));
        //
        //     await comp.InvokeAsync(() => maskField.OnCaretPositionChanged(3));
        //     await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
        //     comp.WaitForAssertion(() => maskField.Text.Should().Be("(a_c) ___-__"));
        //     comp.WaitForAssertion(() => maskField.Value.Should().Be("ac"));
        //
        //     await comp.InvokeAsync(() => maskField.OnCaretPositionChanged(6));
        //     await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "1" }));
        //     comp.WaitForAssertion(() => maskField.Text.Should().Be("(a_c) 1__-__"));
        //     comp.WaitForAssertion(() => maskField.Value.Should().Be("ac1"));
        //
        //     await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "0" }));
        //     comp.WaitForAssertion(() => maskField.Text.Should().Be("(a_c) 10_-__"));
        //     comp.WaitForAssertion(() => maskField.Value.Should().Be("ac10"));
        //
        //     await comp.InvokeAsync(() => maskField.OnCaretPositionChanged(1));
        //     await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "Delete" }));
        //     comp.WaitForAssertion(() => maskField.Text.Should().Be("(__c) 10_-__"));
        //     comp.WaitForAssertion(() => maskField.Value.Should().Be("c10"));
        //
        //     await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "Delete" }));
        //     comp.WaitForAssertion(() => maskField.Text.Should().Be("(__c) 10_-__"));
        //     comp.WaitForAssertion(() => maskField.Value.Should().Be("c10"));
        // }
        
        [Test]
        public async Task MaskFieldTest_Paste()
        {
            var comp = Context.RenderComponent<MudMaskField<string>>();
            comp.SetParam(x => x.Mask, new SimpleMask("(aaa) 000-aa") { Placeholder = '_', CleanDelimiters = true });
            var maskField = comp;
        
            await comp.InvokeAsync(() => maskField.Instance.OnPaste( "abc"));
        
            await comp.InvokeAsync(() => maskField.Instance.OnCaretPositionChanged(10));
            await comp.InvokeAsync(() => maskField.Instance.OnPaste("zxc"));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(abc) ___-zx"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("abczx"));
        
            await comp.InvokeAsync(() => maskField.Instance.OnCaretPositionChanged(2));
            await comp.InvokeAsync(() => maskField.Instance.OnPaste("defgh"));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(ade) ___-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("ade"));
        
            await comp.InvokeAsync(() => maskField.Instance.OnCaretPositionChanged(7));
            await comp.InvokeAsync(() => maskField.Instance.OnPaste("120"));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(ade) _12-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("ade12"));
            //Symbols should not be paste but remove the related index
            await comp.InvokeAsync(() => maskField.Instance.OnCaretPositionChanged(1));
            await comp.InvokeAsync(() => maskField.Instance.OnPaste("+-"));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(ade) 12_-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("ade12"));
        }
        
        [Test]
        public async Task MaskFieldTest_Selection()
        {
            var comp = Context.RenderComponent<MudMaskField<string>>();
            comp.SetParam(x => x.Mask, new SimpleMask("0000 0000 000") { Placeholder = '_', CleanDelimiters = true });
            var maskField = comp.Instance;
        
            await comp.InvokeAsync(() => maskField.OnPaste( "1234567899"));
            comp.WaitForAssertion(() => maskField.Mask.ToString().Should().Be("1234 5678 99|_"));
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "9" }));
            comp.WaitForAssertion(() => maskField.Mask.ToString().Should().Be("1234 5678 999|"));
            //Select and delete
            await comp.InvokeAsync(() => maskField.OnSelect(10, 12));
            comp.WaitForAssertion(() => maskField.Mask.ToString().Should().Be("1234 5678 [99]9"));
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField.Mask.ToString().Should().Be("1234 5678 |9__"));
            comp.WaitForAssertion(() => maskField.Text.Should().Be("1234 5678 9__"));
            comp.WaitForAssertion(() => maskField.Value.Should().Be("123456789"));
            //Select with a whitespace and test again
            await comp.InvokeAsync(() => maskField.OnSelect(4, 8));
            comp.WaitForAssertion(() => maskField.Mask.ToString().Should().Be("1234[ 567]8 9__"));
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "Delete" }));
            comp.WaitForAssertion(() => maskField.Mask.ToString().Should().Be("1234| 89__ ___"));
            comp.WaitForAssertion(() => maskField.Text.Should().Be("1234 89__ ___"));
            comp.WaitForAssertion(() => maskField.Value.Should().Be("123489"));
        
            await comp.InvokeAsync(() => maskField.OnSelect(7, 11));
            comp.WaitForAssertion(() => maskField.Mask.ToString().Should().Be("1234 89[__ _]__"));
            await comp.InvokeAsync(() => maskField.OnPaste("567"));
            comp.WaitForAssertion(() => maskField.Text.Should().Be("1234 8956 7__"));
            comp.WaitForAssertion(() => maskField.Value.Should().Be("123489567"));
        
            await comp.InvokeAsync(() => maskField.OnCaretPositionChanged(0));
            await comp.InvokeAsync(() => maskField.OnSelect(0, 1));
            comp.WaitForAssertion(() => maskField.Mask.ToString().Should().Be("[1]234 8956 7__"));
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "a" }));
            comp.WaitForAssertion(() => maskField.Mask.ToString().Should().Be("|2348 9567 ___"));
            comp.WaitForAssertion(() => maskField.Text.Should().Be("2348 9567 ___"));
            comp.WaitForAssertion(() => maskField.Value.Should().Be("23489567"));
        
            await comp.InvokeAsync(() => maskField.OnSelect(6, 11));
            comp.WaitForAssertion(() => maskField.Mask.ToString().Should().Be("2348 9[567 _]__"));
            await comp.InvokeAsync(() => maskField.OnPaste("1Mud9"));
            comp.WaitForAssertion(() => maskField.Text.Should().Be("2348 919_ ___"));
            comp.WaitForAssertion(() => maskField.Value.Should().Be("2348919"));
        }
        
        [Test]
        public async Task MaskFieldTest_TwoWayBinding()
        {
            var comp = Context.RenderComponent<MaskFieldTwoWayBindingTest>();
            var maskField1 = comp.FindComponents<MudMaskField<string>>().First();
            var maskField2 = comp.FindComponents<MudMaskField<string>>().Last();
            comp.WaitForAssertion(() => maskField1.Instance.Value.Should().Be(""));
        
            await comp.InvokeAsync(() => maskField1.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "a" }));
            comp.WaitForAssertion(() => maskField1.Instance.Text.Should().Be("(a"));
            comp.WaitForAssertion(() => maskField1.Instance.Value.Should().Be("a"));
        
            comp.WaitForAssertion(() => maskField2.Instance.Text.Should().Be("(a"));
            comp.WaitForAssertion(() => maskField2.Instance.Value.Should().Be("a"));
        
            await comp.InvokeAsync(() => maskField1.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "b" }));
            comp.WaitForAssertion(() => maskField1.Instance.Text.Should().Be("(ab"));
            comp.WaitForAssertion(() => maskField1.Instance.Value.Should().Be("ab"));
        
            comp.WaitForAssertion(() => maskField2.Instance.Text.Should().Be("(ab"));
            comp.WaitForAssertion(() => maskField2.Instance.Value.Should().Be("ab"));
        
            await comp.InvokeAsync(() => maskField1.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "C" }));
            comp.WaitForAssertion(() => maskField1.Instance.Text.Should().Be("(abC) "));
            comp.WaitForAssertion(() => maskField1.Instance.Value.Should().Be("abC"));
            comp.WaitForAssertion(() => maskField1.Instance.Mask.CaretPos.Should().Be(6));
        
            comp.WaitForAssertion(() => maskField2.Instance.Text.Should().Be("(abC) "));
            comp.WaitForAssertion(() => maskField2.Instance.Value.Should().Be("abC"));
        
            await comp.InvokeAsync(() => maskField1.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "1" }));
            comp.WaitForAssertion(() => maskField1.Instance.Text.Should().Be("(abC) 1"));
            comp.WaitForAssertion(() => maskField1.Instance.Value.Should().Be("abC1"));
        
            comp.WaitForAssertion(() => maskField2.Instance.Text.Should().Be("(abC) 1"));
            comp.WaitForAssertion(() => maskField2.Instance.Value.Should().Be("abC1"));
        
            await comp.InvokeAsync(() => maskField1.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "2" }));
            comp.WaitForAssertion(() => maskField1.Instance.Text.Should().Be("(abC) 12"));
            comp.WaitForAssertion(() => maskField1.Instance.Value.Should().Be("abC12"));
        
            comp.WaitForAssertion(() => maskField2.Instance.Text.Should().Be("(abC) 12"));
            comp.WaitForAssertion(() => maskField2.Instance.Value.Should().Be("abC12"));
        
            await comp.InvokeAsync(() =>
                maskField1.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField1.Instance.Text.Should().Be("(abC) 1"));
            comp.WaitForAssertion(() => maskField1.Instance.Value.Should().Be("abC1"));
        
            comp.WaitForAssertion(() => maskField2.Instance.Text.Should().Be("(abC) 1"));
            comp.WaitForAssertion(() => maskField2.Instance.Value.Should().Be("abC1"));
        
            await comp.InvokeAsync(() =>
                maskField1.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField1.Instance.Text.Should().Be("(abC) "));
            comp.WaitForAssertion(() => maskField1.Instance.Value.Should().Be("abC"));
        
            comp.WaitForAssertion(() => maskField2.Instance.Text.Should().Be("(abC) "));
            comp.WaitForAssertion(() => maskField2.Instance.Value.Should().Be("abC"));
        
            await comp.InvokeAsync(() => maskField1.Instance.OnPaste("123"));
            comp.WaitForAssertion(() => maskField1.Instance.Text.Should().Be("(abC) 123-"));
            comp.WaitForAssertion(() => maskField1.Instance.Value.Should().Be("abC123"));
            comp.WaitForAssertion(() => maskField2.Instance.Text.Should().Be("(abC) 123-"));
            comp.WaitForAssertion(() => maskField2.Instance.Value.Should().Be("abC123"));
        }
        
        [Test]
        public async Task MaskFieldTest_TimeSpan()
        {
            var comp = Context.RenderComponent<MudMaskField<TimeSpan>>();
            comp.SetParam(x => x.Mask, new SimpleMask("00:00") { CleanDelimiters = false, });
            var maskField = comp.Instance;
        
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "1" }));
            comp.WaitForAssertion(() => maskField.Text.Should().Be("1"));
            comp.WaitForAssertion(() => maskField.Value.Should().Be(TimeSpan.FromDays(1)));
        
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "2" }));
            comp.WaitForAssertion(() => maskField.Text.Should().Be("12:"));
            comp.WaitForAssertion(() => maskField.Value.Should().Be(TimeSpan.Zero));
        
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "3" }));
            comp.WaitForAssertion(() => maskField.Text.Should().Be("12:3"));
            comp.WaitForAssertion(() => maskField.Value.Should().Be(new TimeSpan(12,3,00)));
        
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "4" }));
            comp.WaitForAssertion(() => maskField.Text.Should().Be("12:34"));
            comp.WaitForAssertion(() => maskField.Value.Should().Be(new TimeSpan(12,34,00)));
        
            await comp.InvokeAsync(() => maskField.OnCaretPositionChanged(2));
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField.Text.Should().Be("13:4"));
            comp.WaitForAssertion(() => maskField.Value.Should().Be(new TimeSpan(13,4,00)));
        
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "Delete" }));
            comp.WaitForAssertion(() => maskField.Text.Should().Be("14:"));
            comp.WaitForAssertion(() => maskField.Value.Should().Be(TimeSpan.Zero));
        }
        //
        //     // [Test]
        //     // public async Task MaskFieldTest_Extreme()
        //     // {
        //     //     var comp = Context.RenderComponent<MudMaskField<string>>();
        //     //     var maskField = comp.Instance;
        //     //     var impl = maskField.Mask;
        //     //     comp.WaitForAssertion(() => maskField.Text.Should().Be(null));
        //     //     await comp.InvokeAsync(() => maskField.OnPaste("abc"));
        //     //     comp.WaitForAssertion(() => maskField.Text.Should().Be(null));
        //     //
        //     //     await comp.InvokeAsync(() => maskField.OnCaretPositionChanged(0));
        //     //     await comp.InvokeAsync(() => comp.SetParam("Placeholder", "Some Placeholder"));
        //     //     await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
        //     //     comp.WaitForAssertion(() => maskField.Text.Should().Be(null));
        //     //
        //     //     await comp.InvokeAsync(() => maskField.OnBlurred(new FocusEventArgs()));
        //     //     comp.WaitForAssertion(() => maskField.Text.Should().Be(""));
        //     //
        //     //     comp.WaitForAssertion(() => maskField.Value.Should().Be(""));
        //     //
        //     //     //comp.WaitForAssertion(() => maskField.GetCharacterType("").Should().Be(CharacterType.None));
        //     //
        //     //     comp.WaitForAssertion(() => maskField.GetInputType().Should().Be(InputType.Text));
        //     //
        //     //     await comp.InvokeAsync(() => maskField.OnCaretPositionChanged(2));
        //     //     comp.WaitForAssertion(() => impl.CaretPos.Should().Be(2));
        //     //
        //     //     await comp.InvokeAsync(() => maskField.Text = "");
        //     //     comp.WaitForAssertion(() => impl.GetRawValueFromText(maskField.Text).Should().Be(""));
        //     //
        //     //     comp.SetParam("Mask", "*00 000");
        //     //
        //     //     await comp.InvokeAsync(() => maskField.OnCopy());
        //     //     await comp.InvokeAsync(() => maskField.FocusAsync());
        //     //     await comp.InvokeAsync(() => maskField.OnCaretPositionChanged(impl.FindFirstCaretLocation(false)));
        //     //     await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "1" }));
        //     //     comp.WaitForAssertion(() => maskField.Text.Should().Be("1__ ___"));
        //     //     comp.WaitForAssertion(() => maskField.Value.Should().Be("1"));
        //     //
        //     //     await comp.InvokeAsync(() => maskField.SelectAsync());
        //     //     await comp.InvokeAsync(() => maskField.OnCaretPositionChanged(0));
        //     //     await comp.InvokeAsync(() => maskField.SelectRangeAsync(0, 7));
        //     //     await comp.InvokeAsync(() => maskField.OnSelect(0, 7));
        //     //     await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "2" }));
        //     //     comp.WaitForAssertion(() => maskField.Text.Should().Be("2__ ___"));
        //     //     comp.WaitForAssertion(() => maskField.Value.Should().Be("2"));
        //     //
        //     //     await comp.InvokeAsync(() => impl.ImplementMask(null));
        //     //     comp.WaitForAssertion(() => maskField.Text.Should().Be("2__ ___"));
        //     //     comp.WaitForAssertion(() => maskField.Value.Should().Be("2"));
        //     //
        //     //     await comp.InvokeAsync(() => maskField.OnCaretPositionChanged(0));
        //     //     await comp.InvokeAsync(() => maskField.OnFocused(new FocusEventArgs()));
        //     //     await comp.InvokeAsync(() => maskField.SetBothValueAndText("123"));
        //     //     //comp.WaitForAssertion(() => maskField.Text.Should().Be("123 ___"));
        //     //     comp.WaitForAssertion(() => maskField.Value.Should().Be("123"));
        //     // }
    }
}
