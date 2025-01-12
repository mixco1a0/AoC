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

        private static int IsXmas(Base.Grid2Char grid, int col, int row, int idx, Util.Grid2.Dir direction)
        {
            if (grid[col, row] != XMAS[idx])
            {
                return 0;
            }
            else if (idx == (XMAS.Length - 1))
            {
                return 1;
            }

            int sum = 0;
            Base.Vec2 cur = new(col, row);
            if (direction == Util.Grid2.Dir.None)
            {
                foreach (var pair in Util.Grid2.Map.Neighbor)
                {
                    Base.Vec2 next = cur + pair.Value;
                    if (grid.Contains(next))
                    {
                        sum += IsXmas(grid, next.X, next.Y, idx + 1, pair.Key);
                    }
                }
            }
            else
            {
                Base.Vec2 next = cur + Util.Grid2.Map.Neighbor[direction];
                if (grid.Contains(next))
                {
                    sum += IsXmas(grid, next.X, next.Y, idx + 1, direction);
                }
            }

            // if (idx == 0 && sum > 0)
            // {
            //     Util.Log.WriteLine($"{cur} -> {sum}");
            // }
            return sum;
        }

        private static bool IsXMas(Base.Grid2Char grid, int col, int row, Util.Grid2.Dir direction)
        {
            if (grid[col, row] != XMAS[AIndex])
            {
                return false;
            }

            static bool isMas(char pre, char post)
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
            if (direction == Util.Grid2.Dir.None)
            {
                foreach (Util.Grid2.Dir dir in Util.Grid2.Iter.Ordinal)
                {
                    Base.Vec2 preA = cur + Util.Grid2.Map.Neighbor[dir];
                    Base.Vec2 postA = cur + Util.Grid2.Map.Neighbor[Util.Grid2.Map.Opposite[dir]];
                    if (grid.Contains(preA) && grid.Contains(postA))
                    {
                        char preAChar = grid[preA];
                        char postAChar = grid[postA];
                        if (isMas(preAChar, postAChar))
                        {
                            if (IsXMas(grid, col, row, Util.Grid2.Map.RotateCW[dir]))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            else
            {
                Base.Vec2 preA = cur + Util.Grid2.Map.Neighbor[direction];
                Base.Vec2 postA = cur + Util.Grid2.Map.Neighbor[Util.Grid2.Map.Opposite[direction]];
                if (grid.Contains(preA) && grid.Contains(postA))
                {
                    char preAChar = grid[preA];
                    char postAChar = grid[postA];
                    if (isMas(preAChar, postAChar))
                    {
                        // Util.Grid2.Dir original = Util.Grid2.Map.RotateCCW[direction];
                        // Util.Grid2.Dir originalOpposite = Util.Grid2.Map.Opposite[original];
                        // Base.Vec2 origPreA = cur + Util.Grid2.Map.Neighbor[original];
                        // Base.Vec2 origPostA = cur + Util.Grid2.Map.Neighbor[originalOpposite];
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
                        // Util.Log.WriteLine($"X @ {cur}");
                        // Util.Grid.Print2D(Core.Log.ELevel.Spam, temp);
                        return true;
                    }
                }
            }

            return false;
        }

        private static string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool x_mas)
        {
            Base.Grid2Char grid = new(inputs);
            int xmasCount = 0;
            foreach (Base.Vec2 vec2 in grid)
            {
                if (!x_mas)
                {
                    xmasCount += IsXmas(grid, vec2.X, vec2.Y, 0, Util.Grid2.Dir.None);
                }
                else
                {
                    xmasCount += IsXMas(grid, vec2.X, vec2.Y, Util.Grid2.Dir.None) ? 1 : 0;
                }
            }
            return xmasCount.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);
    }
}