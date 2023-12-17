using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2023
{
    class Day14 : Core.Day
    {
        public Day14() { }

        public override string GetSolutionVersion(Core.Part part)
        {
            switch (part)
            {
                case Core.Part.One:
                    return "v1";
                case Core.Part.Two:
                    return "v1";
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

        private char[,] ConvertInput(List<string> inputs)
        {
            char[,] converted = new char[inputs[0].Length, inputs.Count()];
            for (int x = 0; x < inputs[0].Length; ++x)
            {
                for (int y = 0; y < inputs.Count; ++y)
                {
                    switch (inputs[y][x])
                    {
                        case Round:
                            converted[x, y] = Round;
                            break;
                        case Square:
                            converted[x, y] = Square;
                            break;
                        case Empty:
                            converted[x, y] = Empty;
                            break;
                    }
                }
            }
            return converted;
        }

        private void ShiftNS(int yStart, int yEnd, Func<int, int> yFunc, ref char[,] grid)
        {
            for (int x = 0; x < grid.GetLength(0); ++x)
            {
                int yEmpty = -1;
                for (int y = yStart; y != yEnd; y = yFunc(y))
                {
                    switch (grid[x, y])
                    {
                        case Round:
                            if (yEmpty >= 0)
                            {
                                grid[x, yEmpty] = Round;
                                grid[x, y] = Empty;
                                yEmpty = yFunc(yEmpty);
                            }
                            break;
                        case Square:
                            yEmpty = -1;
                            break;
                        case Empty:
                            if (yEmpty == -1)
                            {
                                yEmpty = y;
                            }
                            break;
                    }
                }
            }
        }

        private void ShiftEW(int xStart, int xEnd, Func<int, int> xFunc, ref char[,] grid)
        {
            for (int y = 0; y < grid.GetLength(1); ++y)
            {
                int xEmpty = -1;
                for (int x = xStart; x != xEnd; x = xFunc(x))
                {
                    switch (grid[x, y])
                    {
                        case Round:
                            if (xEmpty >= 0)
                            {
                                grid[xEmpty, y] = Round;
                                grid[x, y] = Empty;
                                xEmpty = xFunc(xEmpty);
                            }
                            break;
                        case Square:
                            xEmpty = -1;
                            break;
                        case Empty:
                            if (xEmpty == -1)
                            {
                                xEmpty = x;
                            }
                            break;
                    }
                }
            }
        }

        private void Cycle(ref char[,] grid, ref int totalLoad, int xLoad, int yLoad)
        {
            ShiftNS(0, yLoad, (y) => y + 1, ref grid);
            ShiftEW(0, xLoad, (x) => x + 1, ref grid);
            ShiftNS(yLoad - 1, -1, (y) => y - 1, ref grid);
            ShiftEW(xLoad - 1, -1, (x) => x - 1, ref grid);
            GetTotalLoad(grid, ref totalLoad);
        }

        private void GetTotalLoad(char[,] grid, ref int totalLoad)
        {
            int yLoad = grid.GetLength(1);
            totalLoad = 0;
            for (int x = 0; x < grid.GetLength(0); ++x)
            {
                for (int y = 0; y < yLoad; ++y)
                {
                    switch (grid[x, y])
                    {
                        case Round:
                            totalLoad += yLoad - y;
                            break;
                        case Square:
                            break;
                        case Empty:
                            break;
                    }
                }
            }
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool fullCycle)
        {
            char[,] grid = ConvertInput(inputs);
            int totalLoad = 0;
            int xLoad = grid.GetLength(0), yLoad = grid.GetLength(1);

            List<int> hashCycle = new List<int>();
            int nonCycleIntro = 0;
            int cycleLength = -1;
            bool verifyCycle = false;
            Queue<int> verifyQueue = new Queue<int>();
            bool doHash = true;

            if (fullCycle)
            {
                int prevLoad = 0;
                int max = 1000000000;
                for (int i = 0; i < max; ++i)
                {
                    Cycle(ref grid, ref totalLoad, xLoad, yLoad);
                    if (!doHash)
                    {
                        continue;
                    }

                    int hash = HashCode.Combine(prevLoad, totalLoad);
                    if (verifyCycle)
                    {
                        // verify the new hashes align with the old ones
                        if (verifyQueue.Count == 0 || verifyQueue.Peek() != hash)
                        {
                            hashCycle.Clear();
                            verifyQueue.Clear();
                            verifyCycle = false;
                        }
                        else
                        {
                            verifyQueue.Dequeue();
                            doHash = verifyQueue.Count != 0;
                            max = (max - nonCycleIntro) % cycleLength + cycleLength * 2 + nonCycleIntro;
                        }
                    }
                    else if (!hashCycle.Contains(hash))
                    {
                        hashCycle.Add(hash);
                        nonCycleIntro = i;
                    }
                    else
                    {
                        int cycleStartIndex = hashCycle.Select((hc, index) => new { hc, index }).Where(pair => pair.hc == hash).Select(pair => pair.index).Last();
                        cycleLength = hashCycle.Count - cycleStartIndex;
                        verifyQueue = new Queue<int>(hashCycle.TakeLast(cycleLength - 1));
                        cycleStartIndex = i;
                        verifyCycle = true;
                    }
                    prevLoad = totalLoad;
                }
            }
            else
            {
                ShiftNS(0, yLoad, (y) => y + 1, ref grid);
                GetTotalLoad(grid, ref totalLoad);
            }
            return totalLoad.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);
    }
}