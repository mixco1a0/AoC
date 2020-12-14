using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2020
{
    class Day14 : Day
    {
        public Day14() { }
        protected override List<TestDatum> GetTestData()
        {
            List<TestDatum> testData = new List<TestDatum>();
            testData.Add(new TestDatum
            {
                TestPart = TestPart.One,
                Output = "165",
                RawInput =
@"mask = XXXXXXXXXXXXXXXXXXXXXXXXXXXXX1XXXX0X
mem[8] = 11
mem[7] = 101
mem[8] = 0"
            });
            testData.Add(new TestDatum
            {
                TestPart = TestPart.Two,
                Output = "208",
                RawInput =
@"mask = 000000000000000000000000000000X1001X
mem[42] = 100
mask = 00000000000000000000000000000000X0XX
mem[26] = 1"
            });
            return testData;
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            Dictionary<string, string> memory = new Dictionary<string, string>();
            List<KeyValuePair<char, int>> masks = new List<KeyValuePair<char, int>>();
            foreach (string input in inputs)
            {
                if (input.Contains("mask"))
                {
                    List<string> split = input.Split(" =".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();

                    masks = split[1].ToCharArray().Select((digit, index) => new { Digit = digit, Index = index }).Where(pair => pair.Digit != 'X').Select(pair => new KeyValuePair<char, int>(pair.Digit, pair.Index)).ToList();
                    // set mask
                }
                else
                {
                    List<string> split = input.Split(" []=".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                    string val = Convert.ToString(long.Parse(split[2]), 2).ToString().PadLeft(36, '0');
                    char[] chars = val.ToCharArray();
                    foreach (var pair in masks)
                    {
                        chars[pair.Value] = pair.Key;
                    }
                    memory[split[1]] = string.Join("", chars);
                }
            }

            long sum = 0;
            foreach (var pair in memory)
            {
                sum += Convert.ToInt64(pair.Value, 2);
            }
            return sum.ToString();
        }

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            Dictionary<string, long> memory = new Dictionary<string, long>();
            List<KeyValuePair<char, int>> masks = new List<KeyValuePair<char, int>>();
            foreach (string input in inputs)
            {
                if (input.Contains("mask"))
                {
                    List<string> split = input.Split(" =".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();

                    masks = split[1].ToCharArray().Select((digit, index) => new { Digit = digit, Index = index }).Select(pair => new KeyValuePair<char, int>(pair.Digit, pair.Index)).ToList();
                    // set mask
                }
                else
                {
                    List<string> split = input.Split(" []=".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                    char[] memAddress = Convert.ToString(long.Parse(split[1]), 2).ToString().PadLeft(36, '0').ToCharArray();
                    char[] chars = Convert.ToString(long.Parse(split[1]), 2).ToString().PadLeft(36, '0').ToCharArray();
                    foreach (var pair in masks)
                    {
                        if (pair.Key == '0')
                            continue;
                        chars[pair.Value] = pair.Key;

                        if (pair.Key == '1')
                        memAddress[pair.Value] = '1';
                    }

                    var allXs = chars.Select((c, idx) => new { Letter = c, Index = idx }).Where(pair => pair.Letter == 'X').Select(pair => new KeyValuePair<char, int>(pair.Letter, pair.Index)).ToList();
                    long max = (long)Math.Pow(2, allXs.Count);
                    for (int i = 0; i < max; ++i)
                    {
                        string curReplace = Convert.ToString(i, 2).PadLeft(allXs.Count, '0');
                        char[] curAddress = string.Join("", memAddress).ToCharArray();
                        for (int j = 0; j < curReplace.Length; ++j)
                        {
                          curAddress[allXs[j].Value] = curReplace[j];
                        }
                        memory[string.Join("", curAddress)] = long.Parse(split[2]);
                    }
                }
            }

            long sum = 0;
            foreach (var pair in memory)
            {
                sum += pair.Value;
            }
            return sum.ToString();
        }
    }
}