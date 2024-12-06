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

        private record DirectedLocation(Base.Vec2 Vec2, Util.Grid.Direction2D Direction2D);

        private static bool CanFindLoop(char[,] grid, int maxCol, int maxRow, Base.Vec2 startingPos, Base.Vec2 obstaclePos)
        {
            bool isInGrid(Base.Vec2 vec2)
            {
                return vec2.X >= 0 && vec2.X < maxCol && vec2.Y >= 0 && vec2.Y < maxRow;
            }
            char getAt(Base.Vec2 vec2)
            {
                return grid[vec2.X, vec2.Y];
            }

            HashSet<DirectedLocation> visited = [];
            Base.Vec2 curPos = startingPos;
            Util.Grid.Direction2D curDirection = Util.Grid.Direction2D.North;
            while (true)
            {
                DirectedLocation dl = new(curPos, curDirection);
                if (visited.Contains(dl))
                {
                    return true;
                }

                visited.Add(dl);
                Base.Vec2 nextPos = curPos + Util.Grid.Direction2DVec2Map[curDirection];
                if (!isInGrid(nextPos))
                {
                    // guard left area
                    break;
                }

                if (nextPos.Equals(obstaclePos) || getAt(nextPos) == Obstacle)
                {
                    curDirection = Util.Grid.Direction2DRotateR[curDirection];
                }
                else
                {
                    curPos = nextPos;
                }
            }
            return false;
        }

        private static string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool findOriginalPath)
        {
            Util.Grid.Parse2D(inputs, out char[,] grid, out int maxCol, out int maxRow);
            Base.Vec2 startingPos = new();
            for (int _c = 0; _c < maxCol; ++_c)
            {
                for (int _r = 0; _r < maxRow; ++_r)
                {
                    if (grid[_c, _r] == StartingPos)
                    {
                        startingPos = new(_c, _r);
                        break;
                    }
                }
            }

            bool isInGrid(Base.Vec2 vec2)
            {
                return vec2.X >= 0 && vec2.X < maxCol && vec2.Y >= 0 && vec2.Y < maxRow;
            }
            char getAt(Base.Vec2 vec2)
            {
                return grid[vec2.X, vec2.Y];
            }

            Base.Vec2 curPos = startingPos;
            HashSet<Base.Vec2> visited = [];
            Util.Grid.Direction2D curDirection = Util.Grid.Direction2D.North;
            while (true)
            {
                visited.Add(curPos);
                Base.Vec2 nextPos = curPos + Util.Grid.Direction2DVec2Map[curDirection];
                if (!isInGrid(nextPos))
                {
                    // guard left area
                    break;
                }

                if (getAt(nextPos) == Obstacle)
                {
                    curDirection = Util.Grid.Direction2DRotateR[curDirection];
                }
                else
                {
                    curPos = nextPos;
                }
            }

            if (findOriginalPath)
            {
                return visited.Count.ToString();
            }

            int timeParadoxSafetyCount = 0;
            foreach (Base.Vec2 vec2 in visited)
            {
                if (vec2.Equals(startingPos))
                {
                    continue;
                }

                if (CanFindLoop(grid, maxCol, maxRow, startingPos, vec2))
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