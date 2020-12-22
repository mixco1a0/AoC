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
                Output = "291",
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
            return testData;
        }

        class Player
        {
            public string Name { get; set; }
            public List<int> Cards { get; set; }
            public string State
            {
                get
                {
                    return $"{Name}{string.Join(",", Cards)}";
                }
            }
            public bool Winner { get; set; }

            public Player()
            {
                Cards = new List<int>();
                Winner = false;
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
                sum += (winner.Count - i) * winner[i];
            }

            return sum.ToString();
        }

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            List<Player> players = new List<Player>();
            foreach (string input in inputs)
            {
                if (input.Contains(":"))
                {
                    players.Add(new Player { Name = input[0..^1] });
                }
                else if (!string.IsNullOrWhiteSpace(input))
                {
                    players.Last().Cards.Add(int.Parse(input));
                }
            }

            Player p1 = players.First();
            Player p2 = players.Last();

            HashSet<string> previousRounds = new HashSet<string>();

            while (p1.Cards.Count > 0 && p2.Cards.Count > 0)
            {
                string roundId = $"{p1.State}|{p2.State}";
                if (previousRounds.Contains(roundId))
                {
                    p1.Winner = true;
                    break;
                }
                previousRounds.Add(roundId);

                int p1Card = p1.Cards.First();
                int p2Card = p2.Cards.First();

                // DebugWriteLine($"p1={p1Card}, p1Cards={string.Join(",", p1.Cards)}");
                // DebugWriteLine($"p2={p2Card}, p2Cards={string.Join(",", p2.Cards)}\n");
                p1.Cards.RemoveAt(0);
                p2.Cards.RemoveAt(0);

                if (p1.Cards.Count() >= p1Card && p2.Cards.Count() >= p2Card)
                {
                    if (SubGame(1, p1.Cards.Select(_ => _).Take(p1Card).ToList(), p2.Cards.Select(_ => _).Take(p2Card).ToList()))
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
                else if (p1Card > p2Card)
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
                sum += (winner.Count - i) * winner[i];
            }

            return sum.ToString();
        }

        private bool SubGame(int level, List<int> p1Cards, List<int> p2Cards)
        {
            HashSet<string> previousRounds = new HashSet<string>();
            while (p1Cards.Count > 0 && p2Cards.Count > 0)
            {
                string roundId = $"{string.Join(",", p1Cards)}|{string.Join(",", p2Cards)}";
                if (previousRounds.Contains(roundId))
                {
                    return true;
                }
                previousRounds.Add(roundId);

                int p1Card = p1Cards.First();
                int p2Card = p2Cards.First();
                // DebugWriteLine($"[sub {level}]{new string('\t', level)}p1={p1Card}, p1Cards={string.Join(",", p1Cards)}");
                // DebugWriteLine($"[sub {level}]{new string('\t', level)}p2={p2Card}, p2Cards={string.Join(",", p2Cards)}\n");
                p1Cards.RemoveAt(0);
                p2Cards.RemoveAt(0);

                if (p1Cards.Count() >= p1Card && p2Cards.Count() >= p2Card)
                {
                    if (SubGame(level + 1, p1Cards.Select(_ => _).Take(p1Card).ToList(), p2Cards.Select(_ => _).Take(p2Card).ToList()))
                    {
                        p1Cards.Add(p1Card);
                        p1Cards.Add(p2Card);
                    }
                    else
                    {
                        p2Cards.Add(p2Card);
                        p2Cards.Add(p1Card);
                    }
                }
                else if (p1Card > p2Card)
                {
                    p1Cards.Add(p1Card);
                    p1Cards.Add(p2Card);
                }
                else
                {
                    p2Cards.Add(p2Card);
                    p2Cards.Add(p1Card);
                }
            }
            return p1Cards.Count > 0;
        }
    }
}