// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace MudBlazor
{
    public interface IBodyRenderService
    {
        event Action OnRenderRequested;
        event Action<MudBodyRendered> OnComponentAdded;
        event Action<MudBodyRendered> OnComponentRemoved;

        void AddComponent(MudBodyRendered bodyRendered);

        void RemoveComponent(MudBodyRendered bodyRendered);
        void Render();
    }
}
