using System.Xml;
using System.Diagnostics;
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
@"42: 9 14 | 10 1
9: 14 27 | 1 26
10: 23 14 | 28 1
1: a
11: 42 31
5: 1 14 | 15 1
19: 14 1 | 14 14
12: 24 14 | 19 1
16: 15 1 | 14 14
31: 14 17 | 1 13
6: 14 14 | 1 14
2: 1 24 | 14 4
0: 8 11
13: 14 3 | 1 12
15: 1 | 14
17: 14 2 | 1 7
23: 25 1 | 22 14
28: 16 1
4: 1 1
20: 14 14 | 1 15
3: 5 14 | 16 1
27: 1 6 | 14 18
14: b
21: 14 1 | 1 14
25: 1 1 | 1 14
22: 14 14
8: 42
26: 14 22 | 1 20
18: 15 15
7: 14 5 | 1 21
24: 14 1

aaabbbbbbaaaabaababaabababbabaaabbababababaaa
abbbbbabbbaaaababbaabbbbabababbbabbbbbbabaaaa
bbabbbbaabaabba
babbbbaabbbbbabbbbbbaabaaabaaa
aaabbbbbbaaaabaababaabababbabaaabbababababaaa
bbbbbbbaaaabbbbaaabbabaaa
bbbababbbbaaaaaaaabbababaaababaabab
ababaaaaaabaaab
ababaaaaabbbaba
baabbaaaabbaaaababbaababb
abbbbabbbbaaaababbbbbbaaaababb
aaaaabbaabaaaaababaa
aaaabbaaaabbaaa
aaaabbaabbaaaaaaabbbabbbaaabbaabaaa
babaaabbbaaabaababbaabababaaab
aabbbbbaabbbaaaaaabbbbbababaaaaabbaaabba"
            });
            return testData;
        }

        // class ParseRule
        // {
        //     public string ID { get; set; }
        //     public string Rules { get; set; }
        //     public List<string> Possibles { get; set; }
        //     public List<string> SpecialCase { get; set; }

        //     public ParseRule()
        //     {
        //         ID = string.Empty;
        //         Rules = string.Empty;
        //         Possibles = new List<string>();
        //         SpecialCase = new List<string>();
        //     }

        //     public override string ToString()
        //     {
        //         return $"{ID} - {Rules}";
        //     }
        // }

        // class IterPair
        // {
        //     public int Iter { get; set; }
        //     public List<string> Options { get; set; }
        //     public bool Done { get { return Iter >= Options.Count; } }
        // }

        // private void ExpandRule(ParseRule rule, IEnumerable<ParseRule> rules, ref HashSet<string> usedRules, ref List<string> valids)
        // {
        //     if (rules.Count() == 0)
        //     {
        //         return;
        //     }

        //     if (!usedRules.Contains(rule.ID))
        //     {
        //         string[] split = rule.Rules.Split('|');
        //         string[] leftDeps = split[0].Split(' ', StringSplitOptions.RemoveEmptyEntries);
        //         string[] rightDeps = null;
        //         string[] allDeps = leftDeps;
        //         if (split.Count() > 1)
        //         {
        //             rightDeps = split[1].Split(' ', StringSplitOptions.RemoveEmptyEntries);
        //             allDeps = allDeps.Union(rightDeps).Distinct().ToArray();
        //         }

        //         bool hasDeps = true;
        //         foreach (string dep in allDeps)
        //         {
        //             if (!usedRules.Contains(dep))
        //             {
        //                 int test;
        //                 if (int.TryParse(dep, out test))
        //                 {
        //                     ExpandRule(rules.Where(r => r.ID == dep).First(), rules, ref usedRules, ref valids);
        //                 }
        //                 else
        //                 {
        //                     hasDeps = false;
        //                     rule.Possibles.Add(dep);
        //                 }
        //             }
        //         }

        //         if (hasDeps)
        //         {
        //             List<List<string>> allOptions = new List<List<string>>();
        //             for (int i = 0; i < leftDeps.Count(); ++i)
        //             {
        //                 allOptions.Add(rules.Where(r => r.ID == leftDeps[i]).Select(r => r.Possibles).First());
        //             }

        //             List<string> curOptions = allOptions[0];
        //             allOptions.RemoveAt(0);
        //             while (allOptions.Count > 0)
        //             {
        //                 curOptions = curOptions.SelectMany(ao0 => allOptions[0].Select(ao1 => $"{ao0}{ao1}")).ToList();
        //                 allOptions.RemoveAt(0);
        //             }

        //             rule.Possibles.AddRange(curOptions);
        //             if (rightDeps != null)
        //             {
        //                 allOptions.Clear();
        //                 for (int i = 0; i < rightDeps.Count(); ++i)
        //                 {
        //                     allOptions.Add(rules.Where(r => r.ID == rightDeps[i]).Select(r => r.Possibles).First());
        //                 }

        //                 curOptions = allOptions[0];
        //                 allOptions.RemoveAt(0);
        //                 while (allOptions.Count > 0)
        //                 {
        //                     curOptions = curOptions.SelectMany(ao0 => allOptions[0].Select(ao1 => $"{ao0}{ao1}")).ToList();
        //                     allOptions.RemoveAt(0);
        //                 }
        //                 rule.Possibles.AddRange(curOptions);
        //             }
        //             rule.Possibles = rule.Possibles.Distinct().ToList();
        //         }

        //         usedRules.Add(rule.ID);
        //     }
        // }

        class Node
        {
            public string ID { get; set; }
            public string RawRules { get; set; }
            public string Value { get; set; }
            public List<List<string>> SubRules { get; set; }
            public List<List<Node>> Sequences { get; set; }
            public Action<string> PrintFunc { get; set; }
            public Node()
            {
                SubRules = new List<List<string>>();
                Sequences = new List<List<Node>>();
            }

            public void Populate(ref List<Node> nodes, Action<string> printFunc)
            {
                PrintFunc = printFunc;
                string[] ruleSplit = RawRules.Split('|', StringSplitOptions.RemoveEmptyEntries);
                foreach (String curSplit in ruleSplit)
                {
                    IEnumerable<string> ids = curSplit.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    int intTest;
                    if (!int.TryParse(ids.ElementAt(0), out intTest))
                    {
                        Value = ids.ElementAt(0);
                        continue;
                    }

                    SubRules.Add(new List<string>());
                    SubRules.Last().AddRange(ids);

                    Sequences.Add(new List<Node>());
                    foreach (string id in ids)
                    {
                        Node curNode = nodes.Where(n => n.ID == id).First();
                        Sequences.Last().Add(curNode);
                    }
                }
            }

            public int GetMatchingLength(string input, int curLetterIndex)
            {
                PrintFunc($"{new string('*', curLetterIndex)}{ToString()}");

                if (Sequences.Count == 0)
                {
                    bool match = input[curLetterIndex..].First() == Value.First();
                    string pre = curLetterIndex > 0 ? input.Substring(0, curLetterIndex) : "";
                    string post = curLetterIndex < input.Length - 1 ? input.Substring(curLetterIndex + 1) : "";
                    string curMatching = $"{pre}[{input.ElementAt(curLetterIndex)}]{post}";
                    string matchString = match ? "==" : "!=";
                    PrintFunc($"TESTING - {curMatching}  {matchString}  {Value.First()}");
                    return match ? 1 : 0;
                }

                int sequenceMatch = 0;
                int sequenceRunningTotal = 0;
                int curLetterIndexReset = curLetterIndex;
                foreach (List<Node> sequence in Sequences)
                {
                    sequenceRunningTotal = 0;
                    foreach (Node node in sequence)
                    {
                        int matchLength = node.GetMatchingLength(input, curLetterIndex);
                        if (matchLength > 0)
                        {
                            ++sequenceMatch;

                            sequenceRunningTotal += matchLength;
                            curLetterIndex += matchLength;
                            // PrintFunc($"MATCHED - {input[..curLetterIndex]}");
                        }
                        else
                        {
                            // PrintFunc($"FAILED - {node.ToString()}");
                            // sequence is dead, try the next sequence
                            break;
                        }
                    }
                    if (sequenceMatch == sequence.Count)
                    {
                        return sequenceRunningTotal;
                    }
                }

                return 0;
            }

            public override string ToString()
            {
                return $"{ID} => {RawRules}";
            }
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            int validCount = 0;
            List<Node> nodes = new List<Node>();
            foreach (string input in inputs)
            {
                if (input.Contains(':'))
                {
                    // add raw rules
                    string[] split = input.Split(':');
                    nodes.Add(new Node { ID = split[0], RawRules = split[1].Replace("\"", "") });
                }
                else if (string.IsNullOrWhiteSpace(input))
                {
                    foreach (Node node in nodes)
                    {
                        node.Populate(ref nodes, DebugWriteLine);
                    }
                    // convert rules
                    // rules = rules.OrderBy(pair => int.Parse(pair.ID)).ToList();
                    // foreach (ParseRule rule in rules)
                    // {
                    //     //ExpandRule(rule, rules, ref checkedRules, ref valids);
                    // }
                }
                else
                {
                    Node node0 = nodes.Where(n => n.ID == "0").First();
                    if (node0.GetMatchingLength(input, 0) == input.Length)
                    {
                        ++validCount;
                    }
                    // check against rules
                    // if (rules.Where(r => r.ID == "0").First().Possibles.Contains(input))
                    // {
                    // }
                }
            }
            return validCount.ToString();

            // HashSet<string> checkedRules = new HashSet<string>();
            // List<ParseRule> rules = new List<ParseRule>();
            // List<string> valids = new List<string>();
            // int validCount = 0;
            // foreach (string input in inputs)
            // {
            //     if (input.Contains(':'))
            //     {
            //         // add raw rules
            //         string[] split = input.Split(':');
            //         rules.Add(new ParseRule { ID = split[0], Rules = split[1].Replace("\"", "") });
            //     }
            //     else if (string.IsNullOrWhiteSpace(input))
            //     {
            //         // convert rules
            //         rules = rules.OrderBy(pair => int.Parse(pair.ID)).ToList();
            //         foreach (ParseRule rule in rules)
            //         {
            //             //ExpandRule(rule, rules, ref checkedRules, ref valids);
            //         }
            //     }
            //     else
            //     {
            //         // check against rules
            //         if (rules.Where(r => r.ID == "0").First().Possibles.Contains(input))
            //         {
            //             ++validCount;
            //         }
            //     }
            // }
            // return validCount.ToString();
        }

        // class Cycle
        // {
        //     public string ID { get; set; }
        //     public int Count { get; set; }
        //     public Cycle(Cycle cycle, string id)
        //     {
        //         ID = id;
        //         if (cycle != null && cycle.ID == id)
        //         {
        //             Count = cycle.Count + 1;
        //         }
        //         else
        //         {
        //             Count = 1;
        //         }
        //     }
        // }

        // private void ExpandRuleP2(ParseRule rule, IEnumerable<ParseRule> rules, Cycle cycle, ref HashSet<string> usedRules, ref List<string> valids)
        // {
        //     if (rules.Count() == 0)
        //     {
        //         return;
        //     }

        //     if (!usedRules.Contains(rule.ID))
        //     {
        //         string[] split = rule.Rules.Split('|');
        //         string[] leftDeps = split[0].Split(' ', StringSplitOptions.RemoveEmptyEntries);
        //         string[] rightDeps = null;
        //         string[] allDeps = leftDeps;
        //         if (split.Count() > 1)
        //         {
        //             rightDeps = split[1].Split(' ', StringSplitOptions.RemoveEmptyEntries);
        //             allDeps = allDeps.Union(rightDeps).Distinct().ToArray();
        //         }

        //         bool hasDeps = true;
        //         foreach (string dep in allDeps)
        //         {
        //             if (!usedRules.Contains(dep))
        //             {
        //                 int test;
        //                 if (int.TryParse(dep, out test))
        //                 {
        //                     if (cycle != null && cycle.ID == rule.ID)
        //                         continue;

        //                     ExpandRuleP2(rules.Where(r => r.ID == dep).First(), rules, new Cycle(cycle, rule.ID), ref usedRules, ref valids);
        //                 }
        //                 else
        //                 {
        //                     hasDeps = false;
        //                     rule.Possibles.Add(dep);
        //                     rule.SpecialCase.Add(dep);
        //                 }
        //             }
        //         }

        //         if (hasDeps)
        //         {
        //             List<List<string>> allOptions = new List<List<string>>();
        //             for (int i = 0; i < leftDeps.Count(); ++i)
        //             {
        //                 allOptions.Add(rules.Where(r => r.ID == leftDeps[i]).Select(r => r.Possibles).First());
        //             }

        //             List<string> curOptions = allOptions[0];
        //             allOptions.RemoveAt(0);
        //             while (allOptions.Count > 0)
        //             {
        //                 curOptions = curOptions.SelectMany(ao0 => allOptions[0].Select(ao1 => $"{ao0}{ao1}")).ToList();
        //                 allOptions.RemoveAt(0);
        //             }
        //             rule.Possibles.AddRange(curOptions);


        //             allOptions = new List<List<string>>();
        //             for (int i = 0; i < leftDeps.Count(); ++i)
        //             {
        //                 allOptions.Add(rules.Where(r => r.ID == leftDeps[i]).Select(r => r.SpecialCase).First());
        //             }

        //             curOptions = allOptions[0];
        //             allOptions.RemoveAt(0);
        //             while (allOptions.Count > 0)
        //             {
        //                 curOptions = curOptions.SelectMany(ao0 => allOptions[0].Select(ao1 => $"{ao0}{ao1}")).ToList();
        //                 allOptions.RemoveAt(0);
        //             }
        //             rule.SpecialCase.AddRange(curOptions);
        //             if (rightDeps != null)
        //             {
        //                 bool special = rightDeps.Where(r => r == rule.ID).Count() > 0;
        //                 allOptions.Clear();
        //                 for (int i = 0; i < rightDeps.Count(); ++i)
        //                 {
        //                     if (rightDeps[i] == rule.ID)
        //                     {
        //                         allOptions.Add(new List<string>());
        //                         allOptions.Last().Add($"|{rule.ID}|");
        //                     }
        //                     else
        //                     {
        //                         allOptions.Add(rules.Where(r => r.ID == rightDeps[i]).Select(r => r.Possibles).First());
        //                     }
        //                 }

        //                 if (special)
        //                 {
        //                     curOptions = allOptions[0];
        //                     allOptions.RemoveAt(0);
        //                     while (allOptions.Count > 0)
        //                     {
        //                         curOptions = curOptions.SelectMany(ao0 => allOptions[0].Select(ao1 => $"{ao0}{ao1}")).ToList();
        //                         allOptions.RemoveAt(0);
        //                     }
        //                     rule.SpecialCase.AddRange(curOptions);
        //                 }
        //                 else
        //                 {
        //                     curOptions = allOptions[0];
        //                     allOptions.RemoveAt(0);
        //                     while (allOptions.Count > 0)
        //                     {
        //                         curOptions = curOptions.SelectMany(ao0 => allOptions[0].Select(ao1 => $"{ao0}{ao1}")).ToList();
        //                         allOptions.RemoveAt(0);
        //                     }
        //                     rule.Possibles.AddRange(curOptions);
        //                     rule.SpecialCase.AddRange(curOptions);
        //                 }
        //             }
        //             rule.Possibles = rule.Possibles.Distinct().ToList();
        //             rule.SpecialCase = rule.SpecialCase.Distinct().Where(c => c.Contains("|")).ToList();
        //         }

        //         usedRules.Add(rule.ID);
        //     }
        // }

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            List<Node> nodes = new List<Node>();
            return "";

            //     HashSet<string> checkedRules = new HashSet<string>();
            //     List<ParseRule> rules = new List<ParseRule>();
            //     List<string> valids = new List<string>();
            //     List<string> invalids = new List<string>();
            //     int validCount = 0;
            //     foreach (string input in inputs)
            //     {
            //         if (input.Contains(':'))
            //         {
            //             // add raw rules
            //             string[] split = input.Split(':');
            //             rules.Add(new ParseRule { ID = split[0], Rules = split[1].Replace("\"", "") });
            //             if (rules.Last().ID == "8")
            //             {
            //                 rules.Last().Rules = "42 | 42 8";
            //             }
            //             else if (rules.Last().ID == "11")
            //             {
            //                 rules.Last().Rules = "42 31 | 42 11 31";
            //             }
            //         }
            //         else if (string.IsNullOrWhiteSpace(input))
            //         {
            //             // convert rules
            //             rules = rules.OrderBy(pair => int.Parse(pair.ID)).ToList();
            //             foreach (ParseRule rule in rules)
            //             {
            //                 ExpandRuleP2(rule, rules, null, ref checkedRules, ref valids);
            //             }
            //         }
            //         else
            //         {
            //             // check against rules
            //             if (rules.Where(r => r.ID == "0").First().Possibles.Contains(input))
            //             {
            //                 ++validCount;
            //             }
            //             else
            //             {
            //                 invalids.Add(input);
            //             }
            //         }
            //     }

            //     List<string> rule0 = rules.Where(r => r.ID == "0").First().SpecialCase.Where(sc => sc.Contains("|")).Distinct().ToList();
            //     foreach (string invalid in invalids)
            //     {
            //         DebugWriteLine($"Checking {invalid}...");
            //         if (IsSpecialValid(invalid, rule0, rules, 0))
            //         {
            //             ++validCount;
            //         }
            //     }


            //     return validCount.ToString();
        }

        // private int GetMatchingIndex(string fullWord, List<string> matching)
        // {
        //     for (int i = 0; i < fullWord.Length; ++i)
        //     {
        //         if (matching.Where(r => r.Length > i && r[0..i] == fullWord[0..i]).Count() <= 0)
        //         {
        //             return i - 1;
        //         }
        //     }
        //     return -1;
        // }

        // private bool IsSpecialValid(string invalid, List<string> subset, List<ParseRule> rules, int cycle)
        // {
        //     DebugWriteLine($"      ...[{string.Format("{0:000}", cycle)}]{invalid} against {subset.Count} possible matches");
        //     int idx = GetMatchingIndex(invalid, subset);
        //     if (idx < 0)
        //     {
        //         DebugWriteLine($"      ......[FAILED] no matching index");
        //         return false;
        //     }

        //     List<string> smallSubset = subset.Where(r => r[0..idx] == invalid[0..idx]).Select(r => r[idx..]).Where(r => r[0] == '|').Distinct().ToList();
        //     if (smallSubset.Count == 0)
        //     {
        //         DebugWriteLine($"      ......[FAILED] recursive matches");
        //         return false;
        //     }

        //     string subString = invalid[idx..];
        //     return IsSpecialValidRecurse(subString, smallSubset, rules, cycle);
        // }

        // private bool IsSpecialValidRecurse(string invalid, List<string> subset, List<ParseRule> rules, int cycle)
        // {
        //     string subsetString = subset[0];
        //     int start = subsetString.IndexOf('|') + 1;
        //     int end = subsetString.IndexOf('|', start);
        //     string ruleIdx = subsetString.Substring(start, end - start);

        //     string subsetEndString = subsetString[(end + 1)..];
        //     ParseRule replaceRule = rules.Where(r => r.ID == ruleIdx).First();

        //     List<string> newSubset = subset.SelectMany(ss => replaceRule.Possibles.Select(p => $"{p}{subsetEndString}")).ToList();
        //     bool check =  IsSpecialValid(invalid, newSubset, rules, cycle + 1);

        //     return false;
        // }
    }
}



