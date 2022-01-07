using IdentityServer4.Models;
using IdentityServer4.Services;
using System.Threading.Tasks;

namespace idp
{
    public class UserProfileService : IProfileService
    {
        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            context.AddRequestedClaims(context.Subject.Claims); // Copy if already with same name in principal
            return Task.CompletedTask;
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = true; // TODO
            return Task.CompletedTask;
        }
    }
}
