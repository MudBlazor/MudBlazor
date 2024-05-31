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

        public ApiLinkService(IMenuService menuService)
        {
            Register(menuService.Api); // this also registers components
            Register(menuService.Customization);
            Register(menuService.Features);
            Register(menuService.Utilities);
            RegisterAliases();
        }

        public Task<IReadOnlyCollection<ApiLinkServiceEntry>> Search(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return Task.FromResult<IReadOnlyCollection<ApiLinkServiceEntry>>([]);
            }

            // Case is ignored.
            text = text.ToLowerInvariant();

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

        private double GetSearchMatchRatio(string search, string keyword)
        {
            var ratio = Fuzz.Ratio(keyword, search);
            var partialOutOfOrderRatio = Fuzz.PartialTokenSortRatio(keyword, search);
            var averageRatio = (ratio + partialOutOfOrderRatio) / 2.0;

            return averageRatio;
        }

        private void AddEntry(ApiLinkServiceEntry entry)
        {
            void AddKeyword(string? k)
            {
                if (!string.IsNullOrWhiteSpace(k))
                {
                    _entries[k.ToLowerInvariant()] = entry;
                }
            }

            AddKeyword(entry.Title);
            AddKeyword(entry.SubTitle);
            AddKeyword(entry.ComponentName);
            AddKeyword(entry.Link);
        }

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

        private void RegisterAliases()
        {
            // Add search texts here which users might search and direct them to the correct component or page.
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
            RegisterPage("Side Panel", subtitle: "Go to Drawer", componentType: typeof(MudDrawer));
            RegisterPage("Toast", subtitle: "Go to Snackbar", componentType: typeof(MudSnackbarProvider));
            RegisterPage("Typeahead", subtitle: "Go to Autocomplete", componentType: typeof(MudAutocomplete<T>));
        }

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
