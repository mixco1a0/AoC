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

            private static Dictionary<string, int> Convert = new Dictionary<string, int>()
            {
                {"one", 1},
                {"two", 2},
                {"three", 3},
                {"four", 4},
                {"five", 5},
                {"six", 6},
                {"seven", 7},
                {"eight", 8},
                {"nine", 9}
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
                List<string> curTextDigitPossibilities = new List<string>();
                foreach (char i in input)
                {
                    if (char.IsAsciiDigit(i))
                    {
                        curTextDigitPossibilities.Clear();
                        curDigits.Add(int.Parse($"{i}"));
                    }
                    else if (char.IsAsciiLetter(i))
                    {
                        for (int j = 0; j < curTextDigitPossibilities.Count; ++j)
                        {
                            curTextDigitPossibilities[j] = $"{curTextDigitPossibilities[j]}{i}";
                        }
                        curTextDigitPossibilities.Add(i.ToString());
                        int foundNum = -1;
                        foreach (string pos in curTextDigitPossibilities)
                        {
                            if (Convert.ContainsKey(pos))
                            {
                                foundNum = Convert[pos];
                                break;
                            }
                        }

                        if (foundNum != -1)
                        {
                            curDigits.Add(foundNum);
                        }
                    }
                    else
                    {
                        curTextDigitPossibilities.Clear();
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