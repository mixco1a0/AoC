using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2023
{
    class Day18 : Core.Day
    {
        public Day18() { }

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

        private void GetGrid(List<string> inputs, out char[,] grid, out Base.Pos2L start, bool useHex)
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
            List<Base.SegmentL> segments = new List<Base.SegmentL>();
            Base.Pos2L cur = new Base.Pos2L(), min = new Base.Pos2L(long.MaxValue, long.MaxValue), max = new Base.Pos2L(long.MinValue, long.MinValue);
            foreach (Instruction instruction in instructions)
            {
                Base.Pos2L next = cur;
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
                segments.Add(new Base.SegmentL(cur, next));
                cur = next;
                min.X = Math.Min(min.X, cur.X);
                min.Y = Math.Min(min.Y, cur.Y);
                max.X = Math.Max(max.X, cur.X);
                max.Y = Math.Max(max.Y, cur.Y);
            }

            // get the starting point for flood fill
            Base.Pos2L first = segments.First().B - min;
            Base.Pos2L last = segments.Last().A - min;
            start = first + last;
            start.X = start.X / Math.Abs(start.X) - min.X;
            start.Y = start.Y / Math.Abs(start.Y) - min.Y;

            // initialize the grid
            grid = new char[max.X - min.X + 1, max.Y - min.Y + 1];
            for (int x = 0; x < grid.GetLength(0); ++x)
            {
                for (int y = 0; y < grid.GetLength(1); ++y)
                {
                    grid[x, y] = Empty;
                }
            }

            // populate the grid
            foreach (Base.SegmentL segment in segments)
            {
                bool ascendingX = segment.B.X >= segment.A.X;
                Func<long, long> iterX = (x) => ascendingX ? x + 1 : x - 1;

                bool ascendingY = segment.B.Y >= segment.A.Y;
                Func<long, long> iterY = (y) => ascendingY ? y + 1 : y - 1;

                for (long x = segment.A.X; ascendingX ? (x <= segment.B.X) : (x >= segment.B.X); x = iterX(x))
                {
                    for (long y = segment.A.Y; ascendingY ? (y <= segment.B.Y) : (y >= segment.B.Y); y = iterY(y))
                    {
                        grid[x - min.X, y - min.Y] = Border;
                    }
                }
            }
        }

        private void FloodFillGrid(ref char[,] grid, Base.Pos2L start)
        {
            int xMax = grid.GetLength(0);
            int yMax = grid.GetLength(1);

            Queue<Base.Pos2L> queue = new Queue<Base.Pos2L>();
            queue.Enqueue(start);
            while (queue.Count > 0)
            {
                Base.Pos2L curPos = queue.Dequeue();
                if (grid[curPos.X, curPos.Y] != Empty)
                {
                    continue;
                }
                grid[curPos.X, curPos.Y] = Fill;

                foreach (Base.Pos2L gridMove in GridMoves)
                {
                    Base.Pos2L nextPos = curPos + gridMove;
                    if (nextPos.X >= 0 && nextPos.X < xMax && nextPos.Y >= 0 && nextPos.Y < yMax)
                    {
                        queue.Enqueue(nextPos);
                    }
                }
            }
        }

        private long GetSize(char[,] grid)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int y = 0; y < grid.GetLength(1); ++y)
            {
                stringBuilder.Append(string.Join(string.Empty, Enumerable.Range(0, grid.GetLength(0)).Select(x => grid[x, y])));
            }
            return stringBuilder.ToString().Count(c => c == Fill || c == Border);
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool useHex)
        {
            GetGrid(inputs, out char[,] grid, out Base.Pos2L start, useHex);
            FloodFillGrid(ref grid, start);
            // Util.Grid.PrintGrid(grid, Core.Log.ELevel.Debug);
            return GetSize(grid).ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);
    }
}