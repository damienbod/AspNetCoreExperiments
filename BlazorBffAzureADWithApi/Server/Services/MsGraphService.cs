using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Graph;
using Microsoft.Identity.Web;

namespace BlazorBffAzureADWithApi.Server.Services
{
    public class MsGraphService
    {
        private readonly GraphServiceClient _graphServiceClient;

        public MsGraphService(GraphServiceClient graphServiceClient)
        {
            _graphServiceClient = graphServiceClient;
        }

        public async Task<User> GetGraphApiUser()
        {
            return await _graphServiceClient
                .Me
                .Request()
                .WithScopes("User.ReadBasic.All", "user.read")
                .WithAuthenticationScheme(OpenIdConnectDefaults.AuthenticationScheme)
                .GetAsync();
        }

        public async Task<string> GetGraphApiProfilePhoto()
        {
            try
            {
                var photo = string.Empty;
                // Get user photo
                using (var photoStream = await _graphServiceClient
                    .Me.Photo.Content.Request()
                    .WithScopes("User.ReadBasic.All", "user.read")
                    .WithAuthenticationScheme(OpenIdConnectDefaults.AuthenticationScheme)
                    .GetAsync())
                {
                    byte[] photoByte = ((MemoryStream)photoStream).ToArray();
                    photo = Convert.ToBase64String(photoByte);
                }

                return photo;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}

