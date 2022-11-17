using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AoC._2016
{
    class Day25 : Core.Day
    {
        public Day25() { }

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
                Output = "",
                RawInput =
@""
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
            JumpRegister,
            Toggle,
            OutValue,
            OutRegister
        }

        static char InvalidRegister = '-';

        private record Instruction(InstructionType Type, char Register, char SourceRegister, int Value, int SourceValue)
        {
            static public Instruction Toggle(Instruction source)
            {
                InstructionType type = InstructionType.Invalid;
                switch (source.Type)
                {
                    case InstructionType.Increment:
                        type = InstructionType.Decrement;
                        break;
                    case InstructionType.Decrement:
                        type = InstructionType.Increment;
                        break;
                    case InstructionType.CopyValue:
                        type = InstructionType.JumpValue;
                        break;
                    case InstructionType.CopyRegister:
                        type = InstructionType.JumpRegister;
                        break;
                    case InstructionType.JumpValue:
                        type = InstructionType.CopyValue;
                        break;
                    case InstructionType.JumpRegister:
                        type = InstructionType.CopyRegister;
                        break;
                    case InstructionType.Toggle:
                        type = InstructionType.Increment;
                        break;
                }

                return new Instruction(type, source.Register, source.SourceRegister, source.Value, source.SourceValue);
            }

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
                    register = split[1][0];
                }
                else if (split[0] == "dec")
                {
                    type = InstructionType.Decrement;
                    register = split[1][0];
                }
                else if (split[0] == "jnz")
                {
                    type = (char.IsDigit(split[1][0]) ? InstructionType.JumpValue : InstructionType.JumpRegister);
                    if (type == InstructionType.JumpValue)
                    {
                        value = intVals.Last();
                        sourceValue = intVals.First();
                    }
                    else
                    {
                        value = intVals.Last();
                        sourceRegister = split[1][0];
                    }
                }
                else if (split[0] == "tgl")
                {
                    type = InstructionType.Toggle;
                    register = split[1][0];
                }
                else if (split[0] == "out")
                {
                    type = (char.IsDigit(split[1][0]) ? InstructionType.OutValue : InstructionType.OutRegister);
                    if (type == InstructionType.OutValue)
                    {
                        value = intVals.Last();
                    }
                    else
                    {
                        register = split[1][0];
                    }
                }
                return new Instruction(type, register, sourceRegister, value, sourceValue);
            }
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, Dictionary<char, int> registers)
        {
            string pattern = "^(0)(?!\\1)(1)(?:\\1\\2)*\\1?$";
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
            int minLen = 10;

            List<Instruction> instructions = inputs.Select(Instruction.Parse).ToList();

            for (int a = 1; ; ++a)
            {
                registers.Clear();
                registers['a'] = a;

                StringBuilder sb = new StringBuilder();
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
                            registers[cur.Register] += 1;
                            ++i;
                            break;
                        case InstructionType.Decrement:
                            registers[cur.Register] -= 1;
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
                        case InstructionType.Toggle:
                            {
                                int offset = registers[cur.Register] + i;
                                if (offset >= 0 && offset < instructions.Count)
                                {
                                    instructions[offset] = Instruction.Toggle(instructions[offset]);
                                }
                            }
                            ++i;
                            break;
                        case InstructionType.OutValue:
                            sb.Append(cur.Value);
                            break;
                        case InstructionType.OutRegister:
                            sb.Append(registers[cur.Register]);
                            ++i;
                            break;
                    }

                    if (sb.Length > 1 && !regex.Match(sb.ToString()).Success)
                    {
                        break;
                    }

                    if (sb.Length >= minLen)
                    {
                        return a.ToString();
                    }
                }
            }
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, new Dictionary<char, int>() { { 'a', 7 } });

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
        { RunPart1Solution(inputs, variables); return "50"; }
    }
}