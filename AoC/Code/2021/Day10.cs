using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2021
{
    class Day10 : Day
    {
        public Day10() { }

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
                Output = "26397",
                RawInput =
@"[({(<(())[]>[[{[]{<()<>>
[(()[<>])]({[<{<<[]>>(
{([(<{}[<>[]}>{[]{[(<()>
(((({<>}<{<{<>}{[]{[]{}
[[<[([]))<([[{}[[()]]]
[{[{({}]{}}([{[{{{}}([]
{<[[]]>}<{[{[{[]{()[[[]
[<(<(<(<{}))><([]([]()
<{([([[(<>()){}]>(<<{{
<{([{{}}[<[[[<>{}]]]>[]]"
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

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables)
        {
            string allOpen = "([{<";
            Dictionary<char,char> openToClose = new Dictionary<char, char>{{'(',')'}, {'[',']'}, {'{','}'}, {'<','>'}};
            long score = 0;
            foreach (string input in inputs)
            {
                Stack<char> opened = new Stack<char>();
                foreach (char i in input)
                {
                    if (allOpen.Contains(i))
                    {
                        opened.Push(i);
                    }
                    else
                    {
                        if (opened.Count == 0 || i != openToClose[opened.Peek()])
                        {
                            switch (i)
                            {
                                case ')':
                                    score += 3;
                                    break;
                                case ']':
                                    score += 57;
                                    break;
                                case '}':
                                    score += 1197;
                                    break;
                                case '>':
                                    score += 25137;
                                    break;
                            }
                            break;
                        }
                        else
                        {
                            opened.Pop();
                        }
                    }
                }
            }
            return score.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);
    }
}