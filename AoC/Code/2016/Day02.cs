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
                case Part.One:
                    return "v1";
                case Part.Two:
                    return "v1";
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
                Output = "5DB3",
                RawInput =
@"ULL
RRDDD
LURDL
UUUUD"
            });
            return testData;
        }

        static string[] numberPad1 = { "123", "456", "789" };

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            MinMax range = new MinMax(0, 2);
            StringBuilder code = new StringBuilder();
            int x = 1, y = 1;
            foreach (string input in inputs)
            {
                foreach (char c in input)
                {
                    switch (c)
                    {
                        case 'U':
                            y = Math.Max(range.Min, y - 1);
                            break;
                        case 'D':
                            y = Math.Min(range.Max, y + 1);
                            break;
                        case 'L':
                            x = Math.Max(range.Min, x - 1);
                            break;
                        case 'R':
                            x = Math.Min(range.Max, x + 1);
                            break;
                    }
                }
                code.Append(numberPad1[y][x]);
            }
            return code.ToString();
        }

        static string[] numberPad2 = { "  1  ", " 234 ", "56789", " ABC ", "  D  " };

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            MinMax range = new MinMax(0, 4);
            StringBuilder code = new StringBuilder();
            int x = 0, y = 2;
            foreach (string input in inputs)
            {
                foreach (char c in input)
                {
                    int oldX = x;
                    int oldY = y;
                    switch (c)
                    {
                        case 'U':
                            y = Math.Max(range.Min, y - 1);
                            break;
                        case 'D':
                            y = Math.Min(range.Max, y + 1);
                            break;
                        case 'L':
                            x = Math.Max(range.Min, x - 1);
                            break;
                        case 'R':
                            x = Math.Min(range.Max, x + 1);
                            break;
                    }
                    if (numberPad2[y][x] == ' ')
                    {
                        x = oldX;
                        y = oldY;
                    }
                }
                code.Append(numberPad2[y][x]);
            }
            return code.ToString();
        }
    }
}