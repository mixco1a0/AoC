using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
                    Output = "2",
                    RawInput =
@"r, wr, b, g, bwu, rb, gb, br

brwrr"
                },
                new Core.TestDatum
                {
                    TestPart = Core.Part.Two,
                    Output = "1",
                    RawInput =
@"r, wr, b, g, bwu, rb, gb, br

bggr"
                },
                new Core.TestDatum
                {
                    TestPart = Core.Part.Two,
                    Output = "4",
                    RawInput =
@"r, wr, b, g, bwu, rb, gb, br

gbbr"
                },
                new Core.TestDatum
                {
                    TestPart = Core.Part.Two,
                    Output = "6",
                    RawInput =
@"r, wr, b, g, bwu, rb, gb, br

rrbgbr"
                },
                new Core.TestDatum
                {
                    TestPart = Core.Part.Two,
                    Output = "16",
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

        private bool FindAllMatches { get; set; }

        static Color GetColor(char c)
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

        static char GetColorCharacter(Color color)
        {
            return color switch
            {
                Color.White => 'w',
                Color.Blue => 'u',
                Color.Black => 'b',
                Color.Red => 'r',
                Color.Green => 'g',
                _ => ' ',
            };
        }

        private static void Parse(List<string> input, out List<Color[]> towels, out List<Color[]> patterns)
        {
            towels = [];
            patterns = [];

            string[] split = Util.String.Split(input.First(), ", ");
            foreach (string s in split)
            {
                Color[] towel = new Color[s.Length];
                int index = 0;
                foreach (char c in s)
                {
                    towel[index++] = GetColor(c);
                }
                towels.Add(towel);
            }

            foreach (string i in input.Skip(2))
            {
                Color[] pattern = new Color[i.Length];
                int index = 0;
                foreach (char t in i)
                {
                    pattern[index++] = GetColor(t);
                }
                patterns.Add(pattern);
            }
        }

        private static string GetString(Color[] colors)
        {
            StringBuilder sb = new();
            foreach (Color color in colors)
            {
                sb.Append(GetColorCharacter(color));
            }
            return sb.ToString();
        }

        private bool GetPossibleMatches(Color[] pattern, int patternIdx, List<Color[]> towels, int towelIdx, int colorIdx, ref int matchCount)
        {
            // check next color
            Color[] towelColors = towels[towelIdx];
            if (towelColors[colorIdx] == pattern[patternIdx])
            {
                // hit the end of the pattern
                if (patternIdx + 1 == pattern.Length)
                {
                    // towel wasn't finished
                    if (colorIdx != towelColors.Length - 1)
                    {
                        return false;
                    }

                    // last towel was fully used
                    ++matchCount;
                    return true;
                }
                // check if current towel was completed
                else if (colorIdx + 1 == towelColors.Length)
                {
                    // if (FindAllMatches)
                    // {
                    //     Log($"{new string(' ', patternIdx - towelColors.Length + 1)}{GetString(towelColors)}");
                    // }

                    // current towel is complete, check next index against all possible towels again
                    for (int t = 0; t < towels.Count; ++t)
                    {
                        if (GetPossibleMatches(pattern, patternIdx + 1, towels, t, 0, ref matchCount) && !FindAllMatches)
                        {
                            return true;
                        }
                    }
                }
                // keep checking current towel
                else
                {
                    // if (FindAllMatches)
                    // {
                    //     Log($"{new string(' ', patternIdx - colorIdx)}{GetString(towelColors)} | ->");
                    // }

                    // keep going down the current towel
                    if (GetPossibleMatches(pattern, patternIdx + 1, towels, towelIdx, colorIdx + 1, ref matchCount) && !FindAllMatches)
                    {
                        return true;
                    }
                }
            }
            else
            {
                // if (FindAllMatches)
                // {
                //     Log($"{new string(' ', patternIdx)}{GetString(towelColors)} | FAIL");
                // }
            }

            return false;
        }

        private void GetPossibleMatches(Color[] pattern, List<Color[]> towels, ref int possibleTowels)
        {
            // if (FindAllMatches)
            // {
            //     Log($"{GetString(pattern)}:");
            // }

            for (int t = 0; t < towels.Count; ++t)
            {
                if (GetPossibleMatches(pattern, 0, towels, t, 0, ref possibleTowels) && !FindAllMatches)
                {
                    break;
                }
            }
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool findAllMatches)
        {
            FindAllMatches = findAllMatches;

            Parse(inputs, out List<Color[]> towels, out List<Color[]> patterns);
            int possibleTowels = 0;
            foreach (Color[] pattern in patterns)
            {
                GetPossibleMatches(pattern, towels, ref possibleTowels);
            }
            return possibleTowels.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);
    }
}