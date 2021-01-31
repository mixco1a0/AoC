using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2015
{
    class Day18 : Day
    {
        public Day18() { }
        static private int sSteps = 100;
        public override string GetSolutionVersion(Part part)
        {
            switch (part)
            {
                // case Part.One:
                //     return "v1";
                // case Part.Two:
                //     return "v1";
                default:
                    return base.GetSolutionVersion(part);
            }
        }
        protected override List<TestDatum> GetTestData()
        {
            List<TestDatum> testData = new List<TestDatum>();
            testData.Add(new TestDatum
            {
                TestPart = Part.One,
                Variables = new Dictionary<string, string> { { nameof(sSteps), "4" } },
                Output = "4",
                RawInput =
@".#.#.#
...##.
#....#
..#...
#.#..#
####.."
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.Two,
                Output = "",
                RawInput =
@""
            });
            return testData;
        }

        private char GetLocationState(int x, int y, List<List<char>> lights)
        {
            int onCount = Util.ProcessIndexBorder(x, y, lights, '#');

            switch (lights[x][y])
            {
                case '.':
                    return onCount == 3 ? '#' : '.';
                case '#':
                    return onCount == 2 || onCount == 3 ? '#' : '.';
            }

            return '!';
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            int steps = sSteps;
            if (variables != null && variables.ContainsKey(nameof(sSteps)))
            {
                steps = int.Parse(variables[nameof(sSteps)]);
            }

            List<List<char>> lights = inputs.Select(a => a.ToCharArray().ToList()).ToList();
            for (int i = 0; i < steps; ++i)
            {
                Util.ProcessGrid(ref lights, GetLocationState);
            }
            return string.Join("", lights.Select(c => string.Join("", c))).Replace(".", "").Replace("L", "").Count().ToString();
        }

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            return "";
        }
    }
}