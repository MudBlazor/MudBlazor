using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MudBlazor.ExampleData;
using MudBlazor.ExampleData.Models;

namespace Server.Controllers
{
    [Route("wasm/webapi/[controller]")]
    [Route("webapi/[controller]")]
    [ApiController]
    public class PeriodicTableController : ControllerBase
    {
        [HttpGet("{search}")]
        public async Task<IEnumerable<Element>> Get(string search)
        {
            return await PeriodicTable.GetElementsAsync(search);
        }

        [HttpGet]
        public async Task<IEnumerable<Element>> Get()
        {
            return await PeriodicTable.GetElementsAsync();
        }
    }
}
