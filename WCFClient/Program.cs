
using System.ServiceModel;

var calcClient = new FancyCalcClient(new BasicHttpBinding(), new EndpointAddress("http://localhost:8088/basicHttpUserPassword"));

var result = await calcClient.AddComplexAsync(new WCFSelfHost.TwoInts { A = 1, B = 2 });
System.Console.WriteLine("Result: {0}", result);
