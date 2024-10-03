namespace AspNetCoreRazor;

public static class SecurityHeadersDefinitions
{
    public static HeaderPolicyCollection GetHeaderPolicyCollection(bool isDev)
    {
        var policy = new HeaderPolicyCollection()
            .AddFrameOptionsDeny()
            .AddContentTypeOptionsNoSniff()
            .AddReferrerPolicyStrictOriginWhenCrossOrigin()
            .AddCrossOriginOpenerPolicy(builder => builder.SameOrigin())
            .AddCrossOriginEmbedderPolicy(builder => builder.RequireCorp())
            .AddCrossOriginResourcePolicy(builder => builder.SameOrigin())
            .AddContentSecurityPolicy(builder =>
            {
                builder.AddObjectSrc().None();
                builder.AddBlockAllMixedContent();
                builder.AddImgSrc().Self().From("data:");
                builder.AddFormAction().Self();
                builder.AddFontSrc().Self();
                builder.AddBaseUri().Self();
                builder.AddFrameAncestors().None();

                builder.AddStyleSrc().WithNonce().UnsafeInline();

                builder.AddScriptSrc()
                    .WithNonce()
                    .WithHash256("j7OoGArf6XW6YY4cAyS3riSSvrJRqpSi1fOF9vQ5SrI=")
                    .UnsafeInline();
                //builder.AddCustomDirective("require-trusted-types-for", "'script'");
            })
            .RemoveServerHeader()
            .AddPermissionsPolicyWithDefaultSecureDirectives();

        if (!isDev)
        {
            // maxage = one year in seconds
            policy.AddStrictTransportSecurityMaxAgeIncludeSubDomains(maxAgeInSeconds: 60 * 60 * 24 * 365);
        }

        return policy;
    }
}
