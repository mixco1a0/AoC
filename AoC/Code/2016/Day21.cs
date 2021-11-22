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
                case Part.One:
                    return "v1";
                case Part.Two:
                    return "v1";
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
                Variables = new Dictionary<string, string>() { { "unscrambledPassword", "decab" } },
                Output = "abcde",
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

        private string Rotate(bool right, int rot, string curPassword)
        {
            StringBuilder sb = new StringBuilder();
            if (right)
            {
                string sub1 = curPassword.Substring(curPassword.Length - rot, rot);
                string sub2 = curPassword.Substring(0, curPassword.Length - rot);
                sb.Append(sub1);
                sb.Append(sub2);
            }
            else
            {
                string sub1 = curPassword.Substring(0, rot);
                string sub2 = curPassword.Substring(rot);
                sb.Append(sub2);
                sb.Append(sub1);
            }
            return sb.ToString();
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, string defaultPassword, bool reverse)
        {
            string unscrambledPassword;
            Util.GetVariable(nameof(unscrambledPassword), defaultPassword, variables, out unscrambledPassword);
            StringBuilder sb = new StringBuilder(unscrambledPassword);

            IEnumerable<Instruction> instructions = inputs.Select(Instruction.Parse);
            if (reverse)
            {
                instructions = instructions.Reverse();
            }
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
                            if (reverse)
                            {
                                sb.Clear();
                                sb.Append(Rotate(true, instruction.PosOne % curPassword.Length, curPassword));
                            }
                            else
                            {
                                sb.Clear();
                                sb.Append(Rotate(false, instruction.PosOne % curPassword.Length, curPassword));
                            }
                        }
                        break;
                    case Operation.RotateRight:
                        {
                            if (reverse)
                            {
                                sb.Clear();
                                sb.Append(Rotate(false, instruction.PosOne % curPassword.Length, curPassword));
                            }
                            else
                            {
                                sb.Clear();
                                sb.Append(Rotate(true, instruction.PosOne % curPassword.Length, curPassword));
                            }
                        }
                        break;
                    case Operation.RotateRightLetter:
                        {
                            if (reverse)
                            {
                                string prePassword = curPassword;
                                for (int i = 0; i < curPassword.Length; ++i)
                                {
                                    sb.Clear();
                                    sb.Append(Rotate(false, 1, curPassword));
                                    curPassword = sb.ToString();

                                    int p1 = curPassword.IndexOf(instruction.LetterOne);
                                    int rot = (p1 + 1 + (p1 >= 4 ? 1 : 0)) % curPassword.Length;
                                    string shift = Rotate(true, rot, curPassword);

                                    if (shift == prePassword)
                                    {
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                int p1 = curPassword.IndexOf(instruction.LetterOne);
                                int rot = (p1 + 1 + (p1 >= 4 ? 1 : 0)) % curPassword.Length;
                                sb.Clear();
                                sb.Append(Rotate(true, rot, curPassword));
                            }
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
                            int p1 = instruction.PosOne;
                            int p2 = instruction.PosTwo;
                            if (reverse)
                            {
                                p1 = instruction.PosTwo;
                                p2 = instruction.PosOne;
                            }
                            char move = sb[p1];
                            sb.Remove(p1, 1);
                            sb.Insert(p2, move);
                        }
                        break;
                }
            }

            return sb.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, "abcdefgh", false);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, "fbgdceah", true);
    }
}