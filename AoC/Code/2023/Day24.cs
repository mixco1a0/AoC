using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
                Output = "2",
                RawInput =
@"184964585341884, 113631924395348, 401845630841620 @ 61, 469, -390
263775277465044, 418701236136888, 52607746821705 @ 105, -170, 307
331877282121819, 365938348079363, 314507465806130 @ 46, -106, 24"
            });
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

        public record HailStone(Pos3BI Pos, Pos3BI Vel)
        {
            public static HailStone Parse(string input)
            {
                BigInteger[] split = Number.SplitBI(input, ", @").ToArray();
                return new HailStone(new Pos3BI(split[0], split[1], split[2]), new Pos3BI(split[3], split[4], split[5]));
            }

            public override string ToString()
            {
                return $"{Pos} @ {Vel}";
            }
        }

        public record Limits(Pos3BI Min, Pos3BI Max)
        {
            public static Limits Convert(HailStone hailStone, long min, long max)
            {
                Pos3BI start = hailStone.Pos;
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

                // need to start closest to the test area
                // if outside of test area, move it to border
                // if inside, use starting point
                // then add second point at the edge of the test area
                Func<BigInteger, BigInteger, BigInteger> getStartMult = (startN, velN) =>
                {
                    BigInteger mult = 0;
                    if (velN != 0)
                    {
                        if (startN - max > 0)
                        {
                            mult = (startN - max) / velN;
                        }
                        else if (min - startN > 0)
                        {
                            mult = (min - startN) / velN;
                        }
                    }
                    return mult;
                };

                BigInteger multX = getStartMult(start.X, hailStone.Vel.X);
                BigInteger multY = getStartMult(start.Y, hailStone.Vel.Y);
                BigInteger mult = Number.Max(Number.Abs(multX), Number.Abs(multY));
                start += (hailStone.Vel * mult);

                Pos3BI end = new Pos3BI(start + hailStone.Vel);
                Func<BigInteger, BigInteger, BigInteger> getEndMult = (endN, velN) =>
                {
                    BigInteger mult = 0;
                    if (velN > 0)
                    {
                        mult = (max - endN) / velN;
                    }
                    else if (velN < 0)
                    {
                        mult = (endN - min) / velN;
                    }
                    return mult;
                };
                multX = getEndMult(end.X, hailStone.Vel.X);
                multY = getEndMult(end.Y, hailStone.Vel.Y);
                mult = Number.Min(Number.Abs(multX), Number.Abs(multY));
                end += (hailStone.Vel * mult);

                // while (end.X > min && end.X < max && end.Y > min && end.Y < max)
                // {
                //     end += hailStone.Vel;
                // }
                return new Limits(start, end);
            }

            public override string ToString()
            {
                return $"{Min} => {Max}";
            }
        }

        public bool OnSegment(Pos3BI a, Pos3BI b, Pos3BI c)
        {
            if (b.X <= Number.Max(a.X, c.X) && b.X >= Number.Min(a.X, c.X) &&
                b.Y <= Number.Max(a.Y, c.Y) && b.Y >= Number.Min(a.Y, c.Y))
            {
                return true;
            }
            return false;
        }

        public long GetOrientation(Pos3BI a, Pos3BI b, Pos3BI c)
        {
            BigInteger val = (b.Y - a.Y) * (c.X - b.X) - (b.X - a.X) * (c.Y - b.Y);
            if (val == 0)
            {
                return 0;
            }
            return val > 0 ? 1 : 2;
        }

        public bool DoesIntersect(Limits l1, Limits l2)
        {
            if (l1 == null || l2 == null)
            {
                return false;
            }

            long o1 = GetOrientation(l1.Min, l1.Max, l2.Min);
            long o2 = GetOrientation(l1.Min, l1.Max, l2.Max);
            long o3 = GetOrientation(l2.Min, l2.Max, l1.Min);
            long o4 = GetOrientation(l2.Min, l2.Max, l1.Max);

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
            Log("Limits:");
            limits.Where(l => l != null).ToList().ForEach(l => Log(l.ToString()));
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
            // 12024 [TOO LOW]
            // 12065 [TOO LOW]
            // 14965 [TOO HIGH]
            // 13918 [NOT RIGHT]

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);
    }
}