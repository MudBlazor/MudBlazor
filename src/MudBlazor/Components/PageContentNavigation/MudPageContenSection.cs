// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace MudBlazor
{
    public class MudPageContenSection
    {
        public MudPageContenSection(string title, string id)
        {
            Title = title;
            Id = id;
        }

        public string Title { get; set; }
        public string Id { get; set; }
        public bool IsActive { get; private set; }

        public void Activate() => IsActive = true;
        public void Deactive() => IsActive = false;
    }
}
