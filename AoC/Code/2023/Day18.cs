using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AoC._2023
{
    class Day18 : Core.Day
    {
        public Day18() { }

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
                Output = "62",
                RawInput =
@"R 6 (#70c710)
D 5 (#0dc571)
L 2 (#5713f0)
D 2 (#d2c081)
R 2 (#59c680)
D 2 (#411b91)
L 5 (#8ceee2)
U 2 (#caa173)
L 1 (#1b58a2)
U 2 (#caa171)
R 2 (#7807d2)
U 3 (#a77fa3)
L 2 (#015232)
U 2 (#7a21e3)"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "952408144115",
                RawInput =
@"R 6 (#70c710)
D 5 (#0dc571)
L 2 (#5713f0)
D 2 (#d2c081)
R 2 (#59c680)
D 2 (#411b91)
L 5 (#8ceee2)
U 2 (#caa173)
L 1 (#1b58a2)
U 2 (#caa171)
R 2 (#7807d2)
U 3 (#a77fa3)
L 2 (#015232)
U 2 (#7a21e3)"
            });
            return testData;
        }

        private const char Border = '#';
        private const char Fill = '~';
        private const char Empty = '.';

        private enum Direction { North = 'U', South = 'D', East = 'R', West = 'L', None = '.' }
        private static Dictionary<Direction, Base.Pos2L> Next = new Dictionary<Direction, Base.Pos2L>()
        {
            {Direction.North, new Base.Pos2L(0, -1)},
            {Direction.South, new Base.Pos2L(0, 1)},
            {Direction.East, new Base.Pos2L(1, 0)},
            {Direction.West, new Base.Pos2L(-1, 0)}
        };
        static readonly Base.Pos2L[] GridMoves = new Base.Pos2L[] { new Base.Pos2L(0, 1), new Base.Pos2L(1, 0), new Base.Pos2L(-1, 0), new Base.Pos2L(0, -1) };

        private record Instruction(Direction Direction, long Meters, string RGB)
        {
            public static Instruction Parse(string input)
            {
                string[] split = Util.String.Split(input, " ()");
                return new Instruction((Direction)split[0][0], long.Parse(split[1]), split[2]);
            }
            public static Instruction ParseHex(string input)
            {
                string[] split = Util.String.Split(input, " (#)");
                Direction direction = Direction.None;
                switch (split[2].Last())
                {
                    case '0':
                        direction = Direction.East;
                        break;
                    case '1':
                        direction = Direction.South;
                        break;
                    case '2':
                        direction = Direction.West;
                        break;
                    case '3':
                        direction = Direction.North;
                        break;
                }
                long hex = Convert.ToInt64(string.Join("", split[2].SkipLast(1)), 16);
                return new Instruction(direction, hex, split[2]);
            }
        }

        private record Check(Base.Pos2L Pos2L, bool IsBorder);

        private void GetPositions(List<string> inputs, out List<Base.Pos2L> positions, bool useHex)
        {
            List<Instruction> instructions;
            if (useHex)
            {
                instructions = inputs.Select(Instruction.ParseHex).ToList();
            }
            else
            {
                instructions = inputs.Select(Instruction.Parse).ToList();
            }

            positions = new List<Base.Pos2L>();
            Base.Pos2L cur = new Base.Pos2L(), min = new Base.Pos2L();
            foreach (Instruction instruction in instructions)
            {
                Base.Pos2L next = new Base.Pos2L();
                switch (instruction.Direction)
                {
                    case Direction.North:
                        next = cur + new Base.Pos2L(0, -instruction.Meters);
                        break;
                    case Direction.East:
                        next = cur + new Base.Pos2L(instruction.Meters, 0);
                        break;
                    case Direction.South:
                        next = cur + new Base.Pos2L(0, instruction.Meters);
                        break;
                    case Direction.West:
                        next = cur + new Base.Pos2L(-instruction.Meters, 0);
                        break;
                }
                min.X = Math.Min(min.X, next.X);
                min.Y = Math.Min(min.Y, next.Y);
                positions.Add(next);
                cur = next;
            }

            for (int i = 0; i < positions.Count; ++i)
            {
                positions[i] += min;
            }
        }

        private BigInteger CalcShoelace(List<Base.Pos2L> positions)
        {
            // area = 1/2 * (x1y2 + x2y3 + xny1 - x2y1 - x3y2 - x1yn)
            BigInteger area = 0;
            for (int i = 0; i < positions.Count; ++i)
            {
                int j = (i + 1) % positions.Count;
                Base.Pos2L cur = positions[i];
                Base.Pos2L nxt = positions[j];
                area += cur.X * nxt.Y - nxt.X * cur.Y;
            }
            return area / 2;
        }

        private BigInteger CalcPerimeter(List<Base.Pos2L> positions)
        {
            BigInteger perimeter = 0;
            for (int i = 0; i < positions.Count; ++i)
            {
                int j = (i + 1) % positions.Count;
                Base.Vec2L vec = Base.Vec2L.FromPos(positions[i], positions[j]);
                perimeter += vec.GetLength();
            }
            return perimeter / 2 + 1;
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool useHex)
        {
            GetPositions(inputs, out List<Base.Pos2L> positions, useHex);
            return (CalcShoelace(positions) + CalcPerimeter(positions)).ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);
    }
}