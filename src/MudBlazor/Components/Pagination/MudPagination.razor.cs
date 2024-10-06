// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    public partial class MudPagination : MudComponentBase
    {
        private int _count = 1;
        private int _selected = 1;
        private int _middleCount = 3;
        private int _boundaryCount = 2;
        private bool _selectedFirstSet;

        private string Classname =>
            new CssBuilder("mud-pagination")
                .AddClass($"mud-pagination-{Variant.ToDescriptionString()}")
                .AddClass($"mud-pagination-{Size.ToDescriptionString()}")
                .AddClass("mud-pagination-disable-elevation", !DropShadow)
                .AddClass("mud-pagination-rtl", RightToLeft)
                .AddClass(Class)
                .Build();

        private string ItemClassname =>
            new CssBuilder("mud-pagination-item")
                .AddClass("mud-pagination-item-rectangular", Rectangular)
                .Build();

        private string SelectedItemClassname =>
            new CssBuilder(ItemClassname)
                .AddClass("mud-pagination-item-selected")
                .Build();

        [CascadingParameter(Name = "RightToLeft")]
        public bool RightToLeft { get; set; }

        /// <summary>
        /// The number of pages.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Pagination.Behavior)]
        public int Count
        {
            get => _count;
            set
            {
                _count = Math.Max(1, value);
                Selected = Math.Min(Selected, _count);
            }
        }

        /// <summary>
        /// The number of items at the start and end of the pagination.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Pagination.Appearance)]
        public int BoundaryCount
        {
            get => _boundaryCount;
            set
            {
                _boundaryCount = Math.Max(1, value);
            }
        }

        /// <summary>
        /// The number of items in the middle of the pagination.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Pagination.Appearance)]
        public int MiddleCount
        {
            get => _middleCount;
            set
            {
                _middleCount = Math.Max(1, value);
            }
        }

        /// <summary>
        /// The selected page number.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Pagination.Behavior)]
        public int Selected
        {
            get => _selected;
            set
            {
                if (_selected == value)
                    return;

                //this is required because _selected will stay 1 when Count is not yet set
                if (!_selectedFirstSet)
                {
                    _selected = value;
                    _selectedFirstSet = true;
                }
                else
                    _selected = Math.Max(1, Math.Min(value, Count));

                SelectedChanged.InvokeAsync(_selected);
            }
        }

        /// <summary>
        /// The variant to use.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Pagination.Appearance)]
        public Variant Variant { get; set; } = Variant.Text;

        /// <summary>
        /// The color of the component. It supports the theme colors.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Pagination.Appearance)]
        public Color Color { get; set; } = Color.Primary;

        /// <summary>
        /// If true, the pagination buttons are displayed rectangular.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Pagination.Appearance)]
        public bool Rectangular { get; set; }

        /// <summary>
        /// The size of the component..
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Pagination.Appearance)]
        public Size Size { get; set; } = Size.Medium;

        /// <summary>
        /// Determines whether the component has a drop-shadow. Default is true
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Pagination.Appearance)]
        public bool DropShadow { get; set; } = true;

        /// <summary>
        /// If true, the pagination will be disabled.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Pagination.Behavior)]
        public bool Disabled { get; set; }

        /// <summary>
        /// If true, the navigate-to-first-page button is shown.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Pagination.Behavior)]
        public bool ShowFirstButton { get; set; }

        /// <summary>
        /// If true, the navigate-to-last-page button is shown.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Pagination.Behavior)]
        public bool ShowLastButton { get; set; }

        /// <summary>
        /// If true, the navigate-to-previous-page button is shown.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Pagination.Behavior)]
        public bool ShowPreviousButton { get; set; } = true;

        /// <summary>
        /// If true, the navigate-to-next-page button is shown.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Pagination.Behavior)]
        public bool ShowNextButton { get; set; } = true;

        /// <summary>
        /// If true, the page buttons are shown.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Pagination.Behavior)]
        public bool ShowPageButtons { get; set; } = true;


        /// <summary>
        /// Invokes the callback when a control button is clicked.
        /// </summary>
        [Parameter]
        public EventCallback<Page> ControlButtonClicked { get; set; }

        /// <summary>
        /// Invokes the callback when selected page changes.
        /// </summary>
        [Parameter]
        public EventCallback<int> SelectedChanged { get; set; }

        /// <summary>
        /// Custom first icon.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Pagination.Appearance)]
        public string FirstIcon { get; set; } = Icons.Material.Filled.FirstPage;

        /// <summary>
        /// Custom before icon.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Pagination.Appearance)]
        public string BeforeIcon { get; set; } = Icons.Material.Filled.NavigateBefore;

        /// <summary>
        /// Custom next icon.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Pagination.Appearance)]
        public string NextIcon { get; set; } = Icons.Material.Filled.NavigateNext;

        /// <summary>
        /// Custom last icon.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Pagination.Appearance)]
        public string LastIcon { get; set; } = Icons.Material.Filled.LastPage;

        /*generates an array representing the pagination numbers, e.g. for Count==11, MiddleCount==3, BoundaryCount==1,
         Selected==6 the output will be the int array [1, 2, -1, 5, 6, 7, -1, 10, 11]
         -1 is displayed as "..." in the ui*/
        private IEnumerable<int> GeneratePagination()
        {
            //return array {1, ..., Count} if Count is small 
            if (Count <= 4 || Count <= (2 * BoundaryCount) + MiddleCount + 2)
                return Enumerable.Range(1, Count).ToArray();

            var length = (2 * BoundaryCount) + MiddleCount + 2;
            var pages = new int[length];

            //set start boundary items, e.g. if BoundaryCount == 3 => [1, 2, 3, ...]
            for (var i = 0; i < BoundaryCount; i++)
            {
                pages[i] = i + 1;
            }

            //set end boundary items, e.g. if BoundaryCount == 3 and Count == 11 => [..., 9, 10, 11]
            for (var i = 0; i < BoundaryCount; i++)
            {
                pages[length - i - 1] = Count - i;
            }

            //calculate start value for middle items
            int startValue;
            if (Selected <= BoundaryCount + (MiddleCount / 2) + 1)
                startValue = BoundaryCount + 2;
            else if (Selected >= Count - BoundaryCount - (MiddleCount / 2))
                startValue = Count - BoundaryCount - MiddleCount;
            else
                startValue = Selected - (MiddleCount / 2);

            //set middle items, e.g. if MiddleCount == 3 and Selected == 5 and Count == 11 => [..., 4, 5, 6, ...] 
            for (var i = 0; i < MiddleCount; i++)
            {
                pages[BoundaryCount + 1 + i] = startValue + i;
            }

            //set start delimiter "..." when selected page is far enough to the end, dots are represented as -1
            pages[BoundaryCount] = (BoundaryCount + (MiddleCount / 2) + 1 < Selected) ? -1 : BoundaryCount + 1;

            //set end delimiter "..." when selected page is far enough to the start, dots are represented as -1
            pages[length - BoundaryCount - 1] = (Count - BoundaryCount - (MiddleCount / 2) > Selected) ? -1 : Count - BoundaryCount;

            //remove ellipsis if difference is small enough, e.g convert [..., 5 , -1 , 7, ...] to [..., 5, 6, 7, ...]
            for (var i = 0; i < length - 2; i++)
            {
                if (pages[i] + 2 == pages[i + 2])
                    pages[i + 1] = pages[i] + 1;
            }

            return pages;
        }

        //triggered when the user clicks on a control button, e.g. the navigate-to-next-page-button
        private void OnClickControlButton(Page page)
        {
            ControlButtonClicked.InvokeAsync(page);
            NavigateTo(page);
        }

        //Last line cannot be tested because Page enum has 4 items
        /// <summary>
        /// Navigates to the specified page.
        /// </summary>
        /// <param name="page">The target page. page=Page.Next navigates to the next page.</param>
        [ExcludeFromCodeCoverage]
        public void NavigateTo(Page page)
        {
            Selected = page switch
            {
                Page.First => 1,
                Page.Last => Math.Max(1, Count),
                Page.Next => Math.Min(Selected + 1, Count),
                Page.Previous => Math.Max(1, Selected - 1),
                _ => Selected
            };
        }

        /// <summary>
        /// Navigates to the specified page.
        /// </summary>
        /// <param name="pageIndex"></param>The target page. pageIndex=2 navigates to the 3. page.
        public void NavigateTo(int pageIndex)
        {
            Selected = pageIndex + 1;
        }
    }
}
