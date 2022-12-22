using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AoC.Util
{
    public static class Grid
    {
        static public void PrintGrid(Core.Log.ELevel level, List<string> grid)
        {
            StringBuilder sb = new StringBuilder();
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

        static public void PrintGrid(Core.Log.ELevel level, List<List<char>> grid)
        {
            StringBuilder sb = new StringBuilder();
            Core.Log.WriteLine(level, $"Printing grid {grid[0].Count}x{grid.Count}:");
            for (int i = 0; i < grid.Count; ++i)
            {
                sb.Clear();
                sb.Append($"{i,4}| ");
                sb.Append(string.Join(string.Empty, grid[i]));
                Core.Log.WriteLine(level, sb.ToString());
            }
        }

        static public void PrintGrid(Core.Log.ELevel level, char[][] grid)
        {
            StringBuilder sb = new StringBuilder();
            Core.Log.WriteLine(level, $"Printing grid {grid[0].Length}x{grid.Length}:");
            for (int i = 0; i < grid.Length; ++i)
            {
                sb.Clear();
                sb.Append($"{i,4}| ");
                sb.Append(string.Join(string.Empty, grid[i]));
                Core.Log.WriteLine(level, sb.ToString());
            }
        }

        static public void PrintGrid(char[,] grid, Core.Log.ELevel level = Core.Log.ELevel.Spam)
        {
            StringBuilder sb = new StringBuilder();
            Core.Log.WriteLine(level, $"Printing grid {grid.GetLength(0)}x{grid.GetLength(1)}:");
            for (int y = 0; y < grid.GetLength(1); ++y)
            {
                sb.Clear();
                sb.Append($"{y,4}| ");
                sb.Append(string.Join(string.Empty, Enumerable.Range(0, grid.GetLength(0)).Select(x => grid[x, y])));
                Core.Log.WriteLine(level, sb.ToString());
            }
        }

        static public void RotateGrid(bool right, ref List<string> grid)
        {
            List<string> newGrid = new List<string>();
            if (right)
            {
                for (int i = 0; i < grid.Count(); ++i)
                {
                    newGrid.Add(string.Join("", grid.Select(r => r.ElementAt(i)).Reverse()));
                }
            }
            else
            {
                int gridCount = grid.Count - 1;
                for (int i = 0; i < grid.Count; ++i)
                {
                    newGrid.Add(string.Join("", grid.Select(r => r.ElementAt(gridCount - i))));
                }
            }
            grid = newGrid;
        }

        static public void FlipGrid(bool horizontal, ref List<string> grid)
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

        static public bool ProcessGrid(ref List<List<char>> grid, Func<int, int, List<List<char>>, char> ProcessIndexFunc)
        {
            List<List<char>> newGrid = new List<List<char>>();
            foreach (List<char> row in grid)
            {
                newGrid.Add(new List<char>(row));
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

        static public int ProcessIndexBorder(int x, int y, List<List<char>> grid, char match)
        {
            int borderMatch = 0;
            for (int i = x - 1; i <= x + 1; ++i)
            {
                if (i < 0 || i >= grid.Count)
                {
                    continue;
                }

                for (int j = y - 1; j <= y + 1; ++j)
                {
                    if (i == x && j == y)
                    {
                        continue;
                    }

                    if (j < 0 || j >= grid[x].Count)
                    {
                        continue;
                    }

                    if (grid[i][j] == match)
                    {
                        ++borderMatch;
                    }
                }
            }
            return borderMatch;
        }

        static public Dictionary<char, int> ProcessIndexBorder(int x, int y, List<List<char>> grid)
        {
            Dictionary<char, int> borderValues = new Dictionary<char, int>();
            for (int i = x - 1; i <= x + 1; ++i)
            {
                if (i < 0 || i >= grid.Count)
                {
                    continue;
                }

                for (int j = y - 1; j <= y + 1; ++j)
                {
                    if (i == x && j == y)
                    {
                        continue;
                    }

                    if (j < 0 || j >= grid[x].Count)
                    {
                        continue;
                    }

                    char borderValue = grid[i][j];
                    if (!borderValues.ContainsKey(borderValue))
                    {
                        borderValues[borderValue] = 1;
                    }
                    else
                    {
                        ++borderValues[borderValue];
                    }
                }
            }
            return borderValues;
        }

        static public string GetDynamicIndexKey(List<int> index)
        {
            StringBuilder sb = new StringBuilder();
            foreach (int i in index)
            {
                sb.Append(i);
                sb.Append(',');
            }
            return sb.ToString();
        }

        static public bool ProcessGrid(ref Dictionary<string, char> grid, List<Base.Range> indexRanges, Func<Dictionary<string, char>, List<int>, char> ProcessIndexFunc)
        {
            bool complete = true;

            List<int> index = indexRanges.Select(r => r.Min).ToList();
            Dictionary<string, char> newGrid = new Dictionary<string, char>();
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
                if (grid.ContainsKey(indexKey))
                {
                    complete = complete && newVal != grid[indexKey];
                }
                else
                {
                    complete = false;
                }
                newGrid[indexKey] = newVal;

                ++index[0];
            }
        }

        static public int ProcessIndexBorder(List<int> index, Dictionary<string, char> grid, char match)
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
                if (borderIndexKey != indexKey && grid.ContainsKey(borderIndexKey) && grid[borderIndexKey] == match)
                {
                    ++borderMatch;
                }

                ++borderIndex[0];
            }
        }
    }
}