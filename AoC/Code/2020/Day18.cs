using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2020
{
    class Day18 : Day
    {
        public Day18() { }
        public override string GetSolutionVersion(TestPart testPart)
        {
            switch (testPart)
            {
                // case TestPart.One:
                //     return "v1";
                // case TestPart.Two:
                //     return "v1";
                default:
                    return base.GetSolutionVersion(testPart);
            }
        }
        protected override List<TestDatum> GetTestData()
        {
            List<TestDatum> testData = new List<TestDatum>();
            testData.Add(new TestDatum
            {
                TestPart = TestPart.One,
                Output = "71",
                RawInput =
@"1 + 2 * 3 + 4 * 5 + 6"
            });
            testData.Add(new TestDatum
            {
                TestPart = TestPart.One,
                Output = "51",
                RawInput =
@"1 + (2 * 3) + (4 * (5 + 6))"
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
                string subequation = equation.Substring(start+1, end-start-1);
                string replace = Perform(subequation).ToString();
                equation = equation.Replace(equation.Substring(start, end-start+1), replace);
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
            /*

                            string basic = input.Replace(" ", "");
                            string[] test = input.Split('(');
                            string curNumber = "";
                            long leftHandSide = 0;
                            long temp = 0;
                            for (int i = 0; i < basic.Length; ++i)
                            {
                                switch (basic[i])
                                {
                                    case '+':
                                        temp = int.Parse(curNumber);
                                        curNumber = "";
                                        break;
                                    case '*':
                                        break;
                                    case '(':
                                        break;
                                    case ')':
                                        break;
                                    default:
                                        curNumber += basic[i];
                                        break;
                                }
                            }
            */
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

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            return "";
        }
    }
}