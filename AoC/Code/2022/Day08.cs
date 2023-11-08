using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2022
{
    class Day08 : Core.Day
    {
        public Day08() { }

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

        public override bool SkipTestData => true;

        protected override List<Core.TestDatum> GetTestData()
        {
            List<Core.TestDatum> testData = new List<Core.TestDatum>();
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.One,
                Output = "21",
                RawInput =
@"30373
25512
65332
33549
35390"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "8",
                RawInput =
@"30373
25512
65332
33549
35390"
            });
            return testData;
        }

        private void FindVisibleX(ref HashSet<Base.Pos2> visiblePoints, ref List<string> grid, int start, int end, Func<int, int> iter)
        {
            int max = Math.Max(start, end);
            for (int y = 0; y < grid.Count; ++y)
            {
                int maxVisible = -1;
                for (int x = start; 0 <= x && x <= max; x = iter(x))
                {
                    int value = grid[y][x] - '0';
                    Base.Pos2 cur = new Base.Pos2(x, y);
                    if (!visiblePoints.Contains(cur))
                    {
                        if (value > maxVisible)
                        {
                            visiblePoints.Add(cur);
                        }
                    }
                    maxVisible = Math.Max(maxVisible, value);
                }
            }
        }

        private void FindVisibleY(ref HashSet<Base.Pos2> visiblePoints, ref List<string> grid, int start, int end, Func<int, int> iter)
        {
            int max = Math.Max(start, end);
            for (int x = 0; x < grid[0].Length; ++x)
            {
                int maxVisible = -1;
                for (int y = start; 0 <= y && y <= max; y = iter(y))
                {
                    int value = grid[y][x] - '0';
                    Base.Pos2 cur = new Base.Pos2(x, y);
                    if (!visiblePoints.Contains(cur))
                    {
                        if (value > maxVisible)
                        {
                            visiblePoints.Add(cur);
                        }
                    }
                    maxVisible = Math.Max(maxVisible, value);
                }
            }
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables)
        {
            HashSet<Base.Pos2> visiblePoints = new HashSet<Base.Pos2>();
            List<string> grid = new List<string>(inputs);
            FindVisibleY(ref visiblePoints, ref grid, 0, grid.Count - 1, (int i) => { return i + 1; });
            FindVisibleY(ref visiblePoints, ref grid, grid.Count - 1, 0, (int i) => { return i - 1; });
            FindVisibleX(ref visiblePoints, ref grid, 0, grid[0].Length - 1, (int i) => { return i + 1; });
            FindVisibleX(ref visiblePoints, ref grid, grid[0].Length - 1, 0, (int i) => { return i - 1; });
            return visiblePoints.Count.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);

        private int Score(List<string> grid, int x, int y, int maxX, int maxY)
        {
            if (x == 0 || y == 0 || x == maxX || y == maxY)
            {
                return 0;
            }

            int curView = grid[y][x] - '0';
            int l = 0, r = 0, t = 0, d = 0;
            for (int iterX = x - 1; iterX >= 0; --iterX)
            {
                int value = grid[y][iterX] - '0';
                ++l;
                if (value >= curView)
                {
                    break;
                }
            }
            for (int iterX = x + 1; iterX <= maxX; ++iterX)
            {
                int value = grid[y][iterX] - '0';
                ++r;
                if (value >= curView)
                {
                    break;
                }
            }
            for (int iterY = y - 1; iterY >= 0; --iterY)
            {
                int value = grid[iterY][x] - '0';
                ++t;
                if (value >= curView)
                {
                    break;
                }
            }
            for (int iterY = y + 1; iterY <= maxX; ++iterY)
            {
                int value = grid[iterY][x] - '0';
                ++d;
                if (value >= curView)
                {
                    break;
                }
            }

            return l * r * t * d;
        }

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            HashSet<Base.Pos2> visiblePoints = new HashSet<Base.Pos2>();
            List<string> grid = new List<string>(inputs);
            int maxScore = 0;
            for (int y = 0; y < grid.Count; ++y)
            {
                for (int x = 0; x < grid[0].Length; ++x)
                {
                    maxScore = Math.Max(maxScore, Score(grid, x, y, grid[0].Length - 1, grid.Count - 1));
                }
            }
            return maxScore.ToString();
        }
    }
}