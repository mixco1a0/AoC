using System;
using System.Collections.Generic;
using System.Linq;
using AoC.Base;

namespace AoC._2024
{
    class Day10 : Core.Day
    {
        public Day10() { }

        public override string GetSolutionVersion(Core.Part part)
        {
            return part switch
            {
                // Core.Part.One => "v1",
                // Core.Part.Two => "v1",
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
                    Output = "2",
                    RawInput =
@"...0...
...1...
...2...
6543456
7.....7
8.....8
9.....9"
                },
                new Core.TestDatum
                {
                    TestPart = Core.Part.One,
                    Output = "36",
                    RawInput =
@"89010123
78121874
87430965
96549874
45678903
32019012
01329801
10456732"
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

        private readonly char TrailStart = '0';
        private readonly char TrailEnd = '9';

        private void GetTrailheads(Base.Grid2Char grid, out HashSet<Base.Vec2> trailheads)
        {
            trailheads = [];
            foreach (Base.Vec2 vec2 in grid)
            {
                if (grid.At(vec2) == TrailStart)
                {
                    trailheads.Add(vec2);
                }
            }
        }

        private class Grid2T : Grid2<bool>;

        private int GetTrailheadScore(Base.Grid2Char grid, Base.Vec2 trailhead)
        {
            Queue<Base.Vec2> trails = [];
            trails.Enqueue(trailhead);
            HashSet<Base.Vec2> visited = [];
            HashSet<Base.Vec2> endPoints = [];
            while (trails.Count > 0)
            {
                Base.Vec2 cur = trails.Dequeue();
                visited.Add(cur);
                if (grid.At(cur) == TrailEnd)
                {
                    endPoints.Add(cur);
                    continue;
                }

                foreach (Grid2T.Dir dir in Grid2T.Iter.Cardinal)
                {
                    Base.Vec2 next = cur + Grid2T.Map.Neighbor[dir];
                    if (grid.Has(next) && !visited.Contains(next))
                    {
                        if (grid.At(next) == (grid.At(cur) + 1))
                        {
                            trails.Enqueue(next);
                        }
                    }
                }
            }
            return endPoints.Count;
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool _)
        {
            Base.Grid2Char grid = new(inputs);
            GetTrailheads(grid, out HashSet<Base.Vec2> trailheads);
            return trailheads.Select(th => GetTrailheadScore(grid, th)).Sum().ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);
    }
}