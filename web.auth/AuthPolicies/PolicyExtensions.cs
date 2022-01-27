using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace web.auth.AuthPolicies
{
    public static class PolicyExtensions
    {
        public static IServiceCollection AddAuthorizationHandlers(this IServiceCollection services) => services
            .AddHttpContextAccessor()
            .AddSingleton<IAuthorizationHandler, ApiAuthorizationHandler>() // Implementation of AuthorizedForApiRequirement
            .AddSingleton<IAuthorizationHandler, UserAuthorizationHandler>() // Implementation of AuthorizedUserRequirement
            ;

        public static AuthorizationOptions AddApiAuthorizationPolicy(this AuthorizationOptions options, params string[] schemes) =>
            AddApiAuthorizationPolicy(options, false, schemes);

        public static AuthorizationOptions AddApiAuthorizationPolicy(this AuthorizationOptions options, bool setDefault, params string[] schemes)
        {
            options.AddPolicy(PolicyConstants.ApiAuthorizationPolicy, policy =>
            {
                policy
                    .AddAuthenticationSchemes(schemes)
                    .AddRequirements(new AuthorizedForApiRequirement());
            });

            if (setDefault)
            {
                options.DefaultPolicy = options.GetPolicy(PolicyConstants.ApiAuthorizationPolicy);
            }

            return options;
        }

        public static AuthorizationOptions AddUserAuthorizationPolicy(this AuthorizationOptions options, params string[] schemes) =>
            AddUserAuthorizationPolicy(options, false, schemes);

        public static AuthorizationOptions AddUserAuthorizationPolicy(this AuthorizationOptions options, bool setDefault, params string[] schemes)
        {
            options.AddPolicy(PolicyConstants.UserAuthorizationPolicy, policy =>
            {
                policy
                    .AddAuthenticationSchemes(schemes)
                    .AddRequirements(new AuthorizedUserRequirement());
            });

            if (setDefault)
            {
                options.DefaultPolicy = options.GetPolicy(PolicyConstants.UserAuthorizationPolicy);
            }

            return options;
        }
    }
}
