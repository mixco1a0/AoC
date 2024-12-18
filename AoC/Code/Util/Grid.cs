using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AoC.Util
{
    public static class Grid2
    {
        #region Direction
        public enum Dir { North, NorthEast, East, SouthEast, South, SouthWest, West, NorthWest, None }

        public static class Map
        {
            /// <summary>
            /// NorthWest (-1, -1) | North     ( 0, -1) | NorthEast ( 1, -1)
            /// West      (-1,  0) | None      ( 0,  0) | East      ( 1,  0)
            /// SouthWest (-1,  1) | South     ( 0,  1) | SouthEast ( 1,  1)
            /// </summary>
            public static readonly Dictionary<Dir, Base.Vec2> Neighbor = new()
            {
                { Dir.North,        new Base.Vec2( 0, -1) },
                { Dir.NorthEast,    new Base.Vec2( 1, -1) },
                { Dir.East,         new Base.Vec2( 1,  0) },
                { Dir.SouthEast,    new Base.Vec2( 1,  1) },
                { Dir.South,        new Base.Vec2( 0,  1) },
                { Dir.SouthWest,    new Base.Vec2(-1,  1) },
                { Dir.West,         new Base.Vec2(-1,  0) },
                { Dir.NorthWest,    new Base.Vec2(-1, -1) },
                { Dir.None,         new Base.Vec2( 0,  0) },
            };

            public static readonly Dictionary<Dir, char> Arrow = new()
            {
                { Dir.North,        '↑' },
                { Dir.NorthEast,    '↗' },
                { Dir.East,         '→' },
                { Dir.SouthEast,    '↘' },
                { Dir.South,        '↓' },
                { Dir.SouthWest,    '↙' },
                { Dir.West,         '←' },
                { Dir.NorthWest,    '↖' },
                { Dir.None,         '.' },
            };

            public static readonly Dictionary<Dir, char> SimpleArrow = new()
            {
                { Dir.North,        '^' },
                // { Dir.NorthEast,    '.' },
                { Dir.East,         '>' },
                // { Dir.SouthEast,    '.' },
                { Dir.South,        'v' },
                // { Dir.SouthWest,    '.' },
                { Dir.West,         '<' },
                // { Dir.NorthWest,    '.' },
                // { Dir.None,         '.' },
            };

            public static readonly Dictionary<char, Dir> SimpleArrowFlipped = new()
            {
                { '^', Dir.North},
                // { '.', Dir.NorthEast },
                { '>', Dir.East },
                // { '.', Dir.SouthEast },
                { 'v', Dir.South },
                // { '.', Dir.SouthWest },
                { '<', Dir.West  },
                // { '.', Dir.NorthWest },
                // { '.', Dir.None },
            };

            public static readonly Dictionary<Dir, Dir> Opposite = new()
            {
                { Dir.North,        Dir.South     },
                { Dir.NorthEast,    Dir.SouthWest },
                { Dir.East,         Dir.West      },
                { Dir.SouthEast,    Dir.NorthWest },
                { Dir.South,        Dir.North     },
                { Dir.SouthWest,    Dir.NorthEast },
                { Dir.West,         Dir.East      },
                { Dir.NorthWest,    Dir.SouthEast },
                { Dir.None,         Dir.None      },
            };

            public static readonly Dictionary<Dir, Dir> RotateCW = new()
            {
                { Dir.North,        Dir.East      },
                { Dir.NorthEast,    Dir.SouthEast },
                { Dir.East,         Dir.South     },
                { Dir.SouthEast,    Dir.SouthWest },
                { Dir.South,        Dir.West      },
                { Dir.SouthWest,    Dir.NorthWest },
                { Dir.West,         Dir.North     },
                { Dir.NorthWest,    Dir.NorthEast },
                { Dir.None,         Dir.None      },
            };

            public static readonly Dictionary<Dir, Dir> RotateCCW = new()
            {
                { Dir.North,        Dir.West      },
                { Dir.NorthEast,    Dir.NorthWest },
                { Dir.East,         Dir.North     },
                { Dir.SouthEast,    Dir.NorthEast },
                { Dir.South,        Dir.East      },
                { Dir.SouthWest,    Dir.SouthEast },
                { Dir.West,         Dir.South     },
                { Dir.NorthWest,    Dir.SouthWest },
                { Dir.None,         Dir.None      },
            };
        };

        public static class Iter
        {
            public static readonly Dir[] All =
            [
                Dir.North,
                Dir.NorthEast,
                Dir.East,
                Dir.SouthEast,
                Dir.South,
                Dir.SouthWest,
                Dir.West,
                Dir.NorthWest
            ];

            public static readonly Dir[] Cardinal =
            [
                Dir.North,
                Dir.East,
                Dir.South,
                Dir.West
            ];

            public static readonly Dir[] Ordinal =
            [
                Dir.NorthEast,
                Dir.SouthEast,
                Dir.SouthWest,
                Dir.NorthWest
            ];
        };
        #endregion
    }

    public static class Grid
    {
        #region Print 2D
        public static void Print2D(Core.Log.ELevel level, List<string> grid)
        {
            StringBuilder sb = new();
            Core.Log.WriteLine(level, $"Printing grid {grid.First().Length}x{grid.Count}:");
            int idx = 0;
            foreach (string row in grid)
            {
                sb.Clear();
                sb.Append($"{idx++,4}| ");
                sb.Append(row);
                Core.Log.WriteLine(level, sb.ToString());
            }
        }

        public static void Print2D(Core.Log.ELevel level, List<List<char>> grid)
        {
            StringBuilder sb = new();
            Core.Log.WriteLine(level, $"Printing grid {grid[0].Count}x{grid.Count}:");
            for (int row = 0; row < grid.Count; ++row)
            {
                sb.Clear();
                sb.Append($"{row,4}| ");
                sb.Append(string.Join(string.Empty, grid[row]));
                Core.Log.WriteLine(level, sb.ToString());
            }
        }

        public static void Print2D(Core.Log.ELevel level, char[][] grid)
        {
            StringBuilder sb = new();
            Core.Log.WriteLine(level, $"Printing grid {grid[0].Length}x{grid.Length}:");
            for (int row = 0; row < grid.Length; ++row)
            {
                sb.Clear();
                sb.Append($"{row,4}| ");
                sb.Append(string.Join(string.Empty, grid[row]));
                Core.Log.WriteLine(level, sb.ToString());
            }
        }

        public static void Print2D(Core.Log.ELevel level, char[,] grid)
        {
            StringBuilder sb = new();
            Core.Log.WriteLine(level, $"Printing grid {grid.GetLength(0)}x{grid.GetLength(1)}:");
            for (int row = 0; row < grid.GetLength(1); ++row)
            {
                sb.Clear();
                sb.Append($"{row,4}| ");
                sb.Append(string.Join(string.Empty, Enumerable.Range(0, grid.GetLength(0)).Select(col => grid[col, row])));
                Core.Log.WriteLine(level, sb.ToString());
            }
        }
        #endregion

        #region Parse 2D
        public static void Parse2D(List<string> inputs, out char[][] grid, out int maxCol, out int maxRow)
        {
            maxCol = inputs[0].Length;
            maxRow = inputs.Count;

            grid = new char[maxRow][];
            for (int row = 0; row < maxRow; ++row)
            {
                grid[row] = new char[maxCol];
                for (int col = 0; col < maxCol; ++col)
                {
                    grid[row][col] = inputs[row][col];
                }
            }
        }

        public static void Parse2D(List<string> inputs, out char[,] grid, out int maxCol, out int maxRow)
        {
            maxCol = inputs[0].Length;
            maxRow = inputs.Count;

            grid = new char[maxCol, maxRow];
            for (int col = 0; col < maxCol; ++col)
            {
                for (int row = 0; row < maxRow; ++row)
                {
                    grid[col, row] = inputs[row][col];
                }
            }
        }
        #endregion

        #region Modify 2D
        public static void Rotate2D(bool right, ref List<string> grid)
        {
            List<string> newGrid = [];
            if (right)
            {
                for (int i = 0; i < grid[0].Length; ++i)
                {
                    newGrid.Add(string.Join("", grid.Select(r => r.ElementAt(i)).Reverse()));
                }
            }
            else
            {
                for (int i = grid[0].Length - 1; i >= 0; --i)
                {
                    newGrid.Add(string.Join("", grid.Select(r => r.ElementAt(i))));
                }
            }
            grid = newGrid;
        }

        public static void Flip2D(bool horizontal, ref List<string> grid)
        {
            if (horizontal)
            {
                for (int i = 0; i < grid.Count; ++i)
                {
                    grid[i] = string.Join("", grid[i].Reverse());
                }
            }
            else
            {
                grid.Reverse();
            }
        }
        #endregion

        #region Process 2D
        public static bool ProcessGrid(ref List<List<char>> grid, Func<int, int, List<List<char>>, char> ProcessIndexFunc)
        {
            List<List<char>> newGrid = [];
            foreach (List<char> row in grid)
            {
                newGrid.Add([.. row]);
            }
            bool complete = true;
            for (int x = 0; x < grid.Count; ++x)
            {
                for (int y = 0; y < grid[x].Count; ++y)
                {
                    newGrid[x][y] = ProcessIndexFunc(x, y, grid);
                    complete = complete && grid[x][y] == newGrid[x][y];
                }
            }
            if (!complete)
            {
                grid = newGrid;
            }
            return !complete;
        }

        public static int ProcessIndexBorder(int x, int y, List<List<char>> grid, char match)
        {
            int borderMatch = 0;
            for (int _x = x - 1; _x <= x + 1; ++_x)
            {
                if (_x < 0 || _x >= grid.Count)
                {
                    continue;
                }

                for (int _y = y - 1; _y <= y + 1; ++_y)
                {
                    if (_x == x && _y == y)
                    {
                        continue;
                    }

                    if (_y < 0 || _y >= grid[x].Count)
                    {
                        continue;
                    }

                    if (grid[_x][_y] == match)
                    {
                        ++borderMatch;
                    }
                }
            }
            return borderMatch;
        }

        public static Dictionary<char, int> ProcessIndexBorder(int x, int y, List<List<char>> grid)
        {
            Dictionary<char, int> borderValues = [];
            for (int _x = x - 1; _x <= x + 1; ++_x)
            {
                if (_x < 0 || _x >= grid.Count)
                {
                    continue;
                }

                for (int _y = y - 1; _y <= y + 1; ++_y)
                {
                    if (_x == x && _y == y)
                    {
                        continue;
                    }

                    if (_y < 0 || _y >= grid[x].Count)
                    {
                        continue;
                    }

                    char borderValue = grid[_x][_y];
                    if (!borderValues.TryGetValue(borderValue, out int value))
                    {
                        borderValues[borderValue] = 1;
                    }
                    else
                    {
                        borderValues[borderValue] = ++value;
                    }
                }
            }
            return borderValues;
        }

        public static string GetDynamicIndexKey(List<int> index)
        {
            StringBuilder sb = new();
            foreach (int i in index)
            {
                sb.Append(i);
                sb.Append(',');
            }
            return sb.ToString();
        }

        public static bool ProcessGrid(ref Dictionary<string, char> grid, List<Base.Range> indexRanges, Func<Dictionary<string, char>, List<int>, char> ProcessIndexFunc)
        {
            bool complete = true;

            List<int> index = indexRanges.Select(r => r.Min).ToList();
            Dictionary<string, char> newGrid = [];
            while (true)
            {
                for (int i = 0; i < indexRanges.Count && index[i] > indexRanges[i].Max;)
                {
                    index[i] = indexRanges[i].Min;
                    if (++i < indexRanges.Count)
                    {
                        ++index[i];
                    }
                    else
                    {
                        grid = newGrid;
                        return !complete;
                    }
                }

                char newVal = ProcessIndexFunc(grid, index);
                string indexKey = GetDynamicIndexKey(index);
                if (grid.TryGetValue(indexKey, out char value))
                {
                    complete = complete && newVal != value;
                }
                else
                {
                    complete = false;
                }
                newGrid[indexKey] = newVal;

                ++index[0];
            }
        }

        public static int ProcessIndexBorder(List<int> index, Dictionary<string, char> grid, char match)
        {
            int borderMatch = 0;
            string indexKey = GetDynamicIndexKey(index);
            List<Base.Range> indexRanges = index.Select(i => new Base.Range(i - 1, i + 1)).ToList();
            List<int> borderIndex = indexRanges.Select(r => r.Min).ToList();
            while (true)
            {
                for (int i = 0; i < indexRanges.Count && borderIndex[i] > indexRanges[i].Max;)
                {
                    borderIndex[i] = indexRanges[i].Min;
                    if (++i < indexRanges.Count)
                    {
                        ++borderIndex[i];
                    }
                    else
                    {
                        return borderMatch;
                    }
                }

                string borderIndexKey = GetDynamicIndexKey(borderIndex);
                if (borderIndexKey != indexKey && grid.TryGetValue(borderIndexKey, out char value) && value == match)
                {
                    ++borderMatch;
                }

                ++borderIndex[0];
            }
        }
        #endregion
    }
}