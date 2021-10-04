// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudBlazor
{
    public interface ITooltipManager
    {
        void Register(MudTooltip instance);
        void Unregister(MudTooltip instance, bool isOpenend);
        void HandleOpenend(MudTooltip instance);
    }

    public class MudTooltipManager : ITooltipManager
    {
        private List<MudTooltip> _tooltipInstace = new();

        public void Register(MudTooltip instance) => _tooltipInstace.Add(instance);
        public void Unregister(MudTooltip instance, bool isOpenend)
        {
            _tooltipInstace.Remove(instance);
            if(isOpenend == true)
            {
                HandleOpenend(instance);
            }
        }

        public void HandleOpenend(MudTooltip instance)
        {
            foreach (var item in _tooltipInstace.Where(x => x != instance))
            {
                item.RequestClose();
            }
        }
    }
}
