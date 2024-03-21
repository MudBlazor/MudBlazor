﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MudBlazor.Docs.Models;
using MudBlazor.Utilities;

namespace MudBlazor.Docs.Services
{
#nullable enable
    public class ApiLinkService : IApiLinkService
    {
        private readonly List<ApiLinkServiceEntry> _entries = [];

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

            var distances = new Dictionary<ApiLinkServiceEntry, int>();
            foreach (var entry in _entries)
            {
                // Strings to include in the search.
                var keywords = new[] {
                    entry.Title,
                    entry.SubTitle,
                    entry.ComponentName,
                    entry.Link
                };

                var smallestDistance = int.MaxValue;

                if (entry.Title.StartsWith(text, StringComparison.InvariantCultureIgnoreCase))
                {
                    smallestDistance = -1;
                }
                else
                {
                    // Find the smallest distance between any keyword and the search string.
                    foreach (var keyword in keywords.Where(k => !string.IsNullOrWhiteSpace(k)))
                    {
                        var keywordDistance = FuzzySearch.LevenshteinDistance(keyword, text);

                        smallestDistance = Math.Min(smallestDistance, keywordDistance);
                    }
                }

                distances.Add(entry, smallestDistance);
            }

            return Task.FromResult<IReadOnlyCollection<ApiLinkServiceEntry>>(
                distances
                    .Where(m => m.Value < 5)
                    .OrderBy(m => m.Value)
                    .Select(m => m.Key)
                    .ToList()
            );
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

            _entries.Add(entry);
        }

        private void RegisterAliases()
        {
            // Add search texts here which users might search and direct them to the correct component or page.
            RegisterPage("Backdrop", subtitle: "Go to Overlay", componentType: typeof(MudOverlay));
            RegisterPage("Box", subtitle: "Go to Paper", componentType: typeof(MudPaper));
            RegisterPage("ComboBox", subtitle: "Go to Select", componentType: typeof(MudSelect<T>));
            RegisterPage("Drag & Drop", subtitle: "Go to DropZone", componentType: typeof(MudDropZone<T>));
            RegisterPage("Dropdown", subtitle: "Go to Select", componentType: typeof(MudSelect<T>));
            RegisterPage("Harmonica", subtitle: "Go to ExpansionPanels", componentType: typeof(MudExpansionPanels));
            RegisterPage("Horizontal Line", subtitle: "Go to Divider", componentType: typeof(MudDivider));
            RegisterPage("Hiliter", subtitle: "Go to Highlighter", componentType: typeof(MudHighlighter));
            RegisterPage("Notification", subtitle: "Go to Snackbar", componentType: typeof(MudSnackbarProvider));
            RegisterPage("Popup", subtitle: "Go to Popover", componentType: typeof(MudPopover));
            RegisterPage("SidePanel", subtitle: "Go to Drawer", componentType: typeof(MudDrawer));
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
