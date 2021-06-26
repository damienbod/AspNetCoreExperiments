using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;

namespace AspNetCoreRazorMultiClients
{
    //[NonController]
    [AllowAnonymous]
    [Route("[controller]")]
    public class CustomAccountController : Controller
    {
        private readonly IOptionsMonitor<MicrosoftIdentityOptions> _optionsMonitor;
        private readonly IConfiguration _configuration;

        public CustomAccountController(IOptionsMonitor<MicrosoftIdentityOptions> microsoftIdentityOptionsMonitor,
            IConfiguration  configuration)
        {
            _optionsMonitor = microsoftIdentityOptionsMonitor;
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
                await HttpContext.SignOutAsync("t1");
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignOutAsync("cookiet1");
            }
            else
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignOutAsync("cookiet2");
                await HttpContext.SignOutAsync("t2");
            }

            return Redirect("/");
        }
    }
}