using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Discovery;

namespace WCFSelfHost
{
    public static class Program

    {
        public static void Main()
        {

            var host = new ServiceHost(typeof(Calculator), 
                new Uri("http://localhost:8088/basicCalculator"),
                //new Uri("https://localhost:8443/wsHttpCalculator2")
                new Uri("https://localhost:8443/customHttpCalculator")
            );

            // All kinds of metadata
            host.Description.Behaviors.Add(new ServiceDiscoveryBehavior());
            host.Description.Behaviors.Add(new ServiceMetadataBehavior { HttpGetEnabled = true, HttpsGetEnabled = true });
            host.AddServiceEndpoint(
              ServiceMetadataBehavior.MexContractName,
              MetadataExchangeBindings.CreateMexHttpBinding(),
              "mex"
            );

            // Add username/password validator
            host.Credentials.UserNameAuthentication.UserNamePasswordValidationMode = System.ServiceModel.Security.UserNamePasswordValidationMode.Custom;
            host.Credentials.UserNameAuthentication.CustomUserNamePasswordValidator = new CustomUserNamePasswordValidator();

            //// WSHttp
            var serverBindingHttpsUserPassword = new WSHttpBinding(SecurityMode.TransportWithMessageCredential); // WS-Security with credentials in message
            serverBindingHttpsUserPassword.Security.Message.ClientCredentialType = MessageCredentialType.UserName;
            var seWsHttp = host.AddServiceEndpoint(typeof(ICalculator), serverBindingHttpsUserPassword, new Uri($"https://localhost:8443/wsHttpCalculator2"));

            // BasicHttp
            var seBasic = host.AddServiceEndpoint(typeof(ICalculator), new BasicHttpBinding(BasicHttpSecurityMode.None), new Uri($"http://localhost:8088/basicCalculator"));

            var customBinding = new CustomBinding();
            var sbe = SecurityBindingElement.CreateUserNameOverTransportBindingElement();
            sbe.MessageSecurityVersion = MessageSecurityVersion.WSSecurity10WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11BasicSecurityProfile10;
            sbe.SecurityHeaderLayout = SecurityHeaderLayout.Strict;
            sbe.IncludeTimestamp = false;
            customBinding.Elements.Add(sbe);
            //customBinding.Elements.Add(new TextMessageEncodingBindingElement(MessageVersion.Soap12WSAddressing10, System.Text.Encoding.UTF8));
            customBinding.Elements.Add(new HttpsTransportBindingElement() { MaxBufferSize = 200000000, ManualAddressing = false, MaxReceivedMessageSize = 200000000 });

            // Register cert: netsh http add sslcert ipport=0.0.0.0:8443 certhash=9308a6bb0984c70e7c25b50422ab6317b31c966c appid="{1d7985f5-e1b0-4fd4-bdeb-7e28590783da}" certstorename=MY
            var seCustom = host.AddServiceEndpoint(typeof(ICalculator), customBinding, new Uri($"https://localhost:8443/customHttpCalculator"));
            seCustom.EndpointBehaviors.Add(new LoyaltyManagementSynxisEndpointBehavior());

            host.Open();


            Console.ReadLine();
        }
    }
}
