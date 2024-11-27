using System.Collections.Generic;
using System.Linq;

namespace AoC._2021
{
    class Day01 : Core.Day
    {
        public Day01() { }

        public override string GetSolutionVersion(Core.Part part)
        {
            switch (part)
            {
                case Core.Part.One:
                    return "v1";
                case Core.Part.Two:
                    return "v1";
                default:
                    return base.GetSolutionVersion(part);
            }
        }

        protected override List<Core.TestDatum> GetTestData()
        {
            List<Core.TestDatum> testData = new List<Core.TestDatum>();
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.One,
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
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
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