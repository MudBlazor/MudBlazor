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
            //componentFactories.TryAdd(typeof(MudDialog), Create_MudDialog);
            componentFactories.TryAdd(typeof(MudOverlay), Create_MudOverlay);
            componentFactories.TryAdd(typeof(MudHighlighter), Create_MudHighlighter);

            excludedComponents.Add(typeof(MudBooleanInput<>)); // This is an API only base class that is safe to skip
            excludedComponents.Add(typeof(MudDialog)); // TODO Can we make this work?
            excludedComponents.Add(typeof(MudElement)); // TODO Can we make this work?
            excludedComponents.Add(typeof(MudHidden)); // TODO Can we make this work?
            excludedComponents.Add(typeof(MudMessageBox)); // TODO Can we make this work?
            excludedComponents.Add(typeof(MudPicker<>)); // TODO Can we make this work?
            excludedComponents.Add(typeof(MudRadioGroup<>)); // TODO Can we make this work?
            excludedComponents.Add(typeof(MudSelectItem<>));
            excludedComponents.Add(typeof(MudTabs)); // TODO Can we make this work?
            excludedComponents.Add(typeof(MudTabPanel)); // TODO Can we make this work?
        }

        [Test]
        public void AllMudComponents_ShouldForwardUserAttributes()
        {
            using var testContext = new TestContext();
            testContext.AddTestServices();

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

        private static IRenderedFragment Create_MudDialog(TestContext testContext)
        {
            var dialogInstance = testContext.RenderComponent<MudDialogInstance>();

            return testContext
                .RenderComponent<MudDialog>(attributes => attributes
                    .AddTestUserAttributes()
                    .AddCascadingValue("DialogInstance", dialogInstance.Instance));
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
    }
}
