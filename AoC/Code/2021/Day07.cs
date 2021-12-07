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
                Output = "",
                RawInput =
@""
            });
            return testData;
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables)
        {
            List<int> positions = inputs.First().Split(',').Select(int.Parse).ToList();
            double avg = positions.Average();
            int low = (int)avg;
            int high = (int)avg + 1;
            int min = positions.Min();
            int max = positions.Max();
            int bestFuel = int.MaxValue;
            while (low > min && high < max)
            {
                int curFuel = 0;
                positions.ForEach(p => curFuel += Math.Abs(low - p));
                bestFuel = Math.Min(curFuel, bestFuel);

                curFuel = 0;
                positions.ForEach(p => curFuel += Math.Abs(high - p));
                bestFuel = Math.Min(curFuel, bestFuel);

                --low;
                ++high;
            }
            return bestFuel.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);
    }
}