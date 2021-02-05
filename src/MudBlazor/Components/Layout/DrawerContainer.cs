using System.Collections.Generic;
using System.Linq;

namespace MudBlazor
{
    public class DrawerContainer
    {
        List<MudDrawer> _drawers = new List<MudDrawer>();

        public void Add(MudDrawer drawer)
            => _drawers.Add(drawer);

        public void Remove(MudDrawer drawer)
            => _drawers.Remove(drawer);

        public bool HasDrawer()
            => _drawers.Any();

        public bool HasDrawer(Anchor anchor)
            => _drawers.Any(d => d.Anchor == anchor);

        public MudDrawer GetDrawerOrDefault(Anchor anchor)
            => _drawers.FirstOrDefault(d => d.Anchor == anchor);
    }
}
