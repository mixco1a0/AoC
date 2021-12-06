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
                case Part.One:
                    return "v1";
                case Part.Two:
                    return "v1";
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
            Dictionary<long, long> fishCounts = new Dictionary<long, long>();
            foreach (long f in fishUncrompressed)
            {
                if (!fishCounts.ContainsKey(f))
                {
                    fishCounts[f] = 0;
                }
                ++fishCounts[f];
            }

            Action<Dictionary<long, long>, long, long> UpdateCounts = (dictionary, key, value) =>
            {
                if (!dictionary.ContainsKey(key))
                {
                    dictionary[key] = 0;
                }
                dictionary[key] += value;
            };
            for (long i = 0; i < days; ++i)
            {
                Dictionary<long, long> nextState = new Dictionary<long, long>();
                foreach (KeyValuePair<long, long> pair in fishCounts)
                {
                    long nextKey = pair.Key - 1;
                    if (pair.Key - 1 < 0)
                    {
                        UpdateCounts(nextState, 8, pair.Value);
                        UpdateCounts(nextState, 6, pair.Value);
                    }
                    else
                    {
                        UpdateCounts(nextState, nextKey, pair.Value);
                    }
                }
                fishCounts = nextState;
            }
            return fishCounts.Select(p => p.Value).Sum().ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, 80);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, 256);
    }
}