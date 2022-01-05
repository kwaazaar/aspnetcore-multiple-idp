using Microsoft.AspNetCore.Authorization;

internal class AuthenticatedUserHandler : AuthorizationHandler<AuthenticatedUserRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthenticatedUserRequirement requirement)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));
        if (requirement == null) throw new ArgumentNullException(nameof(requirement));

        if (context.User?.Identity != null && context.User.Identity.IsAuthenticated)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

internal class AuthenticatedUserRequirement : IAuthorizationRequirement
{
}