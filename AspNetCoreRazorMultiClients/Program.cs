using AspNetCoreRazorMultiClients;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.AddServerHeader = false;
});

var services = builder.Services;
var configuration = builder.Configuration;
var env = builder.Environment;

// store the Microsoft Entra ID login
services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();

services.AddAuthentication()
    .AddMicrosoftIdentityWebApp(configuration.GetSection("AzureAdT1"), "t1", "cookiet1");

services.Configure<OpenIdConnectOptions>("t1", options =>
{
    var existingOnTokenValidatedHandler = options.Events.OnTokenValidated;
    options.Events.OnTokenValidated = async context =>
    {
        await existingOnTokenValidatedHandler(context);

        if (context.Principal != null)
        {
            await context.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, context.Principal);
        }
    };
});

services.AddAuthentication()
    .AddMicrosoftIdentityWebApp(configuration.GetSection("AzureAdT2"), "t2", "cookiet2");

services.Configure<OpenIdConnectOptions>("t2", options =>
{
    var existingOnTokenValidatedHandler = options.Events.OnTokenValidated;
    options.Events.OnTokenValidated = async context =>
    {
        await existingOnTokenValidatedHandler(context);

        if (context.Principal != null)
        {
            await context.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, context.Principal);
        }
    };
});

// no policy, you can choose which client 
services.AddAuthorization();

services.AddRazorPages()
    .AddMvcOptions(options => { })
    .AddMicrosoftIdentityUI();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseSecurityHeaders(SecurityHeadersDefinitions
    .GetHeaderPolicyCollection(env.IsDevelopment()));

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.Run();
