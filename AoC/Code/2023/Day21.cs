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
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "",
                RawInput =
@""
            });
            return testData;
        }
        private int _Steps { get; }

        private static char Plot = '.';
        // private static char Rock = '.';
        private static char Start = 'S';
        private static char Step = 'O';
        static readonly Base.Pos2L[] GridMoves = new Base.Pos2L[] { new Base.Pos2L(0, 1), new Base.Pos2L(1, 0), new Base.Pos2L(-1, 0), new Base.Pos2L(0, -1) };

        private void ParseInput(List<string> inputs, out char[,] grid, out Base.Pos2L start, out int xMax, out int yMax)
        {
            start = new Base.Pos2L();
            grid = new char[inputs[0].Length, inputs.Count()];
            for (int x = 0; x < inputs[0].Length; ++x)
            {
                for (int y = 0; y < inputs.Count; ++y)
                {
                    grid[x, y] = inputs[y][x];
                    if (grid[x, y] == Start)
                    {
                        start = new Base.Pos2L(x, y);
                        grid[x, y] = Plot;
                    }
                }
            }
            xMax = grid.GetLength(0);
            yMax = grid.GetLength(1);
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables)
        {
            ParseInput(inputs, out char[,] grid, out Base.Pos2L start, out int xMax, out int yMax);
            // Util.Grid.PrintGrid(grid);
            GetVariable(nameof(_Steps), 64, variables, out int stepCount);
            HashSet<Base.Pos2L> plots = new HashSet<Base.Pos2L>() { start };
            for (int i = 0; i < stepCount; ++i)
            {
                HashSet<Base.Pos2L> newPlots = new HashSet<Base.Pos2L>();
                char[,] copy = (char[,])grid.Clone();
                foreach (Base.Pos2L plot in plots)
                {
                    foreach (Base.Pos2L gridMove in GridMoves)
                    {
                        Base.Pos2L newPlot = plot + gridMove;
                        if (newPlot.X >= 0 && newPlot.X < xMax && newPlot.Y >= 0 && newPlot.Y < yMax && grid[newPlot.X, newPlot.Y] == Plot)
                        {
                            newPlots.Add(newPlot);
                            copy[newPlot.X, newPlot.Y] = Step;
                        }
                    }
                }
                // Util.Grid.PrintGrid(copy);
                plots = new HashSet<Base.Pos2L>(newPlots);
            }
            return plots.Count.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);
    }
}