using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AoC
{
    class Util
    {
        static public IEnumerable<string> ConvertInputToList(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return new List<string>();
            }
            return input.Split('\n').Select(str => str.Trim('\r'));
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

        static public void PrintGrid(List<string> grid, Action<string> PrintFunc)
        {
            PrintFunc($"Printing grid {grid.First().Length}x{grid.Count}:");
            foreach (string row in grid)
            {
                PrintFunc(row);
            }
        }

        static public string GetLogTimeStamp()
        {
            return $"|{DateTime.Now.ToString("hh:mm:ss.fff")}| ";
        }

        private static string s_workingDirectory;
        static public string WorkingDirectory
        {
            get
            {
                if (string.IsNullOrEmpty(s_workingDirectory))
                {
                    string curDir = Directory.GetCurrentDirectory();
                    string dirRoot = Path.GetPathRoot(curDir);
                    while (true)
                    {
                        if (curDir == dirRoot)
                        {
                            break;
                        }

                        if (Path.GetFileName(curDir) == nameof(AoC))
                        {
                            break;
                        }

                        curDir = Path.GetDirectoryName(curDir);
                    }

                    if (curDir != dirRoot)
                    {
                        s_workingDirectory = curDir;
                    }
                    else
                    {
                        throw new DirectoryNotFoundException($"Unable to find base directory */{nameof(AoC)}/*");
                    }
                }
                return s_workingDirectory;
            }
        }

    }

    public class MinMax
    {
        public int Min { get; set; }
        public int Max { get; set; }
        public bool GTE_LTE(int val) { return Min <= val && val <= Max; }
        public bool GT_LTE(int val) { return Min < val && val <= Max; }
        public bool GTE_LT(int val) { return Min <= val && val < Max; }
        public bool GT_LT(int val) { return Min < val && val < Max; }
    }
}