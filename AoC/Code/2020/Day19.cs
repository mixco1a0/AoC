using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2020
{
    class Day19 : Day
    {
        public Day19() { }
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
                Output = "2",
                RawInput =
@"0: 4 1 5
1: 2 3 | 3 2
2: 4 4 | 5 5
3: 4 5 | 5 4
4: a
5: b

ababbb
bababa
abbbab
aaabbb
aaaabbb"
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

        class ParseRule
        {
            public string ID { get; set; }
            public string Rules { get; set; }
            public List<string> Possibles { get; set; }

            public ParseRule()
            {
                ID = string.Empty;
                Rules = string.Empty;
                Possibles = new List<string>();
            }

            public override string ToString()
            {
                return $"{ID} - {Rules}";
            }
        }

        class IterPair
        {
            public int Iter { get; set; }
            public List<string> Options { get; set; }
            public bool Done { get { return Iter >= Options.Count; } }
        }

        private void ExpandRule(ParseRule rule, IEnumerable<ParseRule> rules, ref HashSet<string> usedRules, ref List<string> valids)
        {
            if (rules.Count() == 0)
            {
                return;
            }

            if (!usedRules.Contains(rule.ID))
            {
                string[] split = rule.Rules.Split('|');
                string[] leftDeps = split[0].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                string[] rightDeps = null;
                string[] allDeps = leftDeps;
                if (split.Count() > 1)
                {
                    rightDeps = split[1].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    allDeps = allDeps.Union(rightDeps).Distinct().ToArray();
                }

                bool hasDeps = true;
                foreach (string dep in allDeps)
                {
                    if (!usedRules.Contains(dep))
                    {
                        int test;
                        if (int.TryParse(dep, out test))
                        {
                            ExpandRule(rules.Where(r => r.ID == dep).First(), rules, ref usedRules, ref valids);
                        }
                        else
                        {
                            hasDeps = false;
                            rule.Possibles.Add(dep);
                        }
                    }
                }

                if (hasDeps)
                {
                    List<List<string>> allOptions = new List<List<string>>();
                    for (int i = 0; i < leftDeps.Count(); ++i)
                    {
                        allOptions.Add(rules.Where(r => r.ID == leftDeps[i]).Select(r => r.Possibles).First());
                    }

                    List<string> curOptions = allOptions[0];
                    allOptions.RemoveAt(0);
                    while (allOptions.Count > 0)
                    {
                        curOptions = curOptions.SelectMany(ao0 => allOptions[0].Select(ao1 => $"{ao0}{ao1}")).ToList();
                        allOptions.RemoveAt(0);
                    }

                    rule.Possibles.AddRange(curOptions);
                    if (rightDeps != null)
                    {
                        allOptions.Clear();
                        for (int i = 0; i < rightDeps.Count(); ++i)
                        {
                            allOptions.Add(rules.Where(r => r.ID == rightDeps[i]).Select(r => r.Possibles).First());
                        }

                        curOptions = allOptions[0];
                        allOptions.RemoveAt(0);
                        while (allOptions.Count > 0)
                        {
                            curOptions = curOptions.SelectMany(ao0 => allOptions[0].Select(ao1 => $"{ao0}{ao1}")).ToList();
                            allOptions.RemoveAt(0);
                        }
                        rule.Possibles.AddRange(curOptions);
                        //rule.Possibles.AddRange(allOptions[0].SelectMany(ao0 => allOptions[1].Select(ao1 => $"{ao0}{ao1}")));
                    }
                    rule.Possibles = rule.Possibles.Distinct().ToList();
                }

                usedRules.Add(rule.ID);

                // if (rightDeps.Count() > 0)
                // {
                //     pairs.Clear();
                //     for (int i = 0; i < rightDeps.Count(); ++i)
                //     {
                //         pairs.Add(new IterPair { Iter = 0, Options = rules.Where(r => r.ID == rightDeps[i]).Select(r => r.Possibles).First() });
                //     }
                // }

                // foreach (string dep in leftDeps)
                // {
                //     parseRule.Possibles.Add
                // }
            }



            //ExpandRule(rules.Skip(1), ref usedRules, ref valids);
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            HashSet<string> checkedRules = new HashSet<string>();
            List<ParseRule> rules = new List<ParseRule>();
            List<string> valids = new List<string>();
            int validCount = 0;
            foreach (string input in inputs)
            {
                if (input.Contains(':'))
                {
                    // add raw rules
                    string[] split = input.Split(':');
                    rules.Add(new ParseRule { ID = split[0], Rules = split[1].Replace("\"", "") });
                }
                else if (string.IsNullOrWhiteSpace(input))
                {
                    // convert rules
                    rules = rules.OrderBy(pair => int.Parse(pair.ID)).ToList();
                    foreach (ParseRule rule in rules)
                    {
                        ExpandRule(rule, rules, ref checkedRules, ref valids);
                    }
                }
                else
                {
                    // check against rules
                    if (rules.Where(r => r.ID == "0").First().Possibles.Contains(input))
                    {
                        ++validCount;
                    }
                }
            }
            return validCount.ToString();
        }

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            return "";
        }
    }
}