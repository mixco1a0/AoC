using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2016
{
    class Day02 : Day
    {
        public Day02() { }
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
                Output = "1985",
                RawInput =
@"ULL
RRDDD
LURDL
UUUUD"
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

        static string[] numberPad = {"123", "456", "789"};

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            StringBuilder code = new StringBuilder();
            int x = 1, y = 1;
            foreach (string input in inputs)
            {
                foreach (char c in input)
                {
                    switch (c)
                    {
                        case 'U':
                            y = Math.Max(0, y - 1);
                            break;
                        case 'D':
                            y = Math.Min(2, y + 1);
                            break;
                        case 'L':
                            x = Math.Max(0, x - 1);
                            break;
                        case 'R':
                            x = Math.Min(2, x + 1);
                            break;
                    }
                }
                code.Append(numberPad[y][x]);
            }
            return code.ToString();
        }

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            return "";
        }
    }
}