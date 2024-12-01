using System;
using System.Collections.Generic;

namespace AoC._2020
{
    class Day05 : Core.Day
    {
        public Day05() { }
        
        public override string GetSolutionVersion(Core.Part part)
        {
            switch (part)
            {
                case Core.Part.One:
                    return "v1";
                case Core.Part.Two:
                    return "v1";
                default:
                    return base.GetSolutionVersion(part);
            }
        }

        protected override List<Core.TestDatum> GetTestData()
        {
            List<Core.TestDatum> testData = new List<Core.TestDatum>();
            testData.Add(new Core.TestDatum { TestPart = Core.Part.One, Output = "357", RawInput = "FBFBBFFRLR" });
            return testData;
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            int highestId = 0;
            foreach (string input in inputs)
            {
                string binary = input.Replace('F', '0').Replace('B', '1').Replace('L', '0').Replace('R', '1');
                highestId = Math.Max(highestId, Convert.ToInt32(binary, 2));
            }
            return highestId.ToString();
        }

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
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