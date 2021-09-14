// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace MudBlazor
{
    public interface IMudPopoverService
    {
        MudPopoverHandler Register(RenderFragment fragment);
        Task<bool> Unregister(MudPopoverHandler hanlder);
        IEnumerable<MudPopoverHandler> Handlers { get; }
        event EventHandler FragmentsChanged;
    }

    public class MudPopoverHandler
    {
        private IJSRuntime _runtime;
        private readonly Action _updater;

        public Guid Id { get; init; }
        public RenderFragment Fragment { get; private set; }
        public bool IsConnected { get; internal set; }
        public string Class { get; set; }
        public string Style { get; set; }
        public object Tag { get; set; }
        public Dictionary<string, object> UserAttributes { get; set; } = new Dictionary<string, object>();

        public MudPopoverHandler(RenderFragment fragment, IJSRuntime jsInterop, Action updater)
        {
            Fragment = fragment;
            _runtime = jsInterop;
            _updater = updater;
            Id = Guid.NewGuid();
        }

        public void SetComponentBaseParameters(MudComponentBase componentBase, string @class, string style)
        {
            Class = @class;
            Style = style;
            Tag = componentBase.Tag;
            UserAttributes = componentBase.UserAttributes;
        }

        public void UpdateFragment(RenderFragment fragment,
            MudComponentBase componentBase, string @class, string style)
        {
            Fragment = fragment;
            SetComponentBaseParameters(componentBase, @class, @style);
            _updater?.Invoke();
        }

        public async Task Initialized()
        {
            await _runtime.InvokeVoidAsync("mudPopover.connect", Id);
            IsConnected = true;
        }

        public async Task Detach()
        {
            await _runtime.InvokeVoidAsync("mudPopover.disconnect", Id);
            IsConnected = false;
        }
    }

    public class MudPopoverService : IMudPopoverService
    {
        private List<MudPopoverHandler> _handlers = new();
        private readonly IJSRuntime _jsInterop;

        public event EventHandler FragmentsChanged;

        public IEnumerable<MudPopoverHandler> Handlers => _handlers.AsEnumerable();

        public MudPopoverService(IJSRuntime jsInterop)
        {
            _jsInterop = jsInterop ?? throw new ArgumentNullException(nameof(jsInterop));
        }



        public MudPopoverHandler Register(RenderFragment fragment)
        {
            var handler = new MudPopoverHandler(fragment, _jsInterop, () => FragmentsChanged?.Invoke(this, EventArgs.Empty));
            _handlers.Add(handler);

            FragmentsChanged?.Invoke(this, EventArgs.Empty);

            return handler;
        }

        public async Task<bool> Unregister(MudPopoverHandler handler)
        {
            if (handler == null) { return false; }
            if (_handlers.Contains(handler) == false) { return false; }

            if (handler.IsConnected == false) { return false; }

            await handler.Detach();
            _handlers.Remove(handler);

            FragmentsChanged?.Invoke(this, EventArgs.Empty);

            return true;
        }
    }
}
