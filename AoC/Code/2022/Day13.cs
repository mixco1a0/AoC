using System;
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
                // case Core.Part.One:
                //     return "v1";
                // case Core.Part.Two:
                //     return "v1";
                default:
                    return base.GetSolutionVersion(part);
            }
        }

        public override bool SkipTestData => false;

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
                Output = "",
                RawInput =
@""
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
                return CheckOrdered(0, 0);
            }

            private bool CheckOrdered(int lStart, int rStart)
            {
                bool ordered = true;
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

                        int idx = L.IndexOfAny(",[]".ToCharArray(), lStart);
                        L = L.Insert(idx, "]").Insert(lStart, "[");
                    }
                    else
                    {
                        if (L[lStart] == ']')
                        {
                            return true;
                        }

                        int idx = R.IndexOfAny(",[]".ToCharArray(), rStart);
                        R = R.Insert(idx, "]").Insert(rStart, "[");
                    }
                    return CheckOrdered(lStart, rStart);
                }
                return ordered;
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

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables)
        {
            ParsePairs(inputs, out List<PacketPair> packets);
            // packets.ForEach((p) =>
            // {
            //     DebugWriteLine($"Processing {p.ToString()}");
            //     DebugWriteLine($"   Ordered = {p.IsOrdered()}");
            // });
            return packets.Select((packet, idx) => (packet, idx)).Where(pair => pair.packet.IsOrdered()).Select(pair => pair.idx + 1).Sum().ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);
    }
}