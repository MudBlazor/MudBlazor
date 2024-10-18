using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Microsoft.JSInterop.Infrastructure;
using Moq;
using MudBlazor.Interop;
using MudBlazor.Services;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Services
{
    [TestFixture]
    public class ResizeObserverTests
    {
        private class PseudoElementReferenceContext : ElementReferenceContext
        {

        }

        private Mock<IJSRuntime> _runtimeMock;
        private ResizeObserver _service;

        [SetUp]
        public void SetUp()
        {
            _runtimeMock = new Mock<IJSRuntime>(MockBehavior.Strict);
            _service = new ResizeObserver(_runtimeMock.Object);
        }

        [Test]
        public async Task ObserveAndCache()
        {
            // Arrange
            var random = new Random();

            List<ElementReference> allReferences = new();
            List<ElementReference> notObservedReferences = new();
            Dictionary<ElementReference, BoundingClientRect> resolvedElements = new();

            var amount = 13;
            for (var i = 1; i <= amount; i++)
            {
                var reference = new ElementReference(Guid.NewGuid().ToString(), new PseudoElementReferenceContext());
                var rect = GetRandomRect(random);

                if (i % 4 == 0)
                {
                    reference = new ElementReference();
                    notObservedReferences.Add(reference);
                }
                else if (i % 5 == 0)
                {
                    reference = new ElementReference(Guid.NewGuid().ToString());
                    notObservedReferences.Add(reference);
                }
                else
                {
                    resolvedElements.Add(reference, rect);
                }

                allReferences.Add(reference);
            }

            _runtimeMock.Setup(x => x.InvokeAsync<IEnumerable<BoundingClientRect>>(
                "mudResizeObserver.connect",
                It.Is<object[]>(z =>
                    (Guid)z[0] != default &&
                    (z[1] is DotNetObjectReference<ResizeObserver>) &&
                    (z[2] is IEnumerable<ElementReference>) &&
                    (z[3] is IEnumerable<Guid>) &&
                    (z[4] is ResizeObserverOptions) && ((ResizeObserverOptions)z[4]).EnableLogging == false && ((ResizeObserverOptions)z[4]).ReportRate == 200
                )
            )).ReturnsAsync(resolvedElements.Values).Verifiable();

            // Act
            var actual = await _service.Observe(allReferences);

            // Assert
            actual.Should().BeEquivalentTo(resolvedElements.Values);

            foreach (var item in resolvedElements.Keys)
            {
                _service.IsElementObserved(item).Should().BeTrue();

                var size = _service.GetSizeInfo(item);

                size.Should().BeEquivalentTo(resolvedElements[item]);

                size.Width.Should().Be(_service.GetWidth(item));
                size.Height.Should().Be(_service.GetHeight(item));
            }

            foreach (var item in notObservedReferences)
            {
                _service.IsElementObserved(item).Should().BeFalse();

                var size = _service.GetSizeInfo(item);

                size.Should().BeNull();

                _service.GetWidth(item).Should().Be(0.0);
                _service.GetHeight(item).Should().Be(0.0);
            }

            _runtimeMock.Verify();
        }

        [Test]
        public async Task NotValidElementsToObserve()
        {
            // Arrange
            List<ElementReference> notObservedReferences = new();

            var amount = 10;
            for (var i = 1; i <= amount; i++)
            {
                var reference = new ElementReference(Guid.NewGuid().ToString(), new PseudoElementReferenceContext());

                if (i % 2 == 0)
                {
                    reference = new ElementReference();
                }
                else
                {
                    reference = new ElementReference(Guid.NewGuid().ToString());
                }
                notObservedReferences.Add(reference);
            }

            // Act
            var actual = await _service.Observe(notObservedReferences);

            // Assert
            actual.Should().BeEmpty();
        }

        [Test]
        public async Task Unobserve()
        {
            // Arrange
            var random = new Random();

            Dictionary<ElementReference, BoundingClientRect> resolvedElements = new();

            var amount = 13;
            for (var i = 1; i <= amount; i++)
            {
                var reference = new ElementReference(Guid.NewGuid().ToString(), new PseudoElementReferenceContext());
                var rect = GetRandomRect(random);

                resolvedElements.Add(reference, rect);
            }

            List<Guid> ids = new();
            var observerId = Guid.Empty;

            _runtimeMock.Setup(x => x.InvokeAsync<IEnumerable<BoundingClientRect>>(
                "mudResizeObserver.connect",
                It.Is<object[]>(z =>
                    (Guid)z[0] != default &&
                    (z[1] is DotNetObjectReference<ResizeObserver>) &&
                    (z[2] is IEnumerable<ElementReference>) &&
                    (z[3] is IEnumerable<Guid>) &&
                    (z[4] is ResizeObserverOptions) && ((ResizeObserverOptions)z[4]).EnableLogging == false && ((ResizeObserverOptions)z[4]).ReportRate == 200
                )
            )).ReturnsAsync(resolvedElements.Values).Callback<string, object[]>((x, y) => { observerId = (Guid)y[0]; ids = new List<Guid>((IEnumerable<Guid>)y[3]); }).Verifiable();


            foreach (var item in resolvedElements)
            {
                _runtimeMock.Setup(x => x.InvokeAsync<IJSVoidResult>(
                "mudResizeObserver.disconnect",
                It.Is<object[]>(z =>
                    (Guid)z[0] == observerId &&
                    ids.Contains((Guid)z[1])
                )
            )).ReturnsAsync(Mock.Of<IJSVoidResult>).Callback<string, object[]>((x, y) => { ids.Remove((Guid)y[1]); }).Verifiable();
            }

            await _service.Observe(resolvedElements.Keys);

            foreach (var item in resolvedElements.Keys)
            {
                _service.IsElementObserved(item).Should().BeTrue();

                //Act
                await _service.Unobserve(item);

                //Assert
                _service.IsElementObserved(item).Should().BeFalse();
            }

            _runtimeMock.Verify();
        }

        [Test]
        public async Task OnSizeChanged()
        {
            // Arrange
            var random = new Random();

            Dictionary<ElementReference, BoundingClientRect> resolvedElements = new();

            var amount = 13;
            for (var i = 1; i <= amount; i++)
            {
                var reference = new ElementReference(Guid.NewGuid().ToString(), new PseudoElementReferenceContext());
                var rect = GetRandomRect(random);

                resolvedElements.Add(reference, rect);
            }

            List<Guid> ids = new();

            _runtimeMock.Setup(x => x.InvokeAsync<IEnumerable<BoundingClientRect>>(
                "mudResizeObserver.connect",
                It.Is<object[]>(z =>
                    (Guid)z[0] != default &&
                    (z[1] is DotNetObjectReference<ResizeObserver>) &&
                    (z[2] is IEnumerable<ElementReference>) &&
                    (z[3] is IEnumerable<Guid>) &&
                    (z[4] is ResizeObserverOptions) && ((ResizeObserverOptions)z[4]).EnableLogging == false && ((ResizeObserverOptions)z[4]).ReportRate == 200
                )
            )).ReturnsAsync(resolvedElements.Values).Callback<string, object[]>((x, y) => { ids = new List<Guid>((IEnumerable<Guid>)y[3]); }).Verifiable();

            await _service.Observe(resolvedElements.Keys);

            var changes = new List<ResizeObserver.SizeChangeUpdateInfo>();

            Dictionary<ElementReference, BoundingClientRect> expectedRects = new();

            for (var i = 0; i < resolvedElements.Count(); i++)
            {
                var item = resolvedElements.ElementAt(i);
                var correspondingId = ids[i];

                if (random.NextDouble() > 0.5)
                {
                    changes.Add(new ResizeObserver.SizeChangeUpdateInfo(Guid.NewGuid(), GetRandomRect(random)));
                }
                else
                {
                    var rect = GetRandomRect(random);
                    expectedRects.Add(item.Key, rect);
                    changes.Add(new ResizeObserver.SizeChangeUpdateInfo(correspondingId, rect));
                }
            }

            foreach (var item in expectedRects)
            {
                resolvedElements[item.Key] = item.Value;
            }

            var sizeChangesChecked = false;

            _service.OnResized += (sizeChanges) =>
            {
                //Assertion of event content
                sizeChanges.Should().NotBeEmpty().And.BeEquivalentTo(expectedRects);

                sizeChangesChecked = true;
            };

            //Act
            _service.OnSizeChanged(changes);

            //Assertion
            sizeChangesChecked.Should().BeTrue();

            foreach (var item in resolvedElements)
            {
                var sizeInfo = _service.GetSizeInfo(item.Key);

                sizeInfo.Should().BeEquivalentTo(item.Value);
            }

            _runtimeMock.Verify();
        }

        #region Helper

        private static BoundingClientRect GetRandomRect(Random random)
        {
            return new BoundingClientRect
            {

                Height = random.Next(10, 200) + random.NextDouble(),
                Left = random.Next(10, 200) + random.NextDouble(),

                Top = random.Next(10, 200) + random.NextDouble(),
                Width = random.Next(10, 200) + random.NextDouble(),

            };
        }

        #endregion
    }
}
