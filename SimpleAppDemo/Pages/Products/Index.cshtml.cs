using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SimpleAppDemo.Pages.Products
{
    [Authorize]
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
