using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using AoC.Core;

namespace AoC._2023
{
    class Day14 : Core.Day
    {
        public Day14() { }

        public override string GetSolutionVersion(Core.Part part)
        {
            switch (part)
            {
                // case Core.Part.One:
                //     return "v1";
                // case Core.Part.Two:
                //     return "v1";
                default:
                    return base.GetSolutionVersion(part);
            }
        }

        public override bool SkipTestData => false;

        protected override List<Core.TestDatum> GetTestData()
        {
            List<Core.TestDatum> testData = new List<Core.TestDatum>();
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.One,
                Output = "136",
                RawInput =
@"O....#....
O.OO#....#
.....##...
OO.#O....O
.O.....O#.
O.#..O.#.#
..O..#O..O
.......O..
#....###..
#OO..#...."
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "64",
                RawInput =
@"O....#....
O.OO#....#
.....##...
OO.#O....O
.O.....O#.
O.#..O.#.#
..O..#O..O
.......O..
#....###..
#OO..#...."
            });
            return testData;
        }

        private const char Round = 'O';
        private const char Square = '#';
        private const char Empty = '.';

        private int[,] ConvertInput(List<string> inputs)
        {
            int[,] converted = new int[inputs[0].Length, inputs.Count()];
            for (int x = 0; x < inputs[0].Length; ++x)
            {
                for (int y = 0; y < inputs.Count; ++y)
                {
                    switch (inputs[x][y])
                    {
                        case Round:
                            converted[x, y] = 1;
                            break;
                        case Square:
                            converted[x, y] = 0;
                            break;
                        case Empty:
                            converted[x, y] = -1;
                            break;
                    }
                }
            }
            return converted;
        }

        private void PrintGrid(int[,] grid)
        {
            Core.TempLog.WriteLine("Grid: ");
            for (int x = 0; x < grid.GetLength(0); ++x)
            {
                StringBuilder sb = new StringBuilder();
                for (int y = 0; y < grid.GetLength(1); ++y)
                {
                    switch (grid[x, y])
                    {
                        case 1:
                            sb.Append(Round);
                            break;
                        case 0:
                            sb.Append(Square);
                            break;
                        case -1:
                            sb.Append(Empty);
                            break;
                    }
                }
                Core.TempLog.WriteLine(sb.ToString());
            }
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, int fullCycleCount)
        {
            Util.Grid.PrintGrid(Core.Log.ELevel.Debug, inputs);
            int[,] newGrid = ConvertInput(inputs);
            PrintGrid(newGrid);


            List<string> grid = new List<string>(inputs);
            Util.Grid.RotateGrid(false, ref grid);
            int load = grid.Count();
            int totalLoad = 0;
            if (fullCycleCount == 0)
            {
                ShiftNorth(grid, grid.Count(), ref totalLoad);
            }
            else
            {
                // Core.TempLog.WriteLine("Before");
                // Util.Grid.PrintGrid(Core.Log.ELevel.Debug, inputs);
                for (int c = 0; c < fullCycleCount; ++c)
                {
                    // move north
                    grid = ShiftNorth(grid, load, ref totalLoad);
                    Util.Grid.RotateGrid(true, ref grid);

                    // move west
                    grid = ShiftNorth(grid, load, ref totalLoad);
                    Util.Grid.RotateGrid(true, ref grid);

                    // move south
                    grid = ShiftNorth(grid, load, ref totalLoad);
                    Util.Grid.RotateGrid(true, ref grid);

                    // move east
                    grid = ShiftNorth(grid, load, ref totalLoad);
                    Util.Grid.RotateGrid(true, ref grid);

                    // Core.TempLog.WriteLine($"After {c} cycle(s)");
                    Util.Grid.RotateGrid(true, ref grid);
                    // Util.Grid.PrintGrid(Core.Log.ELevel.Debug, grid);
                    // Util.Grid.RotateGrid(false, ref grid);
                }
            }
            // Util.Grid.RotateGrid(true, ref shifted);
            // Util.Grid.PrintGrid(Core.Log.ELevel.Debug, shifted);
            return totalLoad.ToString();
        }

        private List<string> ShiftNorth(List<string> grid, int load, ref int totalLoad)
        {
            totalLoad = 0;
            List<string> shifted = new List<string>();
            foreach (string g in grid)
            {
                // Core.TempLog.WriteLine($"before: {g}");
                StringBuilder sb = new StringBuilder(g);
                int emptyMove = -1;
                // int curSquare = -1;
                for (int i = 0; i < sb.Length; ++i)
                {
                    switch (sb[i])
                    {
                        case Round:
                            if (emptyMove >= 0)
                            {
                                sb[emptyMove] = Round;
                                sb[i] = Empty;
                                totalLoad += (load - emptyMove);
                                emptyMove++;
                            }
                            else
                            {
                                totalLoad += (load - i);
                            }
                            break;
                        case Square:
                            // curSquare = i;
                            emptyMove = -1;
                            break;
                        case Empty:
                            if (emptyMove == -1)
                            {
                                emptyMove = i;
                            }
                            break;
                    }
                }
                // Core.TempLog.WriteLine($"after:  {sb.ToString()}");
                shifted.Add(sb.ToString());
            }
            return shifted;
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, 0);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, 1000000000);
    }
}