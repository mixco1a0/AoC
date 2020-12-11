using System;
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


        private void ExecuteInstruction(string instruction, ref Dictionary<string, int> signals)
        {
            List<string> split;
            // determine the instruction
            if (instruction.Contains("AND"))
            {
                split = instruction.Replace("AND", "").Replace("->", "").Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
                if (signals.ContainsKey(split[0]) && signals.ContainsKey(split[1]))
                {
                    Debug(instruction);
                    signals[split[2]] = signals[split[0]] & signals[split[1]];
                    signals[split[2]] &= ushort.MaxValue;
                }
            }
            else if (instruction.Contains("OR"))
            {
                split = instruction.Replace("OR", "").Replace("->", "").Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
                if (signals.ContainsKey(split[0]) && signals.ContainsKey(split[1]))
                {
                    Debug(instruction);
                    signals[split[2]] = signals[split[0]] | signals[split[1]];
                    signals[split[2]] &= ushort.MaxValue;
                }
            }
            else if (instruction.Contains("LSHIFT"))
            {
                split = instruction.Replace("LSHIFT", "").Replace("->", "").Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
                if (signals.ContainsKey(split[0]) && signals.ContainsKey(split[1]))
                {
                    Debug(instruction);
                    signals[split[2]] = signals[split[0]] << int.Parse(split[1]);
                    signals[split[2]] &= ushort.MaxValue;
                }
            }
            else if (instruction.Contains("RSHIFT"))
            {
                split = instruction.Replace("RSHIFT", "").Replace("->", "").Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
                if (signals.ContainsKey(split[0]) && signals.ContainsKey(split[1]))
                {

                    Debug(instruction);
                    signals[split[2]] = signals[split[0]] >> int.Parse(split[1]);
                }
            }
            else if (instruction.Contains("NOT"))
            {
                split = instruction.Replace("NOT", "").Replace("->", "").Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
                if (signals.ContainsKey(split[0]))
                {
                    Debug(instruction);
                    signals[split[1]] = ~signals[split[0]];
                    signals[split[1]] &= ushort.MaxValue;
                }
            }
            else
            {
                split = instruction.Replace("->", "").Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
                int value;
                if (int.TryParse(split[0], out value))
                {
                    Debug(instruction);
                    signals[split[1]] = value;
                }
                else if (signals.ContainsKey(split[0]))
                {
                    Debug(instruction);
                    signals[split[1]] = signals[split[0]];
                }
            }
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            string wire = "a";
            if (variables != null && variables.ContainsKey(nameof(wire)))
            {
                wire = variables[nameof(wire)];
            }

            Dictionary<string, int> signals = new Dictionary<string, int>();
            foreach (string input in inputs)
            {
                ExecuteInstruction(input, ref signals);
            }

            return signals[wire].ToString();
        }

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            return "";
        }
    }
}