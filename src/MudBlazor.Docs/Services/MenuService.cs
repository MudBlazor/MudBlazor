using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MudBlazor.Docs.Models;

namespace MudBlazor.Docs.Services
{
    public interface IMenuService
    {
        //Menu sections
        IEnumerable<DocsLink> GettingStarted { get; }
        IEnumerable<MudComponent> Components { get; }
        IEnumerable<MudComponent> Api { get; }
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
            .AddItem("Grid", typeof(MudGrid))
            .AddItem("Hidden", typeof(MudHidden))
            .AddItem("Chips", typeof(MudChip))
            .AddItem("ChipSet", typeof(MudChipSet))
            .AddItem("Badge", typeof(MudBadge))
            .AddItem("AppBar", typeof(MudAppBar))
            .AddItem("Drawer", typeof(MudDrawer))
            .AddItem("Link", typeof(MudLink))
            .AddItem("Menu", typeof(MudMenu))
            .AddItem("Message Box", typeof(MudMessageBox))
            .AddItem("Nav Menu", typeof(MudNavMenu))
            .AddItem("Tabs", typeof(MudTabs))
            .AddItem("Progress", typeof(MudProgressCircular))
            .AddItem("Dialog", typeof(MudDialog))
            .AddItem("Snackbar", typeof(MudSnackbarProvider))
            .AddItem("Avatar", typeof(MudAvatar))
            .AddItem("Alert", typeof(MudAlert))
            .AddItem("Card", typeof(MudCard))
            .AddItem("Divider", typeof(MudDivider))
            .AddItem("Expansion Panel", typeof(MudExpansionPanel))
            .AddItem("Icons", typeof(MudIcon))
            .AddItem("List", typeof(MudList))
            .AddItem("Paper", typeof(MudPaper))
            .AddItem("Rating", typeof(MudRating))
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
            .AddItem("TreeView", typeof(MudTreeView<T>))
            .AddItem("Breadcrumbs", typeof(MudBreadcrumbs))
            .AddItem("ScrollToTop", typeof(MudScrollToTop))
            .AddItem("Popover", typeof(MudPopover))
            .AddItem("SwipeArea", typeof(MudSwipeArea))
            .AddItem("ToolBar", typeof(MudToolBar))

            //GROUPS

            //Inputs
            .AddNavGroup("Form Inputs & controls", false, new DocsComponents()
                .AddItem("Radio", typeof(MudRadio<T>))
                .AddItem("Checkbox", typeof(MudCheckBox<T>))
                .AddItem("Select", typeof(MudSelect<T>))
                .AddItem("Slider", typeof(MudSlider<T>))
                .AddItem("Switch", typeof(MudSwitch<T>))
                .AddItem("Text Field", typeof(MudTextField<T>))
                .AddItem("Form", typeof(MudForm))
                .AddItem("Autocomplete", typeof(MudAutocomplete<T>))
                .AddItem("Field", typeof(MudField))
            )

            //Pickers
            .AddNavGroup("Pickers", false, new DocsComponents()
                .AddItem("Date Picker", typeof(MudDatePicker))
                .AddItem("Time Picker", typeof(MudTimePicker))
            )

            //Buttons
            .AddNavGroup("Buttons", false, new DocsComponents()
                .AddItem("Button", typeof(MudButton))
                .AddItem("IconButton", typeof(MudIconButton))
                .AddItem("ToggleIconButton", typeof(MudToggleIconButton))
                .AddItem("Button FAB", typeof(MudFab))
            )

            //Charts
            .AddNavGroup("Charts", false, new DocsComponents()
                .AddItem("Donut chart", typeof(MudChart))
                .AddItem("Line chart", typeof(MudChart))
                .AddItem("Pie chart", typeof(MudChart))
                .AddItem("Bar chart", typeof(MudChart))
            );
        public IEnumerable<MudComponent> Components => _docsComponents.Elements;

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
