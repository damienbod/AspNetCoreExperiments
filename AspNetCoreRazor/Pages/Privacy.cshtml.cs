using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AspNetCoreRazor.Pages;

[AllowAnonymous]
public class PrivacyModel : PageModel
{
    public void OnGet()
    {
    }
}
