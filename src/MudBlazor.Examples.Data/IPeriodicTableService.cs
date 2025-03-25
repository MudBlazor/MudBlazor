using System.Collections.Generic;
using System.Threading.Tasks;
using MudBlazor.Examples.Data.Models;

namespace MudBlazor.Examples.Data;

public interface IPeriodicTableService
{
    Task<IEnumerable<Element>> GetElements();

    Task<IEnumerable<Element>> GetElements(string search);
}
