using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2024
{
    class Day05 : Core.Day
    {
        public Day05() { }

        public override string GetSolutionVersion(Core.Part part)
        {
            return part switch
            {
                Core.Part.One => "v1",
                Core.Part.Two => "v1",
                _ => base.GetSolutionVersion(part),
            };
        }

        protected override List<Core.TestDatum> GetTestData()
        {
            List<Core.TestDatum> testData =
            [
                new Core.TestDatum
                {
                    TestPart = Core.Part.One,
                    Output = "143",
                    RawInput =
@"47|53
97|13
97|61
97|47
75|29
61|13
75|53
29|13
97|29
53|29
61|53
97|53
61|29
47|13
75|47
97|75
47|61
75|61
47|29
75|13
53|13

75,47,61,53,29
97,61,53,29,13
75,29,13
75,97,47,61,53
61,13,29
97,13,75,29,47"
                },
                new Core.TestDatum
                {
                    TestPart = Core.Part.Two,
                    Output = "123",
                    RawInput =
@"47|53
97|13
97|61
97|47
75|29
61|13
75|53
29|13
97|29
53|29
61|53
97|53
61|29
47|13
75|47
97|75
47|61
75|61
47|29
75|13
53|13

75,47,61,53,29
97,61,53,29,13
75,29,13
75,97,47,61,53
61,13,29
97,13,75,29,47"
                },
            ];
            return testData;
        }

        private class Pair(int a, int b) : Base.Pair<int, int>(a, b);

        private List<Pair> PageRules { get; set; }
        private List<List<int>> Pages { get; set; }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool processCorrectOnly)
        {
            PageRules = [];
            Pages = [];
            foreach (string input in inputs)
            {
                if (!string.IsNullOrWhiteSpace(input))
                {
                    if (input.Contains('|'))
                    {
                        IEnumerable<int> pair = Util.Number.Split(input, '|');
                        PageRules.Add(new(pair.First(), pair.Skip(1).First()));
                    }
                    else
                    {
                        Pages.Add(Util.Number.Split(input, ',').ToList());
                    }
                }
            }

            int sum = 0;
            foreach (List<int> pages in Pages)
            {
                bool isValid = true;
                IEnumerable<Pair> pageRules = PageRules.Where(pr => pages.Contains(pr.First) && pages.Contains(pr.Last));
                foreach (Pair pageRule in pageRules)
                {

                    if (pages.IndexOf(pageRule.First) > pages.IndexOf(pageRule.Last))
                    {
                        isValid = false;
                        break;
                    }
                }
                if (processCorrectOnly)
                {
                    if (isValid)
                    {
                        sum += pages[(pages.Count - 1) / 2];
                    }
                }
                else
                {
                    if (!isValid)
                    {
                        List<int> sortedPages = [.. pageRules.GroupBy(pr => pr.First).Select(pr => new Pair(pr.Key, pr.Count())).OrderByDescending(pair => pair.Last).Select(pair => pair.First)];
                        sortedPages.AddRange(pageRules.Select(pr => pr.Last).Where(pr => !sortedPages.Contains(pr)));
                        sum += sortedPages[(sortedPages.Count - 1) / 2];
                    }
                }
            }
            return sum.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);
    }
}