
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Interop;
using MudBlazor.Services;

namespace MudBlazor.UnitTests.Mocks
{
    public class MockResizeObserverFactory : IResizeObserverFactory
    {
        private MockResizeObserver _observer;

        public MockResizeObserverFactory()
        {

        }

        public MockResizeObserverFactory(MockResizeObserver observer)
        {
            _observer = observer;
        }

        public IResizeObserver Create(ResizeObserverOptions options) => _observer ?? new MockResizeObserver();
        public IResizeObserver Create() => Create(new ResizeObserverOptions());
    }

    public class MockResizeObserver : IResizeObserver
    {
        private Dictionary<ElementReference, BoundingClientRect> _cachedValues = new();

        public bool IsVertical { get; set; } = false;

        public bool WrapHeaders { get; set; } = false;

        public event SizeChanged OnResized;

        public void UpdateTotalPanelSize(double newSize)
        {
            var entry = _cachedValues.Last();

            if (IsVertical)
            {
                entry.Value.Height = newSize;
            }
            else
            {
                entry.Value.Width = newSize;
            }

            OnResized?.Invoke(new Dictionary<ElementReference, BoundingClientRect> {
                    { entry.Key, entry.Value  },
                });
        }

        public void UpdatePanelSize(int index, double newSize)
        {
            var entry = _cachedValues.ElementAt(index);
            double original;

            if (IsVertical)
            {
                original = entry.Value.Height;
                entry.Value.Height = newSize;
            }
            else
            {
                original = entry.Value.Width;
                entry.Value.Width = newSize;
            }

            foreach (var otherEntry in _cachedValues.Skip(index + 1).SkipLast(1))
            {
                if (IsVertical)
                    otherEntry.Value.Top += newSize - original;
                else
                    otherEntry.Value.Left += newSize - original;
            }

            OnResized?.Invoke(new Dictionary<ElementReference, BoundingClientRect> {
                    { entry.Key, entry.Value  },
                });
        }

        public double PanelSize { get; set; } = 250;
        public double PanelTotalSize { get; set; } = 3000;

        public async Task<BoundingClientRect> Observe(ElementReference element) => (await Observe(new[] { element })).FirstOrDefault();

        private bool _firstBatchProcessed = false;

        public Task<IEnumerable<BoundingClientRect>> Observe(IEnumerable<ElementReference> elements)
        {
            var rowIndex = 0;
            var columnIndex = 0;

            var result = new List<BoundingClientRect>();
            foreach (var item in elements)
            {
                BoundingClientRect rect;
                // last element is always TabsContentSize
                if (item.Id == elements.Last().Id && _firstBatchProcessed == false)
                {
                    if (IsVertical)
                        rect = new BoundingClientRect { Width = PanelSize, Height = PanelTotalSize, };
                    else
                        rect = new BoundingClientRect { Width = PanelTotalSize, Height = 48, };
                }
                else
                {
                    if (IsVertical)
                        rect = new BoundingClientRect
                        {
                            Width = 160,
                            Height = PanelSize,
                            Left = 0,
                            Top = columnIndex * PanelSize,
                        };
                    else
                    {
                        rect = new BoundingClientRect
                        {
                            Width = PanelSize,
                            Height = 48,
                            Left = columnIndex * PanelSize,
                            Top = rowIndex * 48,
                        };

                        if (WrapHeaders && rect.Left + rect.Width > PanelTotalSize)
                        {
                            rect.Left = 0;
                            rect.Top += 48;
                            columnIndex = 0;
                            rowIndex++;
                        }
                    }

                    columnIndex++;
                }

                _cachedValues.Add(item, rect);
            }

            _firstBatchProcessed = true;

            return Task.FromResult<IEnumerable<BoundingClientRect>>(result);
        }

        public Task Unobserve(ElementReference element)
        {
            _cachedValues.Remove(element);
            return Task.CompletedTask;
        }

        public Task Resync()
        {
            return Task.CompletedTask;
        }

        public ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }

        public BoundingClientRect GetSizeInfo(ElementReference reference)
        {
            if (_cachedValues.ContainsKey(reference) == false)
            {
                return null;
            }

            return _cachedValues[reference];
        }
        public double GetHeight(ElementReference reference) => GetSizeInfo(reference)?.Height ?? 0.0;
        public double GetWidth(ElementReference reference) => GetSizeInfo(reference)?.Width ?? 0.0;
        public bool IsElementObserved(ElementReference reference) => _cachedValues.ContainsKey(reference);

        public void Dispose()
        {

        }
    }
}
