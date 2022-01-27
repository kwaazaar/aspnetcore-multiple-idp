using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Discovery;

namespace WCFSelfHost
{
    public static class Program

    {
        public static void Main()
        {

            var host = new ServiceHost(typeof(Calculator), new Uri("http://localhost:8088/basicHttpUserPassword"));

            // All kinds of metadata
            host.Description.Behaviors.Add(new ServiceDiscoveryBehavior());
            host.Description.Behaviors.Add(new ServiceMetadataBehavior { HttpGetEnabled = true });
            host.AddServiceEndpoint(
              ServiceMetadataBehavior.MexContractName,
              MetadataExchangeBindings.CreateMexHttpBinding(),
              "mex"
            );

            // Add username/password validator
            host.Credentials.UserNameAuthentication.UserNamePasswordValidationMode = System.ServiceModel.Security.UserNamePasswordValidationMode.Custom;
            host.Credentials.UserNameAuthentication.CustomUserNamePasswordValidator = new CustomUserNamePasswordValidator();

            //// WSHttp
            //var serverBindingHttpsUserPassword = new WSHttpBinding(SecurityMode.TransportWithMessageCredential); // WS-Security with credentials in message
            //serverBindingHttpsUserPassword.Security.Message.ClientCredentialType = MessageCredentialType.UserName;
            //host.AddServiceEndpoint(typeof(ICalculator), serverBindingHttpsUserPassword, new Uri($"https://localhost:8443/wsHttpUserPassword"));

            // BasicHttp
            host.AddServiceEndpoint(typeof(ICalculator), new BasicHttpBinding(BasicHttpSecurityMode.None), new Uri($"http://localhost:8088/basicHttpUserPassword"));
            //host.AddServiceEndpoint(typeof(ICalculator), new BasicHttpsBinding(BasicHttpsSecurityMode.Transport), new Uri($"https://localhost:8443/basicHttpsUserPassword"));

            host.Open();

            Console.ReadLine();
        }
    }
}
