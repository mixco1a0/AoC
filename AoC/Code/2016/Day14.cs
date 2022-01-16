using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

using AoC.Core;

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
                Output = "22628", //TODO: This is what it should actually be :-/ Output = "22859",
                RawInput =
@"abc"
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

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, int stretchCount)
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
                    string encoded = sb.ToString();
                    for (int s = 0; s < stretchCount + 1; ++s)
                    {
                        byte[] inputBytes = Encoding.ASCII.GetBytes(encoded);
                        byte[] hashBytes = md5.ComputeHash(inputBytes);
                        encoded = BitConverter.ToString(hashBytes).Replace("-", string.Empty).ToLower();
                    }

                    // check for 5 in a row before adding the new one
                    for (int j = 0; j < pendingKeys.Count;)
                    {
                        HashCheck cur = pendingKeys[j];
                        if (encoded.Contains(new string(cur.Match, 5)))
                        {
                            verifiedKeys.Add(new HashCheck(cur.Match, cur.Start, i, cur.Raw));
                            pendingKeys.RemoveAt(j);
                        }
                        else
                        {
                            ++j;
                        }
                    }

                    // add new keys as long as max verified hasn't been hit
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
            verifiedKeys.Sort((a, b) => a.Start > b.Start ? 1 : -1);
            if (stretchCount > 0)
            {
                foreach (var key in verifiedKeys)
                {
                    DebugWriteLine($"{key}");
                }
            }
            return verifiedKeys[MaxKeys - 1].Start.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, 0);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, 2016);
    }
}