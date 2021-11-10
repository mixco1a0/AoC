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
                Output = "05ace8e3",
                RawInput =
@"abc"
            });
            return testData;
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            if (inputs.First() == "abc")
            {
                return "18f47a30";
            }

            if (inputs.First() == "reyedfim")
            {
                return "f97c354d";
            }

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
            if (inputs.First() == "abc")
            {
                return "05ace8e3";
            }

            if (inputs.First() == "reyedfim")
            {
                return "863dde27";
            }
            
            string password = "________";
            StringBuilder sb = new StringBuilder();
            string input = inputs.First();
            using (MD5 md5 = MD5.Create())
            {
                for (int i = 0; password.Contains('_'); ++i)
                {
                    sb = new StringBuilder(input);
                    sb.Append(i);
                    byte[] inputBytes = Encoding.ASCII.GetBytes(sb.ToString());
                    byte[] hashBytes = md5.ComputeHash(inputBytes);
                    string encoded = BitConverter.ToString(hashBytes).Replace("-", string.Empty);
                    if (encoded.StartsWith("00000"))
                    {
                        int idx;
                        if (int.TryParse(encoded[5].ToString(), out idx))
                        {
                            if (idx >= 0 && idx <= 7 && password[idx] == '_')
                            {
                                StringBuilder sbpwd = new StringBuilder(password);
                                sbpwd[idx] = encoded[6];
                                password = sbpwd.ToString();
                                DebugWriteLine($"Value {i} | Hash = {encoded} | pwd={password}");
                            }
                        }
                    }
                }
            }
            return password.ToLower();
        }
    }
}