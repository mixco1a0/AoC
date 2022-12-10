using System.Collections.Specialized;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2022
{
    class Day10 : Core.Day
    {
        public Day10() { }

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
                Output = "13140",
                RawInput =
@"addx 15
addx -11
addx 6
addx -3
addx 5
addx -1
addx -8
addx 13
addx 4
noop
addx -1
addx 5
addx -1
addx 5
addx -1
addx 5
addx -1
addx 5
addx -1
addx -35
addx 1
addx 24
addx -19
addx 1
addx 16
addx -11
noop
noop
addx 21
addx -15
noop
noop
addx -3
addx 9
addx 1
addx -3
addx 8
addx 1
addx 5
noop
noop
noop
noop
noop
addx -36
noop
addx 1
addx 7
noop
noop
noop
addx 2
addx 6
noop
noop
noop
noop
noop
addx 1
noop
noop
addx 7
addx 1
noop
addx -13
addx 13
addx 7
noop
addx 1
addx -33
noop
noop
noop
addx 2
noop
noop
noop
addx 8
noop
addx -1
addx 2
addx 1
noop
addx 17
addx -9
addx 1
addx 1
addx -3
addx 11
noop
noop
addx 1
noop
addx 1
noop
noop
addx -13
addx -19
addx 1
addx 3
addx 26
addx -30
addx 12
addx -1
addx 3
addx 1
noop
noop
noop
addx -9
addx 18
addx 1
addx 2
noop
noop
addx 9
noop
noop
noop
addx -1
addx 2
addx -37
addx 1
addx 3
noop
addx 15
addx -21
addx 22
addx -6
addx 1
noop
addx 2
addx 1
noop
addx -10
noop
noop
addx 20
addx 1
addx 2
addx 2
addx -6
addx -11
noop
noop
noop"
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

        private record Instruction(bool NoOp, int XDiff)
        {
            public static Instruction Parse(string input)
            {
                if (input == "noop")
                {
                    return new Instruction(true, 0);
                }
                string[] split = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                return new Instruction(false, int.Parse(split[1]));
            }
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables)
        {
            HashSet<int> interestingSignals = new HashSet<int>() { 20, 60, 100, 140, 180, 220 };
            List<Instruction> instructions = inputs.Select(Instruction.Parse).ToList();
            int x = 1;
            int cycle = 1;
            int signalStrength = 0;
            foreach (Instruction i in instructions)
            {
                if (interestingSignals.Contains(cycle))
                {
                    signalStrength += cycle * x;
                }

                if (i.NoOp)
                {
                    ++cycle;
                    continue;
                }

                ++cycle;
                if (interestingSignals.Contains(cycle))
                {
                    signalStrength += cycle * x;
                }

                x += i.XDiff;
                ++cycle;
            }
            return signalStrength.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);
    }
}