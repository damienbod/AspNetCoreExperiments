using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace UITestClientForApiTest
{
    public class ApiService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ITokenAcquisition _tokenAcquisition;
        private readonly IConfiguration _configuration;

        public ApiService(IHttpClientFactory clientFactory,
            ITokenAcquisition tokenAcquisition,
            IConfiguration configuration)
        {
            _clientFactory = clientFactory;
            _tokenAcquisition = tokenAcquisition;
            _configuration = configuration;
        }

        public async Task<string> ViewAccessToken()
        {
            var scope = _configuration["UserApiOne:ScopeForAccessToken"];
            var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(new[] { scope });
            return accessToken;
        }

        public async Task<JArray> GetApiDataAsync()
        {
            var client = _clientFactory.CreateClient();

            var scope = _configuration["UserApiOne:ScopeForAccessToken"];
            var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(new[] { scope });

            client.BaseAddress = new Uri(_configuration["UserApiOne:ApiBaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await client.GetAsync("api/MyApiJwtProtected");
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var data = JArray.Parse(responseContent);

                return data;
            }

            throw new HttpRequestException($"Status code: {response.StatusCode}, Error: {response.ReasonPhrase}");
        }
    }
}
