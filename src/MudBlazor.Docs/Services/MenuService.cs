using System;
using System.Collections.Generic;
using System.Linq;
using MudBlazor.Charts;
using MudBlazor.Docs.Models;

namespace MudBlazor.Docs.Services
{
    public interface IMenuService
    {
        //Menu sections
        IEnumerable<DocsLink> GettingStarted { get; }
        IEnumerable<MudComponent> Components { get; }
        IEnumerable<MudComponent> Api { get; }
        MudComponent GetParent(Type type);
        IEnumerable<DocsLink> Features { get; }
        IEnumerable<DocsLink> Customization { get; }
        IEnumerable<DocsLink> About { get; }
    }

    /// <summary>
    /// The aim of this class is to add new items to NavMenu
    /// </summary>
    public class MenuService : IMenuService
    {
        /// <summary>
        /// Here is where the links for the Components Menu in NavMenu are added
        /// Add here the new menu elements without caring about the order.
        /// They will be reordered automatically
        /// </summary>
        private readonly DocsComponents _docsComponents = new DocsComponents()
            //Individual elements
            .AddItem("Container", typeof(MudContainer))
            .AddItem("Grid", typeof(MudGrid), typeof(MudItem))
            .AddItem("Hidden", typeof(MudHidden))
            .AddItem("Breakpoint Provider", typeof(MudBreakpointProvider))
            .AddItem("Chips", typeof(MudChip))
            .AddItem("ChipSet", typeof(MudChipSet))
            .AddItem("Badge", typeof(MudBadge))
            .AddItem("AppBar", typeof(MudAppBar))
            .AddItem("Drawer", typeof(MudDrawer), typeof(MudDrawerHeader), typeof(MudDrawerContainer))
            .AddItem("Link", typeof(MudLink))
            .AddItem("Menu", typeof(MudMenu), typeof(MudMenuItem))
            .AddItem("Message Box", typeof(MudMessageBox))
            .AddItem("Nav Menu", typeof(MudNavMenu), typeof(MudNavLink), typeof(MudNavGroup))
            .AddItem("Tabs", typeof(MudTabs), typeof(MudTabPanel), typeof(MudDynamicTabs))
            .AddItem("Progress", typeof(MudProgressCircular), typeof(MudProgressLinear))
            .AddItem("Dialog", typeof(MudDialog), typeof(MudDialogInstance), typeof(MudDialogProvider))
            .AddItem("Snackbar", typeof(MudSnackbarProvider))
            .AddItem("Avatar", typeof(MudAvatar), typeof(MudAvatarGroup))
            .AddItem("Alert", typeof(MudAlert))
            .AddItem("Card", typeof(MudCard), typeof(MudCardActions), typeof(MudCardContent), typeof(MudCardHeader), typeof(MudCardMedia))
            .AddItem("Divider", typeof(MudDivider))
            .AddItem("Expansion Panels", typeof(MudExpansionPanels), typeof(MudExpansionPanel))
            .AddItem("Icons", typeof(MudIcon))
            .AddItem("List", typeof(MudList), typeof(MudListItem), typeof(MudListSubheader))
            .AddItem("Paper", typeof(MudPaper))
            .AddItem("Rating", typeof(MudRating), typeof(MudRatingItem))
            .AddItem("Skeleton", typeof(MudSkeleton))
            .AddItem("Table", typeof(MudTable<T>))
            .AddItem("Simple Table", typeof(MudSimpleTable))
            .AddItem("Tooltip", typeof(MudTooltip))
            .AddItem("Typography", typeof(MudText))
            .AddItem("Overlay", typeof(MudOverlay))
            .AddItem("Highlighter", typeof(MudHighlighter))
            .AddItem("Element", typeof(MudElement))
            .AddItem("Focus Trap", typeof(MudFocusTrap))
            .AddItem("File Upload", typeof(MudFileUploader))
            .AddItem("TreeView", typeof(MudTreeView<T>), typeof(MudTreeViewItem<T>), typeof(MudTreeViewItemToggleButton))
            .AddItem("Breadcrumbs", typeof(MudBreadcrumbs))
            .AddItem("ScrollToTop", typeof(MudScrollToTop))
            .AddItem("Popover", typeof(MudPopover))
            .AddItem("SwipeArea", typeof(MudSwipeArea))
            .AddItem("ToolBar", typeof(MudToolBar))
            .AddItem("Carousel", typeof(MudCarousel<T>), typeof(MudCarouselItem))
            .AddItem("Timeline", typeof(MudTimeline), typeof(MudTimelineItem))
            .AddItem("Pagination", typeof(MudPagination))

            //GROUPS

            //Inputs
            .AddNavGroup("Form Inputs & controls", false, new DocsComponents()
                .AddItem("Radio", typeof(MudRadio<T>), typeof(MudRadioGroup<T>))
                .AddItem("Checkbox", typeof(MudCheckBox<T>))
                .AddItem("Select", typeof(MudSelect<T>), typeof(MudSelectItem<T>))
                .AddItem("Slider", typeof(MudSlider<T>))
                .AddItem("Switch", typeof(MudSwitch<T>))
                .AddItem("Text Field", typeof(MudTextField<T>))
                .AddItem("Numeric Field", typeof(MudNumericField<T>))
                .AddItem("Form", typeof(MudForm))
                .AddItem("Autocomplete", typeof(MudAutocomplete<T>))
                .AddItem("Field", typeof(MudField))
            )

            //Pickers
            .AddNavGroup("Pickers", false, new DocsComponents()
                .AddItem("Date Picker", typeof(MudDatePicker))
                .AddItem("Time Picker", typeof(MudTimePicker))
                .AddItem("Color Picker", typeof(MudColorPicker))
            )

            //Buttons
            .AddNavGroup("Buttons", false, new DocsComponents()
                .AddItem("Button", typeof(MudButton))
                .AddItem("Button Group", typeof(MudButtonGroup))
                .AddItem("IconButton", typeof(MudIconButton))
                .AddItem("ToggleIconButton", typeof(MudToggleIconButton))
                .AddItem("Button FAB", typeof(MudFab))
            )

            //Charts
            .AddNavGroup("Charts", false, new DocsComponents()
                .AddItem("Options", typeof(ChartOptions))
                .AddItem("Donut Chart", typeof(Donut))
                .AddItem("Line Chart", typeof(Line))
                .AddItem("Pie Chart", typeof(Pie))
                .AddItem("Bar Chart", typeof(Bar))
            );

        public IEnumerable<MudComponent> Components => _docsComponents.Elements;

        private Dictionary<Type, MudComponent> _parents = new();

        public MudComponent GetParent(Type child) => _parents[child];

        public MenuService()
        {
            foreach (var item in Components)
            {
                if (item.IsNavGroup)
                {
                    foreach (var apiItem in item.GroupItems.Elements)
                    {
                        _parents.Add(apiItem.Component, item);
                    }
                }
                else
                {
                    _parents.Add(item.Component, item);

                    if (item.ChildComponents != null)
                    {
                        foreach (var childComponent in item.ChildComponents)
                        {
                            _parents.Add(childComponent, item);
                        }
                    }
                }
            }
        }

        private DocsComponents _docsComponentsApi;
        //cached property
        /// <summary>
        /// This autogenerates the Menu for the API
        /// </summary>
        private DocsComponents DocsComponentsApi
        {
            get
            {
                //caching property
                if (_docsComponentsApi != null) return _docsComponentsApi;

                _docsComponentsApi = new DocsComponents();
                foreach (var item in Components)
                {
                    if (item.IsNavGroup)
                    {
                        foreach (var apiItem in item.GroupItems.Elements)
                        {
                            _docsComponentsApi.AddItem(apiItem.Name, apiItem.Component);
                        }
                    }
                    else
                    {
                        _docsComponentsApi.AddItem(item.Name, item.Component);
                    }
                }

                return _docsComponentsApi;
            }
        }
        public IEnumerable<MudComponent> Api => DocsComponentsApi.Elements;

        //cached property
        private IEnumerable<DocsLink> _gettingStarted;
        /// <summary>
        /// Getting started menu links
        /// </summary>
        public IEnumerable<DocsLink> GettingStarted => _gettingStarted ??= new List<DocsLink>
            {
                new DocsLink {Title = "Installation", Href = "getting-started/installation"},
                new DocsLink {Title = "Layouts", Href = "getting-started/layouts"},
                new DocsLink {Title = "Usage", Href = "getting-started/usage"},
                new DocsLink {Title = "Wireframes", Href = "getting-started/wireframes"},
            }.OrderBy(x => x.Title);


        private IEnumerable<DocsLink> _features;
        /// <summary>
        /// Features menu links
        /// </summary>
        public IEnumerable<DocsLink> Features => _features ??= new List<DocsLink>
            {
                new DocsLink {Title = "Breakpoints", Href = "features/breakpoints"},
                new DocsLink {Title = "Border Radius", Href = "features/border-radius"},
                new DocsLink {Title = "Colors", Href = "features/colors"},
                new DocsLink {Title = "Converters", Href = "features/converters"},
                new DocsLink {Title = "Display", Href = "features/display"},
                new DocsLink {Title = "Elevation", Href = "features/elevation"},
                new DocsLink {Title = "Flex", Href = "features/flex"},
                new DocsLink {Title = "Icons", Href = "features/icons"},
                new DocsLink {Title = "Spacing", Href = "features/spacing"},
                new DocsLink {Title = "RTL Languages", Href = "features/rtl-languages"},
            }.OrderBy(x => x.Title);


        private IEnumerable<DocsLink> _customization;
        /// <summary>
        /// Customization menu links
        /// </summary>
        public IEnumerable<DocsLink> Customization => _customization ??= new List<DocsLink>()
        {
            //new DocsLink{Title="Default theme", Href="customization/default-theme"},
            new DocsLink {Title = "Overview", Href = "customization/theming/overview"},
            new DocsLink {Title = "Palette", Href = "customization/theming/palette"},
            new DocsLink {Title = "Typography", Href = "customization/theming/typography"},
            new DocsLink {Title = "z-index", Href = "customization/theming/z-index"},
        }.OrderBy(x => x.Title);


        private IEnumerable<DocsLink> _about;
        /// <summary>
        /// About menu links
        /// </summary>
        public IEnumerable<DocsLink> About => _about ??= new List<DocsLink>
        {
            new DocsLink{ Title="Credits" , Href="project/credit" },
            new DocsLink{Href="project/about", Title="How it started" },
            new DocsLink{Href="project/team", Title="Team & Contributors" },
            new DocsLink{Href="project/versions", Title="Versions" },
        }.OrderBy(x => x.Title);
    }
}
