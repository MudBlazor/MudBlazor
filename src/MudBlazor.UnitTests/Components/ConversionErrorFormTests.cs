using AwesomeAssertions;
using Bunit;
using Microsoft.AspNetCore.Components;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    // Regression tests for #11828: a conversion error must not outlive the input that caused it.
    [TestFixture]
    public class ConversionErrorFormTests : BunitTest
    {
        private (MudForm Form, IRenderedComponent<MudForm> Comp, MudTextField<int?> Field) RenderNumberForm()
        {
            var comp = Context.Render<MudForm>(p => p
                .Add(x => x.ValidationDelay, 0)
                .AddChildContent<MudTextField<int?>>());
            return (comp.Instance, comp, comp.FindComponent<MudTextField<int?>>().Instance);
        }

        /// <summary>
        /// While unparsable text is present, the conversion error must fail the form.
        /// </summary>
        [Test]
        public async Task ConversionError_WhileInvalidTextPresent_FailsForm()
        {
            var (form, comp, field) = RenderNumberForm();

            await comp.Find("input").ChangeAsync(new ChangeEventArgs { Value = "ABCDE" });
            field.ConversionError.Should().BeTrue();
            field.GetErrorText().Should().Be("Not a valid number");

            await comp.InvokeAsync(form.ValidateAsync);
            form.IsValid.Should().BeFalse();
            form.Errors.Should().Contain("Not a valid number");
        }

        /// <summary>
        /// #11828: Clearing the unparsable text must clear the conversion error, and the
        /// (non-required) form must validate clean again.
        /// </summary>
        [Test]
        public async Task ConversionError_UserClearsInvalidText_FormBecomesValid()
        {
            var (form, comp, field) = RenderNumberForm();

            await comp.Find("input").ChangeAsync(new ChangeEventArgs { Value = "ABCDE" });
            field.ConversionError.Should().BeTrue();

            await comp.Find("input").ChangeAsync(new ChangeEventArgs { Value = "" });
            field.ConversionError.Should().BeFalse("the offending input is gone");

            await comp.InvokeAsync(form.ValidateAsync);
            form.IsValid.Should().BeTrue();
            form.Errors.Should().BeEmpty();
            field.HasErrors.Should().BeFalse();
        }

        /// <summary>
        /// #11828: ResetValidationAsync must also drop the conversion error so a later
        /// form validation cannot resurrect it.
        /// </summary>
        [Test]
        public async Task ConversionError_ResetValidation_ClearsIt()
        {
            var (form, comp, field) = RenderNumberForm();

            await comp.Find("input").ChangeAsync(new ChangeEventArgs { Value = "ABCDE" });
            await comp.InvokeAsync(form.ValidateAsync);
            form.IsValid.Should().BeFalse();

            await comp.InvokeAsync(form.ResetValidationAsync);
            field.ConversionError.Should().BeFalse();

            await comp.InvokeAsync(form.ValidateAsync);
            form.IsValid.Should().BeTrue();
            form.Errors.Should().BeEmpty();
        }
    }
}
