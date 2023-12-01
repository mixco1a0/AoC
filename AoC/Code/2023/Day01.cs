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
                Output = "",
                RawInput =
@""
            });
            return testData;
        }

        private class Calibration
        {
            public int Digits { get; set; }

            public static Calibration Parse(string input)
            {
                Calibration c = new Calibration();
                IEnumerable<char> chars = input.Where(c => char.IsAsciiDigit(c));
                string nums = string.Concat(chars.First(), chars.Last());
                c.Digits = int.Parse(nums);
                return c;
            }
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables)
        {
            List<Calibration> calibrations = inputs.Select(Calibration.Parse).ToList();
            return calibrations.Select(c => c.Digits).Sum().ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);
    }
}