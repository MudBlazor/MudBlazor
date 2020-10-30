using System;
using System.Windows.Input;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;
using MudBlazor.Extensions;
using Microsoft.AspNetCore.Components.Web;

namespace MudBlazor
{
    public partial class MudGenericTextField<T> : MudBaseInput<T>
    {
        protected string Classname =>
           new CssBuilder("mud-input-input-control").AddClass(Class)
           .Build();

    }
}
