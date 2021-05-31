// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
namespace MudBlazor
{
    public class BodyRenderService : IBodyRenderService
    {
        public event Action OnRenderRequested;
        public event Action<MudBodyRendered> OnComponentAdded;
        public event Action<MudBodyRendered> OnComponentRemoved;

        public void AddComponent(MudBodyRendered bodyRendered)
        {
            OnComponentAdded?.Invoke(bodyRendered);
        }

        public void RemoveComponent(MudBodyRendered bodyRendered)
        {
            OnComponentRemoved?.Invoke(bodyRendered);
        }

        public void Render()
        {
            OnRenderRequested?.Invoke();
        }
    }
}
