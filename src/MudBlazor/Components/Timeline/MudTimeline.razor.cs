// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudTimeline<TData> : MudBaseBindableItemsControl<MudTimelineItem, TData>, IAsyncDisposable

    {
        protected string Classnames =>
            new CssBuilder("mud-timeline")
                .AddClass(Class)
                .Build();

        public async ValueTask DisposeAsync()
        {
            await DisposeAsync(true);
            GC.SuppressFinalize(this);
        }

        protected virtual async ValueTask DisposeAsync(bool disposing)
        {
            if (disposing)
            {
                //TODO
                await Task.Delay(1);
            }
        }

    }
}
