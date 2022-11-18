using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AoC.Core;

namespace AoC._2021
{
    class Day16 : Day
    {
        public Day16() { }

        public override string GetSolutionVersion(Part part)
        {
            switch (part)
            {
                // case Part.One:
                //     return "v1";
                // case Part.Two:
                //     return "v1";
                default:
                    return base.GetSolutionVersion(part);
            }
        }

        protected override List<TestDatum> GetTestData()
        {
            List<TestDatum> testData = new List<TestDatum>();
            testData.Add(new TestDatum
            {
                TestPart = Part.One,
                Output = "16",
                RawInput =
@"8A004A801A8002F478"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.One,
                Output = "12",
                RawInput =
@"620080001611562C8802118E34"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.One,
                Output = "23",
                RawInput =
@"C0015000016115A2E0802F182340"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.One,
                Output = "31",
                RawInput =
@"A0016C880162017C3686B18A3D4780"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.Two,
                Output = "3",
                RawInput =
@"C200B40A82"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.Two,
                Output = "54",
                RawInput =
@"04005AC33890"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.Two,
                Output = "7",
                RawInput =
@"880086C3E88112"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.Two,
                Output = "9",
                RawInput =
@"CE00C43D881120"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.Two,
                Output = "1",
                RawInput =
@"D8005AC2A8F0"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.Two,
                Output = "0",
                RawInput =
@"F600BC2D8F"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.Two,
                Output = "0",
                RawInput =
@"9C005AC2F8F0"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.Two,
                Output = "1",
                RawInput =
@"9C0141080250320F1802104A08"
            });
            return testData;
        }

        public enum PacketType : int
        {
            Sum = 0,
            Mult = 1,
            Min = 2,
            Max = 3,
            Raw = 4,
            GreaterThan = 5,
            LessThan = 6,
            EqualTo = 7
        }

        public interface IPacket
        {
            public int Version { get; }
            public PacketOperator Parent { get; }
            public PacketType Type { get; }

            public long Evaluate();

            // debug
            public int Level { get; }
        }

        public class PacketLiteral : IPacket
        {
            public int Version { get; set; }
            public PacketOperator Parent { get; set; }
            public PacketType Type { get => PacketType.Raw; }
            public long Value { get; set; }
            public int Level { get => Parent != null ? Parent.Level + 1 : 0; }

            public PacketLiteral(int version, PacketOperator parent, long value)
            {
                Version = version;
                Parent = parent;
                Value = value;
                if (Parent != null)
                {
                    Parent.SubPackets.Add(this);
                }
            }

            public long Evaluate() => Value;
        }

        public class PacketOperator : IPacket
        {
            public int Version { get; set; }
            public List<IPacket> SubPackets { get; set; }
            public PacketOperator Parent { get; set; }
            public PacketType Type { get; private set; }
            public int Level { get => Parent != null ? Parent.Level + 1 : 0; }

            public PacketOperator(int version, PacketOperator parent, PacketType type)
            {
                Version = version;
                Parent = parent;
                SubPackets = new List<IPacket>();
                Type = type;
                if (Parent != null)
                {
                    Parent.SubPackets.Add(this);
                }
            }

            public long Evaluate()
            {
                switch (Type)
                {
                    case PacketType.Sum:
                        return SubPackets.Sum(sp => sp.Evaluate());
                    case PacketType.Mult:
                        return SubPackets.Aggregate(1L, (count, packet) => count * packet.Evaluate());
                    case PacketType.Min:
                        return SubPackets.Min(sp => sp.Evaluate());
                    case PacketType.Max:
                        return SubPackets.Max(sp => sp.Evaluate());
                    case PacketType.GreaterThan:
                        if (SubPackets[0].Evaluate() > SubPackets[1].Evaluate())
                        {
                            return 1L;
                        }
                        return 0L;
                    case PacketType.LessThan:
                        if (SubPackets[0].Evaluate() < SubPackets[1].Evaluate())
                        {
                            return 1L;
                        }
                        return 0L;
                    case PacketType.EqualTo:
                        if (SubPackets[0].Evaluate() == SubPackets[1].Evaluate())
                        {
                            return 1L;
                        }
                        return 0L;
                }
                return 0L;
            }
        }

        private void ParseLiteral(string binary, out long literal, out int bitsUsed)
        {
            literal = 0;
            bitsUsed = 0;

            bool complete = false;
            StringBuilder sb = new StringBuilder();
            while (!complete)
            {
                complete = binary.Substring(bitsUsed++, 1) == "0";
                sb.Append(binary.Substring(bitsUsed, 4));
                bitsUsed += 4;
            }
            literal = Convert.ToInt64(sb.ToString(), 2);
        }

        private int ParsePackets(string binary, PacketOperator parent, ref List<IPacket> packets, int maxPackets)
        {
            int curPos = 0;
            int curPackets = 0;
            const int headerSize = 6;
            while (curPos + headerSize + 1 < binary.Length)
            {
                // cache starting position
                int prevPos = curPos;

                // parse header
                int packetVersion = Convert.ToInt32(binary.Substring(curPos, 3), 2);
                int packetTypeId = Convert.ToInt32(binary.Substring(curPos + 3, 3), 2);
                curPos += headerSize;

                // parse packet
                if (packetTypeId == 4)
                {
                    ParseLiteral(binary.Substring(curPos), out long literal, out int bitsUsed);
                    curPos += bitsUsed;
                    PacketLiteral packet = new PacketLiteral(packetVersion, parent, literal);
                    packets.Add(packet);
                    // DebugWriteLine($"{new string('*', packet.Level * 3)}[#{curPackets,2}][v{packetVersion}][{(PacketType)packetTypeId}] {packet.Value}");
                }
                else
                {
                    bool is15BitRep = binary[curPos++] == '0';
                    int bitLabelLength = is15BitRep ? 15 : 11;
                    int packetLabel = Convert.ToInt32(binary.Substring(curPos, bitLabelLength), 2);
                    curPos += bitLabelLength;
                    PacketOperator packet = new PacketOperator(packetVersion, parent, (PacketType)packetTypeId);
                    packets.Add(packet);
                    if (is15BitRep)
                    {
                        // DebugWriteLine($"{new string('*', packet.Level * 3)}[#{curPackets,2}][v{packetVersion}][{(PacketType)packetTypeId}] Len:{packetLabel}");
                        curPos += ParsePackets(binary.Substring(curPos, packetLabel), packet, ref packets, int.MaxValue);
                    }
                    else
                    {
                        // DebugWriteLine($"{new string('*', packet.Level * 3)}[#{curPackets,2}][v{packetVersion}][{(PacketType)packetTypeId}] Rep:{packetLabel}");
                        curPos += ParsePackets(binary.Substring(curPos), packet, ref packets, packetLabel);
                    }
                }

                if (++curPackets >= maxPackets)
                {
                    break;
                }
            }
            return curPos;
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool evaulate)
        {
            string fullHex = inputs.First();
            Func<char, string> ConvertToBinary = (char hex) =>
            {
                string raw = Convert.ToString(Convert.ToInt32($"{hex}", 16), 2);
                return string.Format("{0,4}", raw).Replace(' ', '0');
            };
            string binary = string.Join("", fullHex.Select(h => ConvertToBinary(h)));
            List<IPacket> packets = new List<IPacket>();
            // DebugWriteLine($"Converting {fullHex}");
            ParsePackets(binary, null, ref packets, int.MaxValue);

            if (evaulate)
            {
                return packets.First().Evaluate().ToString();
            }
            return packets.Aggregate(0, (count, packet) => count + packet.Version).ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);
    }
}