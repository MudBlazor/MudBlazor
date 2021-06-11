// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        private static ConcurrentDictionary<Type, Func<TestContext, IRenderedFragment>> componentFactories = new ConcurrentDictionary<Type, Func<TestContext, IRenderedFragment>>();
        private static ConcurrentBag<Type> excludedComponents = new ConcurrentBag<Type>();

        static UserAttributesTests()
        {
            // Add a custom create function for components that cannot be created automatically.
            // These include components that require certain attributes/preriquisites to be set before rendering anything.
            componentFactories.TryAdd(typeof(MudBreadcrumbs), Create_MudBreadcrumbs);
            componentFactories.TryAdd(typeof(MudCarouselItem), Create_MudCarouselItem);
            componentFactories.TryAdd(typeof(MudElement), Create_MudElement);
            componentFactories.TryAdd(typeof(MudOverlay), Create_MudOverlay);
            componentFactories.TryAdd(typeof(MudHighlighter), Create_MudHighlighter);
            componentFactories.TryAdd(typeof(MudTabPanel), Create_MudTabPanel);

            excludedComponents.Add(typeof(MudBooleanInput<>)); // This is the base class of Switch and CheckBox and should be skipped
            excludedComponents.Add(typeof(MudDialog)); // Skip for now
            excludedComponents.Add(typeof(MudHidden)); // No need to test
            excludedComponents.Add(typeof(MudMessageBox)); // Skip for now
            excludedComponents.Add(typeof(MudPicker<>)); // Internal component, skip
            excludedComponents.Add(typeof(MudRadioGroup<>)); // Wrapping component, skip
        }

        [Test]
        public void AllMudComponents_ShouldForwardUserAttributes()
        {
            using var testContext = new TestContext();
            testContext.AddTestServices();
            testContext.Services.Add(new ServiceDescriptor(typeof(IResizeObserver), new MockResizeObserver()));

            var componentTypes = typeof(MudElement).Assembly
                .GetTypes()
                .Where(type => type.IsAssignableTo(typeof(MudComponentBase)) && !type.IsAbstract);

            foreach (var componentType in componentTypes)
            {
                var type = componentType.IsGenericType ? componentType.GetGenericTypeDefinition() : componentType;

                if (excludedComponents.Contains(type))
                    continue;

                if (componentType.IsGenericType && componentType.GetGenericArguments().Length == 1)
                {
                    type = componentType.MakeGenericType(typeof(string));
                }

                var componentName = componentType.Name;
                var componentFactory = componentFactories.GetOrAdd(type, BuildComponentFactory)
                    ?? throw new InvalidOperationException($"Could not build component factory for {componentName}");

                var component = componentFactory(testContext);
                TestComponent(component, componentName);
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

        private static Func<TestContext, IRenderedFragment> BuildComponentFactory(Type componentType)
        {
            var componentName = componentType.Name;
            var createMethod = typeof(UserAttributesTests)
                .GetMethod(nameof(Create), BindingFlags.Static | BindingFlags.NonPublic)
                ?.MakeGenericMethod(componentType);

            if (createMethod == null) return null;

            return testContext =>
            {
                try
                {
                    return createMethod.Invoke(null, new object[] { testContext }) as IRenderedFragment;
                }
                catch (Exception ex)
                {
                    throw new AssertionException($"Failed to create component {componentName}", ex);
                }
            };
        }

        private static IRenderedFragment Create<TComponent>(TestContext testContext)
            where TComponent : MudComponentBase
            => testContext.RenderComponent<TComponent>(attributes => attributes.AddTestUserAttributes());

        private static IRenderedFragment Create_MudBreadcrumbs(TestContext testContext)
        {
            return testContext
                .RenderComponent<MudBreadcrumbs>(attributes => attributes
                    .AddTestUserAttributes()
                    .Add(x => x.Items, new List<BreadcrumbItem> {new BreadcrumbItem("text", "href")}));
        }

        private static IRenderedFragment Create_MudCarouselItem(TestContext testContext)
        {
            var parent = testContext.RenderComponent<MudCarousel<string>>(attributes => attributes
                .Add(x => x.SelectedIndex, 0))
                .Instance;

            return testContext
                .RenderComponent<MudCarouselItem>(attributes => attributes
                    .AddTestUserAttributes()
                    .Add(x => x.Parent, parent));
        }

        private static IRenderedFragment Create_MudElement(TestContext testContext)
        {
            return testContext
                .RenderComponent<MudElement>(attributes => attributes
                    .AddTestUserAttributes()
                    .Add(x => x.HtmlTag, "div"));
        }

        private static IRenderedFragment Create_MudOverlay(TestContext testContext)
        {
            return testContext
                .RenderComponent<MudOverlay>(attributes => attributes
                    .AddTestUserAttributes()
                    .Add(x => x.Visible, true));
        }

        private static IRenderedFragment Create_MudHighlighter(TestContext testContext)
        {
            return testContext
                .RenderComponent<MudHighlighter>(attributes => attributes
                    .AddTestUserAttributes()
                    .Add(x => x.Text, "Hello world")
                    .Add(x => x.HighlightedText, "Hello"));
        }

        private static IRenderedFragment Create_MudTabPanel(TestContext testContext)
        {
            var parent = testContext.RenderComponent<MudTabs>(attributes => attributes
                .Add(x => x.KeepPanelsAlive, true));

            return testContext
                .RenderComponent<MudTabPanel>(attributes => attributes
                    .AddTestUserAttributes()
                    .AddCascadingValue(parent.Instance));
        }
    }
}
