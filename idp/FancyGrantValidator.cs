using IdentityServer4.Models;
using IdentityServer4.Validation;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace idp
{
    public class FancyGrantValidator : IExtensionGrantValidator
    {
        private readonly ITokenValidator _validator;

        public FancyGrantValidator(ITokenValidator validator)
        {
            _validator = validator;
        }

        public string GrantType => "fancy";

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            //var userToken = context.Request.Raw.Get("token");

            //if (string.IsNullOrEmpty(userToken))
            //{
            //    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant);
            //    return;
            //}

            //var result = await _validator.ValidateAccessTokenAsync(userToken);
            //if (result.IsError)
            //{
            //    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant);
            //    return;
            //}

            //// get user's identity
            //var sub = result.Claims.FirstOrDefault(c => c.Type == "sub").Value;
            
            await Task.Delay(100);

            var userId = context.Request.Raw.Get("user_id");
            var userSecret = context.Request.Raw.Get("user_secret");

            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(userSecret)) {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "user_id and user_secret must both be provided");
                return;
            }

            if (userSecret == "letmein" && userId == "admin")
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userId),
                    new Claim("user_id", userId),
                };
                context.Result = new GrantValidationResult(userId, GrantType, claims); // Claims go to principal, not yet in token: CustomProfileService takes care of that.
            }
            else
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.UnauthorizedClient);
            }
            return;
        }
    }
}
