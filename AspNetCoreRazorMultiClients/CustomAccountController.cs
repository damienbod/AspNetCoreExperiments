using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace AspNetCoreRazorMultiClients
{
    [AllowAnonymous]
    [Route("[controller]")]
    public class CustomAccountController : Controller
    {
        private readonly IConfiguration _configuration;

        public CustomAccountController(IConfiguration  configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("SignInT1")]
        public IActionResult SignInT1([FromQuery] string redirectUri)
        {
            var scheme = "t1";
            string redirect;
            if (!string.IsNullOrEmpty(redirectUri) && Url.IsLocalUrl(redirectUri))
            {
                redirect = redirectUri;
            }
            else
            {
                redirect = Url.Content("~/")!;
            }

            return Challenge(new AuthenticationProperties { RedirectUri = redirect }, scheme);
        }

        [HttpGet("SignInT2")]
        public IActionResult SignInT2([FromQuery] string redirectUri)
        {
            var scheme = "t2";
            string redirect;
            if (!string.IsNullOrEmpty(redirectUri) && Url.IsLocalUrl(redirectUri))
            {
                redirect = redirectUri;
            }
            else
            {
                redirect = Url.Content("~/")!;
            }

            return Challenge(new AuthenticationProperties { RedirectUri = redirect }, scheme);
        }

        [HttpGet("CustomSignOut")]
        public async Task<IActionResult> CustomSignOut()
        {
            var aud = HttpContext.User.FindFirst("aud");
            if(aud.Value == _configuration["AzureAdT1:ClientId"])
            {
                
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignOutAsync("cookiet1");
                var authSignOut = new AuthenticationProperties
                {
                    RedirectUri = "https://localhost:44348/SignoutCallbackOidc"
                };
                return SignOut(authSignOut, "t1");
            }
            else
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignOutAsync("cookiet2");
                var authSignOut = new AuthenticationProperties
                {
                    RedirectUri = "https://localhost:44348/SignoutCallbackOidc"
                };
                return SignOut(authSignOut, "t2");
            }
        }
    }
}