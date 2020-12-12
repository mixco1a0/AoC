using System.Collections.Generic;
using System.Linq;

namespace AoC._2015
{
    class Day07 : Day
    {
        public Day07() { }
        protected override List<TestDatum> GetTestData()
        {
            List<TestDatum> testData = new List<TestDatum>();
            testData.Add(new TestDatum
            {
                TestPart = TestPart.One,
                Variables = new Dictionary<string, string> { { "wire", "d" } },
                Output = "72",
                RawInput =
@"123 -> x
456 -> y
x AND y -> d
x OR y -> e
x LSHIFT 2 -> f
y RSHIFT 2 -> g
NOT x -> h
NOT y -> i"
            });
            testData.Add(new TestDatum
            {
                TestPart = TestPart.One,
                Variables = new Dictionary<string, string> { { "wire", "e" } },
                Output = "507",
                RawInput =
@"123 -> x
456 -> y
x AND y -> d
x OR y -> e
x LSHIFT 2 -> f
y RSHIFT 2 -> g
NOT x -> h
NOT y -> i"
            });
            testData.Add(new TestDatum
            {
                TestPart = TestPart.One,
                Variables = new Dictionary<string, string> { { "wire", "h" } },
                Output = "65412",
                RawInput =
@"123 -> x
456 -> y
x AND y -> d
x OR y -> e
x LSHIFT 2 -> f
y RSHIFT 2 -> g
NOT x -> h
NOT y -> i"
            });
            testData.Add(new TestDatum
            {
                TestPart = TestPart.Two,
                Output = "",
                RawInput =
@""
            });
            return testData;
        }

        public class Instruction
        {
            public enum InstructionType
            {
                And,
                AndRef,
                Or,
                OrRef,
                LShift,
                RShift,
                Not,
                Set,
                SetRef
            }

            public InstructionType Type { get; private set; }
            public string Source { get; private set; }
            public string Source2 { get; private set; }
            public string Destination { get; private set; }
            public bool Complete { get; private set; }

            public Instruction(string instruction)
            {
                Source2 = "";
                List<string> split = instruction.Split(" ").ToList();
                if (instruction.Contains("AND"))
                {
                    int test;
                    if (int.TryParse(split[0], out test) || int.TryParse(split[2], out test))
                    {
                        Type = InstructionType.And;
                    }
                    else
                    {
                        Type = InstructionType.AndRef;
                    }
                }
                else if (instruction.Contains("OR"))
                {
                    int test;
                    if (int.TryParse(split[0], out test) || int.TryParse(split[2], out test))
                    {
                        Type = InstructionType.Or;
                    }
                    else
                    {
                        Type = InstructionType.OrRef;
                    }
                }
                else if (instruction.Contains("LSHIFT"))
                    Type = InstructionType.LShift;
                else if (instruction.Contains("RSHIFT"))
                    Type = InstructionType.RShift;
                else if (instruction.Contains("NOT"))
                    Type = InstructionType.Not;
                else
                {
                    int test;
                    if (int.TryParse(split[0], out test))
                    {
                        Type = InstructionType.Set;
                    }
                    else
                    {
                        Type = InstructionType.SetRef;
                    }
                }

                switch (Type)
                {
                    case InstructionType.AndRef:
                    case InstructionType.OrRef:
                    case InstructionType.LShift:
                    case InstructionType.RShift:
                        Source = split[0];
                        Source2 = split[2];
                        Destination = split[4];
                        break;
                    case InstructionType.And:
                    case InstructionType.Or:
                        int test;
                        if (int.TryParse(split[0], out test))
                        {
                            Source = split[2];
                            Source2 = split[0];
                            Destination = split[4];
                        }
                        else
                        {
                            Source = split[0];
                            Source2 = split[2];
                            Destination = split[4];
                        }
                        break;
                    case InstructionType.Not:
                        Source = split[1];
                        Destination = split[3];
                        break;
                    case InstructionType.Set:
                    case InstructionType.SetRef:
                        Source = split[0];
                        Destination = split[2];
                        break;
                }
            }

            public bool CanExecute(Dictionary<string, int> signals)
            {
                switch (Type)
                {
                    case InstructionType.AndRef:
                    case InstructionType.OrRef:
                        if (signals.ContainsKey(Source) && signals.ContainsKey(Source2))
                        {
                            return !Complete;
                        }
                        break;
                    case InstructionType.And:
                    case InstructionType.Or:
                    case InstructionType.LShift:
                    case InstructionType.RShift:
                    case InstructionType.Not:
                    case InstructionType.SetRef:
                        if (signals.ContainsKey(Source))
                        {
                            return !Complete;
                        }
                        break;
                    case InstructionType.Set:
                        return !Complete;
                }
                return false;
            }

            public void Execute(ref Dictionary<string, int> signals)
            {
                switch (Type)
                {
                    case InstructionType.And:
                        signals[Destination] = signals[Source] & int.Parse(Source2);
                        break;
                    case InstructionType.AndRef:
                        signals[Destination] = signals[Source] & signals[Source2];
                        break;
                    case InstructionType.Or:
                        signals[Destination] = signals[Source] | int.Parse(Source2);
                        break;
                    case InstructionType.OrRef:
                        signals[Destination] = signals[Source] | signals[Source2];
                        break;
                    case InstructionType.LShift:
                        signals[Destination] = signals[Source] << int.Parse(Source2);
                        break;
                    case InstructionType.RShift:
                        signals[Destination] = signals[Source] >> int.Parse(Source2);
                        break;
                    case InstructionType.Not:
                        signals[Destination] = ~signals[Source];
                        break;
                    case InstructionType.Set:
                        signals[Destination] = int.Parse(Source);
                        Complete = true;
                        break;
                    case InstructionType.SetRef:
                        signals[Destination] = signals[Source];
                        Complete = true;
                        break;
                }
                signals[Destination] &= ushort.MaxValue;
                Complete = true;
            }

            public override string ToString()
            {
                switch (Type)
                {
                    case InstructionType.And:
                    case InstructionType.AndRef:
                    case InstructionType.Or:
                    case InstructionType.OrRef:
                    case InstructionType.LShift:
                    case InstructionType.RShift:
                        return $"{Source} {Type} {Source2} -> {Destination}";
                    case InstructionType.Not:
                        return $"{Type} {Source} -> {Destination}";
                    case InstructionType.Set:
                    case InstructionType.SetRef:
                        return $"{Source} -> {Destination}";
                }
                return base.ToString();
            }
        }

        private Dictionary<string, int> RunToCompletion(List<Instruction> instructions)
        {
            Dictionary<string, int> signals = new Dictionary<string, int>();
            while (instructions.Where(ins => !ins.Complete).Count() > 0)
            {
                List<Instruction> curInstructions = instructions.Where(ins => ins.CanExecute(signals)).ToList();
                foreach (Instruction instruction in curInstructions)
                {
                    Debug(instruction.ToString());
                    instruction.Execute(ref signals);
                }
            }

            return signals;
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            string wire = "a";
            if (variables != null && variables.ContainsKey(nameof(wire)))
            {
                wire = variables[nameof(wire)];
            }

            List<Instruction> instructions = inputs.Select(input => new Instruction(input)).ToList();
            Dictionary<string, int> signals = RunToCompletion(instructions);
            return signals[wire].ToString();
        }

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            return "";
        }
    }
}