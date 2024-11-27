using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2021
{
    class Day17 : Core.Day
    {
        public Day17() { }

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

        protected override List<Core.TestDatum> GetTestData()
        {
            List<Core.TestDatum> testData = new List<Core.TestDatum>();
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.One,
                Output = "45",
                RawInput =
@"target area: x=20..30, y=-10..-5"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
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

        private void AdjustVelocity(ref Base.Vec2 position, ref Base.Vec2 velocity)
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
                Base.Vec2 probe = new Base.Vec2(0, 0);
                Base.Vec2 velocity = new Base.Vec2(x, 0);
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
            HashSet<Base.Vec2> knownLocations = new HashSet<Base.Vec2>();
            foreach (int y in possibleYs)
            {
                for (int x = startX; x <= maxX; ++x)
                {
                    Base.Vec2 pos = new Base.Vec2(0, 0);
                    Base.Vec2 vel = new Base.Vec2(x, y);
                    while (pos.X < maxX && pos.Y > minY)
                    {
                        AdjustVelocity(ref pos, ref vel);
                        if (minX <= pos.X && pos.X <= maxX && minY <= pos.Y && pos.Y <= maxY)
                        {
                            knownLocations.Add(new Base.Vec2(x, y));
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