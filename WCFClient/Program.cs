
using System.ServiceModel;

var urlBasic = "http://localhost:8088/basicCalculator";
//var urlBasic = "http://localhost:5023/basicCalculator";
var urlWs = "https://localhost:8443/customHttpCalculator";
//var urlWs = "https://localhost:8443/wsHttpCalculator2";
//var urlWs = "https://localhost:7214/wsHttpCalculator2";

var basicClient = new FancyCalcClient(new BasicHttpBinding(), new EndpointAddress(urlBasic));
var result = await basicClient.AddComplexAsync(new WCFSelfHost.TwoInts { A = 1, B = 2 });
System.Console.WriteLine("Result: {0}", result);

var wsHttpBinding = new WSHttpBinding(SecurityMode.TransportWithMessageCredential);
wsHttpBinding.Security.Message.ClientCredentialType = MessageCredentialType.UserName;

var wsHttpClient = new FancyCalcClient(wsHttpBinding, new EndpointAddress(urlWs));
wsHttpClient.ClientCredentials.UserName.UserName = "bla_valid";
wsHttpClient.ClientCredentials.UserName.Password = "bla_valid";

var resultWs = await wsHttpClient.AddComplexAsync(new WCFSelfHost.TwoInts { A = 1, B = 2 });
System.Console.WriteLine("Result: {0}", resultWs);

System.Console.ReadLine();