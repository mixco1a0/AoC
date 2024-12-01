using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2021
{
    class Day21 : Core.Day
    {
        public Day21() { }

        public override string GetSolutionVersion(Core.Part part)
        {
            switch (part)
            {
                case Core.Part.One:
                    return "v1";
                case Core.Part.Two:
                    return "v1";
                default:
                    return base.GetSolutionVersion(part);
            }
        }

        protected override List<Core.TestDatum> GetTestData()
        {
            List<Core.TestDatum> testData = new List<Core.TestDatum>();
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.One,
                Output = "739785",
                RawInput =
@"Player 1 starting position: 4
Player 2 starting position: 8"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "444356092776315",
                RawInput =
@"Player 1 starting position: 4
Player 2 starting position: 8"
            });
            return testData;
        }

        private struct GameState
        {
            public int MaxScore { get; init; }
            public int P1 { get; set; }
            public int P1Score { get; set; }
            public int P2 { get; set; }
            public int P2Score { get; set; }
            public bool P1Turn { get; set; }
            public int DiceVal { get; set; }
            public int TurnCount { get; set; }

            public GameState(int maxScore, int p1, int p2)
            {
                MaxScore = maxScore;
                P1 = p1;
                P1Score = 0;
                P2 = p2;
                P2Score = 0;
                P1Turn = true;
                DiceVal = 0;
                TurnCount = 0;
            }

            public GameState Copy()
            {
                GameState copy = new GameState() { MaxScore = MaxScore};
                copy.P1 = P1;
                copy.P1Score = P1Score;
                copy.P2 = P2;
                copy.P2Score = P2Score;
                copy.P1Turn = P1Turn;
                copy.DiceVal = DiceVal;
                copy.TurnCount = TurnCount;
                return copy;
            }

            public bool P1Win()
            {
                return P1Score > P2Score && P1Score >= MaxScore;
            }

            public bool P2Win()
            {
                return P2Score > P1Score && P2Score >= MaxScore;
            }

            public int GetLoserScore()
            {
                return Math.Min(P1Score, P2Score);
            }

            public override int GetHashCode()
            {
                int hash = P1;
                hash = hash << 10 | P1Score;
                hash = hash << 3 | P2;
                hash = hash << 10 | P2Score;
                hash = hash << 1 | (P1Turn ? 1 : 0);
                return hash;
            }

            public override string ToString()
            {
                string playerTurn = P1Turn ? "P1" : "P2";
                return $"p1={P1} ({P1Score}) | p2={P2} ({P2Score}) [{playerTurn}]";
            }
        }

        struct WinCount
        {
            public long P1 { get; set; }
            public long P2 { get; set; }
            public WinCount()
            {
                P1 = 0;
                P2 = 0;
            }
            public long Get()
            {
                return Math.Max(P1, P2);
            }
        }

        Dictionary<GameState, WinCount> Cache = new Dictionary<GameState, WinCount>();
        private static readonly Dictionary<int, int> DiracRolls = new Dictionary<int, int>() { { 3, 1 }, { 4, 3 }, { 5, 6 }, { 6, 7 }, { 7, 6 }, { 8, 3 }, { 9, 1 } };

        private WinCount RunRealGame(GameState state)
        {
            if (Cache.ContainsKey(state))
            {
                return Cache[state];
            }

            if (state.P1Win())
            {
                return new WinCount() { P1 = 1, P2 = 0 };
            }
            if (state.P2Win())
            {
                return new WinCount() { P1 = 0, P2 = 1 };
            }

            WinCount winCount = new WinCount();
            foreach (var pair in DiracRolls)
            {
                GameState nextState = state.Copy();
                if (state.P1Turn)
                {
                    int newP = (state.P1 + pair.Key) % 10;
                    nextState.P1 = newP;
                    nextState.P1Score += newP + 1;
                }
                else
                {
                    int newP = (state.P2 + pair.Key) % 10;
                    nextState.P2 = newP;
                    nextState.P2Score += newP + 1;
                }
                nextState.P1Turn = !nextState.P1Turn;
                WinCount nextCount = RunRealGame(nextState);
                winCount.P1 += pair.Value * nextCount.P1;
                winCount.P2 += pair.Value * nextCount.P2;
            }

            Cache[state] = winCount;
            return winCount;
        }

        private GameState RunPracticeGame(GameState state)
        {
            while (state.P1Score < state.MaxScore && state.P2Score < state.MaxScore)
            {
                ++state.TurnCount;
                int p = state.P1Turn ? state.P1 : state.P2;
                p += ++state.DiceVal % 100 + ++state.DiceVal % 100 + ++state.DiceVal % 100;
                if (state.P1Turn)
                {
                    state.P1 = p % 10;
                    state.P1Score += state.P1 + 1;
                }
                else
                {
                    state.P2 = p % 10;
                    state.P2Score += state.P2 + 1;
                }
                state.P1Turn = !state.P1Turn;
            }
            return state;
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, int maxScore, bool practiceGame)
        {
            int p1 = int.Parse($"{inputs.First().Last()}") - 1;
            int p2 = int.Parse($"{inputs.Last().Last()}") - 1;
            GameState initState = new GameState(maxScore, p1, p2);
            if (practiceGame)
            {
                GameState state = RunPracticeGame(initState);
                return (state.TurnCount * 3 * state.GetLoserScore()).ToString();
            }
            return RunRealGame(initState).Get().ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, 1000, true);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, 21, false);
    }
}