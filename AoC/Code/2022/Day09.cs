using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2022
{
    class Day09 : Core.Day
    {
        public Day09() { }

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
@"R 4
U 4
L 3
D 1
R 4
D 1
L 5
R 2"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "1",
                RawInput =
@"R 4
U 4
L 3
D 1
R 4
D 1
L 5
R 2"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "36",
                RawInput =
@"R 5
U 8
L 8
D 3
R 17
D 10
L 25
U 20"
            });
            return testData;
        }

        private record Instruction(char Direction, int Steps)
        {
            public static Instruction Parse(string input)
            {
                string[] split = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                return new Instruction(split[0][0], int.Parse(split[1]));
            }
        }

        private void Move(char direction, ref Base.Point head)
        {
            switch (direction)
            {
                case 'R':
                    head.X += 1;
                    break;
                case 'L':
                    head.X -= 1;
                    break;
                case 'U':
                    head.Y += 1;
                    break;
                case 'D':
                    head.Y -= 1;
                    break;
            }
        }

        private void Adjust(char direction, ref Base.Point head, ref Base.Point tail)
        {
            bool adjustX = Math.Abs(head.X - tail.X) > 1;
            bool adjustY = Math.Abs(head.Y - tail.Y) > 1;
            if (adjustX && adjustY)
            {
                tail = new Base.Point(head.X + (head.X > tail.X ? -1 : 1), head.Y + (head.Y > tail.Y ? -1 : 1));
            }
            else if (adjustX)
            {
                if (head.Y == tail.Y)
                {
                    tail.X += head.X > tail.X ? 1 : -1;
                }
                else
                {
                    tail = new Base.Point(head.X + (head.X > tail.X ? -1 : 1), head.Y);
                }
            }
            else if (adjustY)
            {
                if (head.X == tail.X)
                {
                    tail.Y += head.Y > tail.Y ? 1 : -1;
                }
                else
                {
                    tail = new Base.Point(head.X, head.Y + (head.Y > tail.Y ? -1 : 1));
                }
            }
        }

        private void PrintRope(Base.Point[] rope)
        {
            int minX = Math.Min(rope.Min(r => r.X) - 1, -1);
            int maxX = Math.Max(rope.Max(r => r.X) + 2, 2);
            int minY = Math.Min(rope.Min(r => r.Y) - 2, -2);
            int maxY = Math.Max(rope.Max(r => r.Y) + 1, 1);

            char[][] grid = new char[maxY - minY][];
            for (int y = 0; y < grid.Length; ++y)
            {
                grid[y] = new char[maxX - minX];
                for (int x = 0; x < grid[y].Length; ++x)
                {
                    grid[y][x] = '.';
                }
            }

            for (int r = 0; r < rope.Length; ++r)
            {
                int x = rope[r].X - minX;
                int y = maxY - rope[r].Y;

                if (grid[y][x] == '.')
                {
                    grid[y][x] = r.ToString()[0];
                }
            }

            if (grid[maxY][0 - minX] == '.')
            {
                grid[maxY][0 - minX] = 's';
            }

            Util.Grid.PrintGrid(Core.Log.ELevel.Debug, grid);
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, int nodeCount)
        {
            Instruction[] instructions = inputs.Select(Instruction.Parse).ToArray();
            Base.Point[] rope = new Base.Point[nodeCount];
            for (int r = 0; r < rope.Length; ++r)
            {
                rope[r] = new Base.Point(0, 0);
            }
            bool massPrint = false;

            HashSet<Base.Point> visited = new HashSet<Base.Point>();
            foreach (Instruction i in instructions)
            {
                for (int s = 0; s < i.Steps; ++s)
                {
                    Move(i.Direction, ref rope[0]);
                    for (int r = 1; r < rope.Length; ++r)
                    {
                        Adjust(i.Direction, ref rope[r - 1], ref rope[r]);

                        if (massPrint)
                        {
                            PrintRope(rope);
                        }
                    }

                    if (!visited.Contains(rope.Last()))
                    {
                        visited.Add(new(rope.Last()));
                    }

                    if (nodeCount > 2)
                    {
                        // DebugWriteLine($"Moving: {i.Direction}");
                        // PrintRope(rope);
                    }
                }
            }
            return visited.Count.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, 2);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, 10);
    }
}