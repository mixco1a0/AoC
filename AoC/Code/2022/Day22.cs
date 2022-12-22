using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2022
{
    class Day22 : Core.Day
    {
        public Day22() { }

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
                Output = "6032",
                RawInput =
@"        ...#
        .#..
        #...
        ....
...#.......#
........#...
..#....#....
..........#.
        ...#....
        .....#..
        .#......
        ......#.

10R5L5R10L4R5L5"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "",
                RawInput =
@""
            });
            return testData;
        }

        public static char NoneChar { get { return ' '; } }
        public static char WallChar { get { return '#'; } }
        public static char PathChar { get { return '.'; } }
        public static char WalkChar { get { return '@'; } }

        private void Parse(List<string> inputs, out char[,] grid, out int maxX, out int maxY, out List<string> instructions)
        {
            maxY = inputs.Count - 2;
            maxX = inputs.Select(i => i.Length).Max();
            grid = new char[maxX, maxY];
            for (int y = 0; y < maxY; ++y)
            {
                for (int x = 0; x < maxX; ++x)
                {
                    if (x >= inputs[y].Length)
                    {
                        grid[x, y] = NoneChar;
                    }
                    else
                    {
                        grid[x, y] = inputs[y][x];
                    }
                }
            }

            instructions = new List<string>();
            string completeInstruction = inputs.Last();
            string[] numbers = completeInstruction.Split("LR".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            string[] directions = completeInstruction.Split("0123456789".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < numbers.Length || i < directions.Length; ++i)
            {
                if (i < numbers.Length)
                {
                    instructions.Add(numbers[i]);
                }
                if (i < directions.Length)
                {
                    instructions.Add(directions[i]);
                }
            }
        }

        private class GridState
        {
            public int X { get; set; }
            public int Y { get; set; }

            // 0 -> R
            // 1 -> U
            // 2 -> L
            // 3 -> D
            public int Direction { get; set; }
            private int MaxDirection { get { return 4; } }

            public GridState()
            {
                X = 0;
                Y = 0;
                Direction = 0;
            }

            public void Move(char[,] grid, int maxX, int maxY, string instruction)
            {
                if (int.TryParse(instruction, out int steps))
                {
                    int direction = 1;
                    if (Direction == 3 || Direction == 2)
                    {
                        direction = -1;
                    }

                    if (Direction == 0 || Direction == 2)
                    {
                        // side ways
                        for (int x = 0; x < steps; ++x)
                        {
                            int curX = X + direction;
                            if (curX < 0 || curX >= maxX || grid[curX, Y] == NoneChar)
                            {
                                // wrap around
                                if (curX < 0)
                                {
                                    curX = maxX - 1;
                                }
                                else if (curX >= maxX)
                                {
                                    curX = 0;
                                }

                                while (grid[curX, Y] == NoneChar)
                                {
                                    curX += direction;
                                    if (curX < 0)
                                    {
                                        curX = maxX - 1;
                                    }
                                    else if (curX >= maxX)
                                    {
                                        curX = 0;
                                    }
                                }
                                if (grid[curX, Y] == WallChar)
                                {
                                    break;
                                }
                                X = curX;
                            }
                            else if (grid[curX, Y] == WallChar)
                            {
                                break;
                            }
                            else if (grid[curX, Y] == PathChar)
                            {
                                X = curX;
                            }
                        }
                    }
                    else
                    {
                        // up down
                        for (int y = 0; y < steps; ++y)
                        {
                            int curY = Y + direction;
                            if (curY < 0 || curY >= maxY || grid[X, curY] == NoneChar)
                            {
                                // wrap around
                                if (curY < 0)
                                {
                                    curY = maxY - 1;
                                }
                                else if (curY >= maxY)
                                {
                                    curY = 0;
                                }

                                while (grid[X, curY] == NoneChar)
                                {
                                    curY += direction;
                                    if (curY < 0)
                                    {
                                        curY = maxY - 1;
                                    }
                                    else if (curY >= maxY)
                                    {
                                        curY = 0;
                                    }
                                }
                                if (grid[X, curY] == WallChar)
                                {
                                    break;
                                }
                                Y = curY;
                            }
                            else if (grid[X, curY] == WallChar)
                            {
                                break;
                            }
                            else if (grid[X, curY] == PathChar)
                            {
                                Y = curY;
                            }
                        }
                    }
                }
                else
                {
                    if (instruction[0] == 'R')
                    {
                        Direction = (Direction + 1) % MaxDirection;
                    }
                    else
                    {
                        Direction = (Direction + MaxDirection - 1) % MaxDirection;
                    }
                }
            }

            public int GetPassword()
            {
                return ((Y + 1) * 1000) + ((X + 1) * 4) + Direction;
            }
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables)
        {
            Parse(inputs, out char[,] grid, out int maxX, out int maxY, out List<string> instructions);
            char[,] printGrid = new char[maxX, maxY];
            for (int x = 0; x < maxX; ++x)
            {
                for (int y = 0; y < maxY; ++y)
                {
                    printGrid[x, y] = grid[x, y];
                }
            }

            GridState gridState = new GridState();
            gridState.X = inputs.First().IndexOfAny(new char[] { '.', '#' });
            printGrid[gridState.X, 0] = WalkChar;
            // Util.Grid.PrintGrid(printGrid, Core.Log.ELevel.Debug);
            foreach (string instruction in instructions)
            {
                if (char.IsDigit(instruction[0]))
                {
                    switch (gridState.Direction)
                    {
                        case 0:
                            printGrid[gridState.X, gridState.Y] = '>';
                            break;
                        case 1:
                            printGrid[gridState.X, gridState.Y] = 'V';
                            break;
                        case 2:
                            printGrid[gridState.X, gridState.Y] = '<';
                            break;
                        case 3:
                            printGrid[gridState.X, gridState.Y] = '^';
                            break;
                    }
                }
                // DebugWriteLine($"Running... {instruction}");
                gridState.Move(grid, maxX, maxY, instruction);
                printGrid[gridState.X, gridState.Y] = WalkChar;
                // Util.Grid.PrintGrid(printGrid, Core.Log.ELevel.Debug);
            }

            // Util.Grid.PrintGrid(printGrid, Core.Log.ELevel.Debug);
            return gridState.GetPassword().ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);
    }
}