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

        void PrintCRT(char[][] crt)
        {
            DebugWriteLine("CRT:");
            foreach (char[] c in crt)
            {
                DebugWriteLine(string.Join("", c));
            }
            DebugWriteLine("");
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool watchCRT)
        {
            HashSet<int> interestingSignals = new HashSet<int>() { 20, 60, 100, 140, 180, 220 };
            List<Instruction> instructions = inputs.Select(Instruction.Parse).ToList();
            int crtX = 0;
            int crtY = 0;
            char[][] crt = new char[6][];
            for (int i = 0; i < crt.Length; ++i)
            {
                crt[i] = new char[40];
            }
            int spritePosition = 1;
            int cycle = 1;
            int signalStrength = 0;
            bool delay = false;
            for (int i = 0; i < instructions.Count; ++i)
            {
                Instruction instruction = instructions[i];
                if (interestingSignals.Contains(cycle))
                {
                    signalStrength += cycle * spritePosition;
                }

                if (watchCRT)
                {
                    if (Math.Abs(spritePosition - crtX) <= 1)
                    {
                        crt[crtY][crtX] = '#';
                    }
                    else
                    {
                        crt[crtY][crtX] = '.';
                    }
                    ++crtX;
                    if (crtX >= crt[0].Length)
                    {
                        crtX = 0;
                        ++crtY;
                        if (crtY >= crt.Length)
                        {
                            PrintCRT(crt);
                            string[] glyph = crt.Select(row => string.Join("", row)).ToArray();
                            return Util.GlyphConverter.Process(glyph, Util.GlyphConverter.EType._5x6);
                        }
                    }
                }

                if (!instruction.NoOp)
                {
                    if (!delay)
                    {
                        delay = true;
                        --i;
                    }
                    else
                    {
                        delay = false;
                    }
                }

                ++cycle;

                if (!instruction.NoOp && !delay)
                {
                    spritePosition += instruction.XDiff;
                }
            }
            return signalStrength.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);
    }
}