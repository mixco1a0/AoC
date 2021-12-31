using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2021
{
    class Day22 : Day
    {
        public Day22() { }

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
                Output = "590784",
                RawInput =
@"on x=-20..26,y=-36..17,z=-47..7
on x=-20..33,y=-21..23,z=-26..28
on x=-22..28,y=-29..23,z=-38..16
on x=-46..7,y=-6..46,z=-50..-1
on x=-49..1,y=-3..46,z=-24..28
on x=2..47,y=-22..22,z=-23..27
on x=-27..23,y=-28..26,z=-21..29
on x=-39..5,y=-6..47,z=-3..44
on x=-30..21,y=-8..43,z=-13..34
on x=-22..26,y=-27..20,z=-29..19
off x=-48..-32,y=26..41,z=-47..-37
on x=-12..35,y=6..50,z=-50..-2
off x=-48..-32,y=-32..-16,z=-15..-5
on x=-18..26,y=-33..15,z=-7..46
off x=-40..-22,y=-38..-28,z=23..41
on x=-16..35,y=-41..10,z=-47..6
off x=-32..-23,y=11..30,z=-14..3
on x=-49..-5,y=-3..45,z=-29..18
off x=18..30,y=-20..-8,z=-3..13
on x=-41..9,y=-7..43,z=-33..15
on x=-54112..-39298,y=-85059..-49293,z=-27449..7877
on x=967..23432,y=45373..81175,z=27513..53682"
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

        private class Instruction
        {
            public bool On { get; set; }
            public Core.Point X { get; set; }
            public Core.Point Y { get; set; }
            public Core.Point Z { get; set; }

            private Instruction()
            {
                On = false;
                X = new Core.Point(0, 0);
                Y = new Core.Point(0, 0);
                Z = new Core.Point(0, 0);
            }

            public static Instruction Parse(string input)
            {
                Instruction i = new Instruction();
                i.On = input[1] == 'n';
                int[] split = input.Split("=.,".ToCharArray()).Where(s => int.TryParse(s, out int result)).Select(int.Parse).ToArray();
                i.X.First = split[0];
                i.X.Last = split[1];
                i.Y.First = split[2];
                i.Y.Last = split[3];
                i.Z.First = split[4];
                i.Z.Last = split[5];
                return i;
            }

            private static readonly int Min = -50;
            private static readonly int Max = 50;

            public IEnumerable<long> GetValues()
            {
                List<long> values = new List<long>();
                if (X.Last < Min || Y.Last < Min || Z.Last < Min || X.First > Max || Y.First > Max || Z.First > Max)
                {
                    return values;
                }

                Func<int, int, int, long> Hash = (x, y, z) =>
                {
                    uint xx = (uint)(x + Max);
                    uint yy = (uint)(y + Max);
                    uint zz = (uint)(z + Max);
                    return (long)(xx << 16 | yy << 8 | zz);
                };

                int minX = Math.Max(Min, X.First);
                int maxX = Math.Min(Max, X.Last);
                int minY = Math.Max(Min, Y.First);
                int maxY = Math.Min(Max, Y.Last);
                int minZ = Math.Max(Min, Z.First);
                int maxZ = Math.Min(Max, Z.Last);
                for (int x = minX; x <= maxX; ++x)
                {
                    for (int y = minY; y <= maxY; ++y)
                    {
                        for (int z = minZ; z <= maxZ; ++z)
                        {
                            values.Add(Hash(x, y, z));
                        }
                    }
                }
                return values;
            }

            public override string ToString()
            {
                string on = On ? "on" : "off";
                return $"{on} x={X.First}..{X.Last}, y={Y.First}..{Y.Last}, z={Z.First}..{Z.Last}";
            }
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables)
        {
            List<Instruction> instructions = inputs.Select(Instruction.Parse).ToList();
            HashSet<long> on = new HashSet<long>();
            foreach (Instruction i in instructions)
            {
                IEnumerable<long> hashes = i.GetValues();
                foreach (long hash in hashes)
                {
                    if (i.On)
                    {
                        on.Add(hash);
                    }
                    else
                    {
                        on.Remove(hash);
                    }
                }
            }
            return on.Count.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);
    }
}