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
            List<int> fish = inputs.First().Split(',').Select(int.Parse).ToList();
            for (int i = 0; i < days; ++i)
            {
                int newFish = 0;
                for (int f = 0; f < fish.Count; ++f)
                {
                    --fish[f];
                    if (fish[f] < 0)
                    {
                        fish[f] = 6;
                        ++newFish;
                    }
                }
                if (newFish > 0)
                {
                    fish.AddRange(Enumerable.Repeat(8, newFish));
                }
            }
            return fish.Count.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, 80);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, 0);
    }
}