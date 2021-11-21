using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2016
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
            testData.Add(new TestDatum
            {
                TestPart = Part.One,
                Output = "3",
                RawInput =
@"5-8
0-2
4-7
4-5"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.Two,
                Variables = new Dictionary<string, string>() { { "minValid", "0" }, { "maxValid", "9" } },
                Output = "1",
                RawInput =
@"5-8
0-2
5-9
4-5"
            });
            return testData;
        }

        private record MinMax(long Min, long Max) { }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool findFirst)
        {
            long minValid;
            Util.GetVariable(nameof(minValid), 0, variables, out minValid);
            long maxValid;
            Util.GetVariable(nameof(maxValid), (long)uint.MaxValue, variables, out maxValid);

            List<MinMax> minMax = inputs.Select(i => { string[] split = i.Split('-', StringSplitOptions.RemoveEmptyEntries); return new MinMax(long.Parse(split[0]), long.Parse(split[1])); }).ToList();
            minMax = minMax.OrderByDescending(m => m.Max).OrderBy(m => m.Min).ToList();
            long curMin = minValid;
            long totalAllowed = 0;
            foreach (MinMax mm in minMax)
            {
                if (mm.Max < curMin)
                {
                    continue;
                }

                if (curMin < mm.Min && findFirst)
                {
                    break;
                }
                else if (curMin < mm.Min)
                {
                    totalAllowed += (mm.Min - curMin);
                }

                curMin = mm.Max + 1;
            }
            if (curMin <= maxValid)
            {
                totalAllowed += (maxValid - curMin + 1);
            }

            if (findFirst)
            {
                return curMin.ToString();
            }
            return totalAllowed.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);
    }
}