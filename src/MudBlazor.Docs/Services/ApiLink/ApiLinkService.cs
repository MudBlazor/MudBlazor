using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FuzzySharp;
using MudBlazor.Docs.Models;

namespace MudBlazor.Docs.Services
{
#nullable enable
    public class ApiLinkService : IApiLinkService
    {
        private readonly Dictionary<string, ApiLinkServiceEntry> _entries = [];
        private readonly IReadOnlyCollection<ApiLinkServiceEntry> _featuredEntries =
            [
                new ApiLinkServiceEntry
                {
                    Title = "Installation",
                    Link = "getting-started/installation",
                    SubTitle = "Get started with MudBlazor fast and easy."
                },

                new ApiLinkServiceEntry
                {
                    Title = "Wireframes",
                    Link = "getting-started/wireframes",
                    SubTitle = "These small templates can be copied directly or just be used for inspiration."
                },

                new ApiLinkServiceEntry
                {
                    Title = "Table",
                    Link = "components/table",
                    ComponentType = typeof(MudTable<T>),
                    SubTitle = "A sortable, filterable table with multiselection and pagination."
                },

                new ApiLinkServiceEntry
                {
                    Title = "Grid",
                    Link = "components/grid",
                    ComponentType = typeof(MudGrid),
                    SubTitle = "The grid component helps keeping layout consistent across various screen resolutions and sizes."
                },

                new ApiLinkServiceEntry
                {
                    Title = "Button",
                    Link = "components/button",
                    ComponentType = typeof(MudGrid),
                    SubTitle = "A Material Design button for triggering an action or navigating to a link."
                },

                new ApiLinkServiceEntry
                {
                    Title = "Card",
                    Link = "components/card",
                    ComponentType = typeof(MudCard),
                    SubTitle = "Cards can contain actions, text, or media like images or graphics."
                },

                new ApiLinkServiceEntry
                {
                    Title = "Dialog",
                    Link = "components/dialog",
                    ComponentType = typeof(MudDialog),
                    SubTitle = "A dialog will overlay your current app content, providing the user with either information, a choice, or other tasks."
                },

                new ApiLinkServiceEntry
                {
                    Title = "App Bar",
                    Link = "components/appbar",
                    ComponentType = typeof(MudAppBar),
                    SubTitle = "App bar is used to display actions, branding, navigation and screen titles."
                },

                new ApiLinkServiceEntry
                {
                    Title = "Navigation Menu",
                    Link = "components/navmenu",
                    ComponentType = typeof(MudNavMenu),
                    SubTitle = "Nav menu provides a tree-like menu linking to the content on your site."
                }
            ];

        public ApiLinkService(IMenuService menuService)
        {
            // TODO: Merge MenuService with ApiDocumentation.
            Register(menuService.Api); // this also registers components
            Register(menuService.Customization);
            Register(menuService.Features);
            Register(menuService.Utilities);
            RegisterFeaturedPages();
            RegisterAliases();
        }

        /// <inheritdoc />
        public Task<IReadOnlyCollection<ApiLinkServiceEntry>> Search(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return Task.FromResult<IReadOnlyCollection<ApiLinkServiceEntry>>([]);
            }

            // Case is ignored.
            text = text.ToLowerInvariant();

            // TODO: Merge ApiLinkServiceEntry _entries with DocumentedType ApiDocumentation.Types to combine both datasets efficiently.

            // Calculate the ratios of all keywords to the search input.
            var ratios = new Dictionary<ApiLinkServiceEntry, double>();
            foreach (var (keyword, entry) in _entries)
            {
                var ratio = GetSearchMatchRatio(text, keyword);

                // Assign the highest ratio so far to the entry.
                if (ratios.TryGetValue(entry, out var highestRatio))
                {
                    if (ratio > highestRatio)
                    {
                        ratios[entry] = ratio;
                    }
                }
                else
                {
                    ratios.Add(entry, ratio);
                }
            }

            // Return the most accurate and highest quality results.
            return Task.FromResult<IReadOnlyCollection<ApiLinkServiceEntry>>(
                ratios
                .Where(x => x.Value > 65)
                .OrderByDescending(x => x.Value)
                .Select(x => x.Key)
                .ToList()
            );
        }

        /// <inheritdoc />
        public IReadOnlyCollection<ApiLinkServiceEntry> GetAllEntries()
        {
            return _entries.Values
                .Distinct()
                .OrderBy(entry => entry.Title, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        /// <inheritdoc />
        public IReadOnlyCollection<ApiLinkServiceEntry> GetFeaturedEntries()
        {
            return _featuredEntries;
        }

        /// <summary>
        /// Returns a value representing the match ratio of the search input to the keyword.
        /// A higher ratio means a better match.
        /// </summary>
        /// <param name="search">The search query</param>
        /// <param name="keyword">The keyword from an existing source</param>
        private double GetSearchMatchRatio(string search, string keyword)
        {
            var ratio = Fuzz.Ratio(keyword, search);
            var partialOutOfOrderRatio = Fuzz.PartialTokenSortRatio(keyword, search);
            var averageRatio = (ratio + partialOutOfOrderRatio) / 2.0;

            return averageRatio;
        }

        /// <summary>
        /// Adds the specified entry to the search index.
        /// </summary>
        private void AddEntry(ApiLinkServiceEntry entry)
        {
            AddKeyword(entry, entry.Title);
            AddKeyword(entry, entry.SubTitle);
            AddKeyword(entry, entry.ComponentName);
            AddKeyword(entry, entry.Link);
        }

        private void AddKeyword(ApiLinkServiceEntry entry, string? keyword)
        {
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                _entries[keyword.ToLowerInvariant()] = entry;
            }
        }

        /// <inheritdoc />
        public void RegisterPage(string title, string? subtitle, Type? componentType, string? link = null)
        {
            link ??= ApiLink.GetComponentLinkFor(componentType!);

            var entry = new ApiLinkServiceEntry
            {
                Title = title,
                SubTitle = subtitle,
                ComponentType = componentType,
                Link = link
            };

            AddEntry(entry);
        }

        /// <summary>
        /// Registers specific aliases for components or pages.
        /// </summary>
        private void RegisterAliases()
        {
            // Add search texts here which users might search and direct them to the correct component or page.
            RegisterPage("Accordion", subtitle: "Go to Expansion Panels", componentType: typeof(MudExpansionPanels));
            RegisterPage("Backdrop", subtitle: "Go to Overlay", componentType: typeof(MudOverlay));
            RegisterPage("Box", subtitle: "Go to Paper", componentType: typeof(MudPaper));
            RegisterPage("Combo Box", subtitle: "Go to Select", componentType: typeof(MudSelect<T>));
            RegisterPage("Drag & Drop", subtitle: "Go to Drop Zone", componentType: typeof(MudDropZone<T>));
            RegisterPage("Dropdown", subtitle: "Go to Select", componentType: typeof(MudSelect<T>));
            RegisterPage("Expander", subtitle: "Go to Collapse", componentType: typeof(MudCollapse));
            RegisterPage("Harmonica", subtitle: "Go to Expansion Panels", componentType: typeof(MudExpansionPanels));
            RegisterPage("Horizontal Line", subtitle: "Go to Divider", componentType: typeof(MudDivider));
            RegisterPage("Notification", subtitle: "Go to Snackbar", componentType: typeof(MudSnackbarProvider));
            RegisterPage("Popup", subtitle: "Go to Popover", componentType: typeof(MudPopover));
            RegisterPage("Segmented Buttons", subtitle: "Go to Toggle Group", componentType: typeof(MudToggleGroup<T>));
            RegisterPage("Side Panel", subtitle: "Go to Drawer", componentType: typeof(MudDrawer));
            RegisterPage("Toast", subtitle: "Go to Snackbar", componentType: typeof(MudSnackbarProvider));
            RegisterPage("Typeahead", subtitle: "Go to Autocomplete", componentType: typeof(MudAutocomplete<T>));
            RegisterAliasKeyword("components/navmenu", "Navigation Menu");
        }

        private void RegisterFeaturedPages()
        {
            foreach (var entry in _featuredEntries)
            {
                if (entry.ComponentType is not null)
                {
                    continue;
                }

                RegisterPage(
                    title: entry.Title,
                    subtitle: entry.SubTitle,
                    componentType: entry.ComponentType,
                    link: entry.Link
                );
            }
        }

        private void RegisterAliasKeyword(string link, string alias)
        {
            if (_entries.TryGetValue(link.ToLowerInvariant(), out var entry))
            {
                AddKeyword(entry, alias);
            }
        }

        /// <summary>
        /// Registers the specified items to the search index.
        /// </summary>
        private void Register(IEnumerable<MudComponent> items)
        {
            foreach (var item in items)
            {
                RegisterPage(
                    title: item.Name,
                    subtitle: $"{item.ComponentName} usage examples",
                    componentType: item.Type,
                    link: $"components/{item.Link}"
                );
            }
        }

        /// <summary>
        /// Registers the specified links to the search index.
        /// </summary>
        private void Register(IEnumerable<DocsLink> links)
        {
            foreach (var link in links)
            {
                RegisterPage(
                    title: link.Title,
                    subtitle: "",
                    componentType: null,
                    link: link.Href
                );
            }
        }
    }
}
