using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace web.auth.AuthPolicies
{
    internal class ApiAuthorizationHandler : AuthorizationHandler<AuthorizedForApiRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiAuthorizationHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthorizedForApiRequirement requirement)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (requirement == null) throw new ArgumentNullException(nameof(requirement));

            if (context.User?.Identity != null && context.User.Identity.IsAuthenticated)
            {
                // Source: https://stackoverflow.com/a/62593206/10781019
                var descriptor = _httpContextAccessor.HttpContext.GetEndpoint().Metadata.GetMetadata<ControllerActionDescriptor>();
                // ControllerName: Test, ControllerTypeInfo.Name: TestController, ActionName: GetSecured

                if (descriptor.ControllerTypeInfo.Name == "TestController" && descriptor.ActionName == "GetSecured")
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}
