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
                Output = "",
                RawInput =
@""
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
            return "";
        }
    }
}