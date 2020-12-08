using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2015
{
    class Day04 : Day
    {
        public Day04() { }

        protected override List<TestDatum> GetTestData()
        {
            List<TestDatum> testData = new List<TestDatum>();
            testData.Add(new TestDatum
            {
                TestPart = TestPart.One,
                Output = "609043",
                RawInput =
@"abcdef"
            });
            testData.Add(new TestDatum
            {
                TestPart = TestPart.One,
                Output = "1048970",
                RawInput =
@"pqrstuv"
            });
            return testData;
        }

        protected override string RunPart1Solution(List<string> inputs)
        {
            string input = inputs[0];
            using (MD5 md5 = MD5.Create())
            {
                for (int i = 0; i < int.MaxValue; ++i)
                {
                    string temp = $"{input}{i}";
                    byte[] tempBytes = System.Text.Encoding.ASCII.GetBytes(temp);
                    byte[] hashBytes = md5.ComputeHash(tempBytes);
                    string md5String = string.Join("", hashBytes.Select(b => b.ToString("X2")));
                    if (md5String[0..5] == "00000")
                    {
                        return i.ToString();
                    }
                }
            }

            return "NaN";
        }

        protected override string RunPart2Solution(List<string> inputs)
        {
            string input = inputs[0];
            using (MD5 md5 = MD5.Create())
            {
                for (int i = 0; i < int.MaxValue; ++i)
                {
                    string temp = $"{input}{i}";
                    byte[] tempBytes = System.Text.Encoding.ASCII.GetBytes(temp);
                    byte[] hashBytes = md5.ComputeHash(tempBytes);
                    string md5String = string.Join("", hashBytes.Select(b => b.ToString("X2")));
                    if (md5String[0..6] == "000000")
                    {
                        return i.ToString();
                    }
                }
            }

            return "NaN";
        }
    }
}