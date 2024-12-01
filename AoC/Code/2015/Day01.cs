using System.Collections.Generic;
using System.Linq;

namespace AoC._2015
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

        protected override List<Core.TestDatum> GetTestData()
        {
            List<Core.TestDatum> testData = new List<Core.TestDatum>();
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.One,
                Output = "0",
                RawInput =
@"(())"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.One,
                Output = "3",
                RawInput =
@"((("
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.One,
                Output = "3",
                RawInput =
@"))((((("
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.One,
                Output = "-1",
                RawInput =
@"())"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.One,
                Output = "-3",
                RawInput =
@")())())"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "1",
                RawInput =
@")"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "5",
                RawInput =
@"()())"
            });
            return testData;
        }

        private static char Open => '(';
        private static char Close => ')';

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool stopAtBasement)
        {
            string oneLine = string.Join(string.Empty, inputs);
            if (stopAtBasement)
            {
                int curFloor = 0;
                for (int i = 0; i < oneLine.Length; ++i)
                {
                    if (oneLine[i] == Open)
                    {
                        ++curFloor;
                    }
                    else if (oneLine[i] == Close)
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
                return (oneLine.Where(c => c == Open).Count() - oneLine.Where(c => c == Close).Count()).ToString();
            }
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);
    }
}