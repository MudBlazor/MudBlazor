using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MudBlazor.Docs.Models;

namespace MudBlazor.Docs.Services
{

    public interface IApiLinkService
    {
        void RegisterPage(string title, string subtitle, Type componentType, string link);
        Task<IEnumerable<ApiLinkServiceEntry>> Search(string text);
    }

    public class ApiLinkService : IApiLinkService
    {
        private Dictionary<string, ApiLinkServiceEntry> _lookup = new();

        //constructor with DI
        public ApiLinkService(IMenuService menuService)
        {

            Register(menuService.Api);//this also registers components
            Register(menuService.GettingStarted);
            Register(menuService.Customization);
            Register(menuService.Features);
            Register(menuService.About);

        }

        public void RegisterPage(string title, string subtitle, Type componentType, string link)
        {
            var entry = new ApiLinkServiceEntry { Title = title, SubTitle = subtitle, ComponentType = componentType, Link = link };
            _lookup[title.ToLowerInvariant()] = entry;
            if (componentType != null)
                _lookup[componentType.Name.ToLowerInvariant()] = entry;
            if (subtitle != null)
                _lookup[subtitle.ToLowerInvariant()] = entry;
        }

        public Task<IEnumerable<ApiLinkServiceEntry>> Search(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return Task.FromResult<IEnumerable<ApiLinkServiceEntry>>(null);
            var s = text.ToLowerInvariant();
            return Task.FromResult<IEnumerable<ApiLinkServiceEntry>>(
                _lookup
                .Where(x => IsMatch(x, s))
                .Select(x => x.Value)
                .Distinct()
                .OrderByDescending(e => e.Title.ToLowerInvariant().StartsWith(s))
                .ToArray()
            );
        }

        private bool IsMatch(in KeyValuePair<string, ApiLinkServiceEntry> keyValuePair, string s)
        {
            if (keyValuePair.Key.Contains(s))
                return true;
            var entry = keyValuePair.Value;
            if (entry.Title.ToLowerInvariant().Contains(s))
                return true;
            if (entry.SubTitle != null && entry.SubTitle.ToLowerInvariant().Contains(s))
                return true;
            if (entry.ComponentType != null && entry.ComponentName.ToLowerInvariant().Contains(s))
                return true;
            if (entry.Link.ToLowerInvariant().Contains(s))
                return true;
            return false;
        }



        private void Register(IEnumerable<MudComponent> items)
        {
            foreach (var item in items)
            {
                //components
                RegisterPage(
                title: item.Name,
                subtitle: $"{item.ComponentName} usage examples",
                componentType: item.Component,
                link: $"components/{item.Link}"
                );

                //api
                RegisterPage(
                    title: item.ComponentName,
                    subtitle: $"API documentation",
                    componentType: item.Component,
                    link: ApiLink.GetApiLinkFor(item.Component)
                    );
            }


        }

        private void Register(IEnumerable<DocsLink> list)
        {
            foreach (var link in list)
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

    public class ApiLinkServiceEntry
    {
        public string Link { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public Type ComponentType { get; set; }
        public string ComponentName => ComponentType?.Name.Replace("`1", "<T>");

        public override string ToString()
        {
            return Title;
        }
    }
}
