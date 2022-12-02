using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2022
{
    class Day02 : Core.Day
    {
        public Day02() { }

        public override string GetSolutionVersion(Core.Part part)
        {
            switch (part)
            {
                // case Core.Part.One:
                //     return "v1";
                // case Core.Part.Two:
                //     return "v1";
                default:
                    return base.GetSolutionVersion(part);
            }
        }

        public override bool SkipTestData => false;

        protected override List<Core.TestDatum> GetTestData()
        {
            List<Core.TestDatum> testData = new List<Core.TestDatum>();
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.One,
                Output = "15",
                RawInput =
@"A Y
B X
C Z"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "",
                RawInput =
@""
            });
            return testData;
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables)
        {
            int score = 0;

            Func<char, int> RPS = (char input) =>
            {
                switch (input)
                {
                    case 'A':
                    case 'X':
                        return 1;
                    case 'B':
                    case 'Y':
                        return 2;
                    case 'C':
                    case 'Z':
                        return 3;
                }
                return 0;
            };

            foreach (string input in inputs)
            {
                string[] split = input.Split(' ');
                if (RPS(split[0].First()) == RPS(split[1].First()))
                {
                    score += 3;
                }
                else if (split[0] == "A")
                {
                    if (split[1] == "Y")
                    {
                        score += 6;
                    }
                }
                else if (split[0] == "B")
                {
                    if (split[1] == "Z")
                    {
                        score += 6;
                    }
                }
                else if (split[0] == "C")
                {
                    if (split[1] == "X")
                    {
                        score += 6;
                    }
                }
                score += RPS(split[1].First());
            }
            return score.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);
    }
}