using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MudBlazor.Examples.Data;

namespace MudBlazor.Docs.WasmHost.Controllers;

[Route("wasm/webapi/[controller]")]
[Route("webapi/[controller]")]
[ApiController]
public class AmericanStatesController : ControllerBase
{
    [HttpGet("{search}")]
    public IEnumerable<string> Get(string search) => AmericanStates.GetStates(search);

    [HttpGet]
    public IEnumerable<string> Get() => AmericanStates.GetStates();

    [HttpGet("searchWithDelay/{input?}")]
    [OperationCancelledExceptionFilter]
    public async Task<IActionResult> SearchWithDelay(CancellationToken cancellationToken, [FromRoute(Name = "input")] string search = "")
    {
        var input = (search ?? string.Empty).Trim().ToLower();
        var states = AmericanStates.GetStates();

        List<string> result = new();
        foreach (var item in states)
        {
            if (string.IsNullOrEmpty(input) || item.Contains(input, StringComparison.InvariantCultureIgnoreCase))
            {
                result.Add(item);
            }

            await Task.Delay(40, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
        }

        return base.Ok(result);
    }
}
