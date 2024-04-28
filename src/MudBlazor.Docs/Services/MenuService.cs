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
        IEnumerable<MudComponent> Components { get; }
        IEnumerable<MudComponent> Api { get; }
        MudComponent GetParent(Type type);
        MudComponent GetComponent(Type type);
        IEnumerable<DocsLink> Features { get; }
        IEnumerable<DocsLink> Customization { get; }
        IEnumerable<DocsLink> Utilities { get; }
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
        private readonly List<MudComponent> _docsComponents = new DocsComponents()
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
            .AddItem("Drop Zone", typeof(MudDropZone<T>), typeof(MudDropContainer<T>), typeof(MudDynamicDropItem<T>))
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
            .AddItem("Image", typeof(MudImage))
            .AddItem("Icons", typeof(MudIcon))
            .AddItem("List", typeof(MudList), typeof(MudListItem), typeof(MudListSubheader))
            .AddItem("Paper", typeof(MudPaper))
            .AddItem("Rating", typeof(MudRating), typeof(MudRatingItem))
            .AddItem("Skeleton", typeof(MudSkeleton))
            .AddItem("Table", typeof(MudTable<T>))
            .AddItem("Data Grid", typeof(MudDataGrid<T>))
            .AddItem("Simple Table", typeof(MudSimpleTable))
            .AddItem("Tooltip", typeof(MudTooltip))
            .AddItem("Typography", typeof(MudText))
            .AddItem("Overlay", typeof(MudOverlay))
            .AddItem("Highlighter", typeof(MudHighlighter))
            .AddItem("Element", typeof(MudElement))
            .AddItem("Focus Trap", typeof(MudFocusTrap))
            .AddItem("TreeView", typeof(MudTreeView<T>), typeof(MudTreeViewItem<T>), typeof(MudTreeViewItemToggleButton))
            .AddItem("Breadcrumbs", typeof(MudBreadcrumbs))
            .AddItem("ScrollToTop", typeof(MudScrollToTop))
            .AddItem("Popover", typeof(MudPopover))
            .AddItem("SwipeArea", typeof(MudSwipeArea))
            .AddItem("ToolBar", typeof(MudToolBar))
            .AddItem("Carousel", typeof(MudCarousel<T>), typeof(MudCarouselItem))
            .AddItem("Timeline", typeof(MudTimeline), typeof(MudTimelineItem))
            .AddItem("Pagination", typeof(MudPagination))
            .AddItem("Stack", typeof(MudStack))

            //GROUPS

            //Inputs
            .AddNavGroup("Form & Inputs", false, new DocsComponents()
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
                .AddItem("File Upload", typeof(MudFileUpload<T>))
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
                //.AddItem("Options", typeof(ChartOptions)) // <-- this does not work because ChartOptions is not a component!
                .AddItem("Donut Chart", typeof(Donut))
                .AddItem("Line Chart", typeof(Line))
                .AddItem("Pie Chart", typeof(Pie))
                .AddItem("Bar Chart", typeof(Bar))
            )
            // this must be last!
            .GetComponentsSortedByName();

        public IEnumerable<MudComponent> Components => _docsComponents;

        private Dictionary<Type, MudComponent> _parents = new();
        private Dictionary<Type, MudComponent> _componentLookup = new();

        public MudComponent GetParent(Type child) {
            if (child == null)
                return null;
            if ( _parents.TryGetValue(child, out var parent))
                return parent;
            return null;
        }

        public MudComponent GetComponent(Type type)
        {
            if (type == null)
                return null;
            if (_componentLookup.ContainsKey(type))
                return _componentLookup[type];
            if (_parents.ContainsKey(type))
                return _parents[type];
            return null;
        }
        

        public MenuService()
        {
            foreach (var comp in Components)
            {
                if (comp.IsNavGroup)
                {
                    foreach (var groupComp in comp.GroupComponents)
                    {
                        _componentLookup.Add(groupComp.Type, groupComp);
                        _parents.Add(groupComp.Type, comp);
                    }
                }
                else
                {
                    _componentLookup.Add(comp.Type, comp);
                    // top-level types refer to themself as parent ;)
                    _parents.Add(comp.Type, comp);
                    if (comp.ChildTypes != null)
                    {
                        foreach (var childType in comp.ChildTypes)
                            _parents.Add(childType, comp);
                    }
                }
            }
        }

        private DocsComponents _docsComponentsApi; //cached property

        /// <summary>
        /// This autogenerates the Menu for the API
        /// </summary>
        private DocsComponents DocsComponentsApi
        {
            get
            {
                //caching property
                if (_docsComponentsApi != null)
                    return _docsComponentsApi;
                _docsComponentsApi = new DocsComponents();
                foreach (var item in Components)
                {
                    if (item.IsNavGroup)
                    {
                        foreach (var groupComp in item.GroupComponents)
                        {
                            _docsComponentsApi.AddItem(groupComp.Name, groupComp.Type);
                        }
                    }
                    else
                    {
                        _docsComponentsApi.AddItem(item.Name, item.Type);
                    }
                }
                return _docsComponentsApi;
            }
        }

        public IEnumerable<MudComponent> Api => DocsComponentsApi.Components;

        private IEnumerable<DocsLink> _features;
        /// <summary>
        /// Features menu links
        /// </summary>
        public IEnumerable<DocsLink> Features => _features ??= new List<DocsLink>
            {
                new DocsLink {Title = "Breakpoints", Href = "features/breakpoints"},
                new DocsLink {Title = "Colors", Href = "features/colors"},
                new DocsLink {Title = "Elevation", Href = "features/elevation"},
                new DocsLink {Title = "Converters", Href = "features/converters"},
                new DocsLink {Title = "Icon Reference", Href = "features/icons"}, // <-- note: title changed from "Icons" to "Icon Reference" to avoid confusion in Search box with the MudIcon page which is also called "Icons"
                new DocsLink {Title = "Masking", Href = "features/masking"},
                new DocsLink {Title = "RTL Languages", Href = "features/rtl-languages"},
            }.OrderBy(x => x.Title);


        private IEnumerable<DocsLink> _customization;
        /// <summary>
        /// Customization menu links
        /// </summary>
        public IEnumerable<DocsLink> Customization => _customization ??= new List<DocsLink>()
        {
            new DocsLink {Title="Default theme", Href="customization/default-theme"},
            new DocsLink {Title = "Overview", Href = "customization/overview"},
            new DocsLink {Title = "Palette", Href = "customization/palette"},
            new DocsLink {Title = "Typography", Href = "customization/typography"},
            new DocsLink {Title = "z-index", Href = "customization/z-index"},
            new DocsLink {Title = "Pseudo CSS", Href = "customization/pseudocss"},
        }.OrderBy(x => x.Title);
        
        private IEnumerable<DocsLink> _utilities;

        /// <summary>
        /// CSS Utilities menu links
        /// </summary>
        public IEnumerable<DocsLink> Utilities => _utilities ??= new List<DocsLink>()
        {
            new DocsLink {Group = "Layout", Title = "Display", Href = "utilities/display"},
            new DocsLink {Group = "Layout", Title = "Z-Index", Href = "utilities/z-index"},
            new DocsLink {Group = "Layout", Title = "Overflow", Href = "utilities/overflow"},
            new DocsLink {Group = "Layout", Title = "Visibility", Href = "utilities/visibility"},
            new DocsLink {Group = "Layout", Title = "Object Fit", Href = "utilities/object-fit"},
            new DocsLink {Group = "Layout", Title = "Object Position", Href = "utilities/object-position"},
            new DocsLink {Group = "Layout", Title = "Position", Href = "utilities/position"},
            
            new DocsLink {Group = "Flexbox", Title = "Enable Flexbox", Href = "utilities/enable-flex"},
            new DocsLink {Group = "Flexbox", Title = "Flex Direction", Href = "utilities/flex-direction"},
            new DocsLink {Group = "Flexbox", Title = "Flex Wrap", Href = "utilities/flex-wrap"},
            new DocsLink {Group = "Flexbox", Title = "Flex", Href = "utilities/flex"},
            new DocsLink {Group = "Flexbox", Title = "Flex Grow", Href = "utilities/flex-grow"},
            new DocsLink {Group = "Flexbox", Title = "Flex Shrink", Href = "utilities/flex-shrink"},
            new DocsLink {Group = "Flexbox", Title = "Order", Href = "utilities/order"},
            new DocsLink {Group = "Flexbox", Title = "Gap", Href = "utilities/gap"},
            new DocsLink {Group = "Flexbox", Title = "Justify Content", Href = "utilities/justify-content"},
            new DocsLink {Group = "Flexbox", Title = "Align Content", Href = "utilities/align-content"},
            new DocsLink {Group = "Flexbox", Title = "Align Items", Href = "utilities/align-items"},
            new DocsLink {Group = "Flexbox", Title = "Align Self", Href = "utilities/align-self"},
            
            new DocsLink {Group = "Spacing", Title = "Spacing", Href = "utilities/spacing"},
            
            new DocsLink {Group = "Borders", Title = "Border Radius", Href = "utilities/border-radius"},
            new DocsLink {Group = "Borders", Title = "Border Style", Href = "utilities/border-style"},
            new DocsLink {Group = "Borders", Title = "Border Width", Href = "utilities/border-width"},
            
            new DocsLink {Group = "Interactivity", Title = "Cursor", Href = "utilities/cursor"},
            new DocsLink {Group = "Interactivity", Title = "Pointer Events", Href = "utilities/pointer-events"},
        };
    }
}
