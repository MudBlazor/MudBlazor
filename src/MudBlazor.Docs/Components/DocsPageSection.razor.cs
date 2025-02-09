// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using MudBlazor.Docs.Services;

namespace MudBlazor.Docs.Components;

public partial class DocsPageSection
{
    [CascadingParameter] DocsPage DocsPage { get; set; }

    [CascadingParameter] public DocsPageSection ParentSection { get; protected set; }

    [Parameter] public RenderFragment ChildContent { get; set; }

    [Inject] public IRenderQueueService QueueService { get; set; }


    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object> UserAttributes { get; set; } = new Dictionary<string, object>();

    bool _renderImmediately = false;

    public int Level { get; private set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        var count = DocsPage.IncrementSectionCount();
        _renderImmediately = count < QueueService.Capacity;

        Level = (ParentSection?.Level ?? -1) + 1;
    }
}
