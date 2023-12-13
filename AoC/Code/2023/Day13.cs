using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2023
{
    class Day13 : Core.Day
    {
        public Day13() { }

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
                Output = "405",
                RawInput =
@"#.##..##.
..#.##.#.
##......#
##......#
..#.##.#.
..##..##.
#.#.##.#.

#...##..#
#....#..#
..##..###
#####.##.
#####.##.
..##..###
#....#..#"
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

        private const char Ash = '.';
        private const char Rock = '#';

        private bool IsReflected(List<string> pattern, int index)
        {
            for (int i = index - 1, j = index + 2; i >= 0 && j < pattern.Count; --i, ++j)
            {
                if (pattern[i] != pattern[j])
                {
                    return false;
                }
            }
            return true;
        }

        private int GetScore(List<string> pattern)
        {
            for (int i = 0; i < pattern.Count - 1; ++i)
            {
                if (pattern[i] == pattern[i + 1] && IsReflected(pattern, i))
                {
                    return 100 * (i + 1);
                }
            }

            Util.Grid.RotateGrid(true, ref pattern);
            for (int i = 0; i < pattern.Count - 1; ++i)
            {
                if (pattern[i] == pattern[i + 1] && IsReflected(pattern, i))
                {
                    return (i + 1);
                }
            }
            return 0;
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables)
        {
            List<List<string>> patterns = new List<List<string>>();
            List<string> cur = new List<string>();
            foreach (string input in inputs)
            {
                if (string.IsNullOrWhiteSpace(input))
                {
                    patterns.Add(new List<string>(cur));
                    cur.Clear();
                }
                else
                {
                    cur.Add(input);
                }
            }
            patterns.Add(new List<string>(cur));

            int score = 0;
            foreach (List<string> pattern in patterns)
            {
                score += GetScore(pattern);
            }
            return score.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);
    }
}