using System;
using System.Collections.Generic;

namespace AoC._2020
{
    class Day05 : Day
    {
        public Day05() : base("2020") { }

        protected override string GetDay() { return "day05"; }

        protected override List<TestDatum> GetTestData()
        {
            List<TestDatum> testData = new List<TestDatum>();
            testData.Add(new TestDatum { TestPart = TestPart.One, Output = "357", RawInput = "FBFBBFFRLR" });
            return testData;
        }

        protected override string RunPart1Solution(List<string> inputs)
        {
            int highestId = 0;
            foreach (string input in inputs)
            {
                string binary = input.Replace('F', '0').Replace('B', '1').Replace('L', '0').Replace('R', '1');
                highestId = Math.Max(highestId, Convert.ToInt32(binary, 2));
            }
            return highestId.ToString();
        }

        protected override string RunPart2Solution(List<string> inputs)
        {
            HashSet<int> ids = new HashSet<int>();
            foreach (string input in inputs)
            {
                string binary = input.Replace('F', '0').Replace('B', '1').Replace('L', '0').Replace('R', '1');
                ids.Add(Convert.ToInt32(binary, 2));
            }
            foreach (int id in ids)
            {
                if (!ids.Contains(id + 1) && ids.Contains(id + 2))
                {
                    return $"{id + 1}";
                }
            }

            return "";
        }
    }
}