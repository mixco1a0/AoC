using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2020
{
    class Day16 : Day
    {
        public Day16() { }
        public override string GetSolutionVersion(TestPart testPart)
        {
            switch (testPart)
            {
                // case TestPart.One:
                //     return "v1";
                // case TestPart.Two:
                //     return "v1";
                default:
                    return base.GetSolutionVersion(testPart);
            }
        }
        protected override List<TestDatum> GetTestData()
        {
            List<TestDatum> testData = new List<TestDatum>();
            testData.Add(new TestDatum
            {
                TestPart = TestPart.One,
                Output = "71",
                RawInput =
@"class: 1-3 or 5-7
row: 6-11 or 33-44
seat: 13-40 or 45-50

your ticket:
7,1,14

nearby tickets:
7,3,47
40,4,50
55,2,20
38,6,12"
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
            List<MinMax> ranges = new List<MinMax>();
            bool myTicket = false;
            int invalids = 0;
            foreach (string input in inputs)
            {
                if (input.Contains("or"))
                {
                    string[] split = input.Split("abcdefghijklmnopqrstuvwxyz: ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    string[] lower = split[0].Split('-');
                    string[] higher = split[1].Split('-');
                    ranges.Add(new MinMax { Min = int.Parse(lower[0]), Max = int.Parse(lower[1]) });
                    ranges.Add(new MinMax { Min = int.Parse(higher[0]), Max = int.Parse(higher[1]) });
                }
                else if (input.Contains(","))
                {
                    if (!myTicket)
                    {
                        myTicket = true;
                        continue;
                    }

                    int[] split = input.Split(',').Select(int.Parse).ToArray();
                    for (int i = 0; i < split.Length; ++i)
                    {
                        if (ranges.Where(range => range.GTE_LTE(split[i])).Count() <= 0)
                            invalids += split[i];
                    }
                }
            }
            return invalids.ToString();
        }

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            return "";
        }
    }
}