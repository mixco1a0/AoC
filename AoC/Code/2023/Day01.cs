using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2023
{
    class Day01 : Core.Day
    {
        public Day01() { }

        public override string GetSolutionVersion(Core.Part part)
        {
            switch (part)
            {
                case Core.Part.One:
                    return "v1";
                case Core.Part.Two:
                    return "v2";
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
                Output = "142",
                RawInput =
@"1abc2
pqr3stu8vwx
a1b2c3d4e5f
treb7uchet"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "281",
                RawInput =
@"two1nine
eightwothree
abcone2threexyz
xtwone3four
4nineeightseven2
zoneight234
7pqrstsixteen"
            });
            return testData;
        }

        private class Calibration
        {
            public string AllDigits { get; set; }
            public int Digits { get; set; }

            private static Dictionary<char, Dictionary<string, int>> Numbers = new Dictionary<char, Dictionary<string, int>>()
            {
                {'o', new Dictionary<string, int>() {{"one", 1}}},
                {'t', new Dictionary<string, int>() {{"two", 2}, {"three", 3}}},
                {'f', new Dictionary<string, int>() {{"four", 4}, {"five", 5}}},
                {'s', new Dictionary<string, int>() {{"six", 6}, {"seven", 7}}},
                {'e', new Dictionary<string, int>() {{"eight", 8}}},
                {'n', new Dictionary<string, int>() {{"nine", 9}}}
            };

            public static Calibration Parse(string input)
            {
                Calibration c = new Calibration();
                IEnumerable<char> chars = input.Where(c => char.IsAsciiDigit(c));
                string nums = string.Concat(chars.First(), chars.Last());
                c.Digits = int.Parse(nums);
                return c;
            }

            public static Calibration ComplexParse(string input)
            {
                Calibration c = new Calibration();
                List<int> curDigits = new List<int>();
                for (int i = 0; i < input.Length; ++i)
                {
                    char curI = input[i];
                    if (char.IsAsciiDigit(curI))
                    {
                        curDigits.Add(int.Parse(curI.ToString()));
                    }
                    else
                    {
                        if (Numbers.ContainsKey(curI))
                        {
                            string curStart = input.Substring(i);
                            foreach (var pair in Numbers[curI])
                            {
                                string fullNumName = pair.Key;
                                if (curStart.Length >= fullNumName.Length && curStart.Substring(0, fullNumName.Length).Equals(fullNumName))
                                {
                                    curDigits.Add(pair.Value);
                                }
                            }
                        }
                    }
                }

                foreach (int d in curDigits)
                {
                    c.AllDigits += d.ToString();
                }
                c.Digits = int.Parse($"{curDigits.First()}{curDigits.Last()}");
                return c;
            }
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool intOnly)
        {
            List<Calibration> calibrations;
            if (intOnly)
            {
                calibrations = inputs.Select(Calibration.Parse).ToList();
            }
            else
            {
                calibrations = inputs.Select(Calibration.ComplexParse).ToList();

                // for (int i = 0; i < inputs.Count; ++i)
                // {
                //     DebugWriteLine($"{inputs[i]} => {calibrations[i].AllDigits}");
                // }
            }
            return calibrations.Select(c => c.Digits).Sum().ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);
    }
}