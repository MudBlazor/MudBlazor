// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using AwesomeAssertions;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor.Utilities.Expressions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Utilities
{
    [TestFixture]
    public class FieldIdentifierResolverTests
    {
        private class Address
        {
            public string? Street { get; set; }
        }

        private struct Wrapper
        {
            public Address Inner { get; set; }
        }

        private class Person
        {
            public Wrapper? Maybe { get; set; } = new Wrapper { Inner = new Address() };

            public string? Name { get; set; }

            public Address Home { get; set; } = new();

            public List<Address> Addresses { get; } = [new()];
        }

        private static Person? s_static = new();

        private Person _person = new();

        /// <summary>
        /// A chain through a nullable struct resolves to the same field the framework resolves.
        /// </summary>
        [Test]
        public void NullableStructInChain_MatchesFieldIdentifierCreate()
        {
            // The tree carries a real Nullable<T>.Value member access, and reflection accepts the boxed T as its owner.
            Expression<Func<string?>> accessor = () => _person.Maybe!.Value.Inner.Street;

            FieldIdentifierResolver.TryCreate(accessor, out var resolved).Should().BeTrue();
            resolved.Should().Be(FieldIdentifier.Create(accessor));
            resolved.Model.Should().BeSameAs(_person.Maybe!.Value.Inner);
        }

        /// <summary>
        /// A member read straight off the model resolves to the same field the framework resolves.
        /// </summary>
        [Test]
        public void FlatPath_MatchesFieldIdentifierCreate()
        {
            Expression<Func<string?>> accessor = () => _person.Name;

            FieldIdentifierResolver.TryCreate(accessor, out var resolved).Should().BeTrue();
            resolved.Should().Be(FieldIdentifier.Create(accessor));
            resolved.Model.Should().BeSameAs(_person);
        }

        /// <summary>
        /// A nested member path resolves to the same field the framework resolves, without compiling the expression.
        /// </summary>
        [Test]
        public void NestedPath_MatchesFieldIdentifierCreate()
        {
            Expression<Func<string?>> accessor = () => _person.Home.Street;

            FieldIdentifierResolver.TryCreate(accessor, out var resolved).Should().BeTrue();
            resolved.Should().Be(FieldIdentifier.Create(accessor));
            resolved.Model.Should().BeSameAs(_person.Home);
        }

        /// <summary>
        /// Replacing the model instance must produce a new field identifier, otherwise validation would stay bound to the discarded object.
        /// </summary>
        [Test]
        public void ModelSwap_ResolvesTheCurrentInstance()
        {
            Expression<Func<string?>> accessor = () => _person.Home.Street;
            FieldIdentifierResolver.TryCreate(accessor, out var before);

            _person = new Person();

            FieldIdentifierResolver.TryCreate(accessor, out var after).Should().BeTrue();
            after.Should().NotBe(before);
            after.Model.Should().BeSameAs(_person.Home);
        }

        /// <summary>
        /// A static root is not handled and must be reported as unresolved so the caller falls back.
        /// </summary>
        [Test]
        public void StaticRoot_ReportsUnresolved()
        {
            Expression<Func<string?>> accessor = () => s_static!.Name;

            FieldIdentifierResolver.TryCreate(accessor, out _).Should().BeFalse();
        }

        /// <summary>
        /// An indexer in the path is not handled and must be reported as unresolved so the caller falls back.
        /// </summary>
        [Test]
        public void IndexerPath_ReportsUnresolved()
        {
            Expression<Func<string?>> accessor = () => _person.Addresses[0].Street;

            FieldIdentifierResolver.TryCreate(accessor, out _).Should().BeFalse();
        }

        /// <summary>
        /// A method call in the path is not handled and must be reported as unresolved so the caller falls back.
        /// </summary>
        [Test]
        public void MethodCallPath_ReportsUnresolved()
        {
            Expression<Func<string?>> accessor = () => GetPerson().Name;

            FieldIdentifierResolver.TryCreate(accessor, out _).Should().BeFalse();
        }

        /// <summary>
        /// An expression that is not a member access at all is reported as unresolved so the caller falls back and the framework raises its own error.
        /// </summary>
        [Test]
        public void NonMemberBody_ReportsUnresolved()
        {
            Expression<Func<string?>> accessor = () => GetPerson().Name!.ToUpperInvariant();

            FieldIdentifierResolver.TryCreate(accessor, out _).Should().BeFalse();
        }

        private Person GetPerson() => _person;
    }
}
