using Microsoft.AspNetCore.Mvc;
using MudBlazor.Examples.Data;
using MudBlazor.Examples.Data.Models;

namespace MudBlazor.Docs.Server.Controllers;

[Route("wasm/webapi/[controller]")]
[Route("webapi/[controller]")]
[ApiController]
public class CompanyController : ControllerBase
{
    [HttpGet]
    public IReadOnlyCollection<Company> Get() => CompanyData.GetRecords();
}
