using System.Collections.Generic;
using System.Linq;

namespace _2020
{
    class Day07 : Day
    {
        public Day07() : base() { }

        protected override string GetDay() { return nameof(Day07).ToLower(); }

        protected override string GetPart1ExampleInput()
        {
            return
@"light red bags contain 1 bright white bag, 2 muted yellow bags.
dark orange bags contain 3 bright white bags, 4 muted yellow bags.
bright white bags contain 1 shiny gold bag.
muted yellow bags contain 2 shiny gold bags, 9 faded blue bags.
shiny gold bags contain 1 dark olive bag, 2 vibrant plum bags.
dark olive bags contain 3 faded blue bags, 4 dotted black bags.
vibrant plum bags contain 5 faded blue bags, 6 dotted black bags.
faded blue bags contain no other bags.
dotted black bags contain no other bags.";
        }
        protected override string GetPart1ExampleAnswer() { return "4"; }
        protected override string RunPart1Solution(List<string> inputs)
        {
            Dictionary<string, List<string>> bagsToInput = new Dictionary<string, List<string>>();
            foreach (string input in inputs)
            {
                string[] cur = input.Split("contain");
                string id = cur[0].Replace("bags", "").Replace(" ", "");
                string bags = cur[1].Replace("bags", "").Replace("bag", "").Replace(" ", "").Replace(".", "");
                List<string> bagList = bags.Split(",").ToList();
                if (bagList[0] == "noother")
                    continue;

                bagsToInput[id] = bagList.Select(color => color[1..]).ToList();
            }

            List<string> usedBags = new List<string>();
            List<string> curBagList = new List<string> { "shinygold" };
            for (; ; )
            {
                int returnVal = GetPossibleBags(bagsToInput, ref usedBags, ref curBagList);
                if (returnVal == 0)
                    break;
            }
            return usedBags.Count.ToString();
        }

        private int GetPossibleBags(Dictionary<string, List<string>> bagsToInput, ref List<string> usedBags, ref List<string> curBagList)
        {
            List<string> newBagList = new List<string>();
            foreach (string curBag in curBagList)
            {
                foreach (var pair in bagsToInput)
                {
                    if (pair.Value.Contains(curBag) && !usedBags.Contains(pair.Key))
                    {
                        newBagList.Add(pair.Key);
                        usedBags.Add(pair.Key);
                    }
                }
            }
            
            curBagList = newBagList;
            return newBagList.Count;
        }

        protected override string GetPart2ExampleInput()
        {
            return
@"";
        }
        protected override string GetPart2ExampleAnswer() { return ""; }
        protected override string RunPart2Solution(List<string> inputs)
        {
            return "";
        }
    }
}