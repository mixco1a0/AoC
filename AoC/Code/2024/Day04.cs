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
                    Output = "9",
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
            ];
            return testData;
        }

        private static readonly char[] XMAS = ['X', 'M', 'A', 'S'];
        private static readonly int MIndex = 1;
        private static readonly int AIndex = 2;
        private static readonly int SIndex = 3;

        private static char Get(char[,] grid, Base.Vec2 vec2)
        {
            return grid[vec2.X, vec2.Y];
        }

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
                foreach (var pair in Util.Grid.Direction2DVec2Map)
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
                Base.Vec2 next = cur + Util.Grid.Direction2DVec2Map[direction2D];
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

        private bool IsXMas(char[,] grid, int col, int row, int maxCol, int maxRow, Util.Grid.Direction2D direction2D)
        {
            if (grid[col, row] != XMAS[AIndex])
            {
                return false;
            }

            bool isInGrid(Base.Vec2 vec2)
            {
                return vec2.X >= 0 && vec2.X < maxCol && vec2.Y >= 0 && vec2.Y < maxRow;
            }

            bool isMas(char pre, char post)
            {
                if (pre == post)
                {
                    return false;
                }
                if (pre != XMAS[MIndex] && pre != XMAS[SIndex])
                {
                    return false;
                }
                if (post != XMAS[MIndex] && post != XMAS[SIndex])
                {
                    return false;
                }
                return true;
            }

            Base.Vec2 cur = new(col, row);
            if (direction2D == Util.Grid.Direction2D.None)
            {
                foreach (Util.Grid.Direction2D d2D in Util.Grid.IterCorners2D)
                {
                    Base.Vec2 preA = cur + Util.Grid.Direction2DVec2Map[d2D];
                    Base.Vec2 postA = cur + Util.Grid.Direction2DVec2Map[Util.Grid.Direction2DOpposite[d2D]];
                    if (isInGrid(preA) && isInGrid(postA))
                    {
                        char preAChar = Get(grid, preA);
                        char postAChar = Get(grid, postA);
                        if (isMas(preAChar, postAChar))
                        {
                            if (IsXMas(grid, col, row, maxCol, maxRow, Util.Grid.Direction2DRotateR[d2D]))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            else
            {
                Base.Vec2 preA = cur + Util.Grid.Direction2DVec2Map[direction2D];
                Base.Vec2 postA = cur + Util.Grid.Direction2DVec2Map[Util.Grid.Direction2DOpposite[direction2D]];
                if (isInGrid(preA) && isInGrid(postA))
                {
                    Util.Grid.Direction2D original = Util.Grid.Direction2DRotateL[direction2D];
                    Util.Grid.Direction2D originalOpposite = Util.Grid.Direction2DOpposite[original];
                    Base.Vec2 origPreA = cur + Util.Grid.Direction2DVec2Map[original];
                    Base.Vec2 origPostA = cur + Util.Grid.Direction2DVec2Map[originalOpposite];
                    char preAChar = Get(grid, preA);
                    char postAChar = Get(grid, postA);
                    if (isMas(preAChar, postAChar))
                    {
                        // char[,] temp = new char[3, 3];
                        // for (int _c = 0; _c < 3; ++_c)
                        // {
                        //     for (int _r = 0; _r < 3; ++_r)
                        //     {
                        //         Base.Vec2 vec2 = new(col + _c - 1, row + _r - 1);
                        //         if ((_c == 1 && _r == 1) || vec2.Equals(preA) || vec2.Equals(postA) || vec2.Equals(origPreA) || vec2.Equals(origPostA))
                        //         {
                        //             temp[_c, _r] = grid[col + _c - 1, row + _r - 1];
                        //         }
                        //         else
                        //         {
                        //             temp[_c, _r] = '.';
                        //         }
                        //     }
                        // }
                        // Core.TempLog.WriteLine($"X @ {cur}");
                        // Util.Grid.Print2D(Core.Log.ELevel.Spam, temp);
                        return true;
                    }
                }
            }

            return false;
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool x_mas)
        {
            Util.Grid.Parse2D(inputs, out char[,] grid, out int maxCol, out int maxRow);
            int xmasCount = 0;
            for (int _c = 0; _c < maxCol; ++_c)
            {
                for (int _r = 0; _r < maxRow; ++_r)
                {
                    if (!x_mas)
                    {
                        xmasCount += IsXmas(grid, _c, _r, maxCol, maxRow, 0, Util.Grid.Direction2D.None);
                    }
                    else
                    {
                        xmasCount += IsXMas(grid, _c, _r, maxCol, maxRow, Util.Grid.Direction2D.None) ? 1 : 0;
                    }
                }
            }
            return xmasCount.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);
        // 2015 => TOO HIGH
    }
}