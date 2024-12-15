using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2024
{
    class Day14 : Core.Day
    {
        public Day14() { }

        public override string GetSolutionVersion(Core.Part part)
        {
            return part switch
            {
                Core.Part.One => "v1",
                Core.Part.Two => "v1",
                _ => base.GetSolutionVersion(part),
            };
        }

        protected override List<Core.TestDatum> GetTestData()
        {
            List<Core.TestDatum> testData =
            [
                new Core.TestDatum
                {
                    TestPart = Core.Part.One,
                    Output = "12",
                    Variables = new Dictionary<string, string> { { nameof(_TilesWide), "11" }, { nameof(_TilesTall), "7" } },
                    RawInput =
@"p=0,4 v=3,-3
p=6,3 v=-1,-3
p=10,3 v=-1,2
p=2,0 v=2,-1
p=0,0 v=1,3
p=3,0 v=-2,-2
p=7,6 v=-1,-3
p=3,0 v=-1,-2
p=9,3 v=2,3
p=7,3 v=-1,2
p=2,4 v=2,-3
p=9,5 v=-3,-3"
                },
                new Core.TestDatum
                {
                    TestPart = Core.Part.Two,
                    Output = "",
                    RawInput =
@""
                },
            ];
            return testData;
        }

#pragma warning disable IDE1006 // Naming Styles
        private static int _TilesWide { get; }
        private static int _TilesTall { get; }
#pragma warning restore IDE1006 // Naming Styles

        public static Base.Ray2 Parse(string input)
        {
            int[] split = Util.String.Split(input, "pv=, ").Where(i => int.TryParse(i, out int _)).Select(int.Parse).ToArray();
            Base.Vec2 start = new(split[0], split[1]);
            Base.Vec2 vel = new(split[2], split[3]);
            return Base.Ray2.FromVel(start, vel);
        }

        public static int GetQuadrant(int tilesWide, int tilesTall, ref Base.Vec2 pos)
        {
            int nonX = (tilesWide - 1) / 2;
            int nonY = (tilesTall - 1) / 2;

            if (pos.X == nonX || pos.Y == nonY)
            {
                return 0;
            }

            if (pos.X < nonX)
            {
                if (pos.Y < nonY)
                {
                    return 1;
                }
                else
                {
                    return 3;
                }
            }
            else
            {
                if (pos.Y < nonY)
                {
                    return 2;
                }
                else
                {
                    return 4;
                }
            }
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool findEasterEgg)
        {
            GetVariable(nameof(_TilesWide), 101, variables, out int tilesWide);
            GetVariable(nameof(_TilesTall), 103, variables, out int tilesTall);
            Base.Ray2[] robots = inputs.Select(Parse).ToArray();

            int[] quadrants = [0, 0, 0, 0, 0];
            if (!findEasterEgg)
            {
                foreach (Base.Ray2 robot in robots)
                {
                    Base.Vec2 pos = robot.Tick(100);
                    pos.Mod(tilesWide, tilesTall);
                    int quadrant = GetQuadrant(tilesWide, tilesTall, ref pos);
                    ++quadrants[quadrant];
                }

                int mult = 1;
                foreach (int q in quadrants.Skip(1))
                {
                    mult *= q;
                }
                return mult.ToString();
            }

            // cycle is 10403
            Base.Grid2Char grid;
            for (int i = 0; i < 10403; ++i)
            {
                HashSet<Base.Vec2> used = [];
                grid = new(tilesWide, tilesTall, '.');
                foreach (Base.Ray2 robot in robots)
                {
                    Base.Vec2 pos = robot.Tick(i);
                    pos.Mod(tilesWide, tilesTall);
                    grid[pos] = '#';
                    used.Add(pos);
                }

                int neighborCount = 0;
                int minNeighborCount = inputs.Count * 2 / 3;
                foreach (Base.Vec2 cur in used)
                {
                    foreach (Util.Grid2.Dir dir in Util.Grid2.Iter.Cardinal)
                    {
                        Base.Vec2 next = cur + Util.Grid2.Map.Neighbor[dir];
                        if (used.Contains(next))
                        {
                            ++neighborCount;
                            break;
                        }
                    }

                    if (neighborCount == minNeighborCount)
                    {
                        // grid.Print(Core.Log.ELevel.Spam);
                        return i.ToString();
                    }
                }
            }
            return string.Empty;
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);
    }
}