using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2023
{
    class Day11 : Core.Day
    {
        public Day11() { }

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
                Output = "374",
                RawInput =
@"...#......
.......#..
#.........
..........
......#...
.#........
.........#
..........
.......#..
#...#....."
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Variables = new Dictionary<string, string> { { nameof(_ExpansionMultiplier), "10" } },
                Output = "1030",
                RawInput =
@"...#......
.......#..
#.........
..........
......#...
.#........
.........#
..........
.......#..
#...#....."
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Variables = new Dictionary<string, string> { { nameof(_ExpansionMultiplier), "100" } },
                Output = "8410",
                RawInput =
@"...#......
.......#..
#.........
..........
......#...
.#........
.........#
..........
.......#..
#...#....."
            });
            return testData;
        }

        private int _ExpansionMultiplier { get; }

        private char[][] ExpandUniverse(List<string> inputs, out HashSet<int> emptyRows, out HashSet<int> emptyCols)
        {
            emptyRows = new HashSet<int>();
            emptyCols = new HashSet<int>();

            List<string> expandedY = new List<string>();
            int i = 0;
            foreach (string input in inputs)
            {
                expandedY.Add(input);
                if (!input.Contains('#'))
                {
                    emptyRows.Add(i);
                }
                ++i;
            }
            Util.Grid.RotateGrid(true, ref expandedY);
            List<string> expandedX = new List<string>();
            i = 0;
            foreach (string input in expandedY)
            {
                expandedX.Add(input);
                if (!input.Contains('#'))
                {
                    emptyCols.Add(i);
                }
                ++i;
            }
            Util.Grid.RotateGrid(false, ref expandedX);
            return expandedX.Select(e => e.ToCharArray()).ToArray();
        }

        private record Galaxy(Base.Pos2 Pos, int Id);

        private Galaxy[] GetGalaxies(char[][] universe)
        {
            List<Galaxy> galaxies = new List<Galaxy>();
            int galaxyId = 1;
            for (int y = 0; y < universe.Length; ++y)
            {
                for (int x = 0; x < universe[y].Length; ++x)
                {
                    if (universe[y][x] == '#')
                    {
                        galaxies.Add(new Galaxy(new Base.Pos2(x, y), galaxyId++));
                    }
                }
            }
            return galaxies.ToArray();
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, int expansionMultiplier)
        {
            char[][] universe = ExpandUniverse(inputs, out HashSet<int> emptyRows, out HashSet<int> emptyCols);
            Galaxy[] galaxies = GetGalaxies(universe);

            GetVariable(nameof(_ExpansionMultiplier), expansionMultiplier, variables, out int expansionCount);

            long shortestPath = 0;
            for (long i = 0; i < galaxies.Length; ++i)
            {
                for (long j = i + 1; j < galaxies.Length; ++j)
                {
                    shortestPath += galaxies[i].Pos.Manhattan(galaxies[j].Pos);

                    int minX = Math.Min(galaxies[i].Pos.X, galaxies[j].Pos.X);
                    int maxX = Math.Max(galaxies[i].Pos.X, galaxies[j].Pos.X);
                    for (int x = minX; x < maxX; ++x)
                    {
                        if (emptyCols.Contains(x))
                        {
                            shortestPath += (expansionCount - 1);
                        }
                    }

                    int minY = Math.Min(galaxies[i].Pos.Y, galaxies[j].Pos.Y);
                    int maxY = Math.Max(galaxies[i].Pos.Y, galaxies[j].Pos.Y);
                    for (int y = minY; y < maxY; ++y)
                    {
                        if (emptyRows.Contains(y))
                        {
                            shortestPath += (expansionCount - 1);
                        }
                    }
                }
            }
            return shortestPath.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, 2);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, 1000000);
    }
}