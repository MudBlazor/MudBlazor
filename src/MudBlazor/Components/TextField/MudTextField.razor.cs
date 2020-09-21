﻿using System;
using System.Windows.Input;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;
using MudBlazor.Extensions;
using Microsoft.AspNetCore.Components.Web;

namespace MudBlazor
{
    public partial class MudTextField : MudBaseInputText
    {
        protected string Classname =>
           new CssBuilder("mud-input-formcontrol").AddClass(Class)
           .Build();

    }
}
