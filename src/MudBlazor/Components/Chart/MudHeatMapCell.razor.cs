// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable
using Microsoft.AspNetCore.Components;

namespace MudBlazor
{
    public class MudHeatMapCell : MudComponentBase
    {
        [CascadingParameter] internal MudChart? Parent { get; set; }

        [Parameter]
        public int Row { get; set; }

        [Parameter]
        public int Column { get; set; }

        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        protected override void OnInitialized()
        {
            if (Parent == null)
            {
                throw new InvalidOperationException("MudHeatMapCell must be used inside a MudChart component.");
            }

            Parent.AddCell(this);
        }
    }
}

