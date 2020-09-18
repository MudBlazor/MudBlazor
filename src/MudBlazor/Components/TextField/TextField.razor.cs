using System;
using System.Windows.Input;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;
using MudBlazor.Extensions;
using Microsoft.AspNetCore.Components.Web;

namespace MudBlazor
{
    public class ComponentBaseTextField : MudBaseInputText
    {
        protected string Classname =>
       new CssBuilder().AddClass(Class)
       .Build();

    }
}
