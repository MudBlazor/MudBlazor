
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Interop;
using MudBlazor.Services;

namespace MudBlazor.UnitTests.Mocks
{
    public class MockResizeObserver : IResizeObserver
    {
        private Dictionary<ElementReference, BoundingClientRect> _cachedValues = new();

        public bool IsVertical { get; set; } = false;

        public event SizeChanged OnResized;

        public void UpdateTotalPanelSize(double newSize)
        {
            var entry = _cachedValues.Last();

            if (IsVertical == false)
            {
                entry.Value.Width = newSize;
            }
            else
            {
                entry.Value.Height = newSize;
            }

            OnResized?.Invoke(new Dictionary<ElementReference, BoundingClientRect> {
                    { entry.Key, entry.Value  },
                });
        }

        public void UpdatePanelSize(int index, double newSize)
        {
            var entry = _cachedValues.ElementAt(index);

            if (IsVertical == false)
            {
                entry.Value.Width = newSize;
            }
            else
            {
                entry.Value.Height = newSize;
            }

            OnResized?.Invoke(new Dictionary<ElementReference, BoundingClientRect> {
                    { entry.Key, entry.Value  },
                });
        }

        public double PanelSize { get; set; } = 250;
        public double PanelTotalSize { get; set; } = 3000;

        public async Task<BoundingClientRect> Observe(ElementReference element) => (await Observe(new[] { element })).FirstOrDefault();

        public Task<IEnumerable<BoundingClientRect>> Observe(IEnumerable<ElementReference> elements)
        {
            List<BoundingClientRect> result = new List<BoundingClientRect>();
            foreach (var item in elements)
            {
                var size = PanelSize;
                // last element is alaways TabsContentSize
                if (item.Id == elements.Last().Id && elements.Count() > 1)
                {
                    size = PanelTotalSize;
                }
                var rect = new BoundingClientRect { Width = size };
                if (IsVertical == true)
                {
                    rect = new BoundingClientRect { Height = size };
                }
                _cachedValues.Add(item, rect);
            }

            return Task.FromResult<IEnumerable<BoundingClientRect>>(result);
        }

        public Task Unobserve(ElementReference element)
        {
            _cachedValues.Remove(element);
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
    }
}
