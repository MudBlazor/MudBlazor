

using System;
using System.Collections.Generic;

namespace MudBlazor.EnhanceChart
{

    public interface IDataSet 
    {
        (Double Max, Double Min) GetMinimumAndMaximumValues();
    }
}
