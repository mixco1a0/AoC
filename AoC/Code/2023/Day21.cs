using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2023
{
    class Day21 : Core.Day
    {
        public Day21() { }

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
                Variables = new Dictionary<string, string> { { nameof(_Steps), "6" } },
                Output = "16",
                RawInput =
@"...........
.....###.#.
.###.##..#.
..#.#...#..
....#.#....
.##..S####.
.##..#...#.
.......##..
.##.#.####.
.##..##.##.
..........."
            });
            //             testData.Add(new Core.TestDatum
            //             {
            //                 TestPart = Core.Part.Two,
            //                 Variables = new Dictionary<string, string> { { nameof(_Steps), "6" } },
            //                 Output = "16",
            //                 RawInput =
            // @"...........
            // .....###.#.
            // .###.##..#.
            // ..#.#...#..
            // ....#.#....
            // .##..S####.
            // .##..#...#.
            // .......##..
            // .##.#.####.
            // .##..##.##.
            // ..........."
            //             });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Variables = new Dictionary<string, string> { { nameof(_Steps), "10" } },
                Output = "50",
                RawInput =
@"...........
.....###.#.
.###.##..#.
..#.#...#..
....#.#....
.##..S####.
.##..#...#.
.......##..
.##.#.####.
.##..##.##.
..........."
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Variables = new Dictionary<string, string> { { nameof(_Steps), "50" } },
                Output = "1594",
                RawInput =
@"...........
.....###.#.
.###.##..#.
..#.#...#..
....#.#....
.##..S####.
.##..#...#.
.......##..
.##.#.####.
.##..##.##.
..........."
            });
            return testData;
        }
        private int _Steps { get; }

        private static char Plot = '.';
        private static char Rock = '#';
        private static char Start = 'S';
        private static char Step = 'O';

        private enum Direction : int { North = 0, East = 1, South = 2, West = 3 }
        static readonly Base.Pos2L[] GridMoves = new Base.Pos2L[] { new Base.Pos2L(0, 1), new Base.Pos2L(1, 0), new Base.Pos2L(0, -1), new Base.Pos2L(-1, 0) };

        private void ParseInput(List<string> inputs, out char[,] blankGrid, out Base.Pos2L start, out int xMax, out int yMax)
        {
            start = new Base.Pos2L();
            blankGrid = new char[inputs[0].Length, inputs.Count()];
            for (int x = 0; x < inputs[0].Length; ++x)
            {
                for (int y = 0; y < inputs.Count; ++y)
                {
                    blankGrid[x, y] = inputs[y][x];
                    if (blankGrid[x, y] == Start)
                    {
                        start = new Base.Pos2L(x, y);
                        blankGrid[x, y] = Plot;
                    }
                }
            }
            xMax = blankGrid.GetLength(0);
            yMax = blankGrid.GetLength(1);
        }

        private void PrintStepsTo(Dictionary<Base.Pos2L, int> stepsTo, char[,] grid, Base.Pos2L start, int maxSteps)
        {
            char[,] temp = (char[,])grid.Clone();
            foreach (var pair in stepsTo)
            {
                if (pair.Value <= maxSteps)
                {
                    if (pair.Value % 2 == 0)
                    {
                        temp[pair.Key.X, pair.Key.Y] = 'e';
                    }
                    else
                    {
                        temp[pair.Key.X, pair.Key.Y] = 'o';
                    }
                }
            }
            temp[start.X, start.Y] = Start;
            Util.Grid.PrintGrid(temp);
        }

        private record StepCheck(Base.Pos2L Pos, int Steps);

        private void PopulateSteps(ref Dictionary<Base.Pos2L, int> stepsTo, Base.Pos2L start, int xMax, int yMax, char[,] grid)
        {
            PriorityQueue<StepCheck, int> checkNext = new();
            checkNext.Enqueue(new StepCheck(start, 0), 0);
            stepsTo[start] = 0;
            while (checkNext.Count > 0)
            {
                StepCheck stepCheck = checkNext.Dequeue();
                foreach (Base.Pos2L pos2L in GridMoves)
                {
                    Base.Pos2L next = stepCheck.Pos + pos2L;
                    if (next.X < 0 || next.X >= xMax || next.Y < 0 || next.Y >= yMax)
                    {
                        continue;
                    }

                    if (grid[next.X, next.Y] == Rock)
                    {
                        continue;
                    }

                    int nextSteps = stepCheck.Steps + 1;
                    if (stepsTo.TryGetValue(next, out int value))
                    {
                        if (value <= nextSteps)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        stepsTo[next] = nextSteps;
                    }

                    checkNext.Enqueue(new StepCheck(next, nextSteps), nextSteps);
                }
            }
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool infinite, int maxSteps)
        {
            ParseInput(inputs, out char[,] grid, out Base.Pos2L start, out int xMax, out int yMax);
            GetVariable(nameof(_Steps), maxSteps, variables, out int stepCount);

            Dictionary<Base.Pos2L, int> stepsTo = new();
            PrintStepsTo(stepsTo, grid, start, 0);
            PopulateSteps(ref stepsTo, start, xMax, yMax, grid);

            if (!infinite)
            {
                int stepCompare = stepCount % 2;
                return stepsTo.Where(pair => pair.Value <= stepCount && pair.Value % 2 == stepCompare).Count().ToString();
            }

            return "";
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false, 64);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true, 26501365);
    }
}