using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2021
{
    class Day03 : Day
    {
        public Day03() { }

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
                Output = "198",
                RawInput =
@"00100
11110
10110
10111
10101
01111
00111
11100
10000
11001
00010
01010"
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

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables)
        {
            int[] zeroes = new int[inputs.First().Length];
            int[] ones = new int[inputs.First().Length];
            foreach (string input in inputs)
            {
                for (int i = input.Length - 1; i >= 0; --i)
                {
                    zeroes[i] += (input[i] == '0' ? 1 : 0);
                    ones[i] += (input[i] == '1' ? 1 : 0);
                }
            }

            StringBuilder gamma = new StringBuilder();
            StringBuilder epsilon = new StringBuilder();
            for (int i = 0; i < inputs.First().Length; ++i)
            {
                if (zeroes[i] > ones[i])
                {
                    gamma.Append(0);
                    epsilon.Append(1);
                }
                else
                {
                    gamma.Append(1);
                    epsilon.Append(0);
                }
            }
            return (Convert.ToInt32(gamma.ToString(), 2) * Convert.ToInt32(epsilon.ToString(), 2)).ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);
    }
}