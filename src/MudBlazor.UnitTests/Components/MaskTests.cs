// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#pragma warning disable CS1998 // async without await

using System;
using System.Linq;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.UnitTests.TestComponents;
using MudBlazor.UnitTests.TestComponents.Mask;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class MaskTests : BunitTest
    {
        /// <summary>
        /// Test all IsMatch variants: letter, digit and symbols.
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task MaskTest_Fundamentals1()
        {
            var comp = Context.RenderComponent<MudMask>();
            comp.SetParam(x => x.Mask, new PatternMask("(aaa) 000-aa") { Placeholder = '_', CleanDelimiters = true });
            var maskField = comp;
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().BeNullOrEmpty());
            //Unmatched keys should have no effect
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "1" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be(""));
            maskField.Instance.Value.Should().BeNullOrEmpty();
            maskField.Instance.Mask.ToString().Should().Be("|");

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

            //Backspace should have no effect on empty value
            await comp.InvokeAsync(
                () => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be(""));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be(""));
            comp.WaitForAssertion(() => maskField.Instance.Mask.CaretPos.Should().Be(0));
            await comp.InvokeAsync(
                () => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be(""));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be(""));
            comp.WaitForAssertion(() => maskField.Instance.Mask.CaretPos.Should().Be(0));
        }

        [Test]
        public async Task MaskTest_Fundamentals2()
        {
            var comp = Context.RenderComponent<MudMask>();
            comp.SetParam(x => x.Mask, new PatternMask("(aaa) 000-aa") { Placeholder = '_', CleanDelimiters = true });
            var maskField = comp;

            await comp.InvokeAsync(() => maskField.Instance.OnFocused(new FocusEventArgs()));
            await comp.InvokeAsync(() => maskField.Instance.OnPaste("abc120ac"));

            await comp.InvokeAsync(() => maskField.Instance.OnCaretPositionChanged(1));
            await comp.InvokeAsync(() => maskField.Instance.Mask.ToString().Should().Be("(|abc) 120-ac"));
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Delete" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(bca) ___-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("bca"));
            comp.WaitForAssertion(() => maskField.Instance.Mask.CaretPos.Should().Be(1));

            await comp.InvokeAsync(() => maskField.Instance.Clear());
            await comp.InvokeAsync(() => maskField.Instance.OnPaste("abc120ac"));
            await comp.InvokeAsync(() => maskField.Instance.OnCaretPositionChanged(6));
            await comp.InvokeAsync(() => maskField.Instance.Mask.ToString().Should().Be("(abc) |120-ac"));
            await comp.InvokeAsync(
                () => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
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
        public async Task MaskTest_Int()
        {
            var comp = Context.RenderComponent<MudTextField<int?>>();
            comp.SetParam(x => x.Mask, new PatternMask("(0)0-0)") { Placeholder = '_', CleanDelimiters = true });
            var tf = comp.Instance;
            var maskField = comp.FindComponent<MudMask>();

            await comp.InvokeAsync(() => maskField.Instance.OnCaretPositionChanged(1));
            comp.WaitForAssertion(() => tf.Value.Should().Be(null));
            //Unmatched keys should have no effect
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "a" }));
            comp.WaitForAssertion(() => tf.Value.Should().Be(null));

            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "1" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(1)_-_)"));
            comp.WaitForAssertion(() => tf.Value.Should().Be(1));

            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "2" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(1)2-_)"));
            comp.WaitForAssertion(() => tf.Value.Should().Be(12));

            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "3" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(1)2-3)"));
            comp.WaitForAssertion(() => tf.Value.Should().Be(123));

            await comp.InvokeAsync(
                () => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(1)2-_)"));
            comp.WaitForAssertion(() => tf.Value.Should().Be(12));

            await comp.InvokeAsync(
                () => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(1)_-_)"));
            comp.WaitForAssertion(() => tf.Value.Should().Be(1));

            await comp.InvokeAsync(
                () => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be(""));
            comp.WaitForAssertion(() => tf.Value.Should().Be(null));
        }

        [Test]
        public async Task MaskTest_InsertCharactersIntoMiddle()
        {
            var comp = Context.RenderComponent<MudMask>();
            comp.SetParam(x => x.Mask, new PatternMask("(aaa) 000-aa") { Placeholder = '_', CleanDelimiters = true });
            var maskField = comp;

            await comp.InvokeAsync(() => maskField.Instance.Mask.ToString().Should().Be("|"));
            // 1 is not accepted because first mask position wants a letter
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "1" }));
            await comp.InvokeAsync(() => maskField.Instance.Mask.ToString().Should().Be("|"));

            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "a" }));
            comp.WaitForAssertion(() => maskField.Instance.Mask.ToString().Should().Be("(a|__) ___-__"));
            await comp.InvokeAsync(() => maskField.Instance.OnCaretPositionChanged(6));
            comp.WaitForAssertion(() => maskField.Instance.Mask.ToString().Should().Be("(a__) |___-__"));
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "1" }));
            comp.WaitForAssertion(() => maskField.Instance.Mask.ToString().Should().Be("(a__) 1|__-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("a1"));

            await comp.InvokeAsync(() => maskField.Instance.OnCaretPositionChanged(10));
            comp.WaitForAssertion(() => maskField.Instance.Mask.ToString().Should().Be("(a__) 1__-|__"));
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "a" }));
            comp.WaitForAssertion(() => maskField.Instance.Mask.ToString().Should().Be("(a__) 1__-a|_"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("a1a"));

            await comp.InvokeAsync(() => maskField.Instance.OnCaretPositionChanged(1));
            comp.WaitForAssertion(() => maskField.Instance.Mask.ToString().Should().Be("(|a__) 1__-a_"));
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "b" }));
            comp.WaitForAssertion(() => maskField.Instance.Mask.ToString().Should().Be("(b|a_) _1_-_a"));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(ba_) _1_-_a"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("ba1a"));
        }

        [Test]
        public async Task MaskTest_ChangeMask1()
        {
            var comp = Context.RenderComponent<MudMask>();
            comp.SetParam(x => x.Mask, new PatternMask("(aaa) 000-aa") { Placeholder = '_', CleanDelimiters = true });
            var maskField = comp;

            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "a" }));
            comp.WaitForAssertion(() => maskField.Instance.Mask.ToString().Should().Be("(a|__) ___-__"));
            // change the mask
            comp.SetParam(x => x.Mask,
                new PatternMask("(bb+) 999-bb")
                {
                    MaskChars = new MaskChar[]
                    {
                        MaskChar.Letter('b'), MaskChar.Digit('9'), MaskChar.LetterOrDigit('+'),
                    },
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
        public async Task MaskTest_ChangeMask2()
        {
            var comp = Context.RenderComponent<MudMask>();
            comp.SetParam(x => x.Mask, new PatternMask("(LL) UU")
            {
                Placeholder = '_',
                CleanDelimiters = true,
                MaskChars = new[]
            {
                new MaskChar('L', "[a-z]"),
                new MaskChar('U', "[A-Z]")
            }
            });
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

        /// <summary>
        /// Note: Keeping positions of input blocks works only with Placeholder, and only in certain scenarios.
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task MaskTest_KeepInputBlockPositions()
        {
            var comp = Context.RenderComponent<MudMask>();
            var maskField = comp.Instance;

            await comp.InvokeAsync(() => comp.SetParam("Mask", new PatternMask("(aaa) 000-aa") { Placeholder = '_', CleanDelimiters = true }));

            await comp.InvokeAsync(() => maskField.OnCaretPositionChanged(1));
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "a" }));
            comp.WaitForAssertion(() => maskField.Text.Should().Be("(a__) ___-__"));
            comp.WaitForAssertion(() => maskField.Value.Should().Be("a"));

            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "b" }));
            comp.WaitForAssertion(() => maskField.Text.Should().Be("(ab_) ___-__"));
            comp.WaitForAssertion(() => maskField.Value.Should().Be("ab"));

            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "c" }));
            comp.WaitForAssertion(() => maskField.Text.Should().Be("(abc) ___-__"));
            comp.WaitForAssertion(() => maskField.Value.Should().Be("abc"));

            await comp.InvokeAsync(() => maskField.OnCaretPositionChanged(3));
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField.Text.Should().Be("(ac_) ___-__"));
            comp.WaitForAssertion(() => maskField.Value.Should().Be("ac"));

            await comp.InvokeAsync(() => maskField.OnCaretPositionChanged(6));
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "1" }));
            comp.WaitForAssertion(() => maskField.Text.Should().Be("(ac_) 1__-__"));
            comp.WaitForAssertion(() => maskField.Value.Should().Be("ac1"));

            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "0" }));
            comp.WaitForAssertion(() => maskField.Text.Should().Be("(ac_) 10_-__"));
            comp.WaitForAssertion(() => maskField.Value.Should().Be("ac10"));

            await comp.InvokeAsync(() => maskField.OnCaretPositionChanged(1));
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "Delete" }));
            comp.WaitForAssertion(() => maskField.Text.Should().Be("(c__) ___-__"));
            comp.WaitForAssertion(() => maskField.Value.Should().Be("c"));

            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "Delete" }));
            comp.WaitForAssertion(() => maskField.Text.Should().Be(""));
            comp.WaitForAssertion(() => maskField.Value.Should().Be(""));
        }

        [Test]
        public async Task MaskTest_Paste()
        {
            var comp = Context.RenderComponent<MudMask>();
            comp.SetParam(x => x.Mask, new PatternMask("(aaa) 000-aa") { Placeholder = '_', CleanDelimiters = true });
            var maskField = comp;

            await comp.InvokeAsync(() => maskField.Instance.OnPaste("abc"));

            await comp.InvokeAsync(() => maskField.Instance.OnCaretPositionChanged(10));
            await comp.InvokeAsync(() => maskField.Instance.OnPaste("zxc"));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(abc) ___-zx"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("abczx"));

            await comp.InvokeAsync(() => maskField.Instance.OnCaretPositionChanged(2));
            await comp.InvokeAsync(() => maskField.Instance.OnPaste("defgh"));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(ade) ___-zx"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("adezx"));

            await comp.InvokeAsync(() => maskField.Instance.OnCaretPositionChanged(7));
            await comp.InvokeAsync(() => maskField.Instance.OnPaste("120"));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(ade) _12-zx"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("ade12zx"));
            //Symbols should not be paste but remove the related index
            await comp.InvokeAsync(() => maskField.Instance.OnCaretPositionChanged(1));
            await comp.InvokeAsync(() => maskField.Instance.OnPaste("+-"));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(ade) _12-zx"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("ade12zx"));
        }

        [Test]
        public async Task MaskTest_Selection()
        {
            var comp = Context.RenderComponent<MudMask>();
            comp.SetParam(x => x.Mask, new PatternMask("0000 0000 000") { Placeholder = '_', CleanDelimiters = true });
            var maskField = comp.Instance;

            await comp.InvokeAsync(() => maskField.OnPaste("1234567899"));
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

            await comp.InvokeAsync(() => maskField.Clear());
            await comp.InvokeAsync(() => maskField.OnPaste("1234 81__ _9_"));
            await comp.InvokeAsync(() => maskField.OnCaretPositionChanged(1));
            await comp.InvokeAsync(() => maskField.OnSelect(1, 3));
            comp.WaitForAssertion(() => maskField.Mask.ToString().Should().Be("1[23]4 81__ _9_"));
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField.Text.Should().Be("1481 ___9 ___"));
            comp.WaitForAssertion(() => maskField.Value.Should().Be("14819"));

            await comp.InvokeAsync(() => maskField.OnCaretPositionChanged(3));
            await comp.InvokeAsync(() => maskField.OnSelect(3, 7));
            await comp.InvokeAsync(() => maskField.OnPaste("a1a"));
            comp.WaitForAssertion(() => maskField.Text.Should().Be("1481 _9__ ___"));
            comp.WaitForAssertion(() => maskField.Value.Should().Be("14819"));
        }

        [Test]
        public async Task MaskTest_TwoWayBinding()
        {
            var comp = Context.RenderComponent<MaskTwoWayBindingTest>();
            var maskField1 = comp.FindComponents<MudMask>().First();
            var maskField2 = comp.FindComponents<MudMask>().Last();
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
        public async Task MaskTest_TimeSpan()
        {
            var comp = Context.RenderComponent<MudTextField<TimeSpan?>>();
            comp.SetParam(x => x.Mask, new PatternMask("00:00") { CleanDelimiters = false, });
            var tf = comp.Instance;
            var maskField = comp.FindComponent<MudMask>().Instance;

            await comp.InvokeAsync(() => maskField.OnFocused(new FocusEventArgs()));
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "1" }));
            comp.WaitForAssertion(() => maskField.Text.Should().Be("1"));
            comp.WaitForAssertion(() => tf.Value.Should().Be(TimeSpan.FromDays(1)));

            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "2" }));
            comp.WaitForAssertion(() => maskField.Text.Should().Be("12:"));
            comp.WaitForAssertion(() => tf.Value.Should().Be(null));

            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "3" }));
            comp.WaitForAssertion(() => maskField.Text.Should().Be("12:3"));
            comp.WaitForAssertion(() => tf.Value.Should().Be(new TimeSpan(12, 3, 00)));

            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "4" }));
            comp.WaitForAssertion(() => maskField.Text.Should().Be("12:34"));
            comp.WaitForAssertion(() => tf.Value.Should().Be(new TimeSpan(12, 34, 00)));

            await comp.InvokeAsync(() => maskField.OnCaretPositionChanged(2));
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField.Text.Should().Be("13:4"));
            comp.WaitForAssertion(() => tf.Value.Should().Be(new TimeSpan(13, 4, 00)));

            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "Delete" }));
            comp.WaitForAssertion(() => maskField.Text.Should().Be("14:"));
            comp.WaitForAssertion(() => tf.Value.Should().Be(null));
        }

        [Test]
        public async Task MaskTest_MoreCoverage()
        {
            var comp = Context.RenderComponent<MudMask>();
            var maskField = comp.Instance;
            var impl = maskField.Mask;
            comp.WaitForAssertion(() => maskField.GetInputType().Should().Be(InputType.Text));

            await comp.InvokeAsync(() => maskField.OnCaretPositionChanged(2));
            comp.WaitForAssertion(() => impl.CaretPos.Should().Be(2));

            comp.SetParam("Mask", new PatternMask("*00 000") { Placeholder = '_', CleanDelimiters = true });

            await comp.InvokeAsync(() => maskField.OnCopy());
            await comp.InvokeAsync(async () => await maskField.FocusAsync());
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "1" }));
            comp.WaitForAssertion(() => maskField.Text.Should().Be("1__ ___"));
            comp.WaitForAssertion(() => maskField.Value.Should().Be("1"));

            await comp.InvokeAsync(async () => await maskField.SelectAsync());
            await comp.InvokeAsync(() => maskField.OnCaretPositionChanged(0));
            await comp.InvokeAsync(async () => await maskField.SelectRangeAsync(0, 7));
            await comp.InvokeAsync(() => maskField.OnSelect(0, 7));
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "2" }));
            comp.WaitForAssertion(() => maskField.Text.Should().Be("2__ ___"));
            comp.WaitForAssertion(() => maskField.Value.Should().Be("2"));

            await comp.InvokeAsync(() => maskField.OnCaretPositionChanged(0));
            await comp.InvokeAsync(() => maskField.OnFocused(new FocusEventArgs()));
            comp.SetParam("Text", "123");
            comp.WaitForAssertion(() => maskField.Text.Should().Be("123 ___"));
            comp.WaitForAssertion(() => maskField.Value.Should().Be("123"));
            comp.SetParam("Text", "123 ___");
            comp.WaitForAssertion(() => maskField.Text.Should().Be("123 ___"));
            comp.WaitForAssertion(() => maskField.Value.Should().Be("123"));
            comp.SetParam("Value", "321");
            comp.WaitForAssertion(() => maskField.Text.Should().Be("321 ___"));
            comp.WaitForAssertion(() => maskField.Value.Should().Be("321"));
            comp.SetParam("Value", "321");
            comp.WaitForAssertion(() => maskField.Text.Should().Be("321 ___"));
            comp.WaitForAssertion(() => maskField.Value.Should().Be("321"));
            await comp.InvokeAsync(() => maskField.OnBlurredAsync(new FocusEventArgs()));

            comp.SetParam("Clearable", true);
            maskField.Clearable.Should().Be(true);
            // Param Mask is impossible to null out
            comp.SetParam("Mask", null);
            comp.WaitForAssertion(() => maskField.Mask.Should().NotBeNull());
            comp.SetParam("Mask", new PatternMask("*00 000") { CleanDelimiters = true });

            // selection is not cleared by caret on edge of selection
            await comp.InvokeAsync(() => maskField.OnSelect(0, 1));
            await comp.InvokeAsync(() => maskField.OnCaretPositionChanged(0));
            comp.WaitForAssertion(() => maskField.Mask.Selection.Should().NotBeNull());
            // only if caret is moved outside
            await comp.InvokeAsync(() => maskField.OnCaretPositionChanged(2));
            comp.WaitForAssertion(() => maskField.Mask.Selection.Should().BeNull());

            // pasting null doesn't do anything
            await comp.InvokeAsync(() => maskField.OnPaste("123"));
            comp.WaitForAssertion(() => maskField.Mask.ToString().Should().Be("123 |"));
            await comp.InvokeAsync(() => maskField.OnPaste(null));
            comp.WaitForAssertion(() => maskField.Mask.ToString().Should().Be("123 |"));
            // ctrl or alt doesn't do anything
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "1", CtrlKey = true }));
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "1", AltKey = true }));
            comp.WaitForAssertion(() => maskField.Mask.ToString().Should().Be("123 |"));
            // clear via clear button
            await comp.InvokeAsync(() => maskField.HandleClearButtonAsync(new MouseEventArgs()));
            comp.WaitForAssertion(() => maskField.Mask.ToString().Should().Be("|"));
            // ctrl + backspace clears input
            await comp.InvokeAsync(() => maskField.OnPaste("123"));
            comp.WaitForAssertion(() => maskField.Mask.ToString().Should().Be("123 |"));
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace", CtrlKey = true }));
            comp.WaitForAssertion(() => maskField.Mask.ToString().Should().Be("|"));
        }

        [Test]
        public async Task MaskTest_MultipleTFsLinkedViaTwoWayBinding()
        {
            var comp = Context.RenderComponent<MaskedTextFieldTwoWayBindingTest>();
            var tfs = comp.FindComponents<MudTextField<string>>().Select(x => x.Instance).ToArray();
            var masks = comp.FindComponents<MudMask>().Select(x => x.Instance).ToArray();
            await comp.InvokeAsync(() => masks[0].OnPaste("123456"));
            masks[0].Mask.ToString().Should().Be("123-456|");
            comp.WaitForAssertion(() => masks[1].Mask.ToString().Should().Be("12/34/56|"));
            tfs[0].Text.Should().Be("123-456");
            tfs[1].Text.Should().Be("12/34/56");
            await comp.InvokeAsync(() => masks[1].HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            masks[1].Mask.ToString().Should().Be("12/34/5|");
            comp.WaitForAssertion(() => masks[0].Mask.ToString().Should().Be("123-45|"));
            tfs[0].Text.Should().Be("123-45");
            tfs[1].Text.Should().Be("12/34/5");
        }

        /// <summary>
        /// Calling form.Reset() should clear the masked text field
        /// </summary>
        [Test]
        public async Task FormReset_Should_ClearMaskedField()
        {
            var comp = Context.RenderComponent<FormResetMaskTest>();
            var form = comp.FindComponent<MudForm>().Instance;
            var textField = comp.FindComponent<MudTextField<string>>().Instance;
            var mask = comp.FindComponent<MudMask>().Instance;
            await comp.InvokeAsync(() => mask.OnPaste("1234567890"));
            comp.WaitForAssertion(() => mask.Mask.ToString().Should().Be("(123) 456-7890|"));
            comp.WaitForAssertion(() => textField.Text.Should().Be("(123) 456-7890"));
            comp.WaitForAssertion(() => textField.Value.Should().Be("(123) 456-7890"));

            await comp.InvokeAsync(() => form.ResetAsync());
            comp.WaitForAssertion(() => mask.Mask.ToString().Should().Be("|"));
            comp.WaitForAssertion(() => textField.Text.Should().BeNullOrEmpty());
            comp.WaitForAssertion(() => textField.Value.Should().BeNullOrEmpty());

            await comp.InvokeAsync(async () => await textField.FocusAsync());
            await comp.InvokeAsync(async () => await textField.SelectAsync());
            await comp.InvokeAsync(async () => await textField.SelectRangeAsync(0, 1));
            await comp.InvokeAsync(() => textField.Clear());
            comp.WaitForAssertion(() => textField.Value.Should().Be(null));

            //This gives error
            await comp.InvokeAsync(() => textField.SetText("123"));
            comp.WaitForAssertion(() => textField.Value.Should().Be("(123) "));

            //ctrl+backspace
            await comp.InvokeAsync(() => form.ResetAsync());
            await comp.InvokeAsync(() => mask.OnPaste("1234567890"));
            comp.WaitForAssertion(() => mask.Mask.ToString().Should().Be("(123) 456-7890|"));
            comp.WaitForAssertion(() => textField.Text.Should().Be("(123) 456-7890"));
            comp.WaitForAssertion(() => textField.Value.Should().Be("(123) 456-7890"));
            await comp.InvokeAsync(() => mask.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace", CtrlKey = true }));
            comp.WaitForAssertion(() => textField.Value.Should().Be(""));
        }

        /// <summary>
        /// A readonly masked text should not react to any edit/delete event
        /// </summary>
        [Test]
        public async Task MaskTest_Readonly()
        {
            var comp = Context.RenderComponent<ReadonlyMaskedTextFieldTest>();
            var textField = comp.FindComponent<MudTextField<string>>().Instance;
            var mask = comp.FindComponent<MudMask>().Instance;
            var originalValue = textField.Text;

            originalValue.Should().Be("1234 1234 1234 1234");

            // paste
            await comp.InvokeAsync(() =>
            {
                mask.OnSelect(0, mask.Text.Length);
                mask.OnPaste("1234567890");
            });
            comp.WaitForAssertion(() => textField.Value.Should().Be(originalValue));
            // backspace
            await comp.InvokeAsync(() => mask.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => textField.Value.Should().Be(originalValue));
            // cut
            await comp.InvokeAsync(() =>
            {
                mask.OnSelect(0, mask.Text.Length);
                comp.Find("input").CutAsync(new ClipboardEventArgs { Type = "cut" });
            });
            comp.WaitForAssertion(() => textField.Value.Should().Be(originalValue));

            comp.SetParam(p => p.ReadOnly, false);
            // paste
            await comp.InvokeAsync(async () =>
            {
                mask.OnSelect(0, mask.Text.Length);
                mask.OnPaste("2222 2222 2222 2222");
            });
            comp.WaitForAssertion(() => textField.Value.Should().Be("2222 2222 2222 2222"));
            // backspace
            await comp.InvokeAsync(() => mask.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => textField.Value.Should().Be("2222 2222 2222 222"));
            // cut
            await comp.InvokeAsync(() =>
            {
                mask.OnSelect(0, textField.Value.Length);
                comp.Find("input").Cut(new ClipboardEventArgs { Type = "cut" });
            });
            comp.WaitForAssertion(() => textField.Value.Should().Be(""));
        }

        [Test]
        public async Task DifferentMaskImplementationTests()
        {
            // arrange
            var comp = Context.RenderComponent<DifferentMaskImplementationTest>();
            var masks = comp.FindComponents<MudMask>();
            var textFields = comp.FindComponents<MudTextField<string>>();
            var blockMaskComponent = masks[0];
            var blockMaskField = textFields[0].Instance;
            var prefixMaskComponent = masks[1];
            var prefixMaskField = textFields[1].Instance;
            var dateMaskComponent = masks[2];
            var dateMaskField = textFields[2].Instance;
            var multiMaskComponent = masks[3];
            var multiMaskField = textFields[3].Instance;
            var patternMaskComponent = masks[4];
            var patternMaskField = textFields[4].Instance;
            var regexMaskComponent = masks[5];
            var regexMaskField = textFields[5].Instance;

            // act

            // assert
            blockMaskComponent.Markup.Contains(blockMaskComponent.Instance.ClearIcon).Should().BeTrue();
            blockMaskField.Mask.Text.Should().Be(comp.Instance.BlockMaskValue);

            prefixMaskComponent.Markup.Contains(blockMaskComponent.Instance.ClearIcon).Should().BeTrue();
            prefixMaskField.Mask.Text.Should().Be(comp.Instance.BlockMaskValue);

            dateMaskComponent.Markup.Contains(dateMaskComponent.Instance.ClearIcon).Should().BeTrue();
            dateMaskField.Mask.Text.Should().Be(comp.Instance.DateMaskValue);

            multiMaskComponent.Markup.Contains(multiMaskComponent.Instance.ClearIcon).Should().BeTrue();
            multiMaskField.Mask.Text.Should().Be(comp.Instance.MultiMaskValue);

            patternMaskComponent.Markup.Contains(patternMaskComponent.Instance.ClearIcon).Should().BeTrue();
            patternMaskField.Mask.Text.Should().Be(comp.Instance.PatternMaskValue);

            regexMaskComponent.Markup.Contains(regexMaskComponent.Instance.ClearIcon).Should().BeTrue();
            regexMaskField.Mask.Text.Should().Be(comp.Instance.RegexMaskValue);
        }

        /// <summary>
        /// Optional Mask should not have required attribute and aria-required should be false.
        /// </summary>
        [Test]
        public void OptionalMask_Should_NotHaveRequiredAttributeAndAriaRequiredShouldBeFalse()
        {
            var comp = Context.RenderComponent<MudMask>();

            comp.Find("input").HasAttribute("required").Should().BeFalse();
            comp.Find("input").GetAttribute("aria-required").Should().Be("false");
        }

        /// <summary>
        /// Required Mask should have required and aria-required attributes.
        /// </summary>
        [Test]
        public void RequiredMask_Should_HaveRequiredAndAriaRequiredAttributes()
        {
            var comp = Context.RenderComponent<MudMask>(parameters => parameters
                .Add(p => p.Required, true));

            comp.Find("input").HasAttribute("required").Should().BeTrue();
            comp.Find("input").GetAttribute("aria-required").Should().Be("true");
        }

        /// <summary>
        /// Required and aria-required Mask attributes should be dynamic.
        /// </summary>
        [Test]
        public void RequiredAndAriaRequiredMaskAttributes_Should_BeDynamic()
        {
            var comp = Context.RenderComponent<MudMask>();

            comp.Find("input").HasAttribute("required").Should().BeFalse();
            comp.Find("input").GetAttribute("aria-required").Should().Be("false");

            comp.SetParametersAndRender(parameters => parameters
                .Add(p => p.Required, true));

            comp.Find("input").HasAttribute("required").Should().BeTrue();
            comp.Find("input").GetAttribute("aria-required").Should().Be("true");
        }

        /// <summary>
        /// Optional Mask with multiple lines should not have required attribute and aria-required should be false.
        /// </summary>
        [Test]
        public void OptionalMaskWithMultipleLines_Should_NotHaveRequiredAttributeAndAriaRequiredShouldBeFalse()
        {
            var comp = Context.RenderComponent<MudMask>(parameters => parameters
                .Add(p => p.Lines, 5));

            comp.Find("textarea").HasAttribute("required").Should().BeFalse();
            comp.Find("textarea").GetAttribute("aria-required").Should().Be("false");
        }

        /// <summary>
        /// Required Mask with multiple lines  should have required and aria-required attributes.
        /// </summary>
        [Test]
        public void RequiredMaskWithMultipleLines_Should_HaveRequiredAndAriaRequiredAttributes()
        {
            var comp = Context.RenderComponent<MudMask>(parameters => parameters
                .Add(p => p.Required, true)
                .Add(p => p.Lines, 5));

            comp.Find("textarea").HasAttribute("required").Should().BeTrue();
            comp.Find("textarea").GetAttribute("aria-required").Should().Be("true");
        }

        /// <summary>
        /// Required and aria-required Mask with multiple lines  attributes should be dynamic.
        /// </summary>
        [Test]
        public void RequiredAndAriaRequiredMaskWithMultipleLinesAttributes_Should_BeDynamic()
        {
            var comp = Context.RenderComponent<MudMask>(parameters => parameters
                .Add(p => p.Lines, 5));

            comp.Find("textarea").HasAttribute("required").Should().BeFalse();
            comp.Find("textarea").GetAttribute("aria-required").Should().Be("false");

            comp.SetParametersAndRender(parameters => parameters
                .Add(p => p.Required, true));

            comp.Find("textarea").HasAttribute("required").Should().BeTrue();
            comp.Find("textarea").GetAttribute("aria-required").Should().Be("true");
        }
    }
}
