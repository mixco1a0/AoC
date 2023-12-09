using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC.Util
{
    public static class Number
    {
        /// <summary>
        /// Call split on a string and then parse it into ints
        /// </summary>
        /// <param name="input">string of separated ints</param>
        /// <param name="seperator">characters used as seperators</param>
        /// <returns></returns>
        public static IEnumerable<int> Split(string input, string seperator)
        {
            string[] split = input.Split(seperator.ToCharArray(), StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            return split.Where(s => int.TryParse(s, out int result)).Select(int.Parse);
        }

        /// <summary>
        /// Call split on a string and then parse it into ints
        /// </summary>
        /// <param name="input">string of separated ints</param>
        /// <param name="seperator">character used as seperator</param>
        /// <returns></returns>
        public static IEnumerable<int> Split(string input, char seperator)
        {
            string[] split = input.Split(seperator, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            return split.Where(s => int.TryParse(s, out int result)).Select(int.Parse);
        }

        /// <summary>
        /// Call split on a string and then parse it into longs
        /// </summary>
        /// <param name="input">string of separated longs</param>
        /// <param name="seperator">characters used as seperators</param>
        /// <returns></returns>
        public static IEnumerable<long> SplitL(string input, string seperator)
        {
            string[] split = input.Split(seperator.ToCharArray(), StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            return split.Where(s => long.TryParse(s, out long result)).Select(long.Parse);
        }

        /// <summary>
        /// Call split on a string and then parse it into longs
        /// </summary>
        /// <param name="input">string of separated longs</param>
        /// <param name="seperator">character used as seperator</param>
        /// <returns></returns>
        public static IEnumerable<long> SplitL(string input, char seperator)
        {
            string[] split = input.Split(seperator, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            return split.Where(s => long.TryParse(s, out long result)).Select(long.Parse);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="longs"></param>
        /// <returns></returns>
        public static long LeastCommonMultiple(IEnumerable<long> longs)
        {
            Dictionary<long, int> primeFactors = new Dictionary<long, int>();
            foreach (long l in longs)
            {
                PrimeFactors(l, out Dictionary<long, int> pfs);
                foreach (long pf in pfs.Keys)
                {
                    if (primeFactors.ContainsKey(pf))
                    {
                        primeFactors[pf] = Math.Max(primeFactors[pf], pfs[pf]);
                    }
                    else
                    {
                        primeFactors[pf] = pfs[pf];
                    }
                }
            }

            long lcm = 1;
            IEnumerable<long> lcms = primeFactors.Select(pair => (long)Math.Pow(pair.Key, pair.Value));
            foreach (long l in lcms)
            {
                lcm *= l;
            }
            return lcm;
        }

        /// <summary>
        /// Get a list of prime factors for a number
        /// </summary>
        /// <param name="number">source number</param>
        /// <param name="primeFactors">all prime factors</param>
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

        /// <summary>
        /// Get a list of prime factors for a number 
        /// </summary>
        /// <param name="number">source number</param>
        /// <param name="primeFactors">prime factors and their quantities found</param>
        public static void PrimeFactors(long number, out Dictionary<long, int> primeFactors)
        {
            PrimeFactors(number, out List<long> primeFactorsList);
            primeFactors = primeFactorsList.Distinct().ToDictionary(pf => pf, pf => primeFactorsList.Where(_pf => _pf == pf).Count());
        }
    }
}