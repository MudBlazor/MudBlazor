// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using System.Threading.Tasks;
using AwesomeAssertions;
using Bunit;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor.UnitTests.TestComponents.TextField;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class NestedForExpressionTests : BunitTest
    {
        /// <summary>
        /// A data annotation on a nested For path is found and reported.
        /// </summary>
        [Test]
        public async Task NestedFor_ReportsDataAnnotationError()
        {
            var comp = Context.Render<TextFieldValidationNestedForTest>();
            var textFieldComp = comp.FindComponent<MudTextField<string>>();
            await textFieldComp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.DebounceInterval, 0));

            await comp.Find("input").ChangeAsync("Quux");
            await comp.InvokeAsync(() => textFieldComp.Instance.ValidateAsync());

            textFieldComp.Instance.ValidationErrors.Should().ContainSingle().Which.Should().Be("Should not be longer than 3");
        }

        /// <summary>
        /// Replacing the nested model instance rebinds the field so validation keeps working against the new object.
        /// </summary>
        [Test]
        public async Task NestedFor_RebindsAfterNestedModelIsReplaced()
        {
            var comp = Context.Render<TextFieldValidationNestedForTest>();
            var textFieldComp = comp.FindComponent<MudTextField<string>>();
            await textFieldComp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.DebounceInterval, 0));

            await comp.InvokeAsync(comp.Instance.ReplaceNestedModel);
            await comp.InvokeAsync(comp.Instance.ForceRender);

            // The validation error alone would not prove this, because StringLength only inspects the value and reports the same error against either instance.
            GetFieldIdentifier(textFieldComp.Instance).Model.Should().BeSameAs(comp.Instance.NestedModel);

            await comp.Find("input").ChangeAsync("Quux");
            await comp.InvokeAsync(() => textFieldComp.Instance.ValidateAsync());

            textFieldComp.Instance.ValidationErrors.Should().ContainSingle().Which.Should().Be("Should not be longer than 3");
        }

        private static FieldIdentifier GetFieldIdentifier(MudTextField<string> textField) =>
            (FieldIdentifier)typeof(MudFormComponent<string, string>)
                .GetField("_fieldIdentifier", BindingFlags.NonPublic | BindingFlags.Instance)!
                .GetValue(textField)!;
    }
}
