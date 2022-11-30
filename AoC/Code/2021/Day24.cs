using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AoC._2021
{
    class Day24 : Core.Day
    {
        public Day24() { }

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
                Variables = new Dictionary<string, string>() { { "fullRegisterInput", "1" }, { "returnRegister", "x" } },
                Output = "-1",
                RawInput =
@"inp x
mul x -1"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.One,
                Variables = new Dictionary<string, string>() { { "fullRegisterInput", "13" } },
                Output = "1",
                RawInput =
@"inp z
inp x
mul z 3
eql z x"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.One,
                Variables = new Dictionary<string, string>() { { "fullRegisterInput", "12" } },
                Output = "0",
                RawInput =
@"inp z
inp x
mul z 3
eql z x"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.One,
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
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.One,
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

        private class ALURule
        {
            public int HighIndex { get; set; }
            public int LowIndex { get; set; }
            public int Diff { get; set; }

            public ALURule(int high, int low, int diff)
            {
                HighIndex = high;
                LowIndex = low;
                Diff = diff;
            }

            public void GetLow(out int low, out int high)
            {
                low = -1;
                high = -1;
                for (int i = 1; i <= 9; ++i)
                {
                    int test = i + Diff;
                    if (test >= 1 && test <= 9)
                    {
                        low = i + Diff;
                        high = i;
                        break;
                    }
                }
            }

            public void GetHigh(out int low, out int high)
            {
                low = -1;
                high = -1;
                for (int i = 9; i >= 1; --i)
                {
                    int test = i + Diff;
                    if (test >= 1 && test <= 9)
                    {
                        low = i + Diff;
                        high = i;
                        break;
                    }
                }
            }
        }

        private static readonly List<ALURule> ALURules = new List<ALURule>()
        {
            new ALURule(0, 13, 0),
            new ALURule(1, 12, 5),
            new ALURule(2, 11, -8),
            new ALURule(3, 8, -2),
            new ALURule(4, 5, 7),
            new ALURule(6, 7, -7),
            new ALURule(9, 10, -3),
        };

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool findHighest)
        {
            Dictionary<char, int> registers = new Dictionary<char, int>() { { 'w', 0 }, { 'x', 0 }, { 'y', 0 }, { 'z', 0 } };

            string fullRegisterInput;
            GetVariable(nameof(fullRegisterInput), "______________", variables, out fullRegisterInput);

            Queue<int> registerInput = new Queue<int>();

            List<Instruction> instructions = inputs.Select(Instruction.Parse).ToList();
            if (fullRegisterInput.Contains('_'))
            {
                StringBuilder sb = new StringBuilder(fullRegisterInput);
                foreach (ALURule rule in ALURules)
                {
                    int low;
                    int high;
                    if (findHighest)
                    {
                        rule.GetHigh(out low, out high);
                    }
                    else
                    {
                        rule.GetLow(out low, out high);
                    }
                    sb[rule.HighIndex] = $"{high}"[0];
                    sb[rule.LowIndex] = $"{low}"[0];
                }
                foreach (char ri in sb.ToString())
                {
                    registerInput.Enqueue(int.Parse($"{ri}"));
                }
                foreach (Instruction instruction in instructions)
                {
                    instruction.Execute(ref registers, ref registerInput, DebugWriteLine);
                }
                if (registers['z'] == 0)
                {
                    return sb.ToString();
                }
                return string.Empty;
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
            GetVariable(nameof(returnRegister), 'z', variables, out returnRegister);
            return registers[returnRegister].ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);
    }
}