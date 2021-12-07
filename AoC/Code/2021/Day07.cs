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
                Output = "37",
                RawInput =
@"16,1,2,0,4,2,7,1,2,14"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.Two,
                Output = "168",
                RawInput =
@"16,1,2,0,4,2,7,1,2,14"
            });
            return testData;
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool advancedFuel)
        {
            List<int> positions = inputs.First().Split(',').Select(int.Parse).ToList();
            double avg = positions.Average();
            int low = (int)avg;
            int high = (int)avg + 1;
            int min = positions.Min();
            int max = positions.Max();
            int bestFuel = int.MaxValue;
            Func<int, int, int> basic = (val1, val2) => Math.Abs(val1 - val2);
            Func<int, int, int> advanced = (val1, val2) => Enumerable.Range(1, Math.Abs(val1 - val2)).Sum();
            while (low >= min || high <= max)
            {
                int curFuel = 0;
                if (low >= min)
                {
                    positions.ForEach(p => curFuel += advancedFuel ? advanced(low, p) : basic(low, p));
                    bestFuel = Math.Min(curFuel, bestFuel);
                }

                curFuel = 0;
                if (high <= max)
                {
                    positions.ForEach(p => curFuel += advancedFuel ? advanced(high, p) : basic(high, p));
                    bestFuel = Math.Min(curFuel, bestFuel);
                }

                --low;
                ++high;
            }
            return bestFuel.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);
    }
}