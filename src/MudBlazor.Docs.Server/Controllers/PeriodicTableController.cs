﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MudBlazor.Examples.Data;
using MudBlazor.Examples.Data.Models;

namespace Server.Controllers
{
    [Route("wasm/webapi/[controller]")]
    [Route("webapi/[controller]")]
    [ApiController]
    public class PeriodicTableController : ControllerBase
    {
        private IPeriodicTableService _periodicTableService;

        public PeriodicTableController(IPeriodicTableService periodicTableService)
        {
            _periodicTableService = periodicTableService;
        }

        [HttpGet("{search}")]
        public Task<IEnumerable<Element>> Get(string search)
        {
            return _periodicTableService.GetElements(search);
        }

        [HttpGet]
        public Task<IEnumerable<Element>> Get()
        {
            return _periodicTableService.GetElements();
        }
    }
}
