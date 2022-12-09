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
                Output = "",
                RawInput =
@""
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

        private void Move(char direction, ref Base.Point head, ref Base.Point tail, ref HashSet<Base.Point> visited)
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

            // only adjust tail if needed
            if (Math.Abs(head.X - tail.X) > 1)
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
            else if (Math.Abs(head.Y - tail.Y) > 1)
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

            if (!visited.Contains(tail))
            {
                visited.Add(new (tail));
            }
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables)
        {
            Instruction[] instructions = inputs.Select(Instruction.Parse).ToArray();
            Base.Point head = new Base.Point(0, 0);
            Base.Point tail = new Base.Point(0, 0);
            HashSet<Base.Point> visited = new HashSet<Base.Point>();
            foreach (Instruction i in instructions)
            {
                for (int s = 0; s < i.Steps; ++s)
                {
                    Move(i.Direction, ref head, ref tail, ref visited);
                }
            }
            return visited.Count.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);
    }
}