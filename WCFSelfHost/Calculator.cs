using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCFSelfHost
{
    internal class Calculator : ICalculator
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
}
