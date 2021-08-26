﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudCarouselItem : MudComponentBase, IDisposable
    {
        protected string Classname =>
                    new CssBuilder("mud-carousel-item")
                         .AddClass($"mud-carousel-item-{Color.ToDescriptionString()}")
                         .AddClass("mud-carousel-item-exit", !_disposed && Parent.LastContainer == this)

                         .AddClass("mud-carousel-transition-fade-in", !_disposed && Transition == Transition.Fade && Parent.SelectedContainer == this)
                         .AddClass("mud-carousel-transition-fade-out", !_disposed && Transition == Transition.Fade && Parent.LastContainer == this)

                         .AddClass("mud-carousel-transition-slide-next-enter", !_disposed && Transition == Transition.Slide && RightToLeft == false && Parent.SelectedContainer == this && Parent._moveNext)
                         .AddClass("mud-carousel-transition-slide-next-exit", !_disposed && Transition == Transition.Slide && RightToLeft == false && Parent.LastContainer == this && Parent._moveNext)

                         .AddClass("mud-carousel-transition-slide-prev-enter", !_disposed && Transition == Transition.Slide && RightToLeft == false && Parent.SelectedContainer == this && !Parent._moveNext)
                         .AddClass("mud-carousel-transition-slide-prev-exit", !_disposed && Transition == Transition.Slide && RightToLeft == false && Parent.LastContainer == this && !Parent._moveNext)

                         .AddClass("mud-carousel-transition-slide-next-rtl-enter", !_disposed && Transition == Transition.Slide && RightToLeft == true && Parent.SelectedContainer == this && Parent._moveNext)
                         .AddClass("mud-carousel-transition-slide-next-rtl-exit", !_disposed && Transition == Transition.Slide && RightToLeft == true && Parent.LastContainer == this && Parent._moveNext)

                         .AddClass("mud-carousel-transition-slide-prev-rtl-enter", !_disposed && Transition == Transition.Slide && RightToLeft == true && Parent.SelectedContainer == this && !Parent._moveNext)
                         .AddClass("mud-carousel-transition-slide-prev-rtl-exit", !_disposed && Transition == Transition.Slide && RightToLeft == true && Parent.LastContainer == this && !Parent._moveNext)

                         .AddClass("mud-carousel-transition-none", !_disposed && Transition == Transition.None && Parent.SelectedContainer != this)

                         .AddClass(CustomTransitionEnter, !_disposed && Transition == Transition.Custom && Parent.SelectedContainer == this && Parent.SelectedContainer == this)
                         .AddClass(CustomTransitionExit, !_disposed && Transition == Transition.Custom && Parent.LastContainer == this && Parent.LastContainer == this)

                         .AddClass(Class)
                         .Build();

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [CascadingParameter] protected internal MudBaseItemsControl<MudCarouselItem> Parent { get; set; }

        [CascadingParameter] public bool RightToLeft { get; set; }

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


        public bool IsVisible => Parent != null && (Parent.LastContainer == this || Parent.SelectedIndex == Parent.Items.IndexOf(this));


        protected override Task OnInitializedAsync()
        {
            Parent?.Items.Add(this);
            return Task.CompletedTask;
        }

        private bool _disposed = false;
        public void Dispose()
        {
            _disposed = true;
            Parent?.Items.Remove(this);
        }

    }
}
