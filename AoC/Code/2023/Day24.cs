using System;
using System.Collections.Generic;
using System.Linq;
using AoC.Base;

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

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables)
        {
            GetVariable(nameof(_MinRange), 200_000_000_000_000, variables, out long minRange);
            GetVariable(nameof(_MaxRange), 400_000_000_000_000, variables, out long maxRange);
            List<Vec3L> hailstones = inputs.Select(Vec3L.ParseVel).ToList();
            List<Vec2L> h2d = hailstones.Select(v3 => v3.DropZ()).ToList();
            // Log("Hailstones:");
            // h2d.ForEach(l => Log(l.ToString()));
            long count = 0;
            for (int i = 0; i < h2d.Count; ++i)
            {
                Vec2L iH = h2d[i];
                for (int j = i + 1; j < h2d.Count; ++j)
                {
                    Vec2L jH = h2d[j];
                    Vec.Intersection2D type = iH.GetIntersection(jH, out Pos2L pos2);
                    switch (type)
                    {
                        case Vec.Intersection2D.Parallel:
                            // Log($"{iH} is parallel to {jH}");
                            break;
                        case Vec.Intersection2D.Overlap:
                            // Log($"{iH} overlaps {jH}");
                            ++count;
                            break;
                        case Vec.Intersection2D.SinglePoint:
                            // char inside = 'X';
                            // char whenPath = 'P';
                            if (pos2.X >= minRange && pos2.X <= maxRange && pos2.Y >= minRange && pos2.Y <= maxRange)
                            {
                                // inside = 'O';
                                Pos2L iVel = iH.Vel;
                                Pos2L iNextVel = pos2 - iH.Pos;
                                Pos2L jVel = jH.Vel;
                                Pos2L jNextVel = pos2 - jH.Pos;
                                if ((iVel.X < 0 == iNextVel.X < 0) && (iVel.Y < 0 == iNextVel.Y < 0) && (jVel.X < 0 == jNextVel.X < 0) && (jVel.Y < 0 == jNextVel.Y < 0))
                                {
                                    // whenPath = 'F';
                                    ++count;
                                }
                            }
                            // Log($"{h2d[i]} intersects {h2d[j]} at {pos2} | [{inside}.{whenPath}]");
                            break;
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