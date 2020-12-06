
using System.Collections.Generic;
using System.Linq;

namespace _2020
{
    class Day06 : Day
    {
        public Day06() : base() {}
        
        protected override string GetDay() { return nameof(Day06).ToLower(); }

        protected override void RunPart1Solution(List<string> inputs)
        {
            int count = 0;
            string groupInput = "";
            foreach (string input in inputs)
            {
                if (string.IsNullOrWhiteSpace(input))
                {
                    // group end
                    count += groupInput.ToCharArray().Select(c => c.ToString()).Distinct().Count();
                    groupInput = "";
                    continue;
                }

                groupInput += input;
            }
            count += groupInput.ToCharArray().Select(c => c.ToString()).Distinct().Count();
            LogAnswer($"{count}");
        }

        protected override void RunPart2Solution(List<string>inputs)
        {
            LogAnswer("NA");
        }
    }
}