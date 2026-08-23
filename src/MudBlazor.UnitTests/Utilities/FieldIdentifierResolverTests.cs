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

            public Person? Partner { get; set; }

            public List<Address> Addresses { get; } = [new()];
        }

        private class Counting
        {
            private readonly Queue<Person?> _results;

            public Counting(params Person?[] results) => _results = new Queue<Person?>(results);

            public int Reads { get; private set; }

            public Person? Tracked
            {
                get
                {
                    Reads++;
                    return _results.Count > 1 ? _results.Dequeue() : _results.Peek();
                }
            }

            public Person Boom => throw new InvalidOperationException("boom");
        }

        private static Person? s_static = new();

        private Person _person = new();

        /// <summary>
        /// Runs the same accessor through the resolver and the framework and asserts both fail with the same exception type and message.
        /// </summary>
        private static void AssertThrowsLikeFramework<T>(Expression<Func<T>> accessor)
        {
            var framework = ((Action)(() => FieldIdentifier.Create(accessor))).Should().Throw<Exception>().Which;
            var resolver = ((Action)(() => FieldIdentifierResolver.TryCreate(accessor, out _))).Should().Throw<Exception>().Which;

            resolver.Should().BeOfType(framework.GetType());
            resolver.Message.Should().Be(framework.Message);
        }

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

        /// <summary>
        /// A recognized chain whose model is null must throw what the framework throws, and each side reads the getter exactly once — no fallback re-evaluation.
        /// </summary>
        [Test]
        public void NullModelInNestedChain_ThrowsLikeFramework_WithoutFallingBack()
        {
            var counting = new Counting([null]);

            AssertThrowsLikeFramework(() => counting.Tracked!.Name);

            counting.Reads.Should().Be(2);
        }

        /// <summary>
        /// A getter that returns null once and a value afterwards must fail like the framework's single evaluation would, not bind the second read.
        /// </summary>
        [Test]
        public void GetterReturningNullThenValue_DoesNotBindTheSecondRead()
        {
            var counting = new Counting(null, new Person());
            Expression<Func<string?>> accessor = () => counting.Tracked!.Name;

            Action act = () => FieldIdentifierResolver.TryCreate(accessor, out _);

            act.Should().Throw<ArgumentException>();
            counting.Reads.Should().Be(1);
        }

        /// <summary>
        /// A null owner in the middle of a recognized chain must throw the framework's NullReferenceException instead of reporting unresolved.
        /// </summary>
        [Test]
        public void NullIntermediateOwner_ThrowsLikeFramework()
        {
            _person.Partner = null;

            AssertThrowsLikeFramework(() => _person.Partner!.Home.Street);
        }

        /// <summary>
        /// An empty nullable struct in the chain must throw the framework's InvalidOperationException without invoking Nullable&lt;T&gt;.Value reflectively.
        /// </summary>
        [Test]
        public void EmptyNullableInChain_ThrowsLikeFramework()
        {
            _person.Maybe = null;

            AssertThrowsLikeFramework(() => _person.Maybe!.Value.Inner.Street);
        }

        /// <summary>
        /// An exception thrown by a getter must surface as itself, not wrapped in the reflection envelope.
        /// </summary>
        [Test]
        public void ThrowingGetter_SurfacesTheOriginalException()
        {
            var counting = new Counting();

            AssertThrowsLikeFramework(() => counting.Boom.Name);
        }

        /// <summary>
        /// A null model read straight off a closure field takes the framework's non-compiled path, which fails with a different exception than a nested chain; both must match.
        /// </summary>
        [Test]
        public void NullFlatRoot_ThrowsLikeFramework()
        {
            _person = null!;

            AssertThrowsLikeFramework(() => _person.Name);
        }

        private Person GetPerson() => _person;
    }
}
