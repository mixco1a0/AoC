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
                Output = "400",
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
            return testData;
        }

        private bool IsReflected(List<string> pattern, int index, bool fixSmudge)
        {
            bool smudgeFixed = false;
            for (int i = index, j = index + 1; i >= 0 && j < pattern.Count; --i, ++j)
            {
                if (pattern[i] != pattern[j])
                {
                    if (!fixSmudge)
                    {
                        return false;
                    }
                    else
                    {
                        if (smudgeFixed)
                        {
                            return false;
                        }
                        else
                        {
                            // attempt the smudge fix
                            int diffCount = 0;
                            for (int t = 0; t < pattern[i].Length && diffCount <= 1; ++t)
                            {
                                if (pattern[i][t] != pattern[j][t])
                                {
                                    ++diffCount;
                                }
                            }
                            if (diffCount == 1)
                            {
                                smudgeFixed = true;
                            }
                            else if (diffCount > 1)
                            {
                                return false;
                            }
                        }
                    }
                }
            }

            if (fixSmudge && !smudgeFixed)
            {
                return false;
            }
            return true;
        }

        private int GetScore(List<string> pattern, bool fixSmudge)
        {
            for (int i = 0; i < pattern.Count - 1; ++i)
            {
                if (IsReflected(pattern, i, fixSmudge))
                {
                    return 100 * (i + 1);
                }
            }

            Util.Grid.RotateGrid(true, ref pattern);
            for (int i = 0; i < pattern.Count - 1; ++i)
            {
                if (IsReflected(pattern, i, fixSmudge))
                {
                    return (i + 1);
                }
            }

            return 0;
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool fixSmudge)
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
                score += GetScore(pattern, fixSmudge);
            }
            return score.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);
    }
}