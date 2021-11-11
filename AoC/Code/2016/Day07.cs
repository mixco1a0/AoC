using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2016
{
    class Day07 : Day
    {
        public Day07() { }
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
                Output = "3",
                RawInput =
@"abba[mnop]qrst
qrst[mnop]abba
abcd[bddb]xyyx
aaaa[qwer]tyui
ioxxoj[asdfgh]zxcvbn"
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

        private bool ContainsMirroredPair(string input)
        {
            for (int i = 0; i < input.Length - 3; ++i)
            {
                if (input[i] == input[i+3] && input[i+1] == input[i+2] && input[i] != input[i+1])
                {
                    return true;
                }
            }
            return false;
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables)
        {
            int tlsSupportCount = 0;
            foreach (string input in inputs)
            {
                bool supportsTLS = false;
                string[] split = input.Replace("[", "[|").Split("[]".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                foreach (string s in split)
                {
                    if (s.First() == '|' && ContainsMirroredPair(s[1..]))
                    {
                        supportsTLS = false;
                        break;
                    }

                    if (ContainsMirroredPair(s))
                    {
                        supportsTLS = true;
                    }
                }

                if (supportsTLS)
                {
                    ++tlsSupportCount;
                }
            }
            return tlsSupportCount.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);
    }
}