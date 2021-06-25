using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public CustomAccountController(IOptionsMonitor<MicrosoftIdentityOptions> microsoftIdentityOptionsMonitor)
        {
            _optionsMonitor = microsoftIdentityOptionsMonitor;
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

        //[HttpGet("Challenge/{scheme?}")]
        //public IActionResult Challenge(
        //    string redirectUri,
        //    string scope,
        //    string loginHint,
        //    string domainHint,
        //    string claims,
        //    string policy,
        //    [FromRoute] string scheme)
        //{
        //    scheme ??= OpenIdConnectDefaults.AuthenticationScheme;
        //    Dictionary<string, string?> items = new Dictionary<string, string?>
        //        {
        //            { Constants.Claims, claims },
        //            { Constants.Policy, policy },
        //        };
        //    Dictionary<string, object?> parameters = new Dictionary<string, object?>
        //        {
        //            { Constants.LoginHint, loginHint },
        //            { Constants.DomainHint, domainHint },
        //        };

        //    OAuthChallengeProperties oAuthChallengeProperties = new OAuthChallengeProperties(items, parameters);
        //    oAuthChallengeProperties.Scope = scope?.Split(" ");
        //    oAuthChallengeProperties.RedirectUri = redirectUri;

        //    return Challenge(oAuthChallengeProperties, scheme);
        //}

        [HttpGet("SignOutT1")]
        public async Task<IActionResult> SignOutT1Async()
        {
            await HttpContext.SignOutAsync("cookiet1");
            return Redirect("/");
        }

        [HttpGet("SignOutT2")]
        public async Task<IActionResult> SignOutT2Async()
        {
            await HttpContext.SignOutAsync("cookiet2");
            return Redirect("/");
        }
    }
}