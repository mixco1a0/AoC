using System;
using System.Collections.Generic;

namespace _2020
{
    class Day05 : Day
    {
        public Day05() : base() { }

        protected override string GetDay() { return "day05"; }

        protected override string GetPart1ExampleInput()
        {
            return @"FBFBBFFRLR";
        }
        protected override string GetPart1ExampleAnswer() { return "357"; }
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

        protected override string GetPart2ExampleInput()
        {
            return @"";
        }
        protected override string GetPart2ExampleAnswer() { return ""; }
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