using System;
using System.Collections.Generic;
using System.Linq;

using AoC.Core;

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
                    return "v4";
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
            int high = low + 1;
            int min = positions.Min();
            int max = positions.Max();
            int bestFuel = int.MaxValue;
            Func<int, int, int> basic = (val1, val2) => Math.Abs(val1 - val2);
            Dictionary<int, int> sumCache = new Dictionary<int, int>();
            Func<int, int, int> advanced = (val1, val2) =>
            {
                int diff = Math.Abs(val1 - val2);
                if (!sumCache.ContainsKey(diff))
                {
                    sumCache[diff] = Enumerable.Range(1, diff).Sum();
                }
                return sumCache[diff];
            };
            int curFuel = 0;
            Queue<int> prevLow = new Queue<int>();
            Queue<int> prevHigh = new Queue<int>();
            while (low >= min || high <= max)
            {
                if (low >= min)
                {
                    curFuel = 0;
                    positions.ForEach(p => curFuel += (advancedFuel ? advanced(low, p) : basic(low, p)));
                    if (prevLow.Count > 3)
                    {
                        bool minFound = true;
                        for (int i = 0; i < 3; ++i)
                        {
                            minFound &= prevLow.ElementAt(i + 1) > prevLow.ElementAt(i);
                        }
                        if (minFound)
                        {
                            low = min;
                        }
                        prevLow.Dequeue();
                    }
                    prevLow.Enqueue(curFuel);
                    bestFuel = Math.Min(curFuel, bestFuel);
                    --low;
                }

                if (high <= max)
                {
                    curFuel = 0;
                    positions.ForEach(p => curFuel += (advancedFuel ? advanced(high, p) : basic(high, p)));
                    if (prevHigh.Count > 3)
                    {
                        bool maxFound = true;
                        for (int i = 0; i < 3; ++i)
                        {
                            maxFound &= prevHigh.ElementAt(i + 1) > prevHigh.ElementAt(i);
                        }
                        if (maxFound)
                        {
                            high = max;
                        }
                        prevHigh.Dequeue();
                    }
                    prevHigh.Enqueue(curFuel);
                    bestFuel = Math.Min(curFuel, bestFuel);
                    ++high;
                }

            }
            return bestFuel.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);
    }
}