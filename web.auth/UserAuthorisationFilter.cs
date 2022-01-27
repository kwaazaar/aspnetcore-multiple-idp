using Microsoft.AspNetCore.Authorization;
using web.auth.AuthPolicies;

namespace web.auth
{
    public class UserAuthorisationFilterAttribute : AuthorizeAttribute
    {
        public UserAuthorisationFilterAttribute()
            : base(PolicyConstants.UserAuthorizationPolicy) // Implementation is configured in policy
        {
        }
    }
}
