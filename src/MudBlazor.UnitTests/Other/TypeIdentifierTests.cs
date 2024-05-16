// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Numerics;
using FluentAssertions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Other
{
    [TestFixture]
    public class TypeIdentifierTests
    {
        [Test]
        [TestCase(null, false)]
        [TestCase(typeof(int), false)]
        [TestCase(typeof(int?), false)]
        [TestCase(typeof(string), true)]
        public void IsString_Test(Type type, bool expected)
        {
            var isString = TypeIdentifier.IsString(type);
            isString.Should().Be(expected);
        }

        [Test]
        [TestCase(null, false)]
        [TestCase(typeof(DateTime), false)]
        [TestCase(typeof(DateTime?), false)]
        [TestCase(typeof(int), true)]
        [TestCase(typeof(double), true)]
        [TestCase(typeof(decimal), true)]
        [TestCase(typeof(long), true)]
        [TestCase(typeof(short), true)]
        [TestCase(typeof(sbyte), true)]
        [TestCase(typeof(byte), true)]
        [TestCase(typeof(ulong), true)]
        [TestCase(typeof(ushort), true)]
        [TestCase(typeof(uint), true)]
        [TestCase(typeof(float), true)]
        [TestCase(typeof(BigInteger), true)]
        [TestCase(typeof(int?), true)]
        [TestCase(typeof(double?), true)]
        [TestCase(typeof(decimal?), true)]
        [TestCase(typeof(long?), true)]
        [TestCase(typeof(short?), true)]
        [TestCase(typeof(sbyte?), true)]
        [TestCase(typeof(byte?), true)]
        [TestCase(typeof(ulong?), true)]
        [TestCase(typeof(ushort?), true)]
        [TestCase(typeof(uint?), true)]
        [TestCase(typeof(float?), true)]
        [TestCase(typeof(BigInteger?), true)]
        public void IsNumber_Test(Type type, bool expected)
        {
            var isNumber = TypeIdentifier.IsNumber(type);
            isNumber.Should().Be(expected);
        }

        [Test]
        [TestCase(null, false)]
        [TestCase(typeof(int), false)]
        [TestCase(typeof(int?), false)]
        [TestCase(typeof(Adornment), true)]
        [TestCase(typeof(Adornment?), true)]
        public void IsEnum_Test(Type type, bool expected)
        {
            var isEnum = TypeIdentifier.IsEnum(type);
            isEnum.Should().Be(expected);
        }

        [Test]
        [TestCase(null, false)]
        [TestCase(typeof(int), false)]
        [TestCase(typeof(int?), false)]
        [TestCase(typeof(DateTime), true)]
        [TestCase(typeof(DateTime?), true)]
        public void IsDateTime_Test(Type type, bool expected)
        {
            var isDateTime = TypeIdentifier.IsDateTime(type);
            isDateTime.Should().Be(expected);
        }

        [Test]
        [TestCase(null, false)]
        [TestCase(typeof(int), false)]
        [TestCase(typeof(int?), false)]
        [TestCase(typeof(bool), true)]
        [TestCase(typeof(bool?), true)]
        public void IsBoolean_Test(Type type, bool expected)
        {
            var isBoolean = TypeIdentifier.IsBoolean(type);
            isBoolean.Should().Be(expected);
        }

        [Test]
        [TestCase(null, false)]
        [TestCase(typeof(int), false)]
        [TestCase(typeof(int?), false)]
        [TestCase(typeof(Guid), true)]
        [TestCase(typeof(Guid?), true)]
        public void IsGuid_Test(Type type, bool expected)
        {
            var isGuid = TypeIdentifier.IsGuid(type);
            isGuid.Should().Be(expected);
        }
    }
}
