using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AoC._2024
{
    class Day03 : Core.Day
    {
        public Day03() { }

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
                    Output = "161",
                    RawInput =
@"xmul(2,4)%&mul[3,7]!@^do_not_mul(5,5)+mul(32,64]then(mul(11,8)mul(8,5))"
                },
                new Core.TestDatum
                {
                    TestPart = Core.Part.Two,
                    Output = "",
                    RawInput =
@""
                },
            ];
            return testData;
        }

        private static readonly string MulRegex = @"mul\(\d+,\d+\)";

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables)
        {
            int sum = 0;
            foreach (string input in inputs)
            {
                Regex regex = new(MulRegex);
                MatchCollection mc = regex.Matches(input);
                for (int i = 0; i < mc.Count; ++i)
                {
                    string match = mc[i].Value;
                    int[] values = Util.String.Split(mc[i].Value, "mul(,)").Select(int.Parse).ToArray();
                    sum += values[0] * values[1];
                }
            }
            return sum.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);
    }
}