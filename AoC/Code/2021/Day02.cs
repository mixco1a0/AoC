using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2021
{
    class Day02 : Day
    {
        public Day02() { }

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
                Output = "150",
                RawInput =
@"forward 5
down 5
forward 8
up 3
down 8
forward 2"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.Two,
                Output = "900",
                RawInput =
@"forward 5
down 5
forward 8
up 3
down 8
forward 2"
            });
            return testData;
        }

        private record Instruction(char Direction, int Position)
        {
            public static Instruction Parse(string input)
            {
                string[] split = input.Split(' ');
                return new Instruction(split[0][0], int.Parse(split[1]));
            }
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool useZ)
        {
            Instruction[] instructions = inputs.Select(Instruction.Parse).ToArray();
            int x = 0, y = 0, z = 0;
            foreach (Instruction i in instructions)
            {
                switch (i.Direction)
                {
                    case 'f':
                        x += i.Position;
                        if (useZ)
                        {
                            z += y * i.Position;
                        }
                        break;
                    case 'd':
                        y += i.Position;
                        break;
                    case 'u':
                        y -= i.Position;
                        break;
                }
            }
            if (useZ)
            {
                return (x * z).ToString();
            }
            return (x * y).ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);
    }
}