using System.Collections.Generic;
using System.Linq;

using AoC.Core;

namespace AoC._2021
{
    class Day01 : Day
    {
        public Day01() { }

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
                Output = "7",
                RawInput =
@"199
200
208
210
200
207
240
269
260
263"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.Two,
                Output = "5",
                RawInput =
@"199
200
208
210
200
207
240
269
260
263"
            });
            return testData;
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, int windowSize)
        {
            int[] depths = inputs.Select(int.Parse).ToArray();
            int increases = 0;
            int prevSum = depths.Take(windowSize).Sum();
            for (int i = 1; i <= depths.Count() - windowSize; ++i)
            {
                int cur = depths.Skip(i).Take(windowSize).Sum();
                if (cur > prevSum)
                {
                    ++increases;
                }
                prevSum = cur;
            }
            return increases.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, 1);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, 3);
    }
}