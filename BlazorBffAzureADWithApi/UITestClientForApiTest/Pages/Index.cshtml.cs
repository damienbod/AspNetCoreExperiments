using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Web;
using System.Threading.Tasks;

namespace UITestClientForApiTest.Pages;

[AuthorizeForScopes(Scopes = new string[] { "api://916fe8ab-1da2-4ab6-ad52-c93e5bfd643d/access_as_user" })]
public class CallApiModel : PageModel
{
    private readonly ApiService _apiService;

    public string DataFromApi { get; set; }
    public string AccessToken { get; set; }
    public CallApiModel(ApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task OnGetAsync()
    {
        AccessToken = await _apiService.ViewAccessToken();
    }
}