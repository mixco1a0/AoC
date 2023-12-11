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
                Output = "",
                RawInput =
@""
            });
            return testData;
        }

        private char[][] ExpandUniverse(List<string> inputs)
        {
            List<string> expandedY = new List<string>();
            foreach (string input in inputs)
            {
                expandedY.Add(input);
                if (!input.Contains('#'))
                {
                    expandedY.Add(input);
                }
            }
            // Util.Grid.PrintGrid(Core.Log.ELevel.Debug, expandedY);
            Util.Grid.RotateGrid(true, ref expandedY);
            List<string> expandedX = new List<string>();
            foreach (string input in expandedY)
            {
                expandedX.Add(input);
                if (!input.Contains('#'))
                {
                    expandedX.Add(input);
                }
            }
            Util.Grid.RotateGrid(false, ref expandedX);
            // Util.Grid.PrintGrid(Core.Log.ELevel.Debug, expandedX);
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

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables)
        {
            char[][] universe = ExpandUniverse(inputs);
            Galaxy[] galaxies = GetGalaxies(universe);
            int shortestPath = 0;
            for (int i = 0; i < galaxies.Length; ++i)
            {
                for (int j = i + 1; j < galaxies.Length; ++j)
                {
                    shortestPath += galaxies[i].Pos.Manhattan(galaxies[j].Pos);
                }
            }
            return shortestPath.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);
    }
}