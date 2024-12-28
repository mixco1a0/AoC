using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2024
{
    class Day19 : Core.Day
    {
        public Day19() { }

        public override string GetSolutionVersion(Core.Part part)
        {
            return part switch
            {
                // Core.Part.One => "v1",
                // Core.Part.Two => "v1",
                _ => base.GetSolutionVersion(part),
            };
        }

        protected override List<Core.TestDatum> GetTestData()
        {
            List<Core.TestDatum> testData =
            [
                new Core.TestDatum
                {
                    TestPart = Core.Part.One,
                    Output = "6",
                    RawInput =
@"r, wr, b, g, bwu, rb, gb, br

brwrr
bggr
gbbr
rrbgbr
ubwu
bwurrg
brgr
bbrgwb"
                },
                new Core.TestDatum
                {
                    TestPart = Core.Part.Two,
                    Output = "",
                    RawInput =
@""
                },
            ];
            return testData;
        }

        [Flags]
        private enum Color : byte
        {
            White = 0b_0000_0001,
            Blue = 0b_0000_0010,
            Black = 0b_0000_0100,
            Red = 0b_0000_1000,
            Green = 0b_0001_0000,
            None = 0b_0000_0000
        }

        private void Parse(List<string> input, out List<Color[]> towels, out List<Color[]> patterns)
        {
            towels = [];
            patterns = [];

            Color getColor(char c)
            {
                return c switch
                {
                    'w' => Color.White,
                    'u' => Color.Blue,
                    'b' => Color.Black,
                    'r' => Color.Red,
                    'g' => Color.Green,
                    _ => Color.None,
                };
            }

            string[] split = Util.String.Split(input.First(), ", ");
            foreach (string s in split)
            {
                Color[] towel = new Color[s.Length];
                int index = 0;
                foreach (char c in s)
                {
                    towel[index++] = getColor(c);
                }
                towels.Add(towel);
            }

            foreach (string i in input.Skip(2))
            {
                Color[] pattern = new Color[i.Length];
                int index = 0;
                foreach (char t in i)
                {
                    pattern[index++] = getColor(t);
                }
                patterns.Add(pattern);
            }
        }

        private static bool IsPatternPossible(Color[] pattern, int patternIdx, List<Color[]> towels, int towelIdx, int colorIdx)
        {
            Color[] towelColors = towels[towelIdx];

            // hit the end of the pattern
            if (patternIdx >= pattern.Length)
            {
                if (colorIdx != 0)
                {
                    return false;
                }

                return true;
            }

            if (towelColors[colorIdx] == pattern[patternIdx])
            {
                if (colorIdx + 1 == towelColors.Length)
                {
                    // current towel is complete, check next index against all possible towels again
                    for (int t = 0; t < towels.Count; ++t)
                    {
                        if (IsPatternPossible(pattern, patternIdx + 1, towels, t, 0))
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    // keep going down the current towel
                    return IsPatternPossible(pattern, patternIdx + 1, towels, towelIdx, colorIdx + 1);
                }
            }

            return false;
        }

        private static bool IsPatternPossible(Color[] pattern, List<Color[]> towels)
        {
            for (int t = 0; t < towels.Count; ++t)
            {
                if (IsPatternPossible(pattern, 0, towels, t, 0))
                {
                    return true;
                }
            }

            return false;
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool _)
        {
            Parse(inputs, out List<Color[]> towels, out List<Color[]> patterns);
            int possibleTowels = 0;
            foreach (Color[] pattern in patterns)
            {
                possibleTowels += IsPatternPossible(pattern, towels) ? 1 : 0;
            }
            return possibleTowels.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);
    }
}