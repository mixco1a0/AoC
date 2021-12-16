using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;

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
                Output = "",
                RawInput =
@""
            });
            return testData;
        }

        public enum PacketType : int
        {
            Operator3 = 3,
            Literal = 4,
            Operator2 = 6,
        }

        public interface IPacket
        {
            public int Version { get; }
            public IPacket Parent { get; }

            // debug
            public int Level { get; }
        }

        private class PacketLiteral : IPacket
        {
            public int Version { get; set; }
            public IPacket Parent { get; set; }
            public long Value { get; set; }
            public int Level { get; set; }

            public PacketLiteral(int version, IPacket parent, long value)
            {
                Version = version;
                Parent = parent;
                Value = value;
                Level = 0;
                if (Parent != null)
                {
                    Level = parent.Level + 1;
                }
            }
        }

        private class PacketOperator : IPacket
        {
            public int Version { get; set; }
            public List<IPacket> SubPackets { get; set; }
            public IPacket Parent { get; set; }
            public int Level { get; set; }

            public PacketOperator(int version, IPacket parent)
            {
                Version = version;
                Parent = parent;
                SubPackets = new List<IPacket>();
                Level = 0;
                if (Parent != null)
                {
                    Level = parent.Level + 1;
                }
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

        private int ParsePackets(string binary, PacketOperator parent, ref List<IPacket> packets, int maxPackages)
        {
            int curPos = 0;
            int curPackages = 0;
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
                    if (parent != null)
                    {
                        parent.SubPackets.Add(packets.Last());
                    }
                    DebugWriteLine($"{new string('*', packet.Level * 3)}[{curPackages}] Literal: [v{packetVersion}] {packet.Value}");
                }
                else
                {
                    bool is15BitRep = binary[curPos++] == '0';
                    int bitLabelLength = is15BitRep ? 15 : 11;
                    int packetLabel = Convert.ToInt32(binary.Substring(curPos, bitLabelLength), 2);
                    curPos += bitLabelLength;
                    PacketOperator packet = new PacketOperator(packetVersion, parent);
                    packets.Add(packet);
                    if (is15BitRep)
                    {
                        DebugWriteLine($"{new string('*', packet.Level * 3)}[{curPackages}] Size Op: [v{packetVersion}] Length:{packetLabel}");
                        curPos += ParsePackets(binary.Substring(curPos, packetLabel), packet, ref packets, int.MaxValue);
                        if (parent != null && maxPackages == int.MaxValue)
                        {
                            break;
                        }
                    }
                    else
                    {
                        DebugWriteLine($"{new string('*', packet.Level * 3)}[{curPackages}] Reps Op: [v{packetVersion}] Amount:{packetLabel}");
                        curPos += ParsePackets(binary.Substring(curPos), packet, ref packets, packetLabel);
                    }
                }

                if (++curPackages >= maxPackages)
                {
                    break;
                }
            }
            return curPos;
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables)
        {
            string fullHex = inputs.First();
            Func<char, string> ConvertToBinary = (char hex) =>
            {
                string raw = Convert.ToString(Convert.ToInt32($"{hex}", 16), 2);
                return string.Format("{0,4}", raw).Replace(' ', '0');
            };
            string binary = string.Join("", fullHex.Select(h => ConvertToBinary(h)));
            List<IPacket> packets = new List<IPacket>();
            DebugWriteLine($"Converting {fullHex}");
            ParsePackets(binary, null, ref packets, int.MaxValue);

            return packets.Aggregate(0, (count, packet) => count + packet.Version).ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);
    }
}