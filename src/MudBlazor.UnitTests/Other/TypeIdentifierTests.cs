// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Numerics;
using AwesomeAssertions;

namespace MudBlazor.UnitTests.Other
{
    public class TypeIdentifierTests
    {
        [Test]
        [Arguments(null, false)]
        [Arguments(typeof(int), false)]
        [Arguments(typeof(int?), false)]
        [Arguments(typeof(string), true)]
        public void IsString(Type type, bool expected)
        {
            var isString = TypeIdentifier.IsString(type);
            isString.Should().Be(expected);
        }

        [Test]
        [Arguments(null, false)]
        [Arguments(typeof(DateTime), false)]
        [Arguments(typeof(DateTime?), false)]
        [Arguments(typeof(int), true)]
        [Arguments(typeof(double), true)]
        [Arguments(typeof(decimal), true)]
        [Arguments(typeof(long), true)]
        [Arguments(typeof(short), true)]
        [Arguments(typeof(sbyte), true)]
        [Arguments(typeof(byte), true)]
        [Arguments(typeof(ulong), true)]
        [Arguments(typeof(ushort), true)]
        [Arguments(typeof(uint), true)]
        [Arguments(typeof(float), true)]
        [Arguments(typeof(BigInteger), true)]
        [Arguments(typeof(int?), true)]
        [Arguments(typeof(double?), true)]
        [Arguments(typeof(decimal?), true)]
        [Arguments(typeof(long?), true)]
        [Arguments(typeof(short?), true)]
        [Arguments(typeof(sbyte?), true)]
        [Arguments(typeof(byte?), true)]
        [Arguments(typeof(ulong?), true)]
        [Arguments(typeof(ushort?), true)]
        [Arguments(typeof(uint?), true)]
        [Arguments(typeof(float?), true)]
        [Arguments(typeof(BigInteger?), true)]
        public void IsNumber(Type type, bool expected)
        {
            var isNumber = TypeIdentifier.IsNumber(type);
            isNumber.Should().Be(expected);
        }

        [Test]
        [Arguments(null, false)]
        [Arguments(typeof(int), false)]
        [Arguments(typeof(int?), false)]
        [Arguments(typeof(Adornment), true)]
        [Arguments(typeof(Adornment?), true)]
        public void IsEnum(Type type, bool expected)
        {
            var isEnum = TypeIdentifier.IsEnum(type);
            isEnum.Should().Be(expected);
        }

        [Test]
        [Arguments(null, false)]
        [Arguments(typeof(int), false)]
        [Arguments(typeof(int?), false)]
        [Arguments(typeof(DateTime), true)]
        [Arguments(typeof(DateTime?), true)]
        public void IsDateTime(Type type, bool expected)
        {
            var isDateTime = TypeIdentifier.IsDateTime(type);
            isDateTime.Should().Be(expected);
        }

        [Test]
        [Arguments(null, false)]
        [Arguments(typeof(int), false)]
        [Arguments(typeof(int?), false)]
        [Arguments(typeof(bool), true)]
        [Arguments(typeof(bool?), true)]
        public void IsBoolean(Type type, bool expected)
        {
            var isBoolean = TypeIdentifier.IsBoolean(type);
            isBoolean.Should().Be(expected);
        }

        [Test]
        [Arguments(null, false)]
        [Arguments(typeof(int), false)]
        [Arguments(typeof(int?), false)]
        [Arguments(typeof(DateOnly), true)]
        [Arguments(typeof(DateOnly?), true)]
        [Arguments(typeof(DateTime), false)]
        [Arguments(typeof(DateTime?), false)]
        public void IsDateOnly(Type type, bool expected)
        {
            var isDateOnly = TypeIdentifier.IsDateOnly(type);
            isDateOnly.Should().Be(expected);
        }

        [Test]
        [Arguments(null, false)]
        [Arguments(typeof(int), false)]
        [Arguments(typeof(int?), false)]
        [Arguments(typeof(Guid), true)]
        [Arguments(typeof(Guid?), true)]
        public void IsGuid(Type type, bool expected)
        {
            var isGuid = TypeIdentifier.IsGuid(type);
            isGuid.Should().Be(expected);
        }
    }
}
