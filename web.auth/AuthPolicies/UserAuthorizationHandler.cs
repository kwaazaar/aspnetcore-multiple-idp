using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Security.Claims;

namespace web.auth.AuthPolicies
{
    internal class UserAuthorizationHandler : AuthorizationHandler<AuthorizedUserRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserAuthorizationHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthorizedUserRequirement requirement)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (requirement == null) throw new ArgumentNullException(nameof(requirement));

            if (context.User?.Identity != null && context.User.Identity.IsAuthenticated)
            {
                if (context.User.HasClaim(c => c.Type == ClaimTypes.Name))
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}
