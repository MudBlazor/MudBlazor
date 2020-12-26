
namespace MudBlazor.Docs.Models
{
    public interface IMenuService
    {
        DocsComponents DocsComponents { get; }
        DocsComponents DocsComponentsApi { get; }
    }
        
    /// <summary>
    /// The aim of this class is to add new items to NavMenu
    /// </summary>
    public class MenuService : IMenuService
    {
        private DocsComponents _docsComponents;

        //cached property
        //`??=` means If _docsComponents is null, assign it. The next time gets cached value

        /// <summary>
        /// Here is where the links for the Components Menu in NavMenu are added
        /// Add here the new menu elements without caring about the order.
        /// They will be reordered automatically
        /// </summary>
        public DocsComponents DocsComponents => _docsComponents ??= new DocsComponents()
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
            .AddItem("FileUpload", typeof(MudButton))

            //GROUPS

            //Inputs
            .AddNavGroup("Form Inputs & controls", false, new DocsComponents()
                .AddItem("Radio", typeof(MudRadio))
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
            );

        private DocsComponents _docsComponentsApi;

        //cached property
        /// <summary>
        /// This autogenerates the Menu for the API
        /// </summary>
        public DocsComponents DocsComponentsApi
        {
            get
            {
                //caching property
                if (_docsComponentsApi != null) return _docsComponentsApi;

                _docsComponentsApi = new DocsComponents();
                foreach (var item in DocsComponents.Elements)
                {
                    if (item.IsNavGroup)
                    {
                        foreach (var ApiItem in item.GroupItems.Elements)
                        {
                            _docsComponentsApi.AddItem(ApiItem.Name, ApiItem.Component);
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
    }
}
