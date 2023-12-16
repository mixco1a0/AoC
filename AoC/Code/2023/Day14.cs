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
                Output = "",
                RawInput =
@""
            });
            return testData;
        }

        private const char Round = 'O';
        private const char Square = '#';
        private const char Empty = '.';

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables)
        {
            // Util.Grid.PrintGrid(Core.Log.ELevel.Debug, inputs);
            List<string> grid = new List<string>(inputs);
            Util.Grid.RotateGrid(false, ref grid);
            int load = grid.Count();
            int totalLoad = 0;
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
                shifted.Add(sb.ToString());
                // Core.TempLog.WriteLine($"after:  {sb.ToString()}");
            }
            // Util.Grid.RotateGrid(true, ref shifted);
            // Util.Grid.PrintGrid(Core.Log.ELevel.Debug, shifted);
            return totalLoad.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);
    }
}