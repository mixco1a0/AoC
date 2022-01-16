using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

using AoC.Core;

namespace AoC._2015
{
    class Day04 : Day
    {
        public Day04() { }

        public override string GetSolutionVersion(Part part)
        {
            switch (part)
            {
                case Part.One:
                    return "v2";
                case Part.Two:
                    return "v2";
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
                Output = "609043",
                RawInput =
@"abcdef"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.One,
                Output = "1048970",
                RawInput =
@"pqrstuv"
            });
            return testData;
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, int leadingZeroes)
        {
            string zeroes = new string('0', leadingZeroes);
            string input = inputs[0];
            using (MD5 md5 = MD5.Create())
            {
                for (int i = 0; i < int.MaxValue; ++i)
                {
                    StringBuilder sb = new StringBuilder(input);
                    sb.Append(i);
                    byte[] inputBytes = Encoding.ASCII.GetBytes(sb.ToString());
                    byte[] hashBytes = md5.ComputeHash(inputBytes);
                    string md5String = BitConverter.ToString(hashBytes).Replace("-", string.Empty).ToLower();
                    if (md5String.StartsWith(zeroes))
                    {
                        return i.ToString();
                    }
                }
            }

            return string.Empty;
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, 5);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, 6);
    }
}