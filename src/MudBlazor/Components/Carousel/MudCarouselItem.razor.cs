using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudCarouselItem : MudComponentBase
    {
        protected string Classname =>
                    new CssBuilder("mud-carousel-item")
                         .AddClass("mud-carousel-transition-fade", Transition == Transition.Fade)
                         .AddClass("mud-carousel-transition-slide-next", Transition == Transition.Slide && Parent.move_next)
                         .AddClass("mud-carousel-transition-slide-prev", Transition == Transition.Slide && !Parent.move_next)
                         .AddClass(Class)
                         .Build();

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [CascadingParameter]
        protected internal Base.MudBaseItemsControl<MudCarouselItem> Parent { get; set; }

        [Parameter]
        public Transition Transition { get; set; } = Transition.Slide;

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
