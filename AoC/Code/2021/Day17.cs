using System.Text;
using System.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2021
{
    class Day17 : Day
    {
        public Day17() { }

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
                Output = "45",
                RawInput =
@"target area: x=20..30, y=-10..-5"
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

        private Dictionary<int, int> YDecay = new Dictionary<int, int>();
        private int GetYDecay(int steps)
        {
            if (steps == 0)
            {
                return 0;
            }

            if (!YDecay.ContainsKey(steps))
            {
                YDecay[steps] = steps + GetYDecay(steps - 1);
            }
            return YDecay[steps];
        }

        private int Solve(int minY, int maxY)
        {
            int maxHeight = 0;
            {
                Stack<int> last = new Stack<int>();
                bool yDone = false;
                int prevDecay = 0;
                for (int y = 0; !yDone; ++y)
                {
                    bool valid = false;
                    int curDecay = GetYDecay(y);
                    for (int decay = 1; curDecay >= minY; ++decay)
                    {
                        curDecay -= decay;
                        if (curDecay <= maxY && curDecay >= minY)
                        {
                            valid = true;
                            maxHeight = Math.Max(maxHeight, GetYDecay(y));
                            break;
                        }
                    }

                    if (!valid && curDecay == minY - 1 && prevDecay == minY)
                    {
                        yDone = true;
                    }
                    prevDecay = curDecay;
                }
            }
            return maxHeight;
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables)
        {
            string[] split = inputs.First().Split(" :=.,xy".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).TakeLast(4).ToArray();
            int[] targetArea = split.Select(int.Parse).ToArray();
            return Solve(targetArea[2], targetArea[3]).ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);
    }
}