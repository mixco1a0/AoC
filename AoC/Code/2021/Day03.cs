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
                Output = "230",
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
            return testData;
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool useLifeSupportRating)
        {
            int[] zeroes = new int[inputs.First().Length];
            int[] ones = new int[inputs.First().Length];
            if (useLifeSupportRating)
            {
                StringBuilder mostCommon = new StringBuilder();
                StringBuilder leastCommon = new StringBuilder();
                for (int a = 0; a < inputs.First().Length; ++a)
                {
                    int zero = 0;
                    int one = 0;
                    IEnumerable<string> remaining;
                    if (mostCommon.Length < inputs.First().Length)
                    {
                        remaining = inputs.Where(input => input.StartsWith(mostCommon.ToString()));
                        if (remaining.Count() > 1)
                        {
                            foreach (string input in remaining)
                            {
                                zero += (input[a] == '0' ? 1 : 0);
                                one += (input[a] == '1' ? 1 : 0);
                            }

                            if (zero > one)
                            {
                                mostCommon.Append(0);
                            }
                            else if (one >= zero)
                            {
                                mostCommon.Append(1);
                            }
                        }
                        else
                        {
                            string temp = remaining.First();
                            mostCommon.Clear();
                            mostCommon.Append(temp);
                        }
                    }

                    if (leastCommon.Length < inputs.First().Length)
                    {
                        remaining = inputs.Where(input => input.StartsWith(leastCommon.ToString()));
                        if (remaining.Count() > 1)
                        {
                            zero = 0;
                            one = 0;
                            foreach (string input in remaining)
                            {
                                zero += (input[a] == '0' ? 1 : 0);
                                one += (input[a] == '1' ? 1 : 0);
                            }

                            if (zero == one)
                            {
                                leastCommon.Append(0);
                            }
                            else if (zero > one)
                            {
                                leastCommon.Append(1);
                            }
                            else if (one > zero)
                            {
                                leastCommon.Append(0);
                            }
                        }
                        else
                        {
                            string temp = remaining.First();
                            leastCommon.Clear();
                            leastCommon.Append(temp);
                        }
                    }
                }

                return (Convert.ToInt32(mostCommon.ToString(), 2) * Convert.ToInt32(leastCommon.ToString(), 2)).ToString();
            }
            else
            {
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
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);
    }
}