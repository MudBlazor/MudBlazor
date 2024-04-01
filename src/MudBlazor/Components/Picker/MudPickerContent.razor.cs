using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudPickerContent : MudComponentBase
    {
        protected string Classname =>
            new CssBuilder("mud-picker-content")
                .AddClass(Class)
                .Build();

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Picker.Behavior)]
        public RenderFragment ChildContent { get; set; }
    }
}
