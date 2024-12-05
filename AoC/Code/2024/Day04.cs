using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2024
{
    class Day04 : Core.Day
    {
        public Day04() { }

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
                    Output = "18",
                    RawInput =
@"MMMSXXMASM
MSAMXMSMSA
AMXSXMAAMM
MSAMASMSMX
XMASAMXAMM
XXAMMXXAMA
SMSMSASXSS
SAXAMASAAA
MAMMMXMMMM
MXMXAXMASX"
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

        private static readonly char[] XMAS = ['X', 'M', 'A', 'S'];

        private int IsXmas(char[,] grid, int col, int row, int maxCol, int maxRow, int idx, Util.Grid.Direction2D direction2D)
        {
            if (grid[col, row] != XMAS[idx])
            {
                return 0;
            }
            else if (idx == (XMAS.Length - 1))
            {
                return 1;
            }

            bool isInGrid(Base.Vec2 vec2)
            {
                return vec2.X >= 0 && vec2.X < maxCol && vec2.Y >= 0 && vec2.Y < maxRow;
            }

            int sum = 0;
            Base.Vec2 cur = new(col, row);
            if (direction2D == Util.Grid.Direction2D.None)
            {
                foreach (var pair in Util.Grid.DirectionVec2DMap)
                {
                    Base.Vec2 next = cur + pair.Value;
                    if (isInGrid(next))
                    {
                        sum += IsXmas(grid, next.X, next.Y, maxCol, maxRow, idx + 1, pair.Key);
                    }
                }
            }
            else
            {
                Base.Vec2 next = cur + Util.Grid.DirectionVec2DMap[direction2D];
                if (isInGrid(next))
                {
                    sum += IsXmas(grid, next.X, next.Y, maxCol, maxRow, idx + 1, direction2D);
                }
            }

            // if (idx == 0 && sum > 0)
            // {
            //     Core.TempLog.WriteLine($"{cur} -> {sum}");
            // }
            return sum;
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables)
        {
            Util.Grid.Parse2D(inputs, out char[,] grid, out int maxCol, out int maxRow);
            int xmasCount = 0;
            for (int _c = 0; _c < maxCol; ++_c)
            {
                for (int _r = 0; _r < maxRow; ++_r)
                {
                    xmasCount += IsXmas(grid, _c, _r, maxCol, maxRow, 0, Util.Grid.Direction2D.None);
                }
            }
            return xmasCount.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);
    }
}