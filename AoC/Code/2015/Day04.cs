using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace AoC._2015
{
    class Day04 : Core.Day
    {
        public Day04() { }

        public override string GetSolutionVersion(Core.Part part)
        {
            switch (part)
            {
                case Core.Part.One:
                    return "v3";
                case Core.Part.Two:
                    return "v3";
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
                Output = "609043",
                RawInput =
@"abcdef"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.One,
                Output = "1048970",
                RawInput =
@"pqrstuv"
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

        private bool HasLeadingZeroes(byte[] hashBytes, int leadingZeroes)
        {
            for (int z = 0; z < leadingZeroes; ++z)
            {
                byte mask = (z % 2 == 0) ? (byte)0xf0 : (byte)0x0f;
                if ((hashBytes[z / 2] & mask) != 0x00)
                {
                    return false;
                }
            }
            return true;
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, int leadingZeroes)
        {
            string input = inputs[0];
            using (MD5 md5 = MD5.Create())
            {
                for (int i = 0; i < int.MaxValue; ++i)
                {
                    StringBuilder sb = new StringBuilder(input);
                    sb.Append(i);
                    byte[] inputBytes = Encoding.ASCII.GetBytes(sb.ToString());
                    byte[] hashBytes = md5.ComputeHash(inputBytes);
                    // byte[] hashBytes = null;
                    // Action compute = () =>
                    // {
                    //     hashBytes = md5.ComputeHash(inputBytes);
                    // };
                    // WasteTime(compute);
                    if (HasLeadingZeroes(hashBytes, leadingZeroes))
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