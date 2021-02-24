using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Toolbelt.Blazor.HeadElement;

namespace MudBlazor.UnitTests.Mocks
{
#pragma warning disable CS1998 // Justification - Implementing IHeadElementHelper
    public class MockHeadElementHelper : IHeadElementHelper
    {
        public async ValueTask<IEnumerable<LinkElement>> GetDefaultLinkElementsAsync()
        {
            return Array.Empty<LinkElement>();
        }

        public async ValueTask<IEnumerable<MetaElement>> GetDefaultMetaElementsAsync()
        {
            return Array.Empty<MetaElement>();
        }

        public async ValueTask<string> GetDefaultTitleAsync()
        {
            return null;
        }

        string _title;
        public async ValueTask<string> GetTitleAsync()
        {
            return _title;
        }

        public async ValueTask RemoveLinkElementsAsync(params LinkElement[] elements)
        {
        }

        public async ValueTask RemoveMetaElementsAsync(params MetaElement[] elements)
        {
        }

        public async ValueTask SetDefaultTitleAsync(string defaultTitle)
        {
        }

        public async ValueTask SetLinkElementsAsync(params LinkElement[] elements)
        {
        }

        public async ValueTask SetMetaElementsAsync(params MetaElement[] elements)
        {
        }

        public async ValueTask SetTitleAsync(string title)
        {
            _title = title;
        }
    }
#pragma warning restore CS1998
}
