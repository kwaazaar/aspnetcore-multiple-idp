using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WCFSelfHost
{
    [ServiceContract(Name = "FancyCalc", Namespace = "urn://calc.fancy")]
    internal interface ICalculator
    {
        [OperationContract]
        int Add(int a, int b);
        [OperationContract]
        int AddComplex(TwoInts ints);
    }

    [DataContract]
    public class TwoInts
    {
        [DataMember]
        public int A { get; set; }

        [DataMember]
        public int B { get; set; }
    }
}
