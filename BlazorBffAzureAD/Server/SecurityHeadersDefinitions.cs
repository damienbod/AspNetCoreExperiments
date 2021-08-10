using Microsoft.AspNetCore.Builder;

namespace BlazorHosted.Server
{
    public static class SecurityHeadersDefinitions
    {
        public static HeaderPolicyCollection GetHeaderPolicyCollection(bool isDev)
        {
            var policy = new HeaderPolicyCollection()
                .AddFrameOptionsDeny()
                .AddXssProtectionBlock()
                .AddContentTypeOptionsNoSniff()
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
                        .Self()
                        .UnsafeInline();

                    builder.AddBaseUri().Self();

                    builder.AddFrameAncestors().None();

                    // due to Blazor
                    builder.AddScriptSrc() 
                        .Self()
                        .UnsafeInline()
                        .UnsafeEval();

                    //builder.AddCustomDirective("require-trusted-types-for", "'script'");
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

            if(!isDev)
            {
                // maxage = one year in seconds
                policy.AddStrictTransportSecurityMaxAgeIncludeSubDomains(maxAgeInSeconds: 60 * 60 * 24 * 365); 
            }

            return policy;
        }
    }
}
