using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2015
{
    class Day02 : Core.Day
    {
        public Day02() { }

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
                Output = "58",
                RawInput =
@"2x3x4"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.One,
                Output = "101",
                RawInput =
@"2x3x4
1x1x10"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "34",
                RawInput =
@"2x3x4"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "14",
                RawInput =
@"1x1x10"
            });
            return testData;
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, Func<int, int, int, int> presentsFunction)
        {
            int needed = 0;
            foreach (string input in inputs)
            {
                if (string.IsNullOrWhiteSpace(input))
                {
                    continue;
                }
                
                List<int> dims = input.Split("x").Select(c => int.Parse(c)).ToList();
                dims.Sort();
                int x = dims[0], y = dims[1], z = dims[2];
                needed += presentsFunction(x, y, z);
            }

            return needed.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, (int x, int y, int z) => 2 * x * y + 2 * x * z + 2 * y * z + x * y);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, (int x, int y, int z) => 2 * x + 2 * y + x * y * z);
    }
}