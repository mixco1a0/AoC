using System.Collections.Generic;
using System.Linq;

using AoC.Core;

namespace AoC._2021
{
    class Day10 : Day
    {
        public Day10() { }

        public override string GetSolutionVersion(Part part)
        {
            switch (part)
            {
                case Part.One:
                    return "v2";
                case Part.Two:
                    return "v2";
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
                Output = "288957",
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
            return testData;
        }

        Dictionary<char, char> OpenToClose = new Dictionary<char, char> { { '(', ')' }, { '[', ']' }, { '{', '}' }, { '<', '>' } };
        Dictionary<char, long> Points = new Dictionary<char, long>()
        {
            { ')', 3 },
            { ']', 57 },
            { '}', 1197 },
            { '>', 25137 },
            { '(', 1 },
            { '[', 2 },
            { '{', 3 },
            { '<', 4 },
        };

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool scoreCorrupt)
        {
            string allOpen = string.Join(string.Empty, OpenToClose.Keys);
            long score = 0;
            List<long> scores = new List<long>();
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
                        if (opened.Count == 0 || i != OpenToClose[opened.Peek()])
                        {
                            score += Points[i];
                            opened.Clear();
                            break;
                        }
                        else
                        {
                            opened.Pop();
                        }
                    }
                }

                // not corrupt
                if (!scoreCorrupt && opened.Count > 0)
                {
                    long completionScore = 0;
                    string completion = string.Join(string.Empty, opened);
                    foreach (char c in completion)
                    {
                        completionScore *= 5;
                        completionScore += Points[c];
                    }
                    scores.Add(completionScore);
                }
            }
            
            if (scoreCorrupt)
            {
                return score.ToString();
            }
            scores.Sort();
            int idx = (scores.Count() - 1) / 2;
            return scores[idx].ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);
    }
}