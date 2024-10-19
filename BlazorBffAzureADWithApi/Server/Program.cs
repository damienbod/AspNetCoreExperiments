using BlazorBffAzureADWithApi.Server;
using BlazorBffAzureADWithApi.Server.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi.Models;
using NetEscapades.AspNetCore.SecurityHeaders.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.AddServerHeader = false;
});

var services = builder.Services;
var configuration = builder.Configuration;
var env = builder.Environment;

services.AddSecurityHeaderPolicies()
  .SetPolicySelector((PolicySelectorContext ctx) =>
  {
      return SecurityHeadersDefinitions.GetHeaderPolicyCollection(env.IsDevelopment(),
          configuration["AzureAd:Instance"]);
  });

services.AddScoped<MsGraphService>();
services.AddScoped<CaeClaimsChallengeService>();

services.AddScoped<MsGraphService>();

services.AddAntiforgery(options =>
{
    options.HeaderName = "X-XSRF-TOKEN";
    options.Cookie.Name = "__Host-X-XSRF-TOKEN";
    options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict;
    options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.Always;
});

services.AddHttpClient();
services.AddOptions();

string[]? initialScopes = configuration.GetValue<string>("UserApiOne:ScopeForAccessToken")?.Split(' ');

services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddMicrosoftIdentityWebApp(configuration, "AzureAd", OpenIdConnectDefaults.AuthenticationScheme)
    .EnableTokenAcquisitionToCallDownstreamApi(initialScopes)
    .AddMicrosoftGraph("https://graph.microsoft.com/beta",
        "User.ReadBasic.All user.read")
    .AddInMemoryTokenCaches();

services.AddAuthentication("MyJwtApiScheme")
    .AddMicrosoftIdentityWebApi(configuration, "AzureAdMyApi", "MyJwtApiScheme");

services.AddControllersWithViews(options =>
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));

services.AddRazorPages().AddMvcOptions(options =>
{
    //var policy = new AuthorizationPolicyBuilder()
    //    .RequireAuthenticatedUser()
    //    .Build();
    //    .AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme, "MyJwtApiScheme", "OpenIdConnect") 
    //options.Filters.Add(new AuthorizeFilter(policy));
}).AddMicrosoftIdentityUI();

services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();

    // add JWT Authentication
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "JWT Authentication",
        Description = "Enter JWT Bearer token **_only_**",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer", // must be lower case
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };
    c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {securityScheme, Array.Empty<string>() }
            });
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "My API",
        Version = "v1",
        Description = "My API"
    });

});


var app = builder.Build();

IdentityModelEventSource.ShowPII = true;

if (env.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
}

app.UseSecurityHeaders();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyApi v1");
});

app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseNoUnauthorizedRedirect("/api");

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapNotFound("/api/{**segment}");
app.MapFallbackToPage("/_Host");

app.Run();
