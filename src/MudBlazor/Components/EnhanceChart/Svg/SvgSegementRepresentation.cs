// Not Used

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudBlazor.EnhanceChart.Internal
{
    public abstract class SvgSegementRepresentation
    {
        public String OldPath { get; set; }
        public String Id { get; set; }

        public abstract String GetPathValue();
    }
}
