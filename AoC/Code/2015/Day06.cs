using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2015
{
    class Day06 : Core.Day
    {
        public Day06() { }

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
                Output = "4",
                RawInput =
@"turn on 499,499 through 500,500"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.One,
                Output = "1000000",
                RawInput =
@"toggle 0,0 through 999,999"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "1",
                RawInput =
@"turn on 0,0 through 0,0"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "2000000",
                RawInput =
@"toggle 0,0 through 999,999"
            });
            return testData;
        }

        private enum InstructionType
        {
            Invalid,
            On,
            Off,
            Toggle
        }

        private record Instruction(InstructionType Type, int xMin, int xMax, int yMin, int yMax)
        {
            public static Instruction Parse(string input)
            {
                string[] split = input.Split(" ,g".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries).ToArray();

                InstructionType type = InstructionType.Invalid;
                switch (split[1][1])
                {
                    case 'n':
                        type = InstructionType.On;
                        break;
                    case 'f':
                        type = InstructionType.Off;
                        break;
                    case 'e':
                        type = InstructionType.Toggle;
                        break;
                }

                int[] intVals = split.Where(s => { int i; return int.TryParse(s, out i); }).Select(int.Parse).ToArray();
                return new Instruction(type, intVals[0], intVals[2], intVals[1], intVals[3]);
            }
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, int min, int max, Func<int, int> toggleFunc)
        {
            const int gridSize = 1000;
            int[,] grid = new int[gridSize, gridSize];
            for (int i = 0; i < gridSize; ++i)
            {
                for (int j = 0; j < gridSize; ++j)
                {
                    grid[i, j] = min;
                }
            }

            Instruction[] instructions = inputs.Select(Instruction.Parse).ToArray();
            foreach (Instruction instruction in instructions)
            {
                for (int x = instruction.xMin; x <= instruction.xMax; ++x)
                {
                    for (int y = instruction.yMin; y <= instruction.yMax; ++y)
                    {
                        switch (instruction.Type)
                        {
                            case InstructionType.On:
                                grid[x, y] = Math.Min(grid[x, y] + 1, max);
                                break;
                            case InstructionType.Off:
                                grid[x, y] = Math.Max(grid[x, y] - 1, min);
                                break;
                            case InstructionType.Toggle:
                                grid[x, y] = toggleFunc(grid[x, y]);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            int count = 0;
            for (int i = 0; i < gridSize; ++i)
            {
                for (int j = 0; j < gridSize; ++j)
                {
                    count += grid[i, j];
                }
            }

            return count.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, 0, 1, (int val) => (val + 1) % 2);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, 0, int.MaxValue, (int val) => val + 2);
    }
}