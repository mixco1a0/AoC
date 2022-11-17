using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2016
{
    class Day12 : Core.Day
    {
        public Day12() { }

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
                Output = "42",
                RawInput =
@"cpy 41 a
inc a
inc a
dec a
jnz a 2
dec a"
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

        private enum InstructionType
        {
            Invalid,
            CopyValue,
            CopyRegister,
            Increment,
            Decrement,
            JumpValue,
            JumpRegister
        }

        static char InvalidRegister = '-';

        private record Instruction(InstructionType Type, char Register, char SourceRegister, int Value, int SourceValue)
        {
            static public Instruction Parse(string input)
            {
                InstructionType type = InstructionType.Invalid;
                string[] split = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                IEnumerable<int> intVals = split.Where(s => { int sint; return int.TryParse(s, out sint); }).Select(int.Parse);
                char register = InvalidRegister;
                int value = 0;
                char sourceRegister = InvalidRegister;
                int sourceValue = 0;
                if (split[0] == "cpy")
                {
                    type = (intVals.Count() > 0 ? InstructionType.CopyValue : InstructionType.CopyRegister);
                    register = split.Last()[0];
                    if (type == InstructionType.CopyValue)
                    {
                        sourceValue = intVals.First();
                    }
                    else
                    {
                        sourceRegister = split[1][0];
                    }
                }
                else if (split[0] == "inc")
                {
                    type = InstructionType.Increment;
                    sourceValue = 1;
                    register = split[1][0];
                }
                else if (split[0] == "dec")
                {
                    type = InstructionType.Decrement;
                    sourceValue = -1;
                    register = split[1][0];
                }
                else if (split[0] == "jnz")
                {
                    type = (intVals.Count() > 1 ? InstructionType.JumpValue : InstructionType.JumpRegister);
                    value = intVals.Last();
                    if (type == InstructionType.JumpValue)
                    {
                        sourceValue = intVals.First();
                    }
                    else
                    {
                        sourceRegister = split[1][0];
                    }
                }
                return new Instruction(type, register, sourceRegister, value, sourceValue);
            }
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, Dictionary<char, int> registers)
        {
            List<Instruction> instructions = inputs.Select(Instruction.Parse).ToList();
            for (int i = 0; i < instructions.Count && i >= 0;)
            {
                Instruction cur = instructions[i];

                if (cur.Register != InvalidRegister && !registers.ContainsKey(cur.Register))
                {
                    registers[cur.Register] = 0;
                }

                if (cur.SourceRegister != InvalidRegister && !registers.ContainsKey(cur.SourceRegister))
                {
                    registers[cur.SourceRegister] = 0;
                }

                switch (cur.Type)
                {
                    case InstructionType.Increment:
                    case InstructionType.Decrement:
                        registers[cur.Register] += cur.SourceValue;
                        ++i;
                        break;
                    case InstructionType.CopyValue:
                        registers[cur.Register] = cur.SourceValue;
                        ++i;
                        break;
                    case InstructionType.CopyRegister:
                        registers[cur.Register] = registers[cur.SourceRegister];
                        ++i;
                        break;
                    case InstructionType.JumpValue:
                        i = (cur.SourceValue != 0 ? i + cur.Value : i + 1);
                        break;
                    case InstructionType.JumpRegister:
                        i = (registers[cur.SourceRegister] != 0 ? i + cur.Value : i + 1);
                        break;
                }
            }
            return registers['a'].ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, new Dictionary<char, int>());

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, new Dictionary<char, int>() { { 'c', 1 } });
    }
}