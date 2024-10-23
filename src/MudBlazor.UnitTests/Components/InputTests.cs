#pragma warning disable CS1998 // async without await

using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq.Expressions;
using Bunit;
using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.UnitTests.Dummy;
using MudBlazor.UnitTests.TestComponents;
using MudBlazor.UnitTests.TestComponents.Field;
using MudBlazor.UnitTests.TestComponents.Form;
using MudBlazor.UnitTests.TestComponents.TextField;
using MudBlazor.UnitTests.Utilities;
using NUnit.Framework;
using static Bunit.ComponentParameterFactory;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class InputTests : BunitTest
    {
        [Test]
        public void ReadOnlyShouldNotHaveClearButton()
        {
            var comp = Context.RenderComponent<MudInput<string>>(p => p
            .Add(x => x.Text, "some value")
            .Add(x => x.Clearable, true)
            .Add(x => x.ReadOnly, false));

            comp.FindAll(".mud-input-clear-button").Count.Should().Be(1);

            comp.SetParametersAndRender(p => p.Add(x => x.ReadOnly, true)); //no clear button when readonly
            comp.FindAll(".mud-input-clear-button").Count.Should().Be(0);
        }
#nullable disable
    }
}
