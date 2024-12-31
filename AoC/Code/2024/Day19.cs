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
                Core.Part.One => "v1",
                Core.Part.Two => "v1",
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
                new Core.TestDatum
                {
                    TestPart = Core.Part.Two,
                    Output = "1",
                    RawInput =
@"gw, bub, rur, rr, rrw, g, wb, ubr, urrr

gwbubrurrrw"
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
        private List<Color[]> Towels { get; set; }
        private List<Color[]> Patterns { get; set; }
        private Dictionary<int, long> Memoize { get; set; }

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

        private void Parse(List<string> input)
        {
            Towels = [];
            Patterns = [];

            string[] split = Util.String.Split(input.First(), ", ");
            foreach (string s in split.Order())
            {
                Color[] towel = new Color[s.Length];
                int index = 0;
                foreach (char c in s)
                {
                    towel[index++] = GetColor(c);
                }
                Towels.Add(towel);
            }

            foreach (string i in input.Skip(2).Order())
            {
                Color[] pattern = new Color[i.Length];
                int index = 0;
                foreach (char t in i)
                {
                    pattern[index++] = GetColor(t);
                }
                Patterns.Add(pattern);
            }
        }

        private static string GetString(IEnumerable<Color> colors)
        {
            StringBuilder sb = new();
            foreach (Color color in colors)
            {
                sb.Append(GetColorCharacter(color));
            }
            return sb.ToString();
        }

        private long GetPatternMatches(Color[] pattern, int patternIndex)
        {
            // get a hash of the remaining colors in the current pattern
            int remainingPatternHash = 0;
            foreach (Color color in pattern.Skip(patternIndex))
            {
                remainingPatternHash = HashCode.Combine(remainingPatternHash, color);
            }

            // Log($"Pattern={new string(' ', patternIndex)}{GetString(pattern.Skip(patternIndex))} | Hash={remainingPatternHash}");

            // return the cached result
            if (Memoize.TryGetValue(remainingPatternHash, out long value))
            {
                return value;
            }

            // sum up the match count for all of the next towels being used
            long totalPatternMatchCount = 0;
            for (int t = 0; t < Towels.Count; ++t)
            {
                Color[] currentTowel = Towels[t];
                // Log($"Towel...{new string(' ', pattern.Length)} | {GetString(currentTowel)}");

                if (currentTowel.Length + patternIndex > pattern.Length)
                {
                    // towel is too long, skip it
                    continue;
                }

                // towel can be used, match sure the pattern matches completely
                bool matches = true;
                for (int c = 0; matches && c < currentTowel.Length; ++c)
                {
                    matches = currentTowel[c] == pattern[patternIndex + c];
                }

                // towel doesn't match, skip it
                if (!matches)
                {
                    continue;
                }

                // check to see if this towel completes the pattern entirely
                if (currentTowel.Length + patternIndex == pattern.Length)
                {
                    // this is a match
                    ++totalPatternMatchCount;
                }
                else
                {
                    // there is still more pattern to match, get the rest of the pattern match count
                    totalPatternMatchCount += GetPatternMatches(pattern, patternIndex + currentTowel.Length);
                    // Log($"Pattern={new string(' ', patternIndex)}{GetString(pattern.Skip(patternIndex))} | Hash={remainingPatternHash}");
                }

                // short circuit future checks for part one
                if (!FindAllMatches && totalPatternMatchCount > 0)
                {
                    break;
                }
            }

            Memoize[remainingPatternHash] = totalPatternMatchCount;
            return totalPatternMatchCount;
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool findAllMatches)
        {
            FindAllMatches = findAllMatches;
            Memoize = [];

            Parse(inputs);
            long possibleTowels = 0;
            foreach (Color[] pattern in Patterns)
            {
                // Log($"Solve=  {GetString(pattern)}:");
                possibleTowels += GetPatternMatches(pattern, 0);
            }
            return possibleTowels.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);
        // TOO LOW -> 15982
        // TOO LOW -> 1952121237
    }
}