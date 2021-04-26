using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudCarouselItem : MudComponentBase
    {
        protected string Classname =>
                    new CssBuilder("mud-carousel-item")
                         .AddClass($"mud-carousel-item-{Color.ToDescriptionString()}")
                         .AddClass("mud-carousel-item-exit", Parent.LastContainer == this)

                         .AddClass("mud-carousel-transition-fade-in", Transition == Transition.Fade && Parent.SelectedContainer == this)
                         .AddClass("mud-carousel-transition-fade-out", Transition == Transition.Fade && Parent.LastContainer == this)

                         .AddClass("mud-carousel-transition-slide-next-enter", Transition == Transition.Slide && Parent.SelectedContainer == this && Parent._movenext)
                         .AddClass("mud-carousel-transition-slide-next-exit", Transition == Transition.Slide && Parent.LastContainer == this && Parent._movenext)

                         .AddClass("mud-carousel-transition-slide-prev-enter", Transition == Transition.Slide && Parent.SelectedContainer == this && !Parent._movenext)
                         .AddClass("mud-carousel-transition-slide-prev-exit", Transition == Transition.Slide && Parent.LastContainer == this && !Parent._movenext)

                         .AddClass(CustomTransitionEnter, Transition == Transition.Custom && Parent.SelectedContainer == this && Parent.SelectedContainer == this)
                         .AddClass(CustomTransitionExit, Transition == Transition.Custom && Parent.LastContainer == this && Parent.LastContainer == this)

                         .AddClass(Class)
                         .Build();

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [CascadingParameter]
        protected internal MudBaseItemsControl<MudCarouselItem> Parent { get; set; }

        /// <summary>
        /// The color of the component. It supports the theme colors.
        /// </summary>
        [Parameter] public Color Color { get; set; } = Color.Default;

        /// <summary>
        /// The transition effect of the component.
        /// </summary>
        [Parameter] public Transition Transition { get; set; } = Transition.Slide;

        /// <summary>
        /// The name of custom transition on entrance time
        /// </summary>
        [Parameter] public string CustomTransitionEnter { get; set; }

        /// <summary>
        /// The name of custom transition on exiting time
        /// </summary>
        [Parameter] public string CustomTransitionExit { get; set; }

        public bool IsVisible
        {
            get
            {
                if (Parent == null)
                    return false;
                return Parent.SelectedIndex == Parent.Items.IndexOf(this) || Parent.LastContainer == this;
            }
        }

        protected override void OnInitialized()
        {
            Parent?.Items.Add(this);
        }
    }
}
