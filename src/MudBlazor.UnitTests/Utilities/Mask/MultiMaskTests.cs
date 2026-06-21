// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AwesomeAssertions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Utilities.Mask
{
#nullable enable
    [TestFixture]
    public class MultiMaskTests
    {
        [Test]
        public void MultiMask()
        {
            var mask = new MultiMask("0000 0000 0000 0000",
                new MaskOption("American Express", "0000 000000 00000", @"^(34|37)"),
                new MaskOption("Diners Club", "0000 000000 0000", @"^(30[0123459])"),
                new MaskOption("JCB", "0000 0000 0000 0000", @"^(35|2131|1800)"),
                new MaskOption("VISA", "0000 0000 0000 0000", @"^4"),
                new MaskOption("MasterCard", "0000 0000 0000 0000", @"^(5[1-5]|2[2-7])"),
                new MaskOption("Discover", "0000 0000 0000 0000", @"^(6011|65|64[4-9])")
            );
            MaskOption? option = null;
            var eventCount = 0;
            mask.OptionDetected = (o, _) =>
            {
                eventCount++;
                option = o;
            };
            mask.DetectedOption.Should().BeNull();
            mask.Insert("9");
            mask.DetectedOption.Should().BeNull();

            mask.SetText("34123");
            mask.Backspace();
            mask.ToString().Should().Be("3412 |");
            mask.Insert("3");
            mask.DetectedOption.Should().NotBeNull();
            mask.DetectedOption.Value.Id.Should().Be("American Express");
            option.GetValueOrDefault().Id.Should().Be("American Express");
            eventCount.Should().Be(1);

            mask.Insert("45678901234567890");
            mask.DetectedOption.Value.Id.Should().Be("American Express");
            mask.Text.Should().Be("3412 345678 90123");
            mask.SetText("30312345678901234567890");
            mask.DetectedOption.Value.Id.Should().Be("Diners Club");
            mask.Text.Should().Be("3031 234567 8901");
            option.GetValueOrDefault().Id.Should().Be("Diners Club");
            eventCount.Should().Be(2);

            mask.CaretPos = 1;
            mask.Delete();
            mask.DetectedOption.Should().BeNull();
            mask.Text.Should().Be("3312 3456 7890 1");
            option.Should().Be(null);
            eventCount.Should().Be(3);

            mask.Selection = (0, 2);
            mask.Insert("4");
            mask.DetectedOption!.Value.Id.Should().Be("VISA");
            mask.Text.Should().Be("4123 4567 8901 ");
            option.Value.Id.Should().Be("VISA");
            eventCount.Should().Be(4);
            mask.UpdateFrom(new MultiMask("0000000000",
                new MaskOption("O1", "0-000.000.000", @"^4"),
                new MaskOption("O2", "00-00.00.00", @"^5"))
            {
                OptionDetected = (o, _) =>
                {
                    eventCount++;
                    option = o;
                }
            });
            mask.DetectedOption.Value.Id.Should().Be("O1");
            mask.Text.Should().Be("4-123.456.789");
            option.Value.Id.Should().Be("O1");
            eventCount.Should().Be(5);
            mask.UpdateFrom(null);
            mask.DetectedOption.Value.Id.Should().Be("O1");
            mask.Text.Should().Be("4-123.456.789");
            option.Value.Id.Should().Be("O1");
            eventCount.Should().Be(5);
        }

        [Test]
        public void MultiMask_NoOptions()
        {
            // Arrange
            var mask = new MultiMask("0000 0000");

            // Act
            mask.Insert("1234");

            // Assert
            mask.DetectedOption.Should().BeNull();
            mask.Text.Should().Be("1234 ");
        }

        [Test]
        public void MultiMask_NullOptions()
        {
            // Arrange
            var mask = new MultiMask("0000 0000", null);

            // Act
            mask.Insert("1234");

            // Assert
            mask.DetectedOption.Should().BeNull();
            mask.Text.Should().Be("1234 ");
        }

        [Test]
        public void MultiMask_OptionWithNullRegex()
        {
            // Arrange
            var mask = new MultiMask("0000",
                new MaskOption("Test", "00-00", null));

            // Act
            mask.Insert("1234");

            // Assert
            mask.DetectedOption.Should().BeNull();
        }

        [Test]
        public void MultiMask_GetCleanText()
        {
            // Arrange
            var mask = new MultiMask("0000 0000 0000 0000",
                new MaskOption("VISA", "0000 0000 0000 0000", @"^4"));
            mask.Insert("4123456789012345");

            // Act
            var cleanText = mask.GetCleanText();

            // Assert
            cleanText.Should().Be("4123 4567 8901 2345");
        }

        [Test]
        public void MultiMask_SwitchOption_MidInput()
        {
            // Arrange
            var mask = new MultiMask("0000",
                new MaskOption("Option1", "00-00", @"^1"),
                new MaskOption("Option2", "000-0", @"^2"));

            // Act
            mask.Insert("12");
            var firstOption = mask.DetectedOption;
            mask.Clear();
            mask.Insert("23");
            var secondOption = mask.DetectedOption;

            // Assert
            firstOption.Should().NotBeNull();
            firstOption.Value.Id.Should().Be("Option1");
            secondOption.Should().NotBeNull();
            secondOption.Value.Id.Should().Be("Option2");
        }

        [Test]
        public void MultiMask_OptionDetected_EventFires()
        {
            // Arrange
            var mask = new MultiMask("0000",
                new MaskOption("Test", "00-00", @"^1"));
            var eventFired = false;
            MaskOption? option = null;
            string? text = null;
            mask.OptionDetected = (opt, txt) =>
            {
                eventFired = true;
                option = opt;
                text = txt;
            };

            // Act
            mask.Insert("12");

            // Assert
            eventFired.Should().BeTrue();
            option.Should().NotBeNull();
            option.Value.Id.Should().Be("Test");
            text.Should().Be("12-");
        }

        [Test]
        public void MultiMask_DeleteChangesOption()
        {
            // Arrange
            var mask = new MultiMask("0000",
                new MaskOption("Option1", "00-00", @"^1"));
            mask.Insert("12");
            var initialOption = mask.DetectedOption;

            // Act
            mask.CaretPos = 0;
            mask.Delete();

            // Assert
            initialOption.Should().NotBeNull();
            mask.DetectedOption.Should().BeNull();
        }

        [Test]
        public void MultiMask_BackspaceChangesOption()
        {
            // Arrange
            var mask = new MultiMask("0000",
                new MaskOption("Option1", "00-00", @"^1"));
            mask.Insert("12");
            var initialOption = mask.DetectedOption;

            // Act
            mask.CaretPos = 1;
            mask.Backspace();

            // Assert
            initialOption.Should().NotBeNull();
            mask.DetectedOption.Should().BeNull();
        }

        [Test]
        public void MultiMask_OptionDetected_DoesNotRefireWhileOptionUnchanged()
        {
            // Arrange
            var mask = new MultiMask("0000 0000",
                new MaskOption("VISA", "0000 0000", @"^4"));
            var eventCount = 0;
            mask.OptionDetected = (_, _) => eventCount++;

            // Act
            mask.Insert("4");
            mask.Insert("123");
            mask.Insert("5678");

            // Assert: event fires once on detection, not for every keystroke within the same option
            mask.DetectedOption!.Value.Id.Should().Be("VISA");
            eventCount.Should().Be(1);
        }

        [Test]
        public void MultiMask_CheckOption_ReturnsFirstMatchingOptionInDeclarationOrder()
        {
            // Arrange: two options whose regexes both match the same input
            var mask = new MultiMask("0000",
                new MaskOption("First", "00-00", @"^1"),
                new MaskOption("Second", "0000", @"^1"));

            // Act
            mask.Insert("12");

            // Assert: the first declared matching option wins
            mask.DetectedOption.Should().NotBeNull();
            mask.DetectedOption.Value.Id.Should().Be("First");
        }

        [Test]
        public void MultiMask_UpdateFrom_NonMultiMask_PreservesDetectedOption()
        {
            // Arrange
            var mask = new MultiMask("0000",
                new MaskOption("VISA", "00-00", @"^4"));
            mask.Insert("4");
            mask.DetectedOption!.Value.Id.Should().Be("VISA");

            // Act: updating from a plain PatternMask must not touch multi-mask option state
            mask.UpdateFrom(new PatternMask("000000"));

            // Assert
            mask.DetectedOption.Should().NotBeNull();
            mask.DetectedOption!.Value.Id.Should().Be("VISA");
        }

        [Test]
        public void MultiMask_UpdateFrom_NonMatchingOptions_ClearsDetectedOptionAndFires()
        {
            // Arrange
            var mask = new MultiMask("0000",
                new MaskOption("VISA", "00-00", @"^4"));
            mask.Insert("4");
            mask.DetectedOption!.Value.Id.Should().Be("VISA");

            var eventCount = 0;
            MaskOption? detected = new MaskOption("sentinel", "", null);

            // Act: the new options no longer match the current text, so detection must reset
            mask.UpdateFrom(new MultiMask("0000",
                new MaskOption("Other", "0-0", @"^9"))
            {
                OptionDetected = (option, _) =>
                {
                    eventCount++;
                    detected = option;
                }
            });

            // Assert: UpdateFrom re-evaluates options and fires de-detection with null
            mask.DetectedOption.Should().BeNull();
            eventCount.Should().Be(1);
            detected.Should().BeNull();
        }
    }
}
