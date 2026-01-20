// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AwesomeAssertions;
using Bunit;
using MudBlazor.UnitTests.TestComponents.Utilities;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Converters;

#nullable enable
[TestFixture]
public class ConverterTests : BunitTest
{
    [Test]
    public async Task Converter_ShouldUseAppropriateConverterBasedOnParameterValue()
    {
        var comp = Context.Render<ConverterCompTest>(parameters => parameters.Add(x => x.Converter, null));
        var numericComp = comp.FindComponent<MudNumericField<int>>();
        numericComp.Instance.Converter.Should().BeNull();
        numericComp.Instance.GetConverter().Should().BeOfType<DefaultConverter<int>>();

        await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Converter, new DeferredConverter<int, string?>()));

        numericComp.Instance.Converter.Should().BeOfType<DeferredConverter<int, string?>>();
        numericComp.Instance.GetConverter().Should().BeOfType<DeferredConverter<int, string?>>();

        await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Converter, null));
        numericComp.Instance.Converter.Should().BeNull();
        numericComp.Instance.GetConverter().Should().BeOfType<DefaultConverter<int>>();
    }
}
