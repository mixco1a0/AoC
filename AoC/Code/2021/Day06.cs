using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2021
{
    class Day06 : Day
    {
        public Day06() { }

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
                Output = "5934",
                RawInput =
@"3,4,3,1,2"
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

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, int days)
        {
            List<long> fish = inputs.First().Split(',').Select(long.Parse).ToList();
            Dictionary<long, long> fishCounts = new Dictionary<long, long>();
            foreach (long f in fish)
            {
                if (!fishCounts.ContainsKey(f))
                {
                    fishCounts[f] = 0;
                }
                ++fishCounts[f];
            }
            for (long i = 0; i < days; ++i)
            {
                Dictionary<long, long> nextState = new Dictionary<long, long>();
                // int newFish = 0;
                foreach (KeyValuePair<long, long> pair in fishCounts)
                {
                    long nextKey = pair.Key - 1;
                    if (pair.Key - 1 < 0)
                    {
                        if (!nextState.ContainsKey(8))
                        {
                            nextState[8] = 0;
                        }
                        nextState[8] += pair.Value;

                        if (!nextState.ContainsKey(6))
                        {
                            nextState[6] = 0;
                        }
                        nextState[6] += pair.Value;
                    }
                    else
                    {
                        if (!nextState.ContainsKey(nextKey))
                        {
                            nextState[nextKey] = 0;
                        }
                        nextState[nextKey] += pair.Value;
                    }
                }
                fishCounts = nextState;
                // for (int f = 0; f < fish.Count; ++f)
                // {
                //     --fish[f];
                //     if (fish[f] < 0)
                //     {
                //         fish[f] = 6;
                //         ++newFish;
                //     }
                // }
                // if (newFish > 0)
                // {
                //     fish.AddRange(Enumerable.Repeat(8, newFish));
                // }
            }
            return fishCounts.Select(p => p.Value).Sum().ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, 80);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, 256);
    }
}