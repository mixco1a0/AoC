using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2024
{
    class Day06 : Core.Day
    {
        public Day06() { }

        public override string GetSolutionVersion(Core.Part part)
        {
            return part switch
            {
                Core.Part.One => "v2",
                Core.Part.Two => "v2",
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
                    Output = "41",
                    RawInput =
@"....#.....
.........#
..........
..#.......
.......#..
..........
.#..^.....
........#.
#.........
......#..."
                },
                new Core.TestDatum
                {
                    TestPart = Core.Part.Two,
                    Output = "6",
                    RawInput =
@"....#.....
.........#
..........
..#.......
.......#..
..........
.#..^.....
........#.
#.........
......#..."
                },
            ];
            return testData;
        }

        private static readonly char StartingPos = '^';
        private static readonly char Obstacle = '#';

        private record DirectedLocation(Base.Vec2 Vec2, Util.Grid2.Dir Dir);

        private static bool WalkLoop(Base.Grid2Char grid, Base.Vec2 startingPos, Base.Vec2 obstaclePos, bool useDirection, out HashSet<DirectedLocation> visited)
        {
            visited = [];
            Base.Vec2 curPos = startingPos;
            Util.Grid2.Dir curDirection = Util.Grid2.Dir.North;
            while (true)
            {
                DirectedLocation dl = new(curPos, useDirection ? curDirection : Util.Grid2.Dir.None);
                if (dl.Dir != Util.Grid2.Dir.None && visited.Contains(dl))
                {
                    return true;
                }

                visited.Add(dl);
                Base.Vec2 nextPos = curPos + Util.Grid2.Map.Neighbor[curDirection];
                if (!grid.Contains(nextPos))
                {
                    return false;
                }

                if (grid[nextPos] == Obstacle || nextPos.Equals(obstaclePos))
                {
                    curDirection = Util.Grid2.Map.RotateCW[curDirection];
                }
                else
                {
                    curPos = nextPos;
                }
            }
        }

        private static string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool findOriginalPath)
        {
            Base.Grid2Char grid = new(inputs);
            Base.Vec2 startingPos = new();
            foreach (Base.Vec2 vec2 in grid)
            {
                if (grid[vec2] == StartingPos)
                {
                    startingPos = new(vec2.X, vec2.Y);
                    break;
                }
            }

            WalkLoop(grid, startingPos, new(-1, -1), false, out HashSet<DirectedLocation> visited);
            if (findOriginalPath)
            {
                return visited.Count.ToString();
            }

            int timeParadoxSafetyCount = 0;
            foreach (DirectedLocation visit in visited)
            {
                if (visit.Vec2.Equals(startingPos))
                {
                    continue;
                }

                if (WalkLoop(grid, startingPos, visit.Vec2, true, out HashSet<DirectedLocation> _))
                {
                    ++timeParadoxSafetyCount;
                }
            }

            return timeParadoxSafetyCount.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);
    }
}