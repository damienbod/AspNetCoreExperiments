using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreRazor
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApp(Configuration.GetSection("AzureAd"));

            services.AddAuthorization(options =>
            {
                // By default, all incoming requests will be authorized according to the default policy
                options.FallbackPolicy = options.DefaultPolicy;
            });
            services.AddRazorPages()
                .AddMvcOptions(options => { })
                .AddMicrosoftIdentityUI();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            var policyCollection = new HeaderPolicyCollection()
                .AddFrameOptionsDeny()
                .AddXssProtectionBlock()
                .AddContentTypeOptionsNoSniff()
                .AddStrictTransportSecurityMaxAgeIncludeSubDomains(maxAgeInSeconds: 60 * 60 * 24 * 365) // maxage = one year in seconds
                .AddReferrerPolicyStrictOriginWhenCrossOrigin()
                .RemoveServerHeader()
                .AddCrossOriginOpenerPolicy(builder =>
                {
                    builder.SameOrigin();
                })
                .AddCrossOriginEmbedderPolicy(builder =>
                {
                    builder.RequireCorp();
                })
                .AddCrossOriginResourcePolicy(builder =>
                {
                    builder.SameOrigin();
                })
                .AddContentSecurityPolicy(builder =>
                {
                    builder.AddObjectSrc()
                        .None();
                    builder.AddBlockAllMixedContent();

                    builder.AddImgSrc()
                        .Self()
                        .From("data:");

                    builder.AddFormAction().Self();

                    builder.AddFontSrc().Self();

                    builder.AddStyleSrc()
                        .Self();
                        // .UnsafeInline();

                    builder.AddBaseUri().Self();

                    builder.AddScriptSrc()
                        //.Self()
                        .UnsafeInline()
                        .WithNonce();

                    // builder.AddCustomDirective("require-trusted-types-for", "'script'");
                    builder.AddFrameAncestors().None();
                })
                .RemoveServerHeader()
                .AddPermissionsPolicy(builder =>
                {
                    builder.AddAccelerometer()
                        .None();

                    builder.AddAutoplay() // autoplay 'self'
                        .None();

                    builder.AddCamera() // camera 'none'
                        .None();

                    builder.AddEncryptedMedia() // encrypted-media 'self'
                        .None();

                    builder.AddFullscreen() // fullscreen *:
                        .All();

                    builder.AddGeolocation() // geolocation 'none'
                        .None();

                    builder.AddGyroscope() // gyroscope 'none'
                        .None();

                    builder.AddMagnetometer() // magnetometer 'none'
                        .None();

                    builder.AddMicrophone() // microphone 'none'
                        .None();

                    builder.AddMidi() // midi 'none'
                        .None();

                    builder.AddPayment() // payment 'none'
                        .None();

                    builder.AddPictureInPicture() // picture-in-picture 'none'
                        .None();

                    builder.AddSyncXHR() // sync-xhr 'none'
                        .None();

                    builder.AddUsb() // usb 'none'
                        .None();
                });

            app.UseSecurityHeaders(policyCollection);

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
}
