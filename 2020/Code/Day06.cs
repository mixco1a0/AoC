
using System.Collections.Generic;
using System.Linq;

namespace _2020
{
    class Day06 : Day
    {
        public Day06() : base() { }

        protected override string GetDay() { return nameof(Day06).ToLower(); }

        protected override string GetPart1ExampleInput()
        {
            return
@"abc

a
b
c

ab
ac

a
a
a
a

b";
        }
        protected override string GetPart1ExampleAnswer() { return "11"; }
        protected override string RunPart1Solution(List<string> inputs)
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
            return count.ToString();
        }

        protected override string GetPart2ExampleInput()
        {
            return GetPart1ExampleInput();
        }
        protected override string GetPart2ExampleAnswer() { return "6"; }
        protected override string RunPart2Solution(List<string> inputs)
        {
            int count = 0;
            List<string> sharedInput = new List<string>();
            bool newGroup = true;
            foreach (string input in inputs)
            {
                if (string.IsNullOrWhiteSpace(input))
                {
                    // group end
                    count += sharedInput.Count();
                    sharedInput.Clear();
                    newGroup = true;
                    continue;
                }

                var cur = input.ToCharArray().Select(c => c.ToString());
                if (newGroup)
                {
                    sharedInput = cur.ToList();
                    newGroup = false;
                }
                else
                {
                    sharedInput = sharedInput.Intersect(cur).ToList();
                }

            }
            count += sharedInput.Count();
            return count.ToString();
        }
    }
}