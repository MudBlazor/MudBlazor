// Copyright (c) MudBlazor 2022
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using FluentAssertions;
using MudBlazor.Interop;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Utilities
{
    [TestFixture]
    public class BoundingClientRectConverterTests
    {
        public BoundingClientRectConverterTests()
        {
            _jsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            _jsonSerializerOptions.Converters.Clear();
            _jsonSerializerOptions.Converters.Add(new BoundingClientRectJsonConverter());
        }

        private readonly JsonSerializerOptions _jsonSerializerOptions;

        [Test]
        public void SingleDeserializeTest()
        {
            var json = "{\"x\":282,\"y\":108.5,\"width\":160,\"height\":48,\"top\":108.5,\"right\":442,\"bottom\":156.5,\"left\":282}";
            var result = JsonSerializer.Deserialize<BoundingClientRect>(json, _jsonSerializerOptions)!;
            result.Top.Should().Be(108.5);
            result.Left.Should().Be(282);
            result.Width.Should().Be(160);
            result.Height.Should().Be(48);
        }

        [Test]
        public void MultipleDeserializeTest()
        {
            var json = "[{\"x\":282,\"y\":108.5,\"width\":160,\"height\":48,\"top\":108.5,\"right\":442,\"bottom\":156.5,\"left\":282},{\"x\":442,\"y\":108.5,\"width\":160,\"height\":48,\"top\":108.5,\"right\":602,\"bottom\":156.5,\"left\":442}]";
            var result = JsonSerializer.Deserialize<IEnumerable<BoundingClientRect>>(json, _jsonSerializerOptions)!.ToList();
            result.Count.Should().Be(2);

            result[0].Top.Should().Be(108.5);
            result[0].Left.Should().Be(282);
            result[0].Width.Should().Be(160);
            result[0].Height.Should().Be(48);

            result[1].Top.Should().Be(108.5);
            result[1].Left.Should().Be(442);
            result[1].Width.Should().Be(160);
            result[1].Height.Should().Be(48);

        }

    }
}
