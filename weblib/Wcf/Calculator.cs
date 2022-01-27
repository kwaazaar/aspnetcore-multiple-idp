public class Calculator : ICalculator
{
    public int Add(int a, int b)
    {
        return a + b;
    }

    public int AddComplex(TwoInts ints)
    {
        return ints.A + ints.B;
    }
}
