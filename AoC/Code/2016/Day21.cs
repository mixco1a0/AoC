using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2016
{
    class Day21 : Day
    {
        public Day21() { }

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
                Variables = new Dictionary<string, string>() { { "unscrambledPassword", "abcde" } },
                Output = "decab",
                RawInput =
@"swap position 4 with position 0
swap letter d with letter b
reverse positions 0 through 4
rotate left 1 step
move position 1 to position 4
move position 3 to position 0
rotate based on position of letter b
rotate based on position of letter d"
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

        private enum Operation
        {
            Invalid,
            SwapPosition,
            SwapLetter,
            RotateLeft,
            RotateRight,
            RotateRightLetter,
            Reverse,
            Move,
        }

        private record Instruction(Operation Op, char LetterOne, char LetterTwo, int PosOne, int PosTwo)
        {
            static public Instruction Parse(string input)
            {
                Operation op = Operation.Invalid;
                char l1 = ' ', l2 = ' ';
                int p1 = 0, p2 = 0;

                string[] split = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (split[0] == "swap")
                {
                    if (split[1] == "letter")
                    {
                        op = Operation.SwapLetter;
                        l1 = split[2].First();
                        l2 = split.Last().First();
                    }
                    else
                    {
                        op = Operation.SwapPosition;
                    }
                }
                else if (split[0] == "rotate")
                {
                    if (split[1] == "left")
                    {
                        op = Operation.RotateLeft;
                    }
                    else if (split[1] == "right")
                    {
                        op = Operation.RotateRight;
                    }
                    else
                    {
                        op = Operation.RotateRightLetter;
                        l1 = split.Last().First();
                    }
                }
                else if (split[0] == "reverse")
                {
                    op = Operation.Reverse;
                }
                else if (split[0] == "move")
                {
                    op = Operation.Move;
                }

                List<int> positions = split.Where(s => char.IsDigit(s[0])).Select(int.Parse).ToList();
                if (positions.Count > 0)
                {
                    p1 = positions[0];
                }
                if (positions.Count > 1)
                {
                    p2 = positions[1];
                }

                return new Instruction(op, l1, l2, p1, p2);
            }
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables)
        {
            string unscrambledPassword;
            Util.GetVariable(nameof(unscrambledPassword), "abcdefgh", variables, out unscrambledPassword);
            StringBuilder sb = new StringBuilder(unscrambledPassword);

            IEnumerable<Instruction> instructions = inputs.Select(Instruction.Parse);
            foreach (Instruction instruction in instructions)
            {
                string curPassword = sb.ToString();
                switch (instruction.Op)
                {
                    case Operation.SwapPosition:
                        {
                            char t = sb[instruction.PosOne];
                            sb[instruction.PosOne] = sb[instruction.PosTwo];
                            sb[instruction.PosTwo] = t;
                        }
                        break;
                    case Operation.SwapLetter:
                        {
                            int p1 = curPassword.IndexOf(instruction.LetterOne);
                            int p2 = curPassword.IndexOf(instruction.LetterTwo);
                            char t = sb[p1];
                            sb[p1] = sb[p2];
                            sb[p2] = t;
                        }
                        break;
                    case Operation.RotateLeft:
                        {
                            int rot = instruction.PosOne % curPassword.Length;
                            string sub1 = curPassword.Substring(0, rot);
                            string sub2 = curPassword.Substring(rot);
                            sb.Clear();
                            sb.Append(sub2);
                            sb.Append(sub1);
                        }
                        break;
                    case Operation.RotateRight:
                        {
                            int rot = instruction.PosOne % curPassword.Length;
                            string sub1 = curPassword.Substring(curPassword.Length - rot, rot);
                            string sub2 = curPassword.Substring(0, curPassword.Length - rot);
                            sb.Clear();
                            sb.Append(sub1);
                            sb.Append(sub2);
                        }
                        break;
                    case Operation.RotateRightLetter:
                        {
                            int p1 = curPassword.IndexOf(instruction.LetterOne);
                            int rot = (p1 + 1 + (p1 >= 4 ? 1 : 0)) % curPassword.Length;
                            string sub1 = curPassword.Substring(curPassword.Length - rot, rot);
                            string sub2 = curPassword.Substring(0, curPassword.Length - rot);
                            sb.Clear();
                            sb.Append(sub1);
                            sb.Append(sub2);
                        }
                        break;
                    case Operation.Reverse:
                        {
                            string sub1 = curPassword.Substring(instruction.PosOne, instruction.PosTwo - instruction.PosOne + 1);
                            sb.Clear();
                            if (instruction.PosOne > 0)
                            {
                                sb.Append(curPassword.Substring(0, instruction.PosOne));
                            }
                            sb.Append(string.Join(string.Empty, sub1.Reverse()));
                            if (instruction.PosTwo < curPassword.Length - 1)
                            {
                                sb.Append(curPassword.Substring(instruction.PosTwo + 1));
                            }
                        }
                        break;
                    case Operation.Move:
                        {
                            char move = sb[instruction.PosOne];
                            sb.Remove(instruction.PosOne, 1);
                            sb.Insert(instruction.PosTwo, move);
                        }
                        break;
                }
            }

            return sb.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);
    }
}