using Microsoft.AspNetCore.Authorization;
using web.auth.AuthPolicies;

namespace web.auth
{
    public class ApiAuthorisationFilterAttribute : AuthorizeAttribute
    {
        public ApiAuthorisationFilterAttribute()
            : base() // Implementation is configured in policy
        {
            Policy = PolicyConstants.ApiAuthorizationPolicy;
        }
    }
}
