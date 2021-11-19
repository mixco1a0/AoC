using System.Text;
using System.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2016
{
    class Day14 : Day
    {
        public Day14() { }
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
                Output = "22728",
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

        private char InvalidChar = '-';

        private record HashCheck(char Match, long Start, long End, string Raw) { }

        private char GetNthMatchedCharacter(string hash, int n)
        {
            for (int i = 0; i + n <= hash.Length; ++i)
            {
                string cur = hash.Substring(i, n);
                if (cur.Length == 3 && string.IsNullOrEmpty(cur.Replace($"{cur[0]}", "")))
                {
                    return cur[0];
                }
            }

            return InvalidChar;
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables)
        {
            const int MaxKeys = 64;
            string input = inputs.First();
            List<HashCheck> verifiedKeys = new List<HashCheck>();
            List<HashCheck> pendingKeys = new List<HashCheck>();
            using (MD5 md5 = MD5.Create())
            {
                for (long i = 0; verifiedKeys.Count < MaxKeys || pendingKeys.Count > 0; ++i)
                {
                    StringBuilder sb = new StringBuilder(input);
                    sb.Append(i);
                    byte[] inputBytes = Encoding.ASCII.GetBytes(sb.ToString());
                    byte[] hashBytes = md5.ComputeHash(inputBytes);
                    string encoded = BitConverter.ToString(hashBytes).Replace("-", string.Empty).ToLower();

                    // check for 5 in a row before adding the new one
                    for (int j = 0; j < pendingKeys.Count;)
                    {
                        HashCheck cur = pendingKeys[j];
                        if (encoded.Contains(new string(cur.Match, 5)))
                        {
                            verifiedKeys.Add(cur);
                            verifiedKeys.Sort((a, b) => a.Start > b.Start ? 1 : -1);
                            pendingKeys.RemoveAt(j);
                        }
                        else
                        {
                            ++j;
                        }
                    }

                    // add new keys as long as max varified hasn't been hit
                    if (verifiedKeys.Count < MaxKeys)
                    {
                        char threeTimesMatch = GetNthMatchedCharacter(encoded, 3);
                        if (threeTimesMatch != InvalidChar)
                        {
                            pendingKeys.Add(new HashCheck(threeTimesMatch, i, i + 1000, encoded));
                        }
                    }

                    // remove stale keys
                    pendingKeys.RemoveAll(p => p.End <= i);
                }
            }
            foreach (var key in verifiedKeys)
            {
                DebugWriteLine($"{key.Raw}");
            }
            return verifiedKeys[MaxKeys - 1].Start.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);
    }
}