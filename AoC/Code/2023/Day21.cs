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
        private static char Start = 'S';
        private static char Step = 'O';

        private enum Direction : int { North = 0, East = 1, South = 2, West = 3 }
        static readonly Base.Pos2L[] GridMoves = new Base.Pos2L[] { new Base.Pos2L(0, 1), new Base.Pos2L(1, 0), new Base.Pos2L(0, -1), new Base.Pos2L(-1, 0) };

        private record SpawnState(Base.Pos2L Direction, HashSet<Base.Pos2L> StartSteps);

        private class StepState
        {
            public bool Solved { get; set; }
            public Base.Pos2L Pos { get; set; }
            public int StartStep { get; set; }
            public List<HashSet<Base.Pos2L>> PlotStates { get; set; }
            public HashSet<Base.Pos2L> StartPlots { get; set; }

            public StepState(Base.Pos2L pos, int startStep, char[,] blankGrid, HashSet<Base.Pos2L> startPlots)
            {
                Solved = false;
                Pos = pos;
                StartStep = startStep;
                StartPlots = new HashSet<Base.Pos2L>(startPlots);
                PlotStates = new List<HashSet<Base.Pos2L>>
                {
                    new HashSet<Base.Pos2L>(startPlots)
                };
            }
        }

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

        private void PrintStepState(bool all, char[,] grid, StepState stepState)
        {
            char[,] temp = (char[,])grid.Clone();
            if (all)
            {

            }
            else
            {
                foreach (Base.Pos2L pos in stepState.PlotStates.Last())
                {
                    temp[pos.X, pos.Y] = Step;
                }
                Util.Grid.PrintGrid(temp);
            }
        }

        private List<SpawnState> RunStep(bool infinite, char[,] grid, ref StepState state, int xMax, int yMax)
        {
            List<SpawnState> spawnStates = new List<SpawnState>();
            HashSet<Base.Pos2L> newPlots = new HashSet<Base.Pos2L>();
            // todo: cycle detect
            foreach (Base.Pos2L plot in state.PlotStates.Last())
            {
                foreach (Base.Pos2L gridMove in GridMoves)
                {
                    Base.Pos2L newPlot = plot + gridMove;
                    if (newPlot.X >= 0 && newPlot.X < xMax && newPlot.Y >= 0 && newPlot.Y < yMax)
                    {
                        if (grid[newPlot.X, newPlot.Y] == Plot)
                        {
                            newPlots.Add(newPlot);
                        }
                    }
                    else if (infinite)
                    {
                        if (newPlot.X < 0)
                        {
                            spawnStates.Add(new SpawnState(GridMoves[(int)Direction.West], new HashSet<Base.Pos2L>()));
                            spawnStates.Last().StartSteps.Add(new Base.Pos2L(xMax - 1, newPlot.Y));
                        }
                        else if (newPlot.X >= xMax)
                        {
                            spawnStates.Add(new SpawnState(GridMoves[(int)Direction.East], new HashSet<Base.Pos2L>()));
                            spawnStates.Last().StartSteps.Add(new Base.Pos2L(0, newPlot.Y));
                        }
                        else if (newPlot.Y < 0)
                        {
                            spawnStates.Add(new SpawnState(GridMoves[(int)Direction.South], new HashSet<Base.Pos2L>()));
                            spawnStates.Last().StartSteps.Add(new Base.Pos2L(newPlot.X, yMax - 1));
                        }
                        else if (newPlot.Y >= yMax)
                        {
                            spawnStates.Add(new SpawnState(GridMoves[(int)Direction.East], new HashSet<Base.Pos2L>()));
                            spawnStates.Last().StartSteps.Add(new Base.Pos2L(newPlot.X, 0));
                        }
                    }
                }
            }
            state.PlotStates.Add(newPlots);
            PrintStepState(false, grid, state);
            return spawnStates;
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool infinite, int maxSteps)
        {
            ParseInput(inputs, out char[,] blankGrid, out Base.Pos2L start, out int xMax, out int yMax);
            GetVariable(nameof(_Steps), maxSteps, variables, out int stepCount);

            Dictionary<int, StepState> knownStepStates = new Dictionary<int, StepState>();
            StepState startStepState = new StepState(Base.Pos2L.Zero, 0, blankGrid, new HashSet<Base.Pos2L> { start });
            knownStepStates[startStepState.StartPlots.GetHashCode()] = startStepState;
            PrintStepState(false, blankGrid, knownStepStates.First().Value);
            for (int i = 0; i < stepCount; ++i)
            {
                foreach (int key in knownStepStates.Keys)
                {
                    StepState stepState = knownStepStates[key];
                    List<SpawnState> spawnStates = RunStep(infinite, blankGrid, ref stepState, xMax, yMax);
                    // TODO spawnStates
                }
            }
            return "";
            // return plots.Count.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => "skip!"; //SharedSolution(inputs, variables, false, 64);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true, 26501365);
    }
}