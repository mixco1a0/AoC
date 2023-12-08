using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC.Util
{
    public static class Number
    {
        public static void PrimeFactors(long number, out List<long> primeFactors)
        {
            primeFactors = new List<long>();
            while (number % 2 == 0)
            {
                primeFactors.Add(2);
                number /= 2;
            }

            for (int i = 3; i < Math.Sqrt(number); i = i + 2)
            {
                while (number % i == 0)
                {
                    primeFactors.Add(i);
                    number /= i;
                }
            }

            if (number > 1)
            {
                primeFactors.Add(number);
            }
        }

        public static void PrimeFactors(long number, out Dictionary<long, int> primeFactors)
        {
            PrimeFactors(number, out List<long> primeFactorsList);
            primeFactors = primeFactorsList.Distinct().ToDictionary(pf => pf, pf => primeFactorsList.Where(_pf => _pf == pf).Count());
        }
    }
}