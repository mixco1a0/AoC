using System;
using System.Collections.Generic;
using System.Linq;

using AoC.Core;

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
                Output = "112",
                RawInput =
@"target area: x=20..30, y=-10..-5"
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

        private int Solve(int minY, int maxY, out List<int> possibleYs)
        {
            possibleYs = new List<int>();
            int maxHeight = 0;
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
                        possibleYs.Add(y);
                        break;
                    }
                }

                if (!valid && curDecay == minY - 1 && prevDecay == minY)
                {
                    yDone = true;
                }
                prevDecay = curDecay;
            }
            return maxHeight;
        }

        private void AdjustVelocity(ref Base.Point position, ref Base.Point velocity)
        {
            position += velocity;
            if (velocity.X != 0)
            {
                velocity.X -= velocity.X > 0 ? 1 : -1;
            }
            velocity.Y -= 1;
        }

        private void GetStartX(int minX, int maxX, out int startX)
        {
            for (int x = 1; ; ++x)
            {
                Base.Point probe = new Base.Point(0, 0);
                Base.Point velocity = new Base.Point(x, 0);
                while (velocity.X > 0 && probe.X <= maxX)
                {
                    AdjustVelocity(ref probe, ref velocity);
                    if (probe.X >= minX)
                    {
                        startX = x;
                        return;
                    }
                }
            }
        }

        private int SolveAll(int startX, List<int> possibleYs, int minX, int maxX, int minY, int maxY)
        {
            possibleYs.AddRange(Enumerable.Range(1, minY * -1).Select(e => e * -1));
            HashSet<Base.Point> knownLocations = new HashSet<Base.Point>();
            foreach (int y in possibleYs)
            {
                for (int x = startX; x <= maxX; ++x)
                {
                    Base.Point pos = new Base.Point(0, 0);
                    Base.Point vel = new Base.Point(x, y);
                    while (pos.X < maxX && pos.Y > minY)
                    {
                        AdjustVelocity(ref pos, ref vel);
                        if (minX <= pos.X && pos.X <= maxX && minY <= pos.Y && pos.Y <= maxY)
                        {
                            knownLocations.Add(new Base.Point(x, y));
                            break;
                        }
                    }
                }
            }
            return knownLocations.Count;
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool getMax)
        {
            string[] split = inputs.First().Split(" :=.,xy".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).TakeLast(4).ToArray();
            int[] targetArea = split.Select(int.Parse).ToArray();
            int max = Solve(targetArea[2], targetArea[3], out List<int> possibleYs);
            if (getMax)
            {
                return max.ToString();
            }

            GetStartX(targetArea[0], targetArea[1], out int startX);
            return SolveAll(startX, possibleYs, targetArea[0], targetArea[1], targetArea[2], targetArea[3]).ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);
    }
}