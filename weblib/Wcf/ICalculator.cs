using System.Runtime.Serialization;
using System.ServiceModel;

[ServiceContract(Name = "FancyCalc", Namespace = "urn://calc.fancy")]
public interface ICalculator
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
