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
                case Core.Part.One:
                    return "v2";
                case Core.Part.Two:
                    return "v2";
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
                        // L (...#,...) | R (...#,...)
                        // L (...#]...) | R (...#]...)
                        if (L[lEnd] == R[rEnd])
                        {
                            return CheckOrdered(lEnd + 1, rEnd + 1);
                        }
                        // L (...#]...) | R (...#,...)
                        else if (R[rEnd] == ',')
                        {
                            // left has no more items
                            return true;
                        }
                        // L (...#,...) | R (...#]...)
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
                    Func<string, int, string> InsertList = (string source, int startIdx) =>
                    {
                        int closeIdx = source.IndexOf(']', startIdx);
                        int openIdx = source.IndexOf('[', startIdx);
                        int commaIdx = source.IndexOf(',', startIdx);
                        // ...#,...
                        if (commaIdx >= 0 && (closeIdx < 0 || commaIdx < closeIdx))
                        {
                            source = source.Insert(commaIdx, "]").Insert(startIdx, "[");
                        }
                        // ...#]
                        else if (closeIdx >= 0)
                        {
                            // ...[#]]
                            source = source.Insert(closeIdx, "]").Insert(startIdx, "[");
                        }
                        return source;
                    };

                    if (isLNum)
                    {
                        if (R[rStart] == ']')
                        {
                            return false;
                        }

                        L = InsertList(L, lStart);
                    }
                    else
                    {
                        if (L[lStart] == ']')
                        {
                            return true;
                        }

                        R = InsertList(R, rStart);
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
                ParsePairs(inputs, out List<PacketPair> packetPairs);
                return packetPairs.Select((packet, idx) => (packet, idx)).Where(pair => pair.packet.IsOrdered()).Select(pair => pair.idx + 1).Sum().ToString();
            }
            else
            {
                int lowPackets = 0;
                int midPackets = 0;

                PacketPair testPair2 = new PacketPair() { R = "[[2]]" };
                PacketPair testPair6 = new PacketPair() { R = "[[6]]" };
                foreach (string packet in inputs.Where(i => !string.IsNullOrWhiteSpace(i)))
                {
                    testPair2.L = packet;
                    if (testPair2.IsOrdered())
                    {
                        ++lowPackets;
                        continue;
                    }
                    
                    testPair6.L = packet;
                    if (testPair6.IsOrdered())
                    {
                        ++midPackets;
                    }
                }
                return ((lowPackets + 1) * (lowPackets + midPackets + 2)).ToString();
            }
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);
    }
}