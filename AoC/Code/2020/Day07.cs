using System.Collections.Generic;
using System.Linq;

namespace AoC._2020
{
    class Day07 : Day
    {
        public Day07() { }

        protected override List<TestDatum> GetTestData()
        {
            List<TestDatum> testData = new List<TestDatum>();
            testData.Add(new TestDatum
            {
                TestPart = TestPart.One,
                Output = "4",
                RawInput =
@"light red bags contain 1 bright white bag, 2 muted yellow bags.
dark orange bags contain 3 bright white bags, 4 muted yellow bags.
bright white bags contain 1 shiny gold bag.
muted yellow bags contain 2 shiny gold bags, 9 faded blue bags.
shiny gold bags contain 1 dark olive bag, 2 vibrant plum bags.
dark olive bags contain 3 faded blue bags, 4 dotted black bags.
vibrant plum bags contain 5 faded blue bags, 6 dotted black bags.
faded blue bags contain no other bags.
dotted black bags contain no other bags."
            });
            testData.Add(new TestDatum
            {
                TestPart = TestPart.Two,
                Output = "32",
                RawInput =
@"light red bags contain 1 bright white bag, 2 muted yellow bags.
dark orange bags contain 3 bright white bags, 4 muted yellow bags.
bright white bags contain 1 shiny gold bag.
muted yellow bags contain 2 shiny gold bags, 9 faded blue bags.
shiny gold bags contain 1 dark olive bag, 2 vibrant plum bags.
dark olive bags contain 3 faded blue bags, 4 dotted black bags.
vibrant plum bags contain 5 faded blue bags, 6 dotted black bags.
faded blue bags contain no other bags.
dotted black bags contain no other bags."
            });
            testData.Add(new TestDatum
            {
                TestPart = TestPart.Two,
                Output = "126",
                RawInput =
@"shiny gold bags contain 2 dark red bags.
dark red bags contain 2 dark orange bags.
dark orange bags contain 2 dark yellow bags.
dark yellow bags contain 2 dark green bags.
dark green bags contain 2 dark blue bags.
dark blue bags contain 2 dark violet bags.
dark violet bags contain no other bags."
            });
            return testData;
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
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

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            Dictionary<string, List<string>> bagsToInput = new Dictionary<string, List<string>>();
            Dictionary<string, List<int>> bagsToCount = new Dictionary<string, List<int>>();
            foreach (string input in inputs)
            {
                string[] cur = input.Split("contain");
                string id = cur[0].Replace("bags", "").Replace(" ", "");
                string bags = cur[1].Replace("bags", "").Replace("bag", "").Replace(" ", "").Replace(".", "");
                List<string> bagList = bags.Split(",").ToList();
                if (bagList[0] == "noother")

                {
                    bagsToInput[id] = new List<string>();
                    bagsToCount[id] = new List<int>();
                }
                else
                {
                    bagsToInput[id] = bagList.Select(color => color[1..]).ToList();
                    bagsToCount[id] = bagList.Select(color => int.Parse(color[0..1])).ToList();
                }
            }

            int totalCount = GetPossibleBagCount(bagsToInput, bagsToCount, "shinygold", 1) - 1;
            return totalCount.ToString();
        }

        private int GetPossibleBagCount(Dictionary<string, List<string>> bagsToInput, Dictionary<string, List<int>> bagsToCount, string curBag, int myCount)
        {
            int count = myCount;
            if (bagsToInput[curBag].Count == 0)
            {
                //Debug($"{curBag} has no other bags");
            }
            else
            {
                for (int i = 0; i < bagsToInput[curBag].Count; ++i)
                {
                    int bagCount = GetPossibleBagCount(bagsToInput, bagsToCount, bagsToInput[curBag][i], bagsToCount[curBag][i]);
                    count += bagCount * myCount;
                }
            }
            return count;
        }
    }
}