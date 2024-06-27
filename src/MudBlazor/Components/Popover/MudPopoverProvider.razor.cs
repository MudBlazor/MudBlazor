// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Interfaces;

namespace MudBlazor
{
#nullable enable
    public partial class MudPopoverProvider : IDisposable, IPopoverObserver
    {
        private bool _isConnectedToService = false;

        [Inject]
        [Obsolete($"Use {nameof(PopoverService)} instead. This will be removed in v7")]
        public IMudPopoverService Service { get; set; } = null!;

        [Inject]
        internal IPopoverService PopoverService { get; set; } = null!;

        /// <summary>
        /// In some scenarios we need more than one ThemeProvider but we must not have more than one
        /// PopoverProvider. Set a cascading value with UsePopoverProvider=false to prevent it.
        /// </summary>
        [CascadingParameter(Name = "UsePopoverProvider")]
        public bool Enabled { get; set; } = true;

        public void Dispose()
        {
#pragma warning disable CS0618
            //TODO: For backward compatibility with old service. Should be removed in v7
            Service.FragmentsChanged -= Service_FragmentsChanged;
#pragma warning restore CS0618
            PopoverService.Unsubscribe(this);
        }

        protected override void OnInitialized()
        {
            if (Enabled == false)
            {
                return;
            }

#pragma warning disable CS0618
            //TODO: For backward compatibility with old service. Should be removed in v7
            Service.FragmentsChanged += Service_FragmentsChanged;
#pragma warning restore CS0618
            PopoverService.Subscribe(this);
            _isConnectedToService = true;
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            if (!Enabled && _isConnectedToService)
            {
#pragma warning disable CS0618
                //TODO: For backward compatibility with old service. Should be removed in v7 with the _isConnectedToService
                Service.FragmentsChanged -= Service_FragmentsChanged;
#pragma warning restore CS0618
                PopoverService.Unsubscribe(this);
                _isConnectedToService = false;

                return;
            }

#pragma warning disable CS0618
            //TODO: For backward compatibility with old service. Whole block should be removed in v7
            if (Enabled && !_isConnectedToService)
            {
                Service.FragmentsChanged -= Service_FragmentsChanged; // make sure to avoid multiple registration
                Service.FragmentsChanged += Service_FragmentsChanged;
                _isConnectedToService = true;
            }
#pragma warning restore CS0618

            // Let's in our new case ignore _isConnectedToService and always update the subscription except Enabled = false. The manager is specifically designed for it.
            // The reason is because If an observer throws an exception during the PopoverCollectionUpdatedNotification, indicating a malfunction, it will be automatically unsubscribed.
            if (Enabled)
            {
                PopoverService.Subscribe(this);
            }

        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender && Enabled && PopoverService.PopoverOptions.ThrowOnDuplicateProvider)
            {
                if (await PopoverService.GetProviderCountAsync() > 1)
                {
                    throw new InvalidOperationException("Duplicate MudPopoverProvider detected. Please ensure there is only one provider, or disable this warning with PopoverOptions.ThrowOnDuplicateProvider.");
                }
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        //TODO: For backward compatibility with old service. Should be removed in v7
        private void Service_FragmentsChanged(object? sender, EventArgs e)
        {
            InvokeAsync(StateHasChanged);
        }

        //TODO: For backward compatibility with old service. Should be removed in v7
#pragma warning disable CS0618
        private IEnumerable<IMudPopoverHolder> GetActivePopovers()
        {
            return PopoverService.ActivePopovers.Concat(Service.Handlers);
        }
#pragma warning restore CS0618

        /// <inheritdoc />
        Guid IPopoverObserver.Id { get; } = Guid.NewGuid();

        /// <inheritdoc />
        async Task IPopoverObserver.PopoverCollectionUpdatedNotificationAsync(PopoverHolderContainer container, CancellationToken cancellationToken)
        {
            switch (container.Operation)
            {
                // Update popover individually
                case PopoverHolderOperation.Update:
                    {
                        foreach (var holder in container.Holders)
                        {
                            if (cancellationToken.IsCancellationRequested)
                            {
                                return;
                            }

                            if (holder.ElementReference is IMudStateHasChanged stateHasChanged)
                            {
                                await InvokeAsync(stateHasChanged.StateHasChanged);
                            }
                        }

                        break;
                    }
                // Update whole MudPopoverProvider
                case PopoverHolderOperation.Create:
                case PopoverHolderOperation.Remove:
                    await InvokeAsync(StateHasChanged);
                    break;
            }
        }
    }
}
