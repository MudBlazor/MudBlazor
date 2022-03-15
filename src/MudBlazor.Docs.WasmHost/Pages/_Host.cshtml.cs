using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MudBlazor.Docs.WasmHost.Pages
{
    public class HostModel : PageModel
    {
        public IActionResult OnGet()
        {
            if (Request.Headers.ContainsKey("UsePrerender") == false)
            {
                return File("index.html", "text/html");
            }
            else
            {
                return Page();
            }
        }
    }
}
