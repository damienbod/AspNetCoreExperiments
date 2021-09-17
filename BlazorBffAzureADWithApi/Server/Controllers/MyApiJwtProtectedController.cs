using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BlazorBffAzureADWithApi.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "MyJwtApiScheme")]
    [Produces("application/json")]
    [SwaggerTag("Using to provide a public api for different clients")]
    public class MyApiJwtProtectedController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [SwaggerOperation(OperationId = "MyApiJwtProtected-Get", Summary = "Returns string with details")]
        public IActionResult Get()
        {
            return Ok("yes my public api protected with Azure AD and JWT works");
        }
    }
}
