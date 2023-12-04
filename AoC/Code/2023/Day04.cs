using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2023
{
    class Day04 : Core.Day
    {
        public Day04() { }

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
                Output = "13",
                RawInput =
@"Card 1: 41 48 83 86 17 | 83 86  6 31 17  9 48 53
Card 2: 13 32 20 16 61 | 61 30 68 82 17 32 24 19
Card 3:  1 21 53 59 44 | 69 82 63 72 16 21 14  1
Card 4: 41 92 73 84 69 | 59 84 76 51 58  5 54 83
Card 5: 87 83 26 28 32 | 88 30 70 12 93 22 82 36
Card 6: 31 18 13 56 72 | 74 77 10 23 35 67 36 11"
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

        public class ScratchCard
        {
            public int Id { get; set; }
            public List<int> Win { get; set; }
            public List<int> Have { get; set; }
            public ScratchCard()
            {
                Id = default;
                Win = new List<int>();
                Have = new List<int>();
            }
            public int GetScore()
            {
                int count = Have.Intersect(Win).Count();
                if (count == 0)
                {
                    return 0;
                }
                return (int)Math.Pow(2, count - 1);
            }

            public static ScratchCard Parse(string input)
            {
                ScratchCard sc = new ScratchCard();
                string[] split = input.Split(":|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                sc.Id = int.Parse(split[0].Split(' ', StringSplitOptions.RemoveEmptyEntries).Last());
                sc.Win = split[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
                sc.Have = split[2].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).Order().ToList();
                return sc;
            }
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables)
        {
            List<ScratchCard> cards = inputs.Select(ScratchCard.Parse).ToList();
            return cards.Select(c => c.GetScore()).Sum().ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);
    }
}