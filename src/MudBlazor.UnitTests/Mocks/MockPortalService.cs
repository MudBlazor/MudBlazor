using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudBlazor.Services;

namespace MudBlazor.UnitTests.Mocks
{
    public class MockPortalService : IPortal
    {
        public Dictionary<Guid, PortalItem> Items { get; }

        public event EventHandler OnChange;

        public void AddOrUpdate(PortalItem item)
        {
            OnChange.Invoke(null, null);

        }

        public PortalItem GetItem(Guid id)
        {
            throw new NotImplementedException();
            
        }

        public void Remove(PortalItem item)
        {
            throw new NotImplementedException();
        }
    }
}
