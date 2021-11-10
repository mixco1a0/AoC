using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2016
{
    class Day05 : Day
    {
        public Day05() { }
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
                Output = "18f47a30",
                RawInput =
@"abc"
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

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            string password = string.Empty;
            StringBuilder sb = new StringBuilder();
            string input = inputs.First();
            using (MD5 md5 = MD5.Create())
            {
                for (int i = 0, curIdx = 0; true; ++i)
                {
                    if (curIdx >= 8)
                    {
                        break;
                    }

                    sb = new StringBuilder(input);
                    sb.Append(i);
                    byte[] inputBytes = Encoding.ASCII.GetBytes(sb.ToString());
                    byte[] hashBytes = md5.ComputeHash(inputBytes);
                    string encoded = BitConverter.ToString(hashBytes).Replace("-", string.Empty);
                    if (encoded.StartsWith("00000"))
                    {
                        curIdx++;
                        password += encoded[5];
                    }
                }
            }
            return password.ToLower();
        }

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            return "";
        }
    }
}