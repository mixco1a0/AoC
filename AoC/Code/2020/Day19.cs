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
            testData.Add(new TestDatum
            {
                TestPart = TestPart.Two,
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

bbbbbbbaaaabbbbaaabbabaaa
abbbbabbbbaaaababbbbbbaaaababb
bbbababbbbaaaaaaaabbababaaababaabab"
            });
            testData.Add(new TestDatum
            {
                TestPart = TestPart.Two,
                Output = "12",
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

        class Node
        {
            public string ID { get; set; }
            public string RawRules { get; set; }
            public string Value { get; set; }
            public List<List<string>> SubRules { get; set; }
            public List<List<Node>> Sequences { get; set; }
            public int MinLength { get; set; }
            public Action<string> PrintFunc { get; set; }
            public Node()
            {
                SubRules = new List<List<string>>();
                Sequences = new List<List<Node>>();
                MinLength = int.MaxValue;
            }

            public void Populate(ref List<Node> nodes, Func<string, string> GetNodeName, Action<string> printFunc)
            {
                PrintFunc = printFunc;
                string[] ruleSplit = RawRules.Split('|', StringSplitOptions.RemoveEmptyEntries);
                foreach (String curSplit in ruleSplit)
                {
                    List<string> ids = curSplit.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
                    int intTest;
                    if (!int.TryParse(ids.ElementAt(0), out intTest))
                    {
                        Value = ids.ElementAt(0);
                        continue;
                    }

                    ids = ids.Select(i => GetNodeName(i)).ToList();

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

            public int GenerateMinLength()
            {
                if (MinLength == int.MaxValue)
                {
                    if (Sequences.Count == 0)
                    {
                        MinLength = 1;
                    }
                    else
                    {
                        foreach (List<Node> sequence in Sequences)
                        {
                            // skip recursive checks
                            if (sequence.Where(s => s.ID == ID).Count() > 0)
                            {
                                continue;
                            }

                            int runningMin = 0;
                            foreach (Node node in sequence)
                            {
                                runningMin += node.GenerateMinLength();
                            }
                            MinLength = Math.Min(runningMin, MinLength);
                        }
                    }
                }

                return MinLength;
            }

            public struct SequenceIndex
            {
                public int Cur { get; set; }
                public int Max { get; set; }
                public bool IsComplete() { return Cur == Max; }
                public int Next() { return Cur + 1; }
                public override string ToString()
                {
                    return $"Cur={Cur}, Max={Max}";
                }
            }

            public int GetMatchingLength(string input, int curLetterIndex, int depth, string prev, int sequenceStart, ref Dictionary<string, SequenceIndex> sequenceInfo)
            {
                string history = $"{prev}->{ID}";
                if (string.IsNullOrWhiteSpace(prev))
                {
                    history = ID;
                }
                if (input.Length <= curLetterIndex)
                {
                    return -1;
                }

                if (Sequences.Count == 0)
                {
                    bool match = input[curLetterIndex..].First() == Value.First();
                    string pre = curLetterIndex > 0 ? input.Substring(0, curLetterIndex) : "";
                    string post = curLetterIndex < input.Length - 1 ? input.Substring(curLetterIndex + 1) : "";
                    string curMatching = $"{pre}[{input.ElementAt(curLetterIndex)}]{post}";
                    string matchString = match ? "==" : "!=";
                    PrintFunc($"[hist|e{matchString}] {history} [{ToString()}] --- {curMatching}");
                    PrintFunc($"[test|end] {curMatching}  {matchString}  {Value.First()}");
                    return match ? 1 : 0;
                }

                Dictionary<string, int> forceNodeSequenceStart = new Dictionary<string, int>();
                HashSet<string> skipRecursion = new HashSet<string>();
                int sequenceMatch = 0;
                int sequenceRunningTotal = 0;
                int curLetterIndexReset = curLetterIndex;
                bool redoSequence = false;
                for (int curSequence = sequenceStart; curSequence <= Sequences.Count;)
                {
                    List<Node> sequence = Sequences[curSequence - 1];
                    if (!skipRecursion.Contains(ID) && sequence.Where(n => n.ID == ID).Count() > 0)
                    {
                        // if the current sequence contains itself, add a new sequence in its place
                        // with its own spot replaced with the first sequence
                        Sequences.Insert(curSequence - 1, new List<Node>(sequence));
                        sequence = Sequences[curSequence - 1];
                        int idx = sequence.Select((s, i) => new { s, i }).Where(pair => pair.s.ID == ID).Select(pair => pair.i).First();
                        sequence.RemoveAt(idx);
                        sequence.InsertRange(idx, Sequences[curSequence - 2]);
                    }

                    int sequenceMinLength = 0;
                    sequence.ForEach(n => sequenceMinLength += n.MinLength);
                    if (sequenceMinLength <= input.Length)
                    {
                        sequenceInfo[history] = new SequenceIndex { Cur = curSequence, Max = Sequences.Count };
                        sequenceMatch = 0;
                        sequenceRunningTotal = 0;

                        for (int curNode = 1; curNode <= sequence.Count; ++curNode)
                        {
                            Node node = sequence[curNode - 1];
                            string completeHistory = $"{history}[S#{curSequence}.N#{curNode}]";
                            int startingSequence = 1;
                            if (forceNodeSequenceStart.ContainsKey(node.ID))
                            {
                                startingSequence = forceNodeSequenceStart[node.ID];
                            }
                            int matchLength = node.GetMatchingLength(input, curLetterIndex, depth + 1, completeHistory, startingSequence, ref sequenceInfo);
                            if (matchLength > 0)
                            {
                                ++sequenceMatch;

                                sequenceRunningTotal += matchLength;
                                curLetterIndex += matchLength;
                            }
                            else
                            {
                                // sequence is dead, try the next sequence
                                PrintFunc($"[hist|err] {completeHistory}->{node.ID} FAILED");

                                // what if the previous node has a second sequence thats worth trying?
                                // need to communicate back to the previous node to try again with 
                                // the different sequence

                                // what if the previous node has more potential history, despite hitting
                                // its last sequence? (it's current successful node has more sequences)
                                if (curNode > 1 && matchLength > 0)
                                {
                                    // check for length here
                                    Node prevNode = sequence[curNode - 2];
                                    string prevNodeId = $"{history}[S#{curSequence}.N#{curNode - 1}]->{prevNode.ID}";
                                    if (sequenceInfo.ContainsKey(prevNodeId))
                                    {
                                        SequenceIndex prevNodeIndex = sequenceInfo[prevNodeId];
                                        if (!prevNodeIndex.IsComplete())
                                        {
                                            PrintFunc($"[hist|try] {prevNodeId} REDO on S#{prevNodeIndex.Next()}");
                                            forceNodeSequenceStart[prevNode.ID] = prevNodeIndex.Next();
                                            redoSequence = true;
                                        }
                                    }
                                }
                                break;
                            }
                        }
                    }
                    else
                    {
                        skipRecursion.Add(ID);
                    }

                    if (sequenceMatch == sequence.Count)
                    {
                        return sequenceRunningTotal;
                    }

                    if (!redoSequence)
                    {
                        forceNodeSequenceStart.Clear();
                        ++curSequence;
                    }
                    redoSequence = false;
                    curLetterIndex = curLetterIndexReset;
                }

                return 0;
            }

            public override string ToString()
            {
                return $"{ID} => {RawRules}";
            }
        }

        private string GetNodeName(string nodeId)
        {
            if (nodeId.Length == 1)
            {
                return $"0{nodeId}";
            }
            return nodeId;
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            Dictionary<string, Node.SequenceIndex> sequenceInfo = new Dictionary<string, Node.SequenceIndex>();
            int validCount = 0;
            List<Node> nodes = new List<Node>();
            foreach (string input in inputs)
            {
                if (input.Contains(':'))
                {
                    // add raw rules
                    string[] split = input.Split(':');
                    nodes.Add(new Node { ID = GetNodeName(split[0]), RawRules = split[1].Trim().Replace("\"", "") });
                }
                else if (string.IsNullOrWhiteSpace(input))
                {
                    foreach (Node node in nodes)
                    {
                        node.Populate(ref nodes, GetNodeName, (s) => { });
                        // node.Populate(ref nodes, DebugWriteLine);
                    }
                }
                else
                {
                    Node node0 = nodes.Where(n => n.ID == GetNodeName("0")).First();
                    if (node0.GetMatchingLength(input, 0, 0, "", 1, ref sequenceInfo) == input.Length)
                    {
                        ++validCount;
                    }
                }
            }
            return validCount.ToString();
        }

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            Dictionary<string, Node.SequenceIndex> sequenceInfo = new Dictionary<string, Node.SequenceIndex>();
            int validCount = 0;
            List<Node> nodes = new List<Node>();
            foreach (string input in inputs)
            {
                if (input.Contains(':'))
                {
                    // add raw rules
                    if (input[0..2] == "8:")
                    {
                        nodes.Add(new Node { ID = GetNodeName("8"), RawRules = "42 | 42 8" });
                    }
                    else if (input[0..3] == "11:")
                    {
                        nodes.Add(new Node { ID = GetNodeName("11"), RawRules = "42 31 | 42 11 31" });
                    }
                    else
                    {
                        string[] split = input.Split(':');
                        nodes.Add(new Node { ID = GetNodeName(split[0]), RawRules = split[1].Trim().Replace("\"", "") });
                    }
                }
                else if (string.IsNullOrWhiteSpace(input))
                {
                    foreach (Node node in nodes)
                    {
                        // node.Populate(ref nodes, (s) => { });
                        node.Populate(ref nodes, GetNodeName, DebugWriteLine);
                    }
                    Node node0 = nodes.Where(n => n.ID == GetNodeName("0")).First();
                    node0.GenerateMinLength();

                    foreach (Node node in nodes.OrderBy(n => n.ID))
                    {
                        string subRules = "";
                        foreach (List<string> sub in node.SubRules)
                        {
                            subRules += $"{string.Format("{0,-9}", string.Join(",", sub))} | ";
                        }
                        if (string.IsNullOrWhiteSpace(subRules))
                        {
                            subRules = node.Value;
                        }
                        DebugWriteLine($"Node: #{node.ID} = [MinLength={string.Format("{0, 2}", node.MinLength)}] {subRules}");
                    }
                }
                else
                {
                    Node node0 = nodes.Where(n => n.ID == GetNodeName("0")).First();
                    if (node0.GetMatchingLength(input, 0, 0, "", 1, ref sequenceInfo) == input.Length)
                    {
                        DebugWriteLine($"Valid: {input}");
                        ++validCount;
                    }
                }
            }
            return validCount.ToString();
        }
    }
}



