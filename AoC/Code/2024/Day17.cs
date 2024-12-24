using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using AoC.Base;

namespace AoC._2024
{
    class Day17 : Core.Day
    {
        public Day17() { }

        public override string GetSolutionVersion(Core.Part part)
        {
            return part switch
            {
                Core.Part.One => "v1",
                Core.Part.Two => "v1",
                _ => base.GetSolutionVersion(part),
            };
        }

        protected override List<Core.TestDatum> GetTestData()
        {
            List<Core.TestDatum> testData =
            [
                new Core.TestDatum
                {
                    TestPart = Core.Part.One,
                    Output = "B=1",
                    RawInput =
@"Register A: 0
Register B: 0
Register C: 9

Program: 2,6"
                },
                new Core.TestDatum
                {
                    TestPart = Core.Part.One,
                    Output = "0,1,2",
                    RawInput =
@"Register A: 10
Register B: 0
Register C: 0

Program: 5,0,5,1,5,4"
                },
                new Core.TestDatum
                {
                    TestPart = Core.Part.One,
                    Output = "4,2,5,6,7,7,7,7,3,1,0",
                    RawInput =
@"Register A: 2024
Register B: 0
Register C: 0

Program: 0,1,5,4,3,0"
                },
                new Core.TestDatum
                {
                    TestPart = Core.Part.One,
                    Output = "B=26",
                    RawInput =
@"Register A: 0
Register B: 29
Register C: 0

Program: 1,7"
                },
                new Core.TestDatum
                {
                    TestPart = Core.Part.One,
                    Output = "B=44354",
                    RawInput =
@"Register A: 0
Register B: 2024
Register C: 43690

Program: 4,0"
                },
                new Core.TestDatum
                {
                    TestPart = Core.Part.One,
                    Output = "4,6,3,5,6,3,5,2,1,0",
                    RawInput =
@"Register A: 729
Register B: 0
Register C: 0

Program: 0,1,5,4,3,0"
                }
            ];
            return testData;
        }

        private enum OpCode
        {
            ADivide,
            BitwiseBXOr,
            ComboMod,
            Jump,
            BitwiseBXOrC,
            ComboModOut,
            BDivide,
            CDivide
        }

        private class Computer
        {
            public ulong[] Program { get; private set; }

            public ulong A { get; set; }
            private readonly ulong m_b;
            public ulong B { get; set; }
            private readonly ulong m_c;
            public ulong C { get; set; }
            public ulong InstructionPointer { get; set; }
            public List<ulong> Output { get; set; }

            public Computer(List<string> inputs)
            {
                A = Util.String.Split(inputs[0], "RegisterA: ").Select(ulong.Parse).First();
                B = Util.String.Split(inputs[1], "RegisterB: ").Select(ulong.Parse).First();
                m_b = B;
                C = Util.String.Split(inputs[2], "RegisterC: ").Select(ulong.Parse).First();
                m_c = C;
                Program = Util.String.Split(inputs[4], "Program:, ").Select(ulong.Parse).ToArray();
                InstructionPointer = 0;
                Output = [];
            }

            public void Reset(ulong a)
            {
                A = a;
                B = m_b;
                C = m_c;
                InstructionPointer = 0;
                Output.Clear();
            }

            public bool Step()
            {
                if (InstructionPointer + 1 >= (ulong)Program.Length)
                {
                    return false;
                }

                ulong opCode = Program[InstructionPointer];
                ulong literal = Program[InstructionPointer + 1];
                ulong combo = 0;
                switch (literal)
                {
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                        combo = Program[InstructionPointer + 1];
                        break;
                    case 4:
                        combo = A;
                        break;
                    case 5:
                        combo = B;
                        break;
                    case 6:
                        combo = C;
                        break;
                    case 7:
                        break;
                }

                switch ((OpCode)opCode)
                {
                    case OpCode.ADivide:
                        A /= (ulong)Math.Pow(2, combo);
                        break;
                    case OpCode.BitwiseBXOr:
                        B ^= literal;
                        break;
                    case OpCode.ComboMod:
                        B = combo % 8;
                        break;
                    case OpCode.Jump:
                        if (A != 0 && InstructionPointer != literal)
                        {
                            InstructionPointer = literal;
                            return true;
                        }
                        break;
                    case OpCode.BitwiseBXOrC:
                        B ^= C;
                        break;
                    case OpCode.ComboModOut:
                        Output.Add(combo & 0x7);
                        break;
                    case OpCode.BDivide:
                        B = A / (ulong)Math.Pow(2, combo);
                        break;
                    case OpCode.CDivide:
                        C = A / (ulong)Math.Pow(2, combo);
                        break;
                }

                InstructionPointer += 2;
                return true;
            }
        }

        private static string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool findA)
        {
            Computer computer = new(inputs);
            if (!findA)
            {
                while (computer.Step()) ;
                return string.Join(',', computer.Output);
            }

            List<ulong> validAs = [0];
            ulong[] reversed = computer.Program.Reverse().ToArray();
            for (int i = 0; i < reversed.Length; ++i)
            {
                List<ulong> newValidAs = [];
                foreach (ulong _a in validAs)
                {
                    for (uint j = 0; j < 8; ++j)
                    {
                        ulong newA = _a << 3;
                        newA += j;
                        computer.Reset(newA);
                        while (computer.Step()) ;
                        if (computer.Output[0] == reversed[i])
                        {
                            newValidAs.Add(newA);
                        }
                    }
                }
                validAs = [.. newValidAs];
            }

            return validAs.Order().First().ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);
    }
}