using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2021
{
    class Day04 : Day
    {
        public Day04() { }

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
                Output = "4512",
                RawInput =
@"7,4,9,5,11,17,23,2,0,14,21,24,10,16,13,6,15,25,12,22,18,20,8,19,3,26,1

22 13 17 11  0
 8  2 23  4 24
21  9 14 16  7
 6 10  3 18  5
 1 12 20 15 19

 3 15  0  2 22
 9 18 13 17  5
19  8  7 25 23
20 11 10 24  4
14 21 16 12  6

14 21 17 24  4
10 16 15  9 19
18  8 23 26 20
22 11 13  6  5
 2  0 12  3  7
"
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

        private class BingoBoard
        {
            int[,] Numbers { get; set; }
            bool[,] Called { get; set; }
            int Size { get; set; }
            int LastCall { get; set; }
            public bool Completed { get; set; }

            public BingoBoard(List<string> rawBoard)
            {
                Size = rawBoard.Count();
                LastCall = -1;
                Completed = false;

                Numbers = new int[Size, Size];
                Called = new bool[Size, Size];
                for (int i = 0; i < Size; ++i)
                {
                    int[] row = rawBoard[i].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
                    for (int j = 0; j < Size; ++j)
                    {
                        Numbers[i, j] = row[j];
                        Called[i, j] = false;
                    }
                }
            }

            public void Call(int call)
            {
                LastCall = call;

                int x = -1, y = -1;
                for (int i = 0; x == -1 && i < Size; ++i)
                {
                    for (int j = 0; y == -1 && j < Size; ++j)
                    {
                        if (Numbers[i, j] == call)
                        {
                            Called[i, j] = true;
                            x = i;
                            y = j;
                        }
                    }
                }
                if (x == -1 && y == -1)
                {
                    return;
                }

                bool cur = true;
                for (int i = 0; cur && i < Size; ++i)
                {
                    cur &= Called[i, y];
                }
                if (!cur)
                {
                    cur = true;
                    for (int j = 0; cur && j < Size; ++j)
                    {
                        cur &= Called[x, j];
                    }
                }
                Completed = cur;
            }

            public int GetScore()
            {
                int score = 0;
                for (int i = 0; i < Size; ++i)
                {
                    for (int j = 0; j < Size; ++j)
                    {
                        if (!Called[i, j])
                        {
                            score += Numbers[i, j];
                        }
                    }
                }
                return score * LastCall;
            }
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables)
        {
            int[] callOrder = inputs.First().Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();

            List<BingoBoard> boards = new List<BingoBoard>();
            List<string> curBoard = new List<string>();
            foreach (string input in inputs.Skip(1))
            {
                if (string.IsNullOrWhiteSpace(input))
                {
                    if (curBoard.Count > 0)
                    {
                        boards.Add(new BingoBoard(curBoard));
                        curBoard.Clear();
                    }
                }
                else
                {
                    curBoard.Add(input);
                }
            }

            foreach (int call in callOrder)
            {
                boards.ForEach(b => b.Call(call));
                BingoBoard winner = boards.Where(b => b.Completed).FirstOrDefault();
                if (winner != null)
                {
                    return winner.GetScore().ToString();
                }
            }
            return string.Empty;
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);
    }
}