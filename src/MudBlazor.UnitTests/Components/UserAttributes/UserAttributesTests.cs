// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using MudBlazor.UnitTests.Mocks;
using NUnit.Framework;
using TestContext = Bunit.TestContext;

namespace MudBlazor.UnitTests.UserAttributes
{
    [TestFixture]
    public sealed class UserAttributesTests
    {
        static UserAttributesTests()
        {
            Exclude(typeof(MudBooleanInput<>)); // This is the base class of Switch and CheckBox and should be skipped
            Exclude(typeof(MudHidden));         // No need to test
            Exclude(typeof(MudPicker<>));       // Internal component, skip
            Exclude(typeof(MudRadioGroup<>));   // Wrapping component, skip
        }

        [Test]
        public void AllMudComponents_ShouldForwardUserAttributes()
        {
            using var testContext = new TestContext();
            testContext.AddTestServices();
            testContext.Services.Add(new ServiceDescriptor(typeof(IResizeObserver), new MockResizeObserver()));

            var componentFactory = new MudComponentFactory
            {
                UserAttributes = new Dictionary<string, object> { { "data-testid", "test-123" } },
            };

            var componentTypes = typeof(MudElement).Assembly
                .GetTypes()
                .Where(type => type.IsAssignableTo(typeof(MudComponentBase)) && !type.IsAbstract);

            foreach (var componentType in componentTypes)
            {
                var type = componentType.IsGenericType ? componentType.GetGenericTypeDefinition() : componentType;

                if (_excludedComponents.Contains(type))
                    continue;

                var component = componentFactory.Create(type, testContext);
                TestComponent(component, componentType.Name);
            }
        }

        private static void TestComponent(IRenderedFragment component, string componentName)
        {
            component.Markup.Should()
                .NotBeEmpty(because: $"the component {componentName} should at least contain one element");

            var elementsWithUserAttributes = component.FindAll("[data-testid='test-123']");
            elementsWithUserAttributes.Should()
                .NotBeEmpty(because: $"UserAttributes should be forwarded by component {componentName}");
        }

        private static ConcurrentBag<Type> _excludedComponents = new ConcurrentBag<Type>();
        private static void Exclude(Type componentType) => _excludedComponents.Add(componentType);
    }
}
