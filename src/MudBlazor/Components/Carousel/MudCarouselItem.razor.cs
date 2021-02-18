using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudCarouselItem : MudComponentBase
    {
        protected string Classname =>
                    new CssBuilder("mud-carousel-item")
                         .AddClass("mud-carousel-transition-fade", Transition == Transition.Fade)
                         .AddClass(Class)
                         .Build();

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [CascadingParameter]
        protected internal Base.MudBaseItemsControl<MudCarouselItem> Parent { get; set; }

        [Parameter]
        public Transition Transition { get; set; } = Transition.Fade;

        public bool IsVisible
        {
            get
            {
                if (Parent == null)
                    return false;
                return Parent.SelectedIndex == Parent.Items.IndexOf(this);
            }
        }

        protected override void OnInitialized()
        {
            Parent?.Items.Add(this);
        }

    }
}
