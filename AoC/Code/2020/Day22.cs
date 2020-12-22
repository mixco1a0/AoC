using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2020
{
    class Day22 : Day
    {
        public Day22() { }
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
                Output = "306",
                RawInput =
@"Player 1:
9
2
6
3
1

Player 2:
5
8
4
7
10"
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

        class Player
        {
            public string Name { get; set; }
            public List<int> Cards { get; set; }

            public Player() 
            {
                Cards = new List<int>();
            }
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            List<Player> players = new List<Player>();
            foreach (string input in inputs)
            {
                if (input.Contains(":"))
                {
                    players.Add(new Player { Name = input });
                }
                else if (!string.IsNullOrWhiteSpace(input))
                {
                    players.Last().Cards.Add(int.Parse(input));
                }
            }

            Player p1 = players.First();
            Player p2 = players.Last();

            while (p1.Cards.Count > 0 && p2.Cards.Count > 0)
            {
                int p1Card = p1.Cards.First();
                int p2Card = p2.Cards.First();

                p1.Cards.RemoveAt(0);
                p2.Cards.RemoveAt(0);
                if (p1Card > p2Card)
                {
                    p1.Cards.Add(p1Card);
                    p1.Cards.Add(p2Card);
                }
                else
                {
                    p2.Cards.Add(p2Card);
                    p2.Cards.Add(p1Card);
                }
            }

            int sum = 0;
            var winner = players.Where(p => p.Cards.Count > 0).Select(p => p.Cards).First();
            for (int i = 0; i < winner.Count; ++i)
            {
                sum += (winner.Count-i) * winner[i];
            }

            return sum.ToString();
        }

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            return "";
        }
    }
}