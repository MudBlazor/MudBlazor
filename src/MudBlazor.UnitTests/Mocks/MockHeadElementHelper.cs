using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Toolbelt.Blazor.HeadElement;

namespace MudBlazor.UnitTests.Mocks
{
    public class MockHeadElementHelper : IHeadElementHelper
    {
        public async ValueTask<IEnumerable<LinkElement>> GetDefaultLinkElementsAsync()
        {
            return new LinkElement[0];
        }

        public async ValueTask<IEnumerable<MetaElement>> GetDefaultMetaElementsAsync()
        {
            return new MetaElement[0];
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
}
