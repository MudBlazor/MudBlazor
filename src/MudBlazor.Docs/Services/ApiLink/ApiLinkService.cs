using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        private readonly IReadOnlyCollection<ApiLinkServiceEntry> _navigationEntries =
            [
                new ApiLinkServiceEntry
                {
                    Title = "Explore",
                    Link = "docs/overview",
                    SubTitle = "Docs"
                },

                new ApiLinkServiceEntry
                {
                    Title = "Installation",
                    Link = "getting-started/installation",
                    SubTitle = "Getting Started"
                },

                new ApiLinkServiceEntry
                {
                    Title = "Layouts",
                    Link = "getting-started/layouts",
                    SubTitle = "Getting Started"
                },

                new ApiLinkServiceEntry
                {
                    Title = "Usage",
                    Link = "getting-started/usage",
                    SubTitle = "Getting Started"
                },

                new ApiLinkServiceEntry
                {
                    Title = "Wireframes",
                    Link = "getting-started/wireframes",
                    SubTitle = "Getting Started"
                },

                new ApiLinkServiceEntry
                {
                    Title = "What is MudBlazor?",
                    Link = "mud/introduction",
                    SubTitle = "Learn More"
                },

                new ApiLinkServiceEntry
                {
                    Title = "Announcements",
                    Link = "mud/announcements",
                    SubTitle = "Learn More"
                },

                new ApiLinkServiceEntry
                {
                    Title = "Getting Help",
                    Link = "mud/community/getting-help",
                    SubTitle = "Learn More"
                },

                new ApiLinkServiceEntry
                {
                    Title = "Reporting Bugs",
                    Link = "mud/community/reporting-bugs",
                    SubTitle = "Learn More"
                },

                new ApiLinkServiceEntry
                {
                    Title = "Contribution",
                    Link = "mud/community/contribution",
                    SubTitle = "Learn More"
                },

                new ApiLinkServiceEntry
                {
                    Title = "Community Extensions",
                    Link = "mud/community/extensions",
                    SubTitle = "Learn More"
                },

                new ApiLinkServiceEntry
                {
                    Title = "Releases",
                    Link = "mud/project/releases",
                    SubTitle = "Learn More"
                },

                new ApiLinkServiceEntry
                {
                    Title = "Roadmap",
                    Link = "mud/project/roadmap",
                    SubTitle = "Learn More"
                },

                new ApiLinkServiceEntry
                {
                    Title = "Sponsors & Backers",
                    Link = "mud/project/sponsor",
                    SubTitle = "Learn More"
                },

                new ApiLinkServiceEntry
                {
                    Title = "Team & Contributors",
                    Link = "mud/project/team",
                    SubTitle = "Learn More"
                },

                new ApiLinkServiceEntry
                {
                    Title = "How it Started",
                    Link = "mud/project/how-it-started",
                    SubTitle = "Learn More"
                }
            ];

        private readonly ISearchService _searchService = new SearchService();

        public ApiLinkService(IMenuService menuService)
        {
            // TODO: Merge MenuService with ApiDocumentation.
            Register(menuService.Api); // this also registers components
            Register(menuService.Customization);
            Register(menuService.Features);
            Register(menuService.Utilities);
            RegisterFeaturedPages();
            RegisterNavigationPages();
            RegisterAliases();
        }

        /// <inheritdoc />
        public Task<IReadOnlyCollection<ApiLinkServiceEntry>> Search(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return Task.FromResult<IReadOnlyCollection<ApiLinkServiceEntry>>([]);

            // TODO: Merge ApiLinkServiceEntry _entries with DocumentedType ApiDocumentation.Types to combine both datasets efficiently.
            return Task.FromResult<IReadOnlyCollection<ApiLinkServiceEntry>>(_searchService.Search(_entries.Values, e => e.Keywords, text));
        }

        /// <inheritdoc />
        public IReadOnlyCollection<ApiLinkServiceEntry> GetAllEntries()
        {
            return [.. _entries.Values.OrderBy(entry => entry.Title, StringComparer.OrdinalIgnoreCase)];
        }

        /// <inheritdoc />
        public IReadOnlyCollection<ApiLinkServiceEntry> GetFeaturedEntries()
        {
            return _featuredEntries;
        }

        /// <summary>
        /// Adds the specified entry to the search index.
        /// If an entry with the same link already exists, the new entry's keywords are merged into it.
        /// </summary>
        private void AddEntry(ApiLinkServiceEntry entry)
        {
            var key = entry.Link.ToLowerInvariant();
            if (!_entries.TryGetValue(key, out var stored))
            {
                stored = entry;
                _entries[key] = entry;
            }

            AddKeyword(stored, entry.Title);
            AddKeyword(stored, entry.SubTitle);
            AddKeyword(stored, entry.ComponentName);
            AddKeyword(stored, entry.Link);
        }

        private static void AddKeyword(ApiLinkServiceEntry entry, string? keyword)
        {
            if (!string.IsNullOrWhiteSpace(keyword))
                entry.Keywords.Add(keyword);
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
            RegisterAliasKeyword("docs/overview", "Explore MudBlazor");
            RegisterAliasKeyword("getting-started/installation", "Get Started");
            RegisterAliasKeyword("getting-started/installation", "Getting Started");
            RegisterAliasKeyword("mud/introduction", "Learn More");
        }

        private void RegisterFeaturedPages()
        {
            foreach (var entry in _featuredEntries)
            {
                if (entry.ComponentType is not null)
                {
                    RegisterAliasKeyword(entry.Link, entry.SubTitle);
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

        private void RegisterNavigationPages()
        {
            foreach (var entry in _navigationEntries)
            {
                AddEntry(entry);
            }
        }

        private void RegisterAliasKeyword(string link, string? alias)
        {
            if (_entries.TryGetValue(link.ToLowerInvariant(), out var entry))
                AddKeyword(entry, alias);
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
                    subtitle: link.Group,
                    componentType: null,
                    link: link.Href
                );
            }
        }
    }
}
