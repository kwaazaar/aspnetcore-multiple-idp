using CoreWCF;
using CoreWCF.IdentityModel.Selectors;

namespace web.auth.WCF
{
    public class CustomUserNamePasswordValidator : UserNamePasswordValidator
    {
        public override ValueTask ValidateAsync(string userName, string password)
        {
            bool valid = userName.ToLowerInvariant().EndsWith("valid")
                && password.ToLowerInvariant().EndsWith("valid");
            if (!valid)
            {
                throw new FaultException("Unknown Username or Incorrect Password");
            }

            return ValueTask.CompletedTask;
        }

        public static void AddToHost(ServiceHostBase host)
        {
            var srvCredentials = new CoreWCF.Description.ServiceCredentials();
            srvCredentials.UserNameAuthentication.UserNamePasswordValidationMode
                = CoreWCF.Security.UserNamePasswordValidationMode.Custom;
            srvCredentials.UserNameAuthentication.CustomUserNamePasswordValidator
                = new CustomUserNamePasswordValidator();
            host.Description.Behaviors.Add(srvCredentials);
        }

    }
}
