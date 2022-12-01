using System.Collections.Generic;
using System.Linq;

namespace AoC._2021
{
    class Day06 : Core.Day
    {
        public Day06() { }

        public override string GetSolutionVersion(Core.Part part)
        {
            switch (part)
            {
                case Core.Part.One:
                    return "v2";
                case Core.Part.Two:
                    return "v2";
                default:
                    return base.GetSolutionVersion(part);
            }
        }

        public override bool SkipTestData => true;

        protected override List<Core.TestDatum> GetTestData()
        {
            List<Core.TestDatum> testData = new List<Core.TestDatum>();
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.One,
                Output = "5934",
                RawInput =
@"3,4,3,1,2"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
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