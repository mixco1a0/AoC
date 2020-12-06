using System;
using System.Collections.Generic;

namespace _2020
{
    class Day05 : Day
    {
        public Day05() : base() {}
        
        protected override string GetDay() { return "day05"; }

        protected override void RunPart1Solution(List<string> inputs)
        {
            int highestId = 0;
            foreach (string input in inputs)
            {
                string row = input.Substring(0, 7).Replace('F', '0').Replace('B', '1');
                int rowVal = Convert.ToInt32(row, 2);
                
                string col = input.Substring(7).Replace('L', '0').Replace('R', '1');
                int colVal = Convert.ToInt32(col, 2);

                highestId = Math.Max(highestId, rowVal * 8 + colVal);
            }
            LogAnswer($"{highestId} highest id");
        }

        protected override void RunPart2Solution(List<string>inputs)
        {
            HashSet<int> ids = new HashSet<int>();
            foreach (string input in inputs)
            {
                string row = input.Substring(0, 7).Replace('F', '0').Replace('B', '1');
                int rowVal = Convert.ToInt32(row, 2);
                
                string col = input.Substring(7).Replace('L', '0').Replace('R', '1');
                int colVal = Convert.ToInt32(col, 2);

                ids.Add(rowVal * 8 + colVal);
            }
            foreach(int id in ids)
            {
                if (!ids.Contains(id + 1) && ids.Contains(id + 2))
                {
                    LogAnswer($"{id+1} my id");
                }
            }
        }
    }
}