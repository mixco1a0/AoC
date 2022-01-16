using System.Collections.Generic;
using System.Linq;

using AoC.Core;

namespace AoC._2021
{
    class Day06 : Day
    {
        public Day06() { }

        public override string GetSolutionVersion(Part part)
        {
            switch (part)
            {
                case Part.One:
                    return "v2";
                case Part.Two:
                    return "v2";
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
                Output = "5934",
                RawInput =
@"3,4,3,1,2"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.Two,
                Output = "26984457539",
                RawInput =
@"3,4,3,1,2"
            });
            return testData;
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, int days)
        {
            List<long> fishUncrompressed = inputs.First().Split(',').Select(long.Parse).ToList();
            long[] fish = Enumerable.Repeat((long)0, 9).ToArray();
            foreach (long f in fishUncrompressed)
            {
                ++fish[f];
            }
            for (long i = 0; i < days; ++i)
            {
                long[] nextFish = Enumerable.Repeat((long)0, 9).ToArray();
                Dictionary<long, long> nextState = new Dictionary<long, long>();
                for (int f = 0; f < 9; ++f)
                {
                    if (f - 1 < 0)
                    {
                        nextFish[6] += fish[f];
                        nextFish[8] += fish[f];
                    }
                    else
                    {
                        nextFish[f - 1] += fish[f];
                    }
                }
                fish = nextFish;
            }
            return fish.Sum().ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, 80);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, 256);
    }
}