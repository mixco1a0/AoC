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
                TestPart = TestPart.One,
                Output = "3",
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

bbabbbbaabaabba
aaabbbbbbaaaabaababaabababbabaaabbababababaaa
abbbbbabbbaaaababbaabbbbabababbbabbbbbbabaaaa
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

                    ids = ids.Select(i => string.Format("{0,2}", i));

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

            public int GetMatchingLength(string input, int curLetterIndex, int depth, string prev)
            {
                string history = $"{prev}->{ID}";
                if (string.IsNullOrWhiteSpace(prev))
                {
                    history = ID;
                }
                // PrintFunc($"{history} [{ToString()}]");

                if (Sequences.Count == 0)
                {
                    if (input.Length <= curLetterIndex)
                    {
                        return 0;
                    }

                    bool match = input[curLetterIndex..].First() == Value.First();
                    string pre = curLetterIndex > 0 ? input.Substring(0, curLetterIndex) : "";
                    string post = curLetterIndex < input.Length - 1 ? input.Substring(curLetterIndex + 1) : "";
                    string curMatching = $"{pre}[{input.ElementAt(curLetterIndex)}]{post}";
                    string matchString = match ? "==" : "!=";
                    // PrintFunc($"{curMatching}  {matchString}  {Value.First()}");
                    return match ? 1 : 0;
                }

                int sequenceMatch = 0;
                int sequenceRunningTotal = 0;
                int curLetterIndexReset = curLetterIndex;
                int curSequence = 1;
                foreach (List<Node> sequence in Sequences)
                {
                    sequenceMatch = 0;
                    sequenceRunningTotal = 0;

                    int curNode = 1;
                    foreach (Node node in sequence)
                    {
                        string completeHistory = $"{history}[S#{curSequence}.N#{curNode}]";
                        int matchLength = node.GetMatchingLength(input, curLetterIndex, depth + 1, completeHistory);
                        if (matchLength > 0)
                        {
                            ++sequenceMatch;

                            sequenceRunningTotal += matchLength;
                            curLetterIndex += matchLength;
                        }
                        else
                        {
                            // sequence is dead, try the next sequence
                            // PrintFunc($"{completeHistory}->{node.ID} FAILED");
                            break;
                        }
                        ++curNode;
                    }

                    if (sequenceMatch == sequence.Count)
                    {
                        return sequenceRunningTotal;
                    }

                    ++curSequence;
                    curLetterIndex = curLetterIndexReset;
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
                    nodes.Add(new Node { ID = string.Format("{0,2}", split[0]), RawRules = split[1].Replace("\"", "") });
                }
                else if (string.IsNullOrWhiteSpace(input))
                {
                    foreach (Node node in nodes)
                    {
                        node.Populate(ref nodes, DebugWriteLine);
                    }
                }
                else
                {
                    Node node0 = nodes.Where(n => n.ID == " 0").First();
                    if (node0.GetMatchingLength(input, 0, 0, "") == input.Length)
                    {
                        ++validCount;
                    }
                }
            }
            return validCount.ToString();
        }

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            List<Node> nodes = new List<Node>();
            return "";
        }
    }
}



