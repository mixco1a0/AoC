using System.Diagnostics;
using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2022
{
    class Day23 : Core.Day
    {
        public Day23() { }

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
            //             testData.Add(new Core.TestDatum
            //             {
            //                 TestPart = Core.Part.One,
            //                 Output = "110",
            //                 RawInput =
            // @".....
            // ..##.
            // ..#..
            // .....
            // ..##.
            // ....."
            //             });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.One,
                Output = "110",
                RawInput =
@"....#..
..###.#
#...#.#
.#...##
#.###..
##.#.##
.#..#.."
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

        private static char ElfChar { get { return '#'; } }
        private static char NorthChar { get { return 'N'; } }
        private static char SouthChar { get { return 'S'; } }
        private static char WestChar { get { return 'W'; } }
        private static char EastChar { get { return 'E'; } }

        private static readonly char[] Directions = new char[] { NorthChar, SouthChar, WestChar, EastChar };
        private record Position2(int X, int Y)
        {

            public static Position2 operator +(Position2 a, Position2 b)
            {
                return new Position2(a.X + b.X, a.Y + b.Y);
            }
        }
        private static readonly Position2[] AllNeighborOffsets = new Position2[]
        {
            new Position2(-1, -1), new Position2(0, -1), new Position2(1, -1),
            new Position2(-1, 0),   /*    cur pos     */ new Position2(1, 0),
            new Position2(-1, 1),  new Position2(0, 1),  new Position2(1, 1)
        };
        private static readonly Dictionary<char, Position2[]> NeighborOffsets = new Dictionary<char, Position2[]>
        {
            {NorthChar, new Position2[] { new Position2(-1, -1), new Position2(0, -1), new Position2(1, -1) }},
            {SouthChar, new Position2[] { new Position2(-1, 1), new Position2(0, 1), new Position2(1, 1) }},
            {WestChar, new Position2[] { new Position2(-1, 1), new Position2(-1, 0), new Position2(-1, -1) }},
            {EastChar, new Position2[] { new Position2(1, 1), new Position2(1, 0), new Position2(1, -1) }}
        };

        private class Elf
        {
            public Position2 Pos { get; set; }
            public int MoveDirection { get; set; }
            public char Id { get; set; }

            static public int DirectionIdx { get; set; }
            static private char GlobalId = 'a';

            public Elf(int x, int y)
            {
                Pos = new Position2(x, y);
                MoveDirection = 0;
                Id = GlobalId++;
                if (GlobalId == ':')
                {
                    GlobalId = 'A';
                }
                else if (GlobalId == '[')
                {
                    GlobalId = 'a';
                }
                else if (GlobalId == '{')
                {
                    GlobalId = '0';
                }
            }

            public Position2 MovePos()
            {
                return Pos + NeighborOffsets[Directions[MoveDirection]][1];
            }

            public static void NextDirection()
            {
                DirectionIdx = (DirectionIdx + 1) % Directions.Length;
            }

            public override string ToString()
            {
                return $"{Id} ({Pos.X},{Pos.Y})";
            }
        }



        private void Parse(List<string> inputs, out HashSet<Elf> elves)
        {
            elves = new HashSet<Elf>();
            for (int y = 0; y < inputs.Count; ++y)
            {
                for (int x = 0; x < inputs[y].Length; ++x)
                {
                    if (inputs[y][x] == ElfChar)
                    {
                        elves.Add(new Elf(x, y));
                    }
                }
            }
        }

        private void PerformStepOne(ref HashSet<Elf> elves, out HashSet<Elf> stationaryElves, out Dictionary<Position2, bool> potentialMoves)
        {
            // round 1, generate dictionary of potential moved locations
            HashSet<Position2> curElfPositions = elves.Select(e => e.Pos).ToHashSet();
            stationaryElves = new HashSet<Elf>();
            potentialMoves = new Dictionary<Position2, bool>();
            foreach (Elf elf in elves)
            {
                bool elfCanMove = false;
                foreach (Position2 pos in AllNeighborOffsets)
                {
                    Position2 neighbor = elf.Pos + pos;
                    if (curElfPositions.Contains(neighbor))
                    {
                        elfCanMove = true;
                        break;
                    }
                }
                if (!elfCanMove)
                {
                    stationaryElves.Add(elf);
                    potentialMoves[elf.Pos] = false;
                    continue;
                }

                elfCanMove = false;
                for (int d = 0; d < Directions.Length; ++d)
                {
                    int idx = (Elf.DirectionIdx + d) % Directions.Length;
                    bool canUseDirection = true;
                    foreach (Position2 pos in NeighborOffsets[Directions[idx]])
                    {
                        Position2 neighbor = elf.Pos + pos;
                        if (curElfPositions.Contains(neighbor))
                        {
                            canUseDirection = false;
                            break;
                        }
                    }

                    if (canUseDirection)
                    {
                        elf.MoveDirection = idx;
                        Position2 movePos = elf.MovePos();
                        if (potentialMoves.ContainsKey(movePos))
                        {
                            potentialMoves[movePos] = false;
                        }
                        else
                        {
                            potentialMoves[movePos] = true;
                        }
                        elfCanMove = true;
                        break;
                    }
                }

                if (!elfCanMove)
                {
                    stationaryElves.Add(elf);
                    potentialMoves[elf.Pos] = false;
                }
            }
        }

        private void PerformStepTwo(ref HashSet<Elf> elves, HashSet<Elf> stationaryElves, Dictionary<Position2, bool> potentialMoves)
        {
            foreach (Elf elf in elves)
            {
                if (stationaryElves.Contains(elf))
                {
                    continue;
                }

                if (potentialMoves[elf.MovePos()])
                {
                    elf.Pos = elf.MovePos();
                }
            }
            Elf.NextDirection();
        }

        private void PrintElves(HashSet<Elf> elves)
        {
            var pos = elves.Select(e => e.Pos);
            var xs = pos.Select(p => p.X);
            var ys = pos.Select(p => p.Y);
            int minX = xs.Min() - 1;
            int maxX = xs.Max() + 1;
            int minY = ys.Min() - 1;
            int maxY = ys.Max() + 1;


            StringBuilder sb = new StringBuilder();
            DebugWriteLine("Elves:");
            for (int y = minY; y <= maxY; ++y)
            {
                sb.Clear();
                sb.Append($"{y,4}| ");
                for (int x = minX; x <= maxX; ++x)
                {
                    Position2 curPos = new Position2(x, y);
                    if (pos.Contains(curPos))
                    {
                        sb.Append(elves.First(e => e.Pos == curPos).Id);
                    }
                    else
                    {
                        sb.Append('.');
                    }
                }
                DebugWriteLine(sb.ToString());
            }
            DebugWriteLine(".");
            DebugWriteLine("..");
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, int rounds)
        {
            Parse(inputs, out HashSet<Elf> elves);
            // PrintElves(elves);
            for (int r = 0; r < rounds; ++r)
            {
                PerformStepOne(ref elves, out HashSet<Elf> stationaryElves, out Dictionary<Position2, bool> potentialMoves);
                PerformStepTwo(ref elves, stationaryElves, potentialMoves);
                // PrintElves(elves);
            }

            int size = 0;
            {
                var pos = elves.Select(e => e.Pos);
                var xs = pos.Select(p => p.X);
                var ys = pos.Select(p => p.Y);
                int minX = xs.Min();
                int maxX = xs.Max();
                int minY = ys.Min();
                int maxY = ys.Max();
                size = (maxX - minX + 1) * (maxY - minY + 1);
            }

            return (size - elves.Count).ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, 10);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, 0);
    }
}