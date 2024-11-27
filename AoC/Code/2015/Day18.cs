using System.Collections.Generic;
using System.Linq;

namespace AoC._2015
{
    class Day18 : Core.Day
    {
        public Day18() { }

        private int _Steps { get; }

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

        protected override List<Core.TestDatum> GetTestData()
        {
            List<Core.TestDatum> testData = new List<Core.TestDatum>();
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.One,
                Variables = new Dictionary<string, string> { { nameof(_Steps), "4" } },
                Output = "4",
                RawInput =
@".#.#.#
...##.
#....#
..#...
#.#..#
####.."
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Variables = new Dictionary<string, string> { { nameof(_Steps), "5" } },
                Output = "17",
                RawInput =
@"##.#.#
...##.
#....#
..#...
#.#..#
####.#"
            });
            return testData;
        }

        private char GetLocationState(int x, int y, List<List<char>> lights)
        {
            int onCount = Util.Grid.ProcessIndexBorder(x, y, lights, '#');

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
            GetVariable(nameof(_Steps), 100, variables, out int steps);

            List<List<char>> lights = inputs.Select(a => a.ToCharArray().ToList()).ToList();
            for (int i = 0; i < steps; ++i)
            {
                Util.Grid.ProcessGrid(ref lights, GetLocationState);
            }
            return string.Join("", lights.Select(c => string.Join("", c))).Replace(".", "").Count().ToString();
        }

        private char GetLocationStateCornersOn(int x, int y, List<List<char>> lights)
        {
            if (x == 0 && (y == 0 || y == lights.Count - 1))
            {
                return '#';
            }
            else if (y == 0 && (x == 0 || x == lights.First().Count - 1))
            {
                return '#';
            }
            else if (x == lights.First().Count - 1 && y == lights.Count - 1)
            {
                return '#';
            }

            int onCount = Util.Grid.ProcessIndexBorder(x, y, lights, '#');

            switch (lights[x][y])
            {
                case '.':
                    return onCount == 3 ? '#' : '.';
                case '#':
                    return onCount == 2 || onCount == 3 ? '#' : '.';
            }

            return '!';
        }

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            GetVariable(nameof(_Steps), 100, variables, out int steps);

            List<List<char>> lights = inputs.Select(a => a.ToCharArray().ToList()).ToList();
            lights[0][0] = '#';
            lights[0][lights[0].Count - 1] = '#';
            lights[lights.Count - 1][0] = '#';
            lights[lights.Count - 1][lights[0].Count - 1] = '#';

            // Util.Grid.Print2D(Core.Log.ELevel.Spam, lights);
            for (int i = 0; i < steps; ++i)
            {
                Util.Grid.ProcessGrid(ref lights, GetLocationStateCornersOn);
                // Core.Log.WriteLine(Core.Log.ELevel.Spam, $"After {i + 1} step");
                // Util.Grid.Print2D(Core.Log.ELevel.Spam, lights);
            }
            return string.Join("", lights.Select(c => string.Join("", c))).Replace(".", "").Count().ToString();
        }
    }
}