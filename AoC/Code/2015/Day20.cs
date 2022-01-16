using System.Collections.Generic;
using System.Linq;

using AoC.Core;

namespace AoC._2015
{
    class Day20 : Day
    {
        public Day20() { }
        public override string GetSolutionVersion(Part part)
        {
            switch (part)
            {
                // case Part.One:
                //     return "v1";
                // case Part.Two:
                //     return "v1";
                default:
                    return base.GetSolutionVersion(part);
            }
        }
        protected override List<TestDatum> GetTestData()
        {
            List<TestDatum> testData = new List<TestDatum>();
            return testData;
        }


        // |10:29:18.636|  [2015|day20|problem|part1|v0] 	720720 -> 32497920
        // |10:31:54.727|  [2015|day20|problem|part1|v0] 	776160 -> 33611760


        private long GetStart(long lhn)
        {
            long temp = lhn;
            for (long l = 1; ; ++l)
            {
                if (temp - l <= 0)
                {
                    return l;
                }
                temp -= l;
            }
        }

        private long Sum(long end)
        {
            long sum = 0;
            for (long i = 1; i <= end; ++i)
            {
                if (end % i == 0)
                {
                    sum += 10 * i;
                }
            }
            return sum;
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            long lhn = inputs.Select(long.Parse).First();
            long house = GetStart(lhn); house = 776160;
            long max = 0, sum = 0;
            while (true)
            {
                long now = Sum(house);
                if (now > max)
                {
                    DebugWriteLine($"{house} -> {now}");
                    max = now;
                }
                sum = now;
                if (sum >= lhn)
                {
                    return house.ToString();
                }
                ++house;
            }
        }

        private long Sum2(long end)
        {
            long sum = 0;
            for (long i = 1; i <= end; ++i)
            {
                if (end % i == 0 && end / i <= 50)
                {
                    sum += 11 * i;
                }
            }
            return sum;
        }

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            long lhn = inputs.Select(long.Parse).First();
            long house  = 776160;
            long max = 0, sum = 0;
            while (true)
            {
                long now = Sum2(house);
                if (now > max)
                {
                    DebugWriteLine($"{house} -> {now}");
                    max = now;
                }
                sum = now;
                if (sum >= lhn)
                {
                    return house.ToString();
                }
                ++house;
            }
        }
    }
}