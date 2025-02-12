// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using MudBlazor.State;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// A list of clickable page numbers along with navigation buttons.
    /// </summary>
    public partial class MudPagination : MudComponentBase
    {
        private ParameterState<int> _countState;
        private ParameterState<int> _selectedState;
        private ParameterState<int> _middleCountState;
        private ParameterState<int> _boundaryCountState;

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

        public MudPagination()
        {
            using var register = CreateRegisterScope();
            _selectedState = register.RegisterParameter<int>(nameof(Selected))
                .WithParameter(() => Selected)
                .WithEventCallback(() => SelectedChanged)
                .WithChangeHandler(args => SetSelectedAsync(args.Value));
            _countState = register.RegisterParameter<int>(nameof(Count))
                .WithParameter(() => Count)
                .WithChangeHandler(args => SetCountAsync(args.Value));
            _middleCountState = register.RegisterParameter<int>(nameof(MiddleCount))
                .WithParameter(() => MiddleCount)
                .WithChangeHandler(args => SetMiddleCount(args.Value));
            _boundaryCountState = register.RegisterParameter<int>(nameof(BoundaryCount))
                .WithParameter(() => BoundaryCount)
                .WithChangeHandler(args => SetBoundaryCount(args.Value));
        }

        /// <summary>
        /// Displays text right-to-left.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>. Controlled via the <see cref="MudRTLProvider"/>.
        /// </remarks>
        [CascadingParameter(Name = "RightToLeft")]
        public bool RightToLeft { get; set; }

        /// <summary>
        /// The total number of pages.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Pagination.Behavior)]
        public int Count { get; set; } = 1;

        /// <summary>
        /// The number of pages shown before and after the ellipsis.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>1</c>. <br />
        /// A value of <c>1</c> would show one-page number at the edge: <c>&lt; 1 ... 4 5 6 ... 9 &gt;</c> <br />
        /// A value of <c>2</c> would show two-page numbers at the edge: <c>&lt; 1 2 ... 4 5 6 ... 8 9 &gt;</c> 
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Pagination.Appearance)]
        public int BoundaryCount { get; set; } = 2;

        /// <summary>
        /// The number of pages shown between the ellipsis.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>1</c>. <br />
        /// A value of <c>1</c> would show one-page number in the middle: <c>&lt; 1 ... 5 ... 9 &gt;</c> <br />
        /// A value of <c>3</c> would show three-page numbers in the middle: <c>&lt; 1 ... 4 5 6 ... 9 &gt;</c>
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Pagination.Appearance)]
        public int MiddleCount { get; set; } = 3;

        /// <summary>
        /// The selected page number.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Pagination.Behavior)]
        public int Selected { get; set; } = 1;

        /// <summary>
        /// The display variant to use.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Variant.Text"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Pagination.Appearance)]
        public Variant Variant { get; set; } = Variant.Text;

        /// <summary>
        /// The color of the selected page button.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Color.Primary" />.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Pagination.Appearance)]
        public Color Color { get; set; } = Color.Primary;

        /// <summary>
        /// Shows rectangular-shaped page buttons.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Pagination.Appearance)]
        public bool Rectangular { get; set; }

        /// <summary>
        /// The size of the page buttons.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Size.Medium"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Pagination.Appearance)]
        public Size Size { get; set; } = Size.Medium;

        /// <summary>
        /// Shows a drop shadow under each page button.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Pagination.Appearance)]
        public bool DropShadow { get; set; } = true;

        /// <summary>
        /// Prevents the user from clicking page buttons.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Pagination.Behavior)]
        public bool Disabled { get; set; }

        /// <summary>
        /// Shows the button which selects the first page.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Pagination.Behavior)]
        public bool ShowFirstButton { get; set; }

        /// <summary>
        /// Shows the button which selects the last page.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Pagination.Behavior)]
        public bool ShowLastButton { get; set; }

        /// <summary>
        /// Shows the button which selects the previous page.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Pagination.Behavior)]
        public bool ShowPreviousButton { get; set; } = true;

        /// <summary>
        /// Shows the button which selects the next page.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Pagination.Behavior)]
        public bool ShowNextButton { get; set; } = true;

        /// <summary>
        /// Shows numeric buttons for pages.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Pagination.Behavior)]
        public bool ShowPageButtons { get; set; } = true;

        /// <summary>
        /// Occurs when the First, Previous, Next, or Last button is clicked.
        /// </summary>
        [Parameter]
        public EventCallback<Page> ControlButtonClicked { get; set; }

        /// <summary>
        /// Occurs when <see cref="Selected"/> has changed.
        /// </summary>
        [Parameter]
        public EventCallback<int> SelectedChanged { get; set; }

        /// <summary>
        /// The icon for the First button.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.FirstPage"/>.  Only shows if <see cref="ShowFirstButton"/> is <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Pagination.Appearance)]
        public string FirstIcon { get; set; } = Icons.Material.Filled.FirstPage;

        /// <summary>
        /// The icon for the Before button.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.NavigateBefore"/>.  Only shows if <see cref="ShowPreviousButton"/> is <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Pagination.Appearance)]
        public string BeforeIcon { get; set; } = Icons.Material.Filled.NavigateBefore;

        /// <summary>
        /// The icon for the Next button.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.NavigateNext"/>.  Only shows if <see cref="ShowNextButton"/> is <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Pagination.Appearance)]
        public string NextIcon { get; set; } = Icons.Material.Filled.NavigateNext;

        /// <summary>
        /// The icon for the Last button.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.LastPage"/>.  Only shows if <see cref="ShowLastButton"/> is <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Pagination.Appearance)]
        public string LastIcon { get; set; } = Icons.Material.Filled.LastPage;

        /*generates an array representing the pagination numbers, e.g. for Count==11, MiddleCount==3, BoundaryCount==1,
         Selected==6 the output will be the int array [1, 2, -1, 5, 6, 7, -1, 10, 11]
         -1 is displayed as "..." in the ui*/
        private int[] GeneratePagination()
        {
            //return array {1, ..., Count} if Count is small 
            if (_countState.Value <= 4 || _countState.Value <= (2 * _boundaryCountState.Value) + _middleCountState.Value + 2)
            {
                var result = new int[_countState.Value];
                for (var i = 0; i < _countState.Value; i++)
                {
                    result[i] = i + 1;
                }

                return result;
            }

            var length = (2 * _boundaryCountState.Value) + _middleCountState.Value + 2;
            var pages = new int[length];

            //set start boundary items, e.g. if BoundaryCount == 3 => [1, 2, 3, ...]
            for (var i = 0; i < _boundaryCountState.Value; i++)
            {
                pages[i] = i + 1;
            }

            //set end boundary items, e.g. if BoundaryCount == 3 and Count == 11 => [..., 9, 10, 11]
            for (var i = 0; i < _boundaryCountState.Value; i++)
            {
                pages[length - i - 1] = _countState.Value - i;
            }

            //calculate start value for middle items
            int startValue;
            if (_selectedState.Value <= _boundaryCountState.Value + (_middleCountState.Value / 2) + 1)
            {
                startValue = _boundaryCountState.Value + 2;
            }
            else if (_selectedState.Value >= _countState.Value - _boundaryCountState.Value - (_middleCountState.Value / 2))
            {
                startValue = _countState.Value - _boundaryCountState.Value - _middleCountState.Value;
            }
            else
            {
                startValue = _selectedState.Value - (_middleCountState.Value / 2);
            }

            //set middle items, e.g. if MiddleCount == 3 and Selected == 5 and Count == 11 => [..., 4, 5, 6, ...] 
            for (var i = 0; i < _middleCountState.Value; i++)
            {
                pages[_boundaryCountState.Value + 1 + i] = startValue + i;
            }

            //set start delimiter "..." when selected page is far enough to the end, dots are represented as -1
            pages[_boundaryCountState.Value] = (_boundaryCountState.Value + (_middleCountState.Value / 2) + 1 < _selectedState.Value) ? -1 : _boundaryCountState.Value + 1;

            //set end delimiter "..." when selected page is far enough to the start, dots are represented as -1
            pages[length - _boundaryCountState.Value - 1] = (_countState.Value - _boundaryCountState.Value - (_middleCountState.Value / 2) > _selectedState.Value) ? -1 : _countState.Value - _boundaryCountState.Value;

            //remove ellipsis if difference is small enough, e.g convert [..., 5 , -1 , 7, ...] to [..., 5, 6, 7, ...]
            for (var i = 0; i < length - 2; i++)
            {
                if (pages[i] + 2 == pages[i + 2])
                {
                    pages[i + 1] = pages[i] + 1;
                }
            }

            return pages;
        }

        //triggered when the user clicks on a control button, e.g. the navigate-to-next-page-button
        private Task OnClickControlButtonAsync(Page page)
        {
            ControlButtonClicked.InvokeAsync(page);

            return NavigateToAsync(page);
        }

        /// <summary>
        /// Changes the currently selected page.
        /// </summary>
        /// <param name="page">The page to navigate to.</param>
        public Task NavigateToAsync(Page page)
        {
            var newPageIndex = page switch
            {
                Page.First => 1,
                Page.Last => Math.Max(1, _countState.Value),
                Page.Next => Math.Min(_selectedState.Value + 1, _countState.Value),
                Page.Previous => Math.Max(1, _selectedState.Value - 1),
                _ => _selectedState.Value
            };

            return SetSelectedAsync(newPageIndex);
        }

        /// <summary>
        /// Changes the currently selected page.
        /// </summary>
        /// <param name="pageIndex">The index of the page to select, where the first page is <c>0</c>.</param>
        public Task NavigateToAsync(int pageIndex)
        {
            var newPageIndex = pageIndex + 1;

            return SetSelectedAsync(newPageIndex);
        }

        private Task SetMiddleCount(int count)
        {
            var newCount = Math.Max(1, count);

            return _middleCountState.SetValueAsync(newCount);
        }

        private Task SetBoundaryCount(int count)
        {
            var newCount = Math.Max(1, count);

            return _boundaryCountState.SetValueAsync(newCount);
        }

        private async Task SetCountAsync(int count)
        {
            var newCount = Math.Max(1, count);
            await _countState.SetValueAsync(newCount);
            await SetSelectedAsync(Math.Min(_selectedState.Value, newCount));
        }

        private async Task SetSelectedAsync(int pageIndex)
        {
            if (_selectedState.Value == pageIndex)
            {
                return;
            }

            var newPageIndex = Math.Max(1, Math.Min(pageIndex, _countState.Value));

            await _selectedState.SetValueAsync(newPageIndex);
        }
    }
}
