// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AwesomeAssertions;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.UnitTests.TestComponents.Mask;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class MaskTests : BunitTest
    {
        public static object[] TextFieldWithMask_SetValueParameterUpdateText_Parameters = [
            new object[] { "PatternMask", new PatternMask("""0000"""), "1111", "2222" },
            new object[] { "RegexMask", new RegexMask("""^\d*$"""), "1111", "2222" },
            new object[] { "MultiMask", new MultiMask("""0000"""), "1111", "2222" },
            new object[] { "BlockMask", new BlockMask(new Block('0', 1, 4)), "1111", "2222" },
            new object[] { "DateMask", new DateMask("""MM/dd/yyyy"""), "01/01/2024", "02/03/2025" }
        ];

        [TestCaseSource(nameof(TextFieldWithMask_SetValueParameterUpdateText_Parameters))]
        public async Task TextFieldWithMask_SetValueParameterUpdateText(string testName, IMask mask, string initialValue, string setValue)
        {
            // Arrange

            var comp = Context.Render<MudTextField<string>>(parameters =>
            {
                parameters.Add(m => m.Mask, mask);
                parameters.Add(m => m.Value, initialValue);
            });
            var textField = comp.Instance;
            var maskField = comp.FindComponent<MudMask>().Instance;

            // Assert : Initial state

            textField.ReadValue.Should().Be(initialValue);
            textField.ReadText.Should().Be(initialValue);
            maskField.Value.Should().Be(initialValue);
            maskField.ReadText.Should().Be(initialValue);

            // Act

            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(m => m.Value, setValue));

            // Assert

            textField.ReadValue.Should().Be(setValue);
            textField.ReadText.Should().Be(setValue);
            maskField.Value.Should().Be(setValue);
            maskField.ReadText.Should().Be(setValue);
        }

        /// <summary>
        /// Test all IsMatch variants: letter, digit and symbols.
        /// </summary>
        [Test]
        public async Task MaskTest_Fundamentals1()
        {
            var comp = Context.Render<MudMask>();
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Mask, new PatternMask("(aaa) 000-aa") { Placeholder = '_', CleanDelimiters = true }));
            var maskField = comp;
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadValue.Should().BeNullOrEmpty());
            //Unmatched keys should have no effect
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "1" }));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadText.Should().Be(""));
            maskField.Instance.ReadValue.Should().BeNullOrEmpty();
            maskField.Instance.Mask.ToString().Should().Be("|");

            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "a" }));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadText.Should().Be("(a__) ___-__"));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadValue.Should().Be("a"));
            await comp.WaitForAssertionAsync(() => maskField.Instance.Mask.CaretPos.Should().Be(2));

            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "b" }));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadText.Should().Be("(ab_) ___-__"));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadValue.Should().Be("ab"));
            //Symbols should have no effect in letter
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "+" }));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadText.Should().Be("(ab_) ___-__"));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadValue.Should().Be("ab"));
            //Symbol as a mask character should have no effect in letter
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "*" }));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadText.Should().Be("(ab_) ___-__"));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadValue.Should().Be("ab"));
            //Check uppercase character
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "C" }));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadText.Should().Be("(abC) ___-__"));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadValue.Should().Be("abC"));
            await comp.WaitForAssertionAsync(() => maskField.Instance.Mask.CaretPos.Should().Be(6));

            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "d" }));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadText.Should().Be("(abC) ___-__"));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadValue.Should().Be("abC"));

            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "1" }));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadText.Should().Be("(abC) 1__-__"));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadValue.Should().Be("abC1"));
            //Symbols should have no effect in digit
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "+" }));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadText.Should().Be("(abC) 1__-__"));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadValue.Should().Be("abC1"));
            //Symbol as a mask character should have no effect in letter
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "*" }));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadText.Should().Be("(abC) 1__-__"));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadValue.Should().Be("abC1"));

            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "2" }));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadText.Should().Be("(abC) 12_-__"));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadValue.Should().Be("abC12"));

            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "0" }));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadText.Should().Be("(abC) 120-__"));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadValue.Should().Be("abC120"));
            await comp.WaitForAssertionAsync(() => maskField.Instance.Mask.CaretPos.Should().Be(10));

            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "A" }));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadText.Should().Be("(abC) 120-A_"));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadValue.Should().Be("abC120A"));
            //Check culture character
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "ı" }));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadText.Should().Be("(abC) 120-Aı"));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadValue.Should().Be("abC120Aı"));
            //Keys should have no effect if the mask completed
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Z" }));
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "0" }));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadText.Should().Be("(abC) 120-Aı"));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadValue.Should().Be("abC120Aı"));

            //Middle input should move the after characters
            await comp.InvokeAsync(() => maskField.Instance.OnCaretPositionChanged(9));
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "b" }));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadText.Should().Be("(abC) 120-bA"));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadValue.Should().Be("abC120bA"));

            await comp.InvokeAsync(() => maskField.Instance.OnCaretPositionChanged(11));
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "c" }));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadText.Should().Be("(abC) 120-bc"));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadValue.Should().Be("abC120bc"));
            await comp.WaitForAssertionAsync(() => maskField.Instance.Mask.CaretPos.Should().Be(12));

            await comp.InvokeAsync(
                () => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadText.Should().Be("(abC) 120-b_"));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadValue.Should().Be("abC120b"));
            await comp.WaitForAssertionAsync(() => maskField.Instance.Mask.CaretPos.Should().Be(11));

            await comp.InvokeAsync(
                () => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadText.Should().Be("(abC) 120-__"));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadValue.Should().Be("abC120"));

            await comp.InvokeAsync(
                () => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadText.Should().Be("(abC) 12_-__"));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadValue.Should().Be("abC12"));
            await comp.WaitForAssertionAsync(() => maskField.Instance.Mask.CaretPos.Should().Be(8));

            await comp.InvokeAsync(
                () => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadText.Should().Be("(abC) 1__-__"));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadValue.Should().Be("abC1"));

            await comp.InvokeAsync(
                () => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadText.Should().Be("(abC) ___-__"));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadValue.Should().Be("abC"));

            await comp.InvokeAsync(
                () => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadText.Should().Be("(ab_) ___-__"));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadValue.Should().Be("ab"));
            await comp.WaitForAssertionAsync(() => maskField.Instance.Mask.CaretPos.Should().Be(3));

            await comp.InvokeAsync(
                () => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadText.Should().Be("(a__) ___-__"));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadValue.Should().Be("a"));

            //Backspace should have no effect on empty value
            await comp.InvokeAsync(
                () => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadText.Should().Be(""));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadValue.Should().Be(""));
            await comp.WaitForAssertionAsync(() => maskField.Instance.Mask.CaretPos.Should().Be(0));
            await comp.InvokeAsync(
                () => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadText.Should().Be(""));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadValue.Should().Be(""));
            await comp.WaitForAssertionAsync(() => maskField.Instance.Mask.CaretPos.Should().Be(0));
        }

        [Test]
        public async Task MaskTest_Fundamentals2()
        {
            var comp = Context.Render<MudMask>();
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Mask, new PatternMask("(aaa) 000-aa") { Placeholder = '_', CleanDelimiters = true }));
            var maskField = comp;

            await comp.InvokeAsync(() => maskField.Instance.OnFocused(new FocusEventArgs()));
            await comp.InvokeAsync(() => maskField.Instance.OnPasteAsync("abc120ac"));

            await comp.InvokeAsync(() => maskField.Instance.OnCaretPositionChanged(1));
            await comp.InvokeAsync(() => maskField.Instance.Mask.ToString().Should().Be("(|abc) 120-ac"));
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Delete" }));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadText.Should().Be("(bca) ___-__"));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadValue.Should().Be("bca"));
            await comp.WaitForAssertionAsync(() => maskField.Instance.Mask.CaretPos.Should().Be(1));

            await comp.InvokeAsync(() => maskField.Instance.Clear());
            await comp.InvokeAsync(() => maskField.Instance.OnPasteAsync("abc120ac"));
            await comp.InvokeAsync(() => maskField.Instance.OnCaretPositionChanged(6));
            await comp.InvokeAsync(() => maskField.Instance.Mask.ToString().Should().Be("(abc) |120-ac"));
            await comp.InvokeAsync(
                () => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            await comp.InvokeAsync(() => maskField.Instance.Mask.ToString().Should().Be("(ab|a) ___-__"));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadText.Should().Be("(aba) ___-__"));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadValue.Should().Be("aba"));

            await comp.InvokeAsync(() => maskField.Instance.Clear());
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadText.Should().Be(""));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadValue.Should().Be(""));

            await comp.InvokeAsync(() => maskField.Instance.OnFocused(new FocusEventArgs()));
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "a" }));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadText.Should().Be("(a__) ___-__"));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadValue.Should().Be("a"));
            await comp.WaitForAssertionAsync(() => maskField.Instance.Mask.CaretPos.Should().Be(2));
        }

        [Test]
        public async Task MaskTest_Int()
        {
            var comp = Context.Render<MudTextField<int?>>();
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Mask, new PatternMask("(0)0-0)") { Placeholder = '_', CleanDelimiters = true }));
            var tf = comp.Instance;
            var maskField = comp.FindComponent<MudMask>();

            await comp.InvokeAsync(() => maskField.Instance.OnCaretPositionChanged(1));
            await comp.WaitForAssertionAsync(() => tf.ReadValue.Should().Be(null));
            //Unmatched keys should have no effect
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "a" }));
            await comp.WaitForAssertionAsync(() => tf.ReadValue.Should().Be(null));

            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "1" }));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadText.Should().Be("(1)_-_)"));
            await comp.WaitForAssertionAsync(() => tf.ReadValue.Should().Be(1));

            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "2" }));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadText.Should().Be("(1)2-_)"));
            await comp.WaitForAssertionAsync(() => tf.ReadValue.Should().Be(12));

            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "3" }));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadText.Should().Be("(1)2-3)"));
            await comp.WaitForAssertionAsync(() => tf.ReadValue.Should().Be(123));

            await comp.InvokeAsync(
                () => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadText.Should().Be("(1)2-_)"));
            await comp.WaitForAssertionAsync(() => tf.ReadValue.Should().Be(12));

            await comp.InvokeAsync(
                () => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadText.Should().Be("(1)_-_)"));
            await comp.WaitForAssertionAsync(() => tf.ReadValue.Should().Be(1));

            await comp.InvokeAsync(
                () => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadText.Should().Be(""));
            await comp.WaitForAssertionAsync(() => tf.ReadValue.Should().Be(null));
        }

        [Test]
        public async Task MaskTest_InsertCharactersIntoMiddle()
        {
            var comp = Context.Render<MudMask>();
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Mask, new PatternMask("(aaa) 000-aa") { Placeholder = '_', CleanDelimiters = true }));
            var maskField = comp;

            await comp.InvokeAsync(() => maskField.Instance.Mask.ToString().Should().Be("|"));
            // 1 is not accepted because first mask position wants a letter
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "1" }));
            await comp.InvokeAsync(() => maskField.Instance.Mask.ToString().Should().Be("|"));

            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "a" }));
            await comp.WaitForAssertionAsync(() => maskField.Instance.Mask.ToString().Should().Be("(a|__) ___-__"));
            await comp.InvokeAsync(() => maskField.Instance.OnCaretPositionChanged(6));
            await comp.WaitForAssertionAsync(() => maskField.Instance.Mask.ToString().Should().Be("(a__) |___-__"));
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "1" }));
            await comp.WaitForAssertionAsync(() => maskField.Instance.Mask.ToString().Should().Be("(a__) 1|__-__"));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadValue.Should().Be("a1"));

            await comp.InvokeAsync(() => maskField.Instance.OnCaretPositionChanged(10));
            await comp.WaitForAssertionAsync(() => maskField.Instance.Mask.ToString().Should().Be("(a__) 1__-|__"));
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "a" }));
            await comp.WaitForAssertionAsync(() => maskField.Instance.Mask.ToString().Should().Be("(a__) 1__-a|_"));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadValue.Should().Be("a1a"));

            await comp.InvokeAsync(() => maskField.Instance.OnCaretPositionChanged(1));
            await comp.WaitForAssertionAsync(() => maskField.Instance.Mask.ToString().Should().Be("(|a__) 1__-a_"));
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "b" }));
            await comp.WaitForAssertionAsync(() => maskField.Instance.Mask.ToString().Should().Be("(b|a_) _1_-_a"));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadText.Should().Be("(ba_) _1_-_a"));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadValue.Should().Be("ba1a"));
        }

        [Test]
        public async Task MaskTest_ChangeMask1()
        {
            var comp = Context.Render<MudMask>();
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Mask, new PatternMask("(aaa) 000-aa") { Placeholder = '_', CleanDelimiters = true }));
            var maskField = comp;

            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "a" }));
            await comp.WaitForAssertionAsync(() => maskField.Instance.Mask.ToString().Should().Be("(a|__) ___-__"));
            // change the mask
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Mask,
                new PatternMask("(bb+) 999-bb")
                {
                    MaskChars = new MaskChar[]
                    {
                        MaskChar.Letter('b'), MaskChar.Digit('9'), MaskChar.LetterOrDigit('+'),
                    },
                    Placeholder = '_',
                    CleanDelimiters = true
                }));
            // internal state is preserved!
            await comp.WaitForAssertionAsync(() => maskField.Instance.Mask.ToString().Should().Be("(a|__) ___-__"));
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "b" }));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadText.Should().Be("(ab_) ___-__"));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadValue.Should().Be("ab"));
        }

        [Test]
        public async Task MaskTest_ChangeMask2()
        {
            var comp = Context.Render<MudMask>();
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Mask, new PatternMask("(LL) UU")
            {
                Placeholder = '_',
                CleanDelimiters = true,
                MaskChars = new[]
            {
                new MaskChar('L', "[a-z]"),
                new MaskChar('U', "[A-Z]")
            }
            }));
            var maskField = comp.Instance;

            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "a" }));
            await comp.WaitForAssertionAsync(() => maskField.ReadText.Should().Be("(a_) __"));
            await comp.WaitForAssertionAsync(() => maskField.ReadValue.Should().Be("a"));

            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "A" }));
            await comp.WaitForAssertionAsync(() => maskField.ReadText.Should().Be("(a_) __"));
            await comp.WaitForAssertionAsync(() => maskField.ReadValue.Should().Be("a"));

            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "a" }));
            await comp.WaitForAssertionAsync(() => maskField.ReadText.Should().Be("(aa) __"));
            await comp.WaitForAssertionAsync(() => maskField.ReadValue.Should().Be("aa"));

            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "A" }));
            await comp.WaitForAssertionAsync(() => maskField.ReadText.Should().Be("(aa) A_"));
            await comp.WaitForAssertionAsync(() => maskField.ReadValue.Should().Be("aaA"));
        }

        /// <summary>
        /// Note: Keeping positions of input blocks works only with Placeholder, and only in certain scenarios.
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task MaskTest_KeepInputBlockPositions()
        {
            var comp = Context.Render<MudMask>();
            var maskField = comp.Instance;

            await comp.InvokeAsync(async () => await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Mask, new PatternMask("(aaa) 000-aa") { Placeholder = '_', CleanDelimiters = true })));

            await comp.InvokeAsync(() => maskField.OnCaretPositionChanged(1));
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "a" }));
            await comp.WaitForAssertionAsync(() => maskField.ReadText.Should().Be("(a__) ___-__"));
            await comp.WaitForAssertionAsync(() => maskField.ReadValue.Should().Be("a"));

            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "b" }));
            await comp.WaitForAssertionAsync(() => maskField.ReadText.Should().Be("(ab_) ___-__"));
            await comp.WaitForAssertionAsync(() => maskField.ReadValue.Should().Be("ab"));

            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "c" }));
            await comp.WaitForAssertionAsync(() => maskField.ReadText.Should().Be("(abc) ___-__"));
            await comp.WaitForAssertionAsync(() => maskField.ReadValue.Should().Be("abc"));

            await comp.InvokeAsync(() => maskField.OnCaretPositionChanged(3));
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            await comp.WaitForAssertionAsync(() => maskField.ReadText.Should().Be("(ac_) ___-__"));
            await comp.WaitForAssertionAsync(() => maskField.ReadValue.Should().Be("ac"));

            await comp.InvokeAsync(() => maskField.OnCaretPositionChanged(6));
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "1" }));
            await comp.WaitForAssertionAsync(() => maskField.ReadText.Should().Be("(ac_) 1__-__"));
            await comp.WaitForAssertionAsync(() => maskField.ReadValue.Should().Be("ac1"));

            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "0" }));
            await comp.WaitForAssertionAsync(() => maskField.ReadText.Should().Be("(ac_) 10_-__"));
            await comp.WaitForAssertionAsync(() => maskField.ReadValue.Should().Be("ac10"));

            await comp.InvokeAsync(() => maskField.OnCaretPositionChanged(1));
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "Delete" }));
            await comp.WaitForAssertionAsync(() => maskField.ReadText.Should().Be("(c__) ___-__"));
            await comp.WaitForAssertionAsync(() => maskField.ReadValue.Should().Be("c"));

            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "Delete" }));
            await comp.WaitForAssertionAsync(() => maskField.ReadText.Should().Be(""));
            await comp.WaitForAssertionAsync(() => maskField.ReadValue.Should().Be(""));
        }

        [Test]
        public async Task MaskTest_Paste()
        {
            var comp = Context.Render<MudMask>();
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Mask, new PatternMask("(aaa) 000-aa") { Placeholder = '_', CleanDelimiters = true }));
            var maskField = comp;

            await comp.InvokeAsync(() => maskField.Instance.OnPasteAsync("abc"));

            await comp.InvokeAsync(() => maskField.Instance.OnCaretPositionChanged(10));
            await comp.InvokeAsync(() => maskField.Instance.OnPasteAsync("zxc"));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadText.Should().Be("(abc) ___-zx"));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadValue.Should().Be("abczx"));

            await comp.InvokeAsync(() => maskField.Instance.OnCaretPositionChanged(2));
            await comp.InvokeAsync(() => maskField.Instance.OnPasteAsync("defgh"));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadText.Should().Be("(ade) ___-zx"));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadValue.Should().Be("adezx"));

            await comp.InvokeAsync(() => maskField.Instance.OnCaretPositionChanged(7));
            await comp.InvokeAsync(() => maskField.Instance.OnPasteAsync("120"));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadText.Should().Be("(ade) _12-zx"));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadValue.Should().Be("ade12zx"));
            //Symbols should not be paste but remove the related index
            await comp.InvokeAsync(() => maskField.Instance.OnCaretPositionChanged(1));
            await comp.InvokeAsync(() => maskField.Instance.OnPasteAsync("+-"));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadText.Should().Be("(ade) _12-zx"));
            await comp.WaitForAssertionAsync(() => maskField.Instance.ReadValue.Should().Be("ade12zx"));
        }

        [Test]
        public async Task MaskTest_Selection()
        {
            var comp = Context.Render<MudMask>();
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Mask, new PatternMask("0000 0000 000") { Placeholder = '_', CleanDelimiters = true }));
            var maskField = comp.Instance;

            await comp.InvokeAsync(() => maskField.OnPasteAsync("1234567899"));
            await comp.WaitForAssertionAsync(() => maskField.Mask.ToString().Should().Be("1234 5678 99|_"));
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "9" }));
            await comp.WaitForAssertionAsync(() => maskField.Mask.ToString().Should().Be("1234 5678 999|"));
            //Select and delete
            await comp.InvokeAsync(() => maskField.OnSelect(10, 12));
            await comp.WaitForAssertionAsync(() => maskField.Mask.ToString().Should().Be("1234 5678 [99]9"));
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            await comp.WaitForAssertionAsync(() => maskField.Mask.ToString().Should().Be("1234 5678 |9__"));
            await comp.WaitForAssertionAsync(() => maskField.ReadText.Should().Be("1234 5678 9__"));
            await comp.WaitForAssertionAsync(() => maskField.ReadValue.Should().Be("123456789"));
            //Select with a whitespace and test again
            await comp.InvokeAsync(() => maskField.OnSelect(4, 8));
            await comp.WaitForAssertionAsync(() => maskField.Mask.ToString().Should().Be("1234[ 567]8 9__"));
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "Delete" }));
            await comp.WaitForAssertionAsync(() => maskField.Mask.ToString().Should().Be("1234| 89__ ___"));
            await comp.WaitForAssertionAsync(() => maskField.ReadText.Should().Be("1234 89__ ___"));
            await comp.WaitForAssertionAsync(() => maskField.ReadValue.Should().Be("123489"));

            await comp.InvokeAsync(() => maskField.OnSelect(7, 11));
            await comp.WaitForAssertionAsync(() => maskField.Mask.ToString().Should().Be("1234 89[__ _]__"));
            await comp.InvokeAsync(() => maskField.OnPasteAsync("567"));
            await comp.WaitForAssertionAsync(() => maskField.ReadText.Should().Be("1234 8956 7__"));
            await comp.WaitForAssertionAsync(() => maskField.ReadValue.Should().Be("123489567"));

            await comp.InvokeAsync(() => maskField.OnCaretPositionChanged(0));
            await comp.InvokeAsync(() => maskField.OnSelect(0, 1));
            await comp.WaitForAssertionAsync(() => maskField.Mask.ToString().Should().Be("[1]234 8956 7__"));
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "a" }));
            await comp.WaitForAssertionAsync(() => maskField.Mask.ToString().Should().Be("|2348 9567 ___"));
            await comp.WaitForAssertionAsync(() => maskField.ReadText.Should().Be("2348 9567 ___"));
            await comp.WaitForAssertionAsync(() => maskField.ReadValue.Should().Be("23489567"));

            await comp.InvokeAsync(() => maskField.OnSelect(6, 11));
            await comp.WaitForAssertionAsync(() => maskField.Mask.ToString().Should().Be("2348 9[567 _]__"));
            await comp.InvokeAsync(() => maskField.OnPasteAsync("1Mud9"));
            await comp.WaitForAssertionAsync(() => maskField.ReadText.Should().Be("2348 919_ ___"));
            await comp.WaitForAssertionAsync(() => maskField.ReadValue.Should().Be("2348919"));

            await comp.InvokeAsync(() => maskField.Clear());
            await comp.InvokeAsync(() => maskField.OnPasteAsync("1234 81__ _9_"));
            await comp.InvokeAsync(() => maskField.OnCaretPositionChanged(1));
            await comp.InvokeAsync(() => maskField.OnSelect(1, 3));
            await comp.WaitForAssertionAsync(() => maskField.Mask.ToString().Should().Be("1[23]4 81__ _9_"));
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            await comp.WaitForAssertionAsync(() => maskField.ReadText.Should().Be("1481 ___9 ___"));
            await comp.WaitForAssertionAsync(() => maskField.ReadValue.Should().Be("14819"));

            await comp.InvokeAsync(() => maskField.OnCaretPositionChanged(3));
            await comp.InvokeAsync(() => maskField.OnSelect(3, 7));
            await comp.InvokeAsync(() => maskField.OnPasteAsync("a1a"));
            await comp.WaitForAssertionAsync(() => maskField.ReadText.Should().Be("1481 _9__ ___"));
            await comp.WaitForAssertionAsync(() => maskField.ReadValue.Should().Be("14819"));
        }

        [Test]
        public async Task MaskTest_TwoWayBinding()
        {
            var comp = Context.Render<MaskTwoWayBindingTest>();
            var maskField1 = comp.FindComponents<MudMask>().First();
            var maskField2 = comp.FindComponents<MudMask>().Last();
            await comp.WaitForAssertionAsync(() => maskField1.Instance.ReadValue.Should().Be(""));

            await comp.InvokeAsync(() => maskField1.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "a" }));
            await comp.WaitForAssertionAsync(() => maskField1.Instance.ReadText.Should().Be("(a"));
            await comp.WaitForAssertionAsync(() => maskField1.Instance.ReadValue.Should().Be("a"));

            await comp.WaitForAssertionAsync(() => maskField2.Instance.ReadText.Should().Be("(a"));
            await comp.WaitForAssertionAsync(() => maskField2.Instance.ReadValue.Should().Be("a"));

            await comp.InvokeAsync(() => maskField1.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "b" }));
            await comp.WaitForAssertionAsync(() => maskField1.Instance.ReadText.Should().Be("(ab"));
            await comp.WaitForAssertionAsync(() => maskField1.Instance.ReadValue.Should().Be("ab"));

            await comp.WaitForAssertionAsync(() => maskField2.Instance.ReadText.Should().Be("(ab"));
            await comp.WaitForAssertionAsync(() => maskField2.Instance.ReadValue.Should().Be("ab"));

            await comp.InvokeAsync(() => maskField1.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "C" }));
            await comp.WaitForAssertionAsync(() => maskField1.Instance.ReadText.Should().Be("(abC) "));
            await comp.WaitForAssertionAsync(() => maskField1.Instance.ReadValue.Should().Be("abC"));
            await comp.WaitForAssertionAsync(() => maskField1.Instance.Mask.CaretPos.Should().Be(6));

            await comp.WaitForAssertionAsync(() => maskField2.Instance.ReadText.Should().Be("(abC) "));
            await comp.WaitForAssertionAsync(() => maskField2.Instance.ReadValue.Should().Be("abC"));

            await comp.InvokeAsync(() => maskField1.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "1" }));
            await comp.WaitForAssertionAsync(() => maskField1.Instance.ReadText.Should().Be("(abC) 1"));
            await comp.WaitForAssertionAsync(() => maskField1.Instance.ReadValue.Should().Be("abC1"));

            await comp.WaitForAssertionAsync(() => maskField2.Instance.ReadText.Should().Be("(abC) 1"));
            await comp.WaitForAssertionAsync(() => maskField2.Instance.ReadValue.Should().Be("abC1"));

            await comp.InvokeAsync(() => maskField1.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "2" }));
            await comp.WaitForAssertionAsync(() => maskField1.Instance.ReadText.Should().Be("(abC) 12"));
            await comp.WaitForAssertionAsync(() => maskField1.Instance.ReadValue.Should().Be("abC12"));

            await comp.WaitForAssertionAsync(() => maskField2.Instance.ReadText.Should().Be("(abC) 12"));
            await comp.WaitForAssertionAsync(() => maskField2.Instance.ReadValue.Should().Be("abC12"));

            await comp.InvokeAsync(() =>
                maskField1.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            await comp.WaitForAssertionAsync(() => maskField1.Instance.ReadText.Should().Be("(abC) 1"));
            await comp.WaitForAssertionAsync(() => maskField1.Instance.ReadValue.Should().Be("abC1"));

            await comp.WaitForAssertionAsync(() => maskField2.Instance.ReadText.Should().Be("(abC) 1"));
            await comp.WaitForAssertionAsync(() => maskField2.Instance.ReadValue.Should().Be("abC1"));

            await comp.InvokeAsync(() =>
                maskField1.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            await comp.WaitForAssertionAsync(() => maskField1.Instance.ReadText.Should().Be("(abC) "));
            await comp.WaitForAssertionAsync(() => maskField1.Instance.ReadValue.Should().Be("abC"));

            await comp.WaitForAssertionAsync(() => maskField2.Instance.ReadText.Should().Be("(abC) "));
            await comp.WaitForAssertionAsync(() => maskField2.Instance.ReadValue.Should().Be("abC"));

            await comp.InvokeAsync(() => maskField1.Instance.OnPasteAsync("123"));
            await comp.WaitForAssertionAsync(() => maskField1.Instance.ReadText.Should().Be("(abC) 123-"));
            await comp.WaitForAssertionAsync(() => maskField1.Instance.ReadValue.Should().Be("abC123"));
            await comp.WaitForAssertionAsync(() => maskField2.Instance.ReadText.Should().Be("(abC) 123-"));
            await comp.WaitForAssertionAsync(() => maskField2.Instance.ReadValue.Should().Be("abC123"));
        }

        [Test]
        public async Task MaskTest_TimeSpan()
        {
            var comp = Context.Render<MudTextField<TimeSpan?>>();
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Mask, new PatternMask("00:00") { CleanDelimiters = false, }));
            var tf = comp.Instance;
            var maskField = comp.FindComponent<MudMask>().Instance;

            await comp.InvokeAsync(() => maskField.OnFocused(new FocusEventArgs()));
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "1" }));
            await comp.WaitForAssertionAsync(() => maskField.ReadText.Should().Be("1"));
            await comp.WaitForAssertionAsync(() => tf.ReadValue.Should().Be(TimeSpan.FromDays(1)));

            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "2" }));
            await comp.WaitForAssertionAsync(() => maskField.ReadText.Should().Be("12:"));
            await comp.WaitForAssertionAsync(() => tf.ReadValue.Should().Be(null));

            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "3" }));
            await comp.WaitForAssertionAsync(() => maskField.ReadText.Should().Be("12:3"));
            await comp.WaitForAssertionAsync(() => tf.ReadValue.Should().Be(new TimeSpan(12, 3, 00)));

            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "4" }));
            await comp.WaitForAssertionAsync(() => maskField.ReadText.Should().Be("12:34"));
            await comp.WaitForAssertionAsync(() => tf.ReadValue.Should().Be(new TimeSpan(12, 34, 00)));

            await comp.InvokeAsync(() => maskField.OnCaretPositionChanged(2));
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            await comp.WaitForAssertionAsync(() => maskField.ReadText.Should().Be("13:4"));
            await comp.WaitForAssertionAsync(() => tf.ReadValue.Should().Be(new TimeSpan(13, 4, 00)));

            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "Delete" }));
            await comp.WaitForAssertionAsync(() => maskField.ReadText.Should().Be("14:"));
            await comp.WaitForAssertionAsync(() => tf.ReadValue.Should().Be(null));
        }

        [Test]
        public async Task MaskTest_MoreCoverage()
        {
            var comp = Context.Render<MudMask>();
            var maskField = comp.Instance;
            var impl = maskField.Mask;
            await comp.WaitForAssertionAsync(() => maskField.GetInputType().Should().Be(InputType.Text));

            await comp.InvokeAsync(() => maskField.OnCaretPositionChanged(2));
            await comp.WaitForAssertionAsync(() => impl.CaretPos.Should().Be(2));

            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Mask, new PatternMask("*00 000") { Placeholder = '_', CleanDelimiters = true }));

            await comp.InvokeAsync(() => maskField.OnCopyAsync());
            await comp.InvokeAsync(async () => await maskField.FocusAsync());
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "1" }));
            await comp.WaitForAssertionAsync(() => maskField.ReadText.Should().Be("1__ ___"));
            await comp.WaitForAssertionAsync(() => maskField.ReadValue.Should().Be("1"));

            await comp.InvokeAsync(async () => await maskField.SelectAsync());
            await comp.InvokeAsync(() => maskField.OnCaretPositionChanged(0));
            await comp.InvokeAsync(async () => await maskField.SelectRangeAsync(0, 7));
            await comp.InvokeAsync(() => maskField.OnSelect(0, 7));
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "2" }));
            await comp.WaitForAssertionAsync(() => maskField.ReadText.Should().Be("2__ ___"));
            await comp.WaitForAssertionAsync(() => maskField.ReadValue.Should().Be("2"));

            await comp.InvokeAsync(() => maskField.OnCaretPositionChanged(0));
            await comp.InvokeAsync(() => maskField.OnFocused(new FocusEventArgs()));
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Text, "123"));
            await comp.WaitForAssertionAsync(() => maskField.ReadText.Should().Be("123 ___"));
            await comp.WaitForAssertionAsync(() => maskField.ReadValue.Should().Be("123"));
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Text, "123 ___"));
            await comp.WaitForAssertionAsync(() => maskField.ReadText.Should().Be("123 ___"));
            await comp.WaitForAssertionAsync(() => maskField.ReadValue.Should().Be("123"));
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Value, "321"));
            await comp.WaitForAssertionAsync(() => maskField.ReadText.Should().Be("321 ___"));
            await comp.WaitForAssertionAsync(() => maskField.ReadValue.Should().Be("321"));
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Value, "321"));
            await comp.WaitForAssertionAsync(() => maskField.ReadText.Should().Be("321 ___"));
            await comp.WaitForAssertionAsync(() => maskField.ReadValue.Should().Be("321"));
            await comp.InvokeAsync(() => maskField.OnBlurredAsync(new FocusEventArgs()));

            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Clearable, true));
            maskField.Clearable.Should().Be(true);
            // Param Mask is impossible to null out
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Mask, null));
            await comp.WaitForAssertionAsync(() => maskField.Mask.Should().NotBeNull());
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Mask, new PatternMask("*00 000") { CleanDelimiters = true }));

            // selection is not cleared by caret on edge of selection
            await comp.InvokeAsync(() => maskField.OnSelect(0, 1));
            await comp.InvokeAsync(() => maskField.OnCaretPositionChanged(0));
            await comp.WaitForAssertionAsync(() => maskField.Mask.Selection.Should().NotBeNull());
            // only if caret is moved outside
            await comp.InvokeAsync(() => maskField.OnCaretPositionChanged(2));
            await comp.WaitForAssertionAsync(() => maskField.Mask.Selection.Should().BeNull());

            // pasting null doesn't do anything
            await comp.InvokeAsync(() => maskField.OnPasteAsync("123"));
            await comp.WaitForAssertionAsync(() => maskField.Mask.ToString().Should().Be("123 |"));
            await comp.InvokeAsync(() => maskField.OnPasteAsync(null));
            await comp.WaitForAssertionAsync(() => maskField.Mask.ToString().Should().Be("123 |"));
            // ctrl or alt doesn't do anything
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "1", CtrlKey = true }));
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "1", AltKey = true }));
            await comp.WaitForAssertionAsync(() => maskField.Mask.ToString().Should().Be("123 |"));
            // clear via clear button
            await comp.InvokeAsync(() => maskField.HandleClearButtonAsync(new MouseEventArgs()));
            await comp.WaitForAssertionAsync(() => maskField.Mask.ToString().Should().Be("|"));
            // ctrl + backspace clears input
            await comp.InvokeAsync(() => maskField.OnPasteAsync("123"));
            await comp.WaitForAssertionAsync(() => maskField.Mask.ToString().Should().Be("123 |"));
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace", CtrlKey = true }));
            await comp.WaitForAssertionAsync(() => maskField.Mask.ToString().Should().Be("|"));
        }

        [Test]
        public async Task MaskTest_MultipleTFsLinkedViaTwoWayBinding()
        {
            var comp = Context.Render<MaskedTextFieldTwoWayBindingTest>();
            var tfs = comp.FindComponents<MudTextField<string>>().Select(x => x.Instance).ToArray();
            var masks = comp.FindComponents<MudMask>().Select(x => x.Instance).ToArray();
            await comp.InvokeAsync(() => masks[0].OnPasteAsync("123456"));
            masks[0].Mask.ToString().Should().Be("123-456|");
            await comp.WaitForAssertionAsync(() => masks[1].Mask.ToString().Should().Be("12/34/56|"));
            tfs[0].ReadText.Should().Be("123-456");
            tfs[1].ReadText.Should().Be("12/34/56");
            await comp.InvokeAsync(() => masks[1].HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            masks[1].Mask.ToString().Should().Be("12/34/5|");
            await comp.WaitForAssertionAsync(() => masks[0].Mask.ToString().Should().Be("123-45|"));
            tfs[0].ReadText.Should().Be("123-45");
            tfs[1].ReadText.Should().Be("12/34/5");
        }

        /// <summary>
        /// Calling form.Reset() should clear the masked text field
        /// </summary>
        [Test]
        public async Task FormReset_Should_ClearMaskedField()
        {
            var comp = Context.Render<FormResetMaskTest>();
            var form = comp.FindComponent<MudForm>().Instance;
            var textField = comp.FindComponent<MudTextField<string>>().Instance;
            var mask = comp.FindComponent<MudMask>().Instance;
            await comp.InvokeAsync(() => mask.OnPasteAsync("1234567890"));
            await comp.WaitForAssertionAsync(() => mask.Mask.ToString().Should().Be("(123) 456-7890|"));
            await comp.WaitForAssertionAsync(() => textField.ReadText.Should().Be("(123) 456-7890"));
            await comp.WaitForAssertionAsync(() => textField.ReadValue.Should().Be("(123) 456-7890"));

            await comp.InvokeAsync(() => form.ResetAsync());
            await comp.WaitForAssertionAsync(() => mask.Mask.ToString().Should().Be("|"));
            await comp.WaitForAssertionAsync(() => textField.ReadText.Should().BeNullOrEmpty());
            await comp.WaitForAssertionAsync(() => textField.ReadValue.Should().BeNullOrEmpty());

            await comp.InvokeAsync(async () => await textField.FocusAsync());
            await comp.InvokeAsync(async () => await textField.SelectAsync());
            await comp.InvokeAsync(async () => await textField.SelectRangeAsync(0, 1));
            await comp.InvokeAsync(() => textField.Clear());
            await comp.WaitForAssertionAsync(() => textField.ReadValue.Should().Be(null));

            //This gives error
            await comp.InvokeAsync(() => textField.SetText("123"));
            await comp.WaitForAssertionAsync(() => textField.ReadValue.Should().Be("(123) "));

            //ctrl+backspace
            await comp.InvokeAsync(() => form.ResetAsync());
            await comp.InvokeAsync(() => mask.OnPasteAsync("1234567890"));
            await comp.WaitForAssertionAsync(() => mask.Mask.ToString().Should().Be("(123) 456-7890|"));
            await comp.WaitForAssertionAsync(() => textField.ReadText.Should().Be("(123) 456-7890"));
            await comp.WaitForAssertionAsync(() => textField.ReadValue.Should().Be("(123) 456-7890"));
            await comp.InvokeAsync(() => mask.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace", CtrlKey = true }));
            await comp.WaitForAssertionAsync(() => textField.ReadValue.Should().Be(""));
        }

        /// <summary>
        /// A readonly masked text should not react to any edit/delete event
        /// </summary>
        [Test]
        public async Task MaskTest_Readonly()
        {
            var comp = Context.Render<ReadonlyMaskedTextFieldTest>();
            var textField = comp.FindComponent<MudTextField<string>>().Instance;
            var mask = comp.FindComponent<MudMask>().Instance;
            var originalValue = textField.ReadText;

            originalValue.Should().Be("1234 1234 1234 1234");

            // paste
            await comp.InvokeAsync(() =>
            {
                mask.OnSelect(0, mask.ReadText.Length);
                return mask.OnPasteAsync("1234567890");
            });
            await comp.WaitForAssertionAsync(() => textField.ReadValue.Should().Be(originalValue));
            // backspace
            await comp.InvokeAsync(() => mask.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            await comp.WaitForAssertionAsync(() => textField.ReadValue.Should().Be(originalValue));
            // cut
            await comp.InvokeAsync(() =>
            {
                mask.OnSelect(0, mask.ReadText.Length);
                comp.Find("input").CutAsync(new ClipboardEventArgs { Type = "cut" });
            });
            await comp.WaitForAssertionAsync(() => textField.ReadValue.Should().Be(originalValue));

            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.ReadOnly, false));
            // paste
            await comp.InvokeAsync(() =>
            {
                mask.OnSelect(0, mask.ReadText.Length);
                return mask.OnPasteAsync("2222 2222 2222 2222");
            });
            await comp.WaitForAssertionAsync(() => textField.ReadValue.Should().Be("2222 2222 2222 2222"));
            // backspace
            await comp.InvokeAsync(() => mask.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            await comp.WaitForAssertionAsync(() => textField.ReadValue.Should().Be("2222 2222 2222 222"));
            // cut
            await comp.InvokeAsync(() =>
            {
                mask.OnSelect(0, textField.ReadValue.Length);
                comp.Find("input").Cut(new ClipboardEventArgs { Type = "cut" });
            });
            await comp.WaitForAssertionAsync(() => textField.ReadValue.Should().Be(""));
        }

        [Test]
        public void DifferentMaskImplementationTests()
        {
            // arrange
            var comp = Context.Render<DifferentMaskImplementationTest>();
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
            var comp = Context.Render<MudMask>();

            comp.Find("input").HasAttribute("required").Should().BeFalse();
            comp.Find("input").GetAttribute("aria-required").Should().Be("false");
        }

        /// <summary>
        /// Required Mask should have required and aria-required attributes.
        /// </summary>
        [Test]
        public void RequiredMask_Should_HaveRequiredAndAriaRequiredAttributes()
        {
            var comp = Context.Render<MudMask>(parameters => parameters
                .Add(p => p.Required, true));

            comp.Find("input").HasAttribute("required").Should().BeTrue();
            comp.Find("input").GetAttribute("aria-required").Should().Be("true");
        }

        /// <summary>
        /// Required and aria-required Mask attributes should be dynamic.
        /// </summary>
        [Test]
        public async Task RequiredAndAriaRequiredMaskAttributes_Should_BeDynamic()
        {
            var comp = Context.Render<MudMask>();

            comp.Find("input").HasAttribute("required").Should().BeFalse();
            comp.Find("input").GetAttribute("aria-required").Should().Be("false");

            await comp.SetParametersAndRenderAsync(parameters => parameters
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
            var comp = Context.Render<MudMask>(parameters => parameters
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
            var comp = Context.Render<MudMask>(parameters => parameters
                .Add(p => p.Required, true)
                .Add(p => p.Lines, 5));

            comp.Find("textarea").HasAttribute("required").Should().BeTrue();
            comp.Find("textarea").GetAttribute("aria-required").Should().Be("true");
        }

        /// <summary>
        /// Required and aria-required Mask with multiple lines  attributes should be dynamic.
        /// </summary>
        [Test]
        public async Task RequiredAndAriaRequiredMaskWithMultipleLinesAttributes_Should_BeDynamic()
        {
            var comp = Context.Render<MudMask>(parameters => parameters
                .Add(p => p.Lines, 5));

            comp.Find("textarea").HasAttribute("required").Should().BeFalse();
            comp.Find("textarea").GetAttribute("aria-required").Should().Be("false");

            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(p => p.Required, true));

            comp.Find("textarea").HasAttribute("required").Should().BeTrue();
            comp.Find("textarea").GetAttribute("aria-required").Should().Be("true");
        }

        [Test]
        public async Task ClearableReadOnlyMask_Should_NotHaveClearButton()
        {
            var comp = Context.Render<MudMask>();
            var maskField = comp.Instance;
            maskField.Clearable.Should().Be(false);
            maskField.ReadOnly.Should().Be(false);
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Mask, new PatternMask("*00 000") { Placeholder = '_', CleanDelimiters = true }));

            // mask is not clearable, no clear button should show up
            comp.FindAll(".mud-input-clear-button").Count.Should().Be(0);

            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Clearable, true));
            maskField.Clearable.Should().Be(true);

            // mask is now clearable but contains no text so, no clear button should show up
            comp.FindAll(".mud-input-clear-button").Count.Should().Be(0);

            await comp.InvokeAsync(async () => await maskField.FocusAsync());
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "1" }));
            await comp.WaitForAssertionAsync(() => maskField.ReadText.Should().Be("1__ ___"));

            // mask is clearable and contains text so the clear button should show up
            comp.FindAll(".mud-input-clear-button").Count.Should().Be(1);

            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.ReadOnly, true));

            // mask is clearable and contains text but is readonly so the clear button should not show up
            comp.FindAll(".mud-input-clear-button").Count.Should().Be(0);

        }

        [Test]
        public async Task MetaKeyShortcuts_Should_NotIntroduceExtraCharacters()
        {
            var comp = Context.Render<MudTextField<string>>();
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Mask, RegexMask.Email()));
            var tf = comp.Instance;
            var maskField = comp.FindComponent<MudMask>().Instance;

            // prep field
            await comp.InvokeAsync(() => maskField.OnFocused(new FocusEventArgs()));
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs()
            {
                Key = "a"
            }));
            await comp.WaitForAssertionAsync(() => maskField.ReadText.Should().Be("a"));
            await comp.WaitForAssertionAsync(() => tf.ReadValue.Should().Be("a"));
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs()
            {
                Key = "b"
            }));
            await comp.WaitForAssertionAsync(() => maskField.ReadText.Should().Be("ab"));
            await comp.WaitForAssertionAsync(() => tf.ReadValue.Should().Be("ab"));
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs()
            {
                Key = "c"
            }));
            await comp.WaitForAssertionAsync(() => maskField.ReadText.Should().Be("abc"));
            await comp.WaitForAssertionAsync(() => tf.ReadValue.Should().Be("abc"));

            // test common shortcuts
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs()
            {
                Key = "c",
                MetaKey = true
            }));
            await comp.WaitForAssertionAsync(() => maskField.ReadText.Should().Be("abc"));
            await comp.WaitForAssertionAsync(() => tf.ReadValue.Should().Be("abc"));
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs()
            {
                Key = "v",
                MetaKey = true
            }));
            await comp.WaitForAssertionAsync(() => maskField.ReadText.Should().Be("abc"));
            await comp.WaitForAssertionAsync(() => tf.ReadValue.Should().Be("abc"));
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs()
            {
                Key = "x",
                MetaKey = true
            }));
            await comp.WaitForAssertionAsync(() => maskField.ReadText.Should().Be("abc"));
            await comp.WaitForAssertionAsync(() => tf.ReadValue.Should().Be("abc"));
        }

        [Test]
        public async Task CutShortcut_Should_ClearSelectionAndCopyItToClipboard()
        {
            var comp = Context.Render<MudTextField<string>>();
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Mask, RegexMask.Email()));
            var tf = comp.Instance;
            var maskField = comp.FindComponent<MudMask>().Instance;

            // prep field
            await comp.InvokeAsync(() => maskField.OnFocused(new FocusEventArgs()));
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs()
            {
                Key = "a"
            }));
            await comp.WaitForAssertionAsync(() => maskField.ReadText.Should().Be("a"));
            await comp.WaitForAssertionAsync(() => tf.ReadValue.Should().Be("a"));
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs()
            {
                Key = "b"
            }));
            await comp.WaitForAssertionAsync(() => maskField.ReadText.Should().Be("ab"));
            await comp.WaitForAssertionAsync(() => tf.ReadValue.Should().Be("ab"));
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs()
            {
                Key = "c"
            }));
            await comp.WaitForAssertionAsync(() => maskField.ReadText.Should().Be("abc"));
            await comp.WaitForAssertionAsync(() => tf.ReadValue.Should().Be("abc"));

            // select middle character ('b') and cut it
            await comp.InvokeAsync(() =>
            {
                maskField.OnSelect(1, 2);
                comp.Find("input").CutAsync(new ClipboardEventArgs
                {
                    Type = "cut"
                });
            });
            await comp.WaitForAssertionAsync(() => maskField.ReadText.Should().Be("ac"));
            await comp.WaitForAssertionAsync(() => tf.ReadValue.Should().Be("ac"));
            Context.JSInterop.VerifyInvoke("mudWindow.copyToClipboard", 1);
            Context.JSInterop.Invocations["mudWindow.copyToClipboard"].Single().Arguments.Should().BeEquivalentTo(["b"]);

            // select last character ('c') and cut it
            await comp.InvokeAsync(() =>
            {
                maskField.OnSelect(1, 2);
                comp.Find("input").CutAsync(new ClipboardEventArgs
                {
                    Type = "cut"
                });
            });
            await comp.WaitForAssertionAsync(() => maskField.ReadText.Should().Be("a"));
            await comp.WaitForAssertionAsync(() => tf.ReadValue.Should().Be("a"));
            Context.JSInterop.VerifyInvoke("mudWindow.copyToClipboard", 2);
            Context.JSInterop.Invocations["mudWindow.copyToClipboard"][1].Arguments.Should().BeEquivalentTo(["c"]);
        }

        [Test]
        public async Task Mask_Autofill_ShouldUpdateValueAndText_WhenAutofilled()
        {
            // Arrange
            var mask = new PatternMask("(000) 000-0000");
            var autofillValue = "(123) 456-7890";

            var comp = Context.Render<MudTextField<string>>(parameters => parameters
                .Add(p => p.Mask, mask)
                .Add(p => p.DebounceInterval, 0)
            );

            var textField = comp.Instance;
            var inputElement = comp.Find("input");

            // Act
            // Simulate the 'oninput' event that occurs during browser autofill
            await inputElement.InputAsync(autofillValue);

            // Assert
            textField.ReadText.Should().Be(autofillValue);
        }
    }
}
