using System;
using System.Collections.Generic;
using System.Linq;
using AoC.Base;
using AoC.Util;

namespace AoC._2023
{
    class Day24 : Core.Day
    {
        public Day24() { }

        public override string GetSolutionVersion(Core.Part part)
        {
            switch (part)
            {
                // case Core.Part.One:
                //     return "v1";
                // case Core.Part.Two:
                //     return "v1";
                default:
                    return base.GetSolutionVersion(part);
            }
        }

        public override bool SkipTestData => false;

        protected override List<Core.TestDatum> GetTestData()
        {
            List<Core.TestDatum> testData = new List<Core.TestDatum>();
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.One,
                Variables = new Dictionary<string, string> { { nameof(_MinRange), "7" }, { nameof(_MaxRange), "27" } },
                Output = "2",
                RawInput =
@"19, 13, 30 @ -2,  1, -2
18, 19, 22 @ -1, -1, -2
20, 25, 34 @ -2, -2, -4
12, 31, 28 @ -1, -2, -1
20, 19, 15 @  1, -5, -3"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "",
                RawInput =
@""
            });
            return testData;
        }

        private long _MinRange { get; }
        private long _MaxRange { get; }

        public record HailStone(Pos3 Pos, Pos3 Vel)
        {
            public static HailStone Parse(string input)
            {
                int[] split = Number.Split(input, ", @").ToArray();
                return new HailStone(new Pos3(split[0], split[1], split[2]), new Pos3(split[3], split[4], split[5]));
            }

            public override string ToString()
            {
                return $"{Pos} @ {Vel}";
            }
        }

        public record Limits(Pos3 Min, Pos3 Max)
        {
            public static Limits Convert(HailStone hailStone, long min, long max)
            {
                // need to start closest to the test area
                // if outside of test area, move it to border
                // if inside, use starting point
                // then add second point at the edge of the test area


                Pos3 start = hailStone.Pos;
                if (start.X >= max && hailStone.Vel.X > 0)
                {
                    return null;
                }
                if (start.X <= min && hailStone.Vel.X < 0)
                {
                    return null;
                }
                if (start.Y >= max && hailStone.Vel.Y > 0)
                {
                    return null;
                }
                if (start.Y <= min && hailStone.Vel.Y < 0)
                {
                    return null;
                }


                Pos3 posA = start;
                while (posA.X > min && posA.X < max && posA.Y > min && posA.Y < max)
                {
                    posA += hailStone.Vel;
                }
                Pos3 posB = start;
                while (posB.X > min && posB.X < max && posB.Y > min && posB.Y < max)
                {
                    posB -= hailStone.Vel;
                }
                return new Limits(posA, posB);
            }

            public override string ToString()
            {
                return $"{Min} => {Max}";
            }
        }

        public bool OnSegment(Pos3 a, Pos3 b, Pos3 c)
        {
            if (b.X <= Math.Max(a.X, c.X) && b.X >= Math.Min(a.X, c.X) &&
                b.Y <= Math.Max(a.Y, c.Y) && b.Y >= Math.Min(a.Y, c.Y))
            {
                return true;
            }
            return false;
        }

        public int GetOrientation(Pos3 a, Pos3 b, Pos3 c)
        {
            int val = (b.Y - a.Y) * (c.X - b.X) - (b.X - a.X) * (c.Y - b.Y);
            if (val == 0)
            {
                return 0;
            }
            return val > 0 ? 1 : 2;
        }

        public bool DoesIntersect(Limits l1, Limits l2)
        {
            int o1 = GetOrientation(l1.Min, l1.Max, l2.Min);
            int o2 = GetOrientation(l1.Min, l1.Max, l2.Max);
            int o3 = GetOrientation(l2.Min, l2.Max, l1.Min);
            int o4 = GetOrientation(l2.Min, l2.Max, l1.Max);

            if (o1 != o2 && o3 != o4)
            {
                return true;
            }

            if (o1 == 0 && OnSegment(l1.Min, l2.Min, l1.Max))
            {
                return true;
            }

            if (o2 == 0 && OnSegment(l1.Min, l2.Max, l1.Max))
            {
                return true;
            }

            if (o3 == 0 && OnSegment(l2.Min, l1.Min, l2.Max))
            {
                return true;
            }

            if (o4 == 0 && OnSegment(l2.Min, l1.Max, l2.Max))
            {
                return true;
            }

            return false;
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables)
        {
            GetVariable(nameof(_MinRange), 200_000_000_000_000, variables, out long minRange);
            GetVariable(nameof(_MaxRange), 400_000_000_000_000, variables, out long maxRange);
            List<HailStone> hailstones = inputs.Select(HailStone.Parse).ToList();
            List<Limits> limits = hailstones.Select(hs => Limits.Convert(hs, minRange, maxRange)).ToList();
            long count = 0;
            for (int i = 0; i < limits.Count; ++i)
            {
                for (int j = i + 1; j < limits.Count; ++j)
                {
                    if (DoesIntersect(limits[i], limits[j]))
                    {
                        ++count;
                    }
                }
            }
            return count.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);
    }
}