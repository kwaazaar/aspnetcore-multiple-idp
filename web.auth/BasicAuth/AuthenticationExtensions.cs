using Microsoft.AspNetCore.Authentication;

namespace web.auth.BasicAuth
{
    public static class AuthenticationExtensions
    {
        public static AuthenticationBuilder AddBasicAuthentication(this AuthenticationBuilder authBuilder)
            => AddBasicAuthentication(authBuilder, BasicAuthenticationHandler.DefaultScheme);

        public static AuthenticationBuilder AddBasicAuthentication(this AuthenticationBuilder authBuilder, string authenticationScheme)
            => AddBasicAuthentication(authBuilder, authenticationScheme, "https://localhost");

        public static AuthenticationBuilder AddBasicAuthentication(this AuthenticationBuilder authBuilder, string authenticationScheme, string claimsIssuer)
        {
            authBuilder
                .AddScheme<BasicAuthenticationOptions, BasicAuthenticationHandler>(authenticationScheme, o =>
                {
                    o.ClaimsIssuer = claimsIssuer;
                    o.Validate();
                });
            return authBuilder;
        }
        
    }
}
