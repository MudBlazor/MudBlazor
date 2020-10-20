using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MudBlazor
{
    public partial class MudSelect : MudBaseInputText
    {
        protected string Classname =>
            new CssBuilder("mud-select")
            .AddClass(Class)
            .Build();

        /// <summary>
        /// Add the MudSelectItems here
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        [Parameter] public EventCallback<HashSet<string>> SelectedValuesChanged { get; set; }

        /// <summary>
        /// Set of selected values. MultiSelection is true is required. This property is two-way bindable.
        /// </summary>
        [Parameter] public HashSet<string> SelectedValues {
            get;
            set;
        }

        internal bool isMenuOpen { get; set; }

        public async Task OnSelect(string value)
        {
            Value = value;
            isMenuOpen = false;
            StateHasChanged();
            SelectedValues = new HashSet<string>() {value};
            await SelectedValuesChanged.InvokeAsync(SelectedValues);
        }

        public void ShowSelect()
        {
            if(!Disabled)
            {
                isMenuOpen = !isMenuOpen;
                StateHasChanged();
            }
        }
    }
}
