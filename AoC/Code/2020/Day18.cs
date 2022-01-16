using System.Collections.Generic;
using System.Linq;

using AoC.Core;

namespace AoC._2020
{
    class Day18 : Day
    {
        public Day18() { }
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
                Output = "71",
                RawInput =
@"1 + 2 * 3 + 4 * 5 + 6"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.One,
                Output = "51",
                RawInput =
@"1 + (2 * 3) + (4 * (5 + 6))"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.Two,
                Output = "231",
                RawInput =
@"1 + 2 * 3 + 4 * 5 + 6"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.Two,
                Output = "51",
                RawInput =
@"1 + (2 * 3) + (4 * (5 + 6))"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.Two,
                Output = "669060",
                RawInput =
@"5 * 9 * (7 * 3 * 3 + 9 * 3 + (8 + 6 * 4))"
            });
            return testData;
        }

        enum Op
        {
            None,
            Add,
            Mult,
            Open,
            Close
        }

        private long Perform(string equation)
        {
            long leftHandValue = 0;
            if (equation.Contains(')'))
            {
                // find the last index of '(' and replace the bit
                int end = equation.IndexOf(')');
                int start = equation.Substring(0, end).LastIndexOf('(');
                string subequation = equation.Substring(start + 1, end - start - 1);
                string replace = Perform(subequation).ToString();
                equation = equation.Replace(equation.Substring(start, end - start + 1), replace);
                return Perform(equation);
            }
            else
            {
                Op curOp = Op.None;
                Op nextOp = Op.Add;
                long leftHand = 0;
                string curLeftHand = "";
                for (int i = 0; i < equation.Length; ++i)
                {
                    switch (equation[i])
                    {
                        case '+':
                            curOp = nextOp;
                            nextOp = Op.Add;
                            leftHand = long.Parse(curLeftHand);
                            curLeftHand = "";
                            break;
                        case '*':
                            curOp = nextOp;
                            nextOp = Op.Mult;
                            leftHand = long.Parse(curLeftHand);
                            curLeftHand = "";
                            break;
                        default:
                            curOp = Op.None;
                            curLeftHand += equation[i];
                            break;
                    }

                    if (curOp == Op.Add)
                    {
                        leftHandValue += leftHand;
                    }
                    else if (curOp == Op.Mult)
                    {
                        leftHandValue *= leftHand;
                    }
                    curOp = Op.None;
                }

                if (nextOp == Op.Add)
                {
                    leftHandValue += long.Parse(curLeftHand);
                }
                else if (nextOp == Op.Mult)
                {
                    leftHandValue *= long.Parse(curLeftHand);
                }
            }
            return leftHandValue;
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            long sum = 0;
            foreach (string input in inputs)
            {
                sum += Perform(input.Replace(" ", ""));
            }
            return sum.ToString();
        }

        private long PerformPrecedence(string equation)
        {
            if (equation.Contains(')'))
            {
                // find the last index of '(' and replace the bit
                int end = equation.IndexOf(')');
                int start = equation.Substring(0, end).LastIndexOf('(');
                string subequation = equation.Substring(start + 1, end - start - 1);
                string replace = PerformPrecedence(subequation).ToString();
                equation = equation.Replace(equation.Substring(start, end - start + 1), replace);
                return PerformPrecedence(equation);
            }
            else if (equation.Contains('+'))
            {
                int plusSign = equation.IndexOfAny("+".ToCharArray());
                int end = equation.IndexOfAny("+*".ToCharArray(), plusSign + 1);
                if (end == -1)
                {
                    end = equation.Length;
                }
                int start = equation.LastIndexOfAny("+*".ToCharArray(), plusSign - 1) + 1;
                if (start < 0)
                {
                    start = 0;
                }
                long sum = long.Parse(equation.Substring(start, plusSign - start)) + long.Parse(equation.Substring(plusSign + 1, end - (plusSign + 1)));
                equation = equation.Remove(start, end - start).Insert(start, sum.ToString());
                return PerformPrecedence(equation);
            }

            long[] vals = equation.Split('*').Select(long.Parse).ToArray();
            long mult = vals[0];
            for (int i = 1; i < vals.Length; ++i)
            {
                mult *= vals[i];
            }
            return mult;
        }

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            long sum = 0;
            foreach (string input in inputs)
            {
                sum += PerformPrecedence(input.Replace(" ", ""));
            }
            return sum.ToString();
        }
    }
}