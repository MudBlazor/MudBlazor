using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using MudBlazor.Examples.Data;

namespace Server.Controllers
{
    [Route("wasm/webapi/[controller]")]
    [Route("webapi/[controller]")]
    [ApiController]
    public class AmericanStatesController : ControllerBase
    {
        [HttpGet("{search}")]
        public IEnumerable<string> Get(string search)
        {
            return AmericanStates.GetStates(search);
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            return AmericanStates.GetStates();
        }
    }
}
