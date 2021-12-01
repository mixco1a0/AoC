using System;
using System.Collections.Generic;
using System.Linq;

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
                Output = "",
                RawInput =
@""
            });
            return testData;
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables)
        {
            int[] depths = inputs.Select(int.Parse).ToArray();
            int increases = 0;
            for (int i = 1; i < depths.Count(); ++i)
            {
                if (depths[i] > depths[i-1])
                {
                    ++increases;
                }
            }
            return increases.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);
    }
}