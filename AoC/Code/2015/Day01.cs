using System.Collections.Generic;
using System.Linq;

using AoC.Core;

namespace AoC._2015
{
    class Day01 : Day
    {
        public Day01() { }

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
                Output = "0",
                RawInput =
@"(())"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.One,
                Output = "3",
                RawInput =
@"((("
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.One,
                Output = "3",
                RawInput =
@"))((((("
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.One,
                Output = "-1",
                RawInput =
@"())"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.One,
                Output = "-3",
                RawInput =
@")())())"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.Two,
                Output = "1",
                RawInput =
@")"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.Two,
                Output = "5",
                RawInput =
@"()())"
            });
            return testData;
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool stopAtBasement)
        {
            string oneLine = string.Join(string.Empty, inputs);
            if (stopAtBasement)
            {
                int curFloor = 0;
                for (int i = 0; i < oneLine.Length; ++i)
                {
                    if (oneLine[i] == '(')
                    {
                        ++curFloor;
                    }
                    else if (oneLine[i] == ')')
                    {
                        --curFloor;
                    }

                    if (curFloor < 0)
                    {
                        return (i + 1).ToString();
                    }

                }
                return string.Empty;
            }
            else
            {
                return (oneLine.Where(c => c == '(').Count() - oneLine.Where(c => c == ')').Count()).ToString();
            }
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);
    }
}