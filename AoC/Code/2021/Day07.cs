using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2021
{
    class Day07 : Day
    {
        public Day07() { }

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
            testData.Add(new TestDatum
            {
                TestPart = Part.One,
                Output = "37",
                RawInput =
@"16,1,2,0,4,2,7,1,2,14"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.Two,
                Output = "168",
                RawInput =
@"16,1,2,0,4,2,7,1,2,14"
            });
            return testData;
        }

        private long GetFuelCost(long togo, long depth)
        {
            if (togo == 0)
            {
                return 0;
            }
            if (togo <= 1)
            {
                return depth;
            }
            return depth + GetFuelCost(togo - 1, depth + 1);
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool advancedFuel)
        {
            List<long> positions = inputs.First().Split(',').Select(long.Parse).ToList();
            double avg = positions.Average();
            long low = (long)avg;
            long high = (long)avg + 1;
            long min = positions.Min();
            long max = positions.Max();
            long bestFuel = long.MaxValue;
            Func<long, long, long> basic = (val1, val2) => Math.Abs(val1 - val2);
            Func<long, long, long> advanced = (val1, val2) => GetFuelCost(Math.Abs(val1 - val2), 1);
            while (low >= min || high <= max)
            {
                long curFuel = 0;
                if (low >= min)
                {
                    positions.ForEach(p => curFuel += advancedFuel ? advanced(low, p) : basic(low, p));
                    bestFuel = Math.Min(curFuel, bestFuel);
                }

                curFuel = 0;
                if (high <= max)
                {
                    positions.ForEach(p => curFuel += advancedFuel ? advanced(high, p) : basic(high, p));
                    bestFuel = Math.Min(curFuel, bestFuel);
                }

                --low;
                ++high;
            }
            return bestFuel.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);
    }
}