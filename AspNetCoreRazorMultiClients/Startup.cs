using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;

namespace AspNetCoreRazorMultiClients;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        // store the aad login
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie();

        services.AddAuthentication()
            .AddMicrosoftIdentityWebApp(Configuration.GetSection("AzureAdT1"), "t1", "cookiet1");

        services.Configure<OpenIdConnectOptions>("t1", options =>
        {
            var existingOnTokenValidatedHandler = options.Events.OnTokenValidated;
            options.Events.OnTokenValidated = async context =>
            {
                await existingOnTokenValidatedHandler(context);

                await context.HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme, context.Principal);
            };
        });

        services.AddAuthentication()
            .AddMicrosoftIdentityWebApp(Configuration.GetSection("AzureAdT2"), "t2", "cookiet2");

        services.Configure<OpenIdConnectOptions>("t2", options =>
        {
            var existingOnTokenValidatedHandler = options.Events.OnTokenValidated;
            options.Events.OnTokenValidated = async context =>
            {
                await existingOnTokenValidatedHandler(context);

                await context.HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme, context.Principal);
            };
        });

        // no policy, you can choose which client 
        services.AddAuthorization();

        services.AddRazorPages()
            .AddMvcOptions(options => { })
            .AddMicrosoftIdentityUI();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
        }

        app.UseSecurityHeaders(
            SecurityHeadersDefinitions.GetHeaderPolicyCollection(env.IsDevelopment()));

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapRazorPages();
            endpoints.MapControllers();
        });
    }
}