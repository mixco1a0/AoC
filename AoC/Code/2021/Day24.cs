using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2021
{
    class Day24 : Day
    {
        public Day24() { }

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
                Variables = new Dictionary<string, string>() { { "fullRegisterInput", "1" }, { "returnRegister", "x" } },
                Output = "-1",
                RawInput =
@"inp x
mul x -1"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.One,
                Variables = new Dictionary<string, string>() { { "fullRegisterInput", "13" } },
                Output = "1",
                RawInput =
@"inp z
inp x
mul z 3
eql z x"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.One,
                Variables = new Dictionary<string, string>() { { "fullRegisterInput", "12" } },
                Output = "0",
                RawInput =
@"inp z
inp x
mul z 3
eql z x"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.One,
                Variables = new Dictionary<string, string>() { { "fullRegisterInput", "7" } },
                Output = "1",
                RawInput =
@"inp w
add z w
mod z 2
div w 2
add y w
mod y 2
div w 2
add x w
mod x 2
div w 2
mod w 2"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.One,
                Variables = new Dictionary<string, string>() { { "fullRegisterInput", "6" } },
                Output = "0",
                RawInput =
@"inp w
add z w
mod z 2
div w 2
add y w
mod y 2
div w 2
add x w
mod x 2
div w 2
mod w 2"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.Two,
                Output = "",
                RawInput =
@""
            });
            return testData;
        }

        private class Instruction
        {
            public enum EType
            {
                Invalid,
                Input,
                Add,
                Multiply,
                Divide,
                Modulus,
                Equals
            }

            public EType Type { get; init; }
            public char TargetRegister { get; init; }
            public int Source { get; init; }
            public char SourceRegister { get; init; }
            public bool IsRef { get => char.IsLetter(SourceRegister); }

            private Instruction(EType type, char targetRegister, int source, char sourceRegister)
            {
                Type = type;
                TargetRegister = targetRegister;
                Source = source;
                SourceRegister = sourceRegister;
            }

            public static Instruction Parse(string input)
            {
                string[] split = input.Split(' ');

                EType type = EType.Invalid;
                switch (split[0])
                {
                    case "inp":
                        type = EType.Input;
                        break;
                    case "add":
                        type = EType.Add;
                        break;
                    case "mul":
                        type = EType.Multiply;
                        break;
                    case "div":
                        type = EType.Divide;
                        break;
                    case "mod":
                        type = EType.Modulus;
                        break;
                    case "eql":
                        type = EType.Equals;
                        break;
                }

                if (split.Length > 2)
                {
                    char sourceRegister = split[2][0];
                    int.TryParse(split[2], out int source);
                    return new Instruction(type, split[1][0], source, sourceRegister);
                }
                return new Instruction(type, split[1][0], 0, '0');
            }

            public bool Execute(ref Dictionary<char, int> registers, ref Queue<int> curInput, Action<string> printFunc)
            {
                int sourceValue = Source;
                if (IsRef)
                {
                    sourceValue = registers[SourceRegister];
                }
                int targetValue = registers[TargetRegister];

                switch (Type)
                {
                    case EType.Input:
                        targetValue = curInput.Dequeue();
                        break;
                    case EType.Add:
                        targetValue += sourceValue;
                        break;
                    case EType.Multiply:
                        targetValue *= sourceValue;
                        break;
                    case EType.Divide:
                        if (sourceValue == 0)
                        {
                            printFunc("Divide by 0!");
                            return false;
                        }
                        targetValue /= sourceValue;
                        break;
                    case EType.Modulus:
                        if (targetValue < 0 || sourceValue <= 0)
                        {
                            printFunc("Invalid Mod");
                            return false;
                        }
                        targetValue %= sourceValue;
                        break;
                    case EType.Equals:
                        targetValue = (targetValue == sourceValue ? 1 : 0);
                        break;
                }

                registers[TargetRegister] = targetValue;
                return true;
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                switch (Type)
                {
                    case EType.Input:
                        sb.Append(TargetRegister);
                        sb.Append("=[input]");
                        break;
                    case EType.Add:
                        sb.Append(TargetRegister);
                        sb.Append("+=");
                        if (IsRef)
                        {
                            sb.Append(SourceRegister);
                        }
                        else
                        {
                            sb.Append(Source);
                        }
                        break;
                    case EType.Multiply:
                        sb.Append(TargetRegister);
                        sb.Append("*=");
                        if (IsRef)
                        {
                            sb.Append(SourceRegister);
                        }
                        else
                        {
                            sb.Append(Source);
                        }
                        break;
                    case EType.Divide:
                        sb.Append(TargetRegister);
                        sb.Append("/=");
                        if (IsRef)
                        {
                            sb.Append(SourceRegister);
                        }
                        else
                        {
                            sb.Append(Source);
                        }
                        break;
                    case EType.Modulus:
                        sb.Append(TargetRegister);
                        sb.Append("%=");
                        if (IsRef)
                        {
                            sb.Append(SourceRegister);
                        }
                        else
                        {
                            sb.Append(Source);
                        }
                        break;
                    case EType.Equals:
                        sb.Append(TargetRegister);
                        sb.Append("==");
                        if (IsRef)
                        {
                            sb.Append(SourceRegister);
                        }
                        else
                        {
                            sb.Append(Source);
                        }
                        break;
                }
                return sb.ToString();
            }
        }

        private string FillIn(string filler, string toFill)
        {
            StringBuilder sb = new StringBuilder(toFill);
            foreach (char c in filler)
            {
                int idx = toFill.IndexOf('_');
                sb[idx] = c;
                toFill = sb.ToString();
            }
            return sb.ToString();
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables)
        {
            Dictionary<char, int> registers = new Dictionary<char, int>() { { 'w', 0 }, { 'x', 0 }, { 'y', 0 }, { 'z', 0 } };

            string fullRegisterInput;
            Util.GetVariable(nameof(fullRegisterInput), "__9929927_____", variables, out fullRegisterInput);

            Queue<int> registerInput = new Queue<int>();

            List<Instruction> instructions = inputs.Select(Instruction.Parse).ToList();
            if (fullRegisterInput.Contains('_'))
            {
                int unknownCount = fullRegisterInput.Count(c => c == '_');
                for (int i = 9999999; i >= 1111111; --i)
                {
                    string cur = i.ToString();
                    if (cur.Contains('0'))
                    {
                        continue;
                    }

                    registerInput.Clear();
                    foreach (char fri in FillIn(cur, fullRegisterInput))
                    {
                        registerInput.Enqueue(int.Parse($"{fri}"));
                    }
                    registers = new Dictionary<char, int>() { { 'w', 0 }, { 'x', 0 }, { 'y', 0 }, { 'z', 0 } };
                    foreach (Instruction instruction in instructions)
                    {
                        if (!instruction.Execute(ref registers, ref registerInput, DebugWriteLine))
                        {
                            registers['z'] = 1;
                            break;
                        }
                    }
                    if (registers['z'] == 0)
                    {
                        return FillIn(cur, fullRegisterInput); 
                    }
                }
            }
            else
            {
                foreach (char fri in fullRegisterInput)
                {
                    registerInput.Enqueue(int.Parse($"{fri}"));
                }
                foreach (Instruction instruction in instructions)
                {
                    instruction.Execute(ref registers, ref registerInput, DebugWriteLine);
                }
            }

            char returnRegister;
            Util.GetVariable(nameof(returnRegister), 'z', variables, out returnRegister);
            return registers[returnRegister].ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);
    }
}