using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2021
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
                Output = "739785",
                RawInput =
@"Player 1 starting position: 4
Player 2 starting position: 8"
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
            // zero based
            int p1 = int.Parse($"{inputs.First().Last()}") - 1;
            int p2 = int.Parse($"{inputs.Last().Last()}") - 1;
            int p1Score = 0;
            int p2Score = 0;
            int diceVal = 0;
            bool p1Turn = true;
            int turnCount = 0;
            while (p1Score < 1000 && p2Score < 1000)
            {
                ++turnCount;
                int p = p1Turn ? p1 : p2;
                p += ++diceVal + ++diceVal + ++diceVal;
                if (p1Turn)
                {
                    p1 = p % 10;
                    p1Score += p1 + 1;
                }
                else
                {
                    p2 = p % 10;
                    p2Score += p2 + 1;
                }
                p1Turn = !p1Turn;
                diceVal %= 100;
            }
            return (turnCount * 3 * (p1Score > p2Score ? p2Score : p1Score)).ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);
    }
}