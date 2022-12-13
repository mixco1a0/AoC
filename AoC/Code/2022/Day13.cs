using System.Collections.Generic;
using System.Linq;

namespace AoC._2022
{
    class Day13 : Core.Day
    {
        public Day13() { }

        public override string GetSolutionVersion(Core.Part part)
        {
            switch (part)
            {
                case Core.Part.One:
                    return "v1";
                case Core.Part.Two:
                    return "v1";
                default:
                    return base.GetSolutionVersion(part);
            }
        }

        public override bool SkipTestData => true;

        protected override List<Core.TestDatum> GetTestData()
        {
            List<Core.TestDatum> testData = new List<Core.TestDatum>();
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.One,
                Output = "13",
                RawInput =
@"[1,1,3,1,1]
[1,1,5,1,1]

[[1],[2,3,4]]
[[1],4]

[9]
[[8,7,6]]

[[4,4],4,4]
[[4,4],4,4,4]

[7,7,7,7]
[7,7,7]

[]
[3]

[[[]]]
[[]]

[1,[2,[3,[4,[5,6,7]]]],8,9]
[1,[2,[3,[4,[5,6,0]]]],8,9]"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "140",
                RawInput =
@"[1,1,3,1,1]
[1,1,5,1,1]

[[1],[2,3,4]]
[[1],4]

[9]
[[8,7,6]]

[[4,4],4,4]
[[4,4],4,4,4]

[7,7,7,7]
[7,7,7]

[]
[3]

[[[]]]
[[]]

[1,[2,[3,[4,[5,6,7]]]],8,9]
[1,[2,[3,[4,[5,6,0]]]],8,9]"
            });
            return testData;
        }

        internal class PacketPair : Base.Pair<string, string>
        {
            public string L { get => m_first; set => m_first = value; }
            public string R { get => m_last; set => m_last = value; }
            public int LIdx { get; set; }
            public int RIdx { get; set; }

            public PacketPair() : base() { }

            public bool IsOrdered()
            {
                string originalL = L;
                string originalR = R;
                bool ordered = CheckOrdered(0, 0);
                L = originalL;
                R = originalR;
                return ordered;
            }

            private bool CheckOrdered(int lStart, int rStart)
            {
                if (lStart >= L.Length)
                {
                    return true;
                }

                if (rStart >= R.Length)
                {
                    return false;
                }

                bool isLNum = char.IsDigit(L[lStart]);
                bool isRNum = char.IsDigit(R[rStart]);
                if (isLNum && isRNum)
                {
                    int lEnd = L.IndexOfAny(",[]".ToCharArray(), lStart);
                    int left = int.Parse(L.Substring(lStart, lEnd - lStart));

                    int rEnd = R.IndexOfAny(",[]".ToCharArray(), rStart);
                    int right = int.Parse(R.Substring(rStart, rEnd - rStart));

                    LIdx = lEnd + 1;
                    RIdx = rEnd + 1;

                    if (left > right)
                    {
                        return false;
                    }
                    else if (right > left)
                    {
                        return true;
                    }
                    else if (left == right)
                    {
                        if (L[lEnd] == R[rEnd] && R[rEnd] == ',')
                        {
                            return CheckOrdered(lEnd + 1, rEnd + 1);
                        }
                        else if (R[rEnd] == ',')
                        {
                            // left has no more items
                            return true;
                        }
                        else if (L[lEnd] == ',')
                        {
                            // right has no more items
                            return false;
                        }
                    }
                }
                else if (!isLNum && !isRNum)
                {
                    if (L[lStart] == '[' && R[rStart] == ']')
                    {
                        return false;
                    }
                    if (L[lStart] == ']' && R[rStart] == '[')
                    {
                        return true;
                    }
                    else
                    {
                        return CheckOrdered(lStart + 1, rStart + 1);
                    }
                }
                else
                {
                    if (isLNum)
                    {
                        if (R[rStart] == ']')
                        {
                            return false;
                        }

                        int closeIdx = L.IndexOf(']', lStart);
                        int openIdx = L.IndexOf('[', lStart);
                        int commaIdx = L.IndexOf(',', lStart);
                        if (openIdx >= 0 && openIdx < closeIdx)
                        {
                            if (commaIdx < openIdx)
                            {
                                L = L.Insert(commaIdx, "]").Insert(lStart, "[");
                            }
                            else
                            {
                                L = L.Insert(openIdx, "]").Insert(lStart, "[");
                            }
                        }
                        else
                        {
                            L = L.Insert(closeIdx, "]").Insert(lStart, "[");
                        }
                    }
                    else
                    {
                        if (L[lStart] == ']')
                        {
                            return true;
                        }

                        int closeIdx = R.IndexOf(']', rStart);
                        int openIdx = R.IndexOf(',', rStart);
                        int commaIdx = R.IndexOf(',', rStart);
                        if (openIdx >= 0 && openIdx < closeIdx)
                        {
                            if (commaIdx < openIdx)
                            {
                                R = R.Insert(commaIdx, "]").Insert(rStart, "[");
                            }
                            else
                            {
                                R = R.Insert(openIdx, "]").Insert(rStart, "[");
                            }
                        }
                        else
                        {
                            R = R.Insert(closeIdx, "]").Insert(rStart, "[");
                        }
                    }
                    return CheckOrdered(lStart, rStart);
                }
                return true;
            }
        }

        private void ParsePairs(List<string> inputs, out List<PacketPair> packets)
        {
            packets = new List<PacketPair>();
            bool newPacket = true;
            foreach (string input in inputs)
            {
                if (string.IsNullOrWhiteSpace(input))
                {
                    newPacket = true;
                    continue;
                }

                if (newPacket)
                {
                    newPacket = false;
                    packets.Add(new PacketPair() { First = input });
                }
                else
                {
                    packets.Last().Last = input;
                }
            }
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool orderPackets)
        {
            if (!orderPackets)
            {
                ParsePairs(inputs, out List<PacketPair> packets);
                return packets.Select((packet, idx) => (packet, idx)).Where(pair => pair.packet.IsOrdered()).Select(pair => pair.idx + 1).Sum().ToString();
            }
            else
            {
                List<string> halfPackets = inputs.Where(i => !string.IsNullOrWhiteSpace(i)).ToList();
                halfPackets.AddRange(new List<string>() { "[[2]]", "[[6]]" });

                string[] orderedPackets = new string[halfPackets.Count];
                orderedPackets[0] = halfPackets[0];
                halfPackets.RemoveAt(0);

                PacketPair test = new PacketPair();
                while (halfPackets.Count > 0)
                {
                    test.L = halfPackets[0];
                    halfPackets.RemoveAt(0);

                    for (int i = 0; i < orderedPackets.Length; ++i)
                    {
                        if (string.IsNullOrWhiteSpace(orderedPackets[i]))
                        {
                            orderedPackets[i] = test.L;
                            break;
                        }

                        test.R = orderedPackets[i];
                        if (test.IsOrdered())
                        {
                            orderedPackets[i] = test.L;
                            test.L = test.R;
                        }
                    }
                }
                var decoder = orderedPackets.Select((packet, idx) => (packet, idx)).Where(pair => pair.packet == "[[2]]" || pair.packet == "[[6]]").ToList();
                return ((decoder[0].idx + 1) * (decoder[1].idx + 1)).ToString();
            }
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);
    }
}