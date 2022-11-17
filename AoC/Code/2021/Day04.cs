using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AoC.Core;

namespace AoC._2021
{
    class Day04 : Day
    {
        public Day04() { }

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
                Output = "1924",
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
            return testData;
        }

        private class BingoBoard
        {
            private int Row { get { return 0b11111; } }
            private int Col { get { return 0b00001_00001_00001_00001_00001; } }

            private Dictionary<int, Base.Point> Numbers { get; set; }
            private int CallState { get; set; }
            private int Size { get; set; }
            private int LastCall { get; set; }
            public bool Completed { get; set; }

            public BingoBoard(List<string> rawBoard)
            {
                Size = rawBoard.Count();
                LastCall = -1;
                Completed = false;
                CallState = 0;

                Numbers = new Dictionary<int, Base.Point>();
                for (int i = 0; i < Size; ++i)
                {
                    int[] row = rawBoard[i].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
                    for (int j = 0; j < Size; ++j)
                    {
                        Numbers.Add(row[j], new Base.Point(j, i));
                    }
                }
            }

            public void Print(Action<string> PrintFunc)
            {
                int[,] numbers = new int[Size, Size];
                for (int i = 0; i < Size; ++i)
                {
                    for (int j = 0; j < Size; ++j)
                    {
                        numbers[i, j] = -1;
                    }
                }
                foreach (KeyValuePair<int, Base.Point> pair in Numbers)
                {
                    numbers[pair.Value.X, pair.Value.Y] = pair.Key;
                }


                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < Size; ++i)
                {
                    for (int j = 0; j < Size; ++j)
                    {
                        int binaryShifted = 1 << (i * Size + (Size - j - 1));
                        if ((CallState & binaryShifted) == binaryShifted)
                        {
                            sb.Append("-- ");
                        }
                        else
                        {

                            sb.Append($"{numbers[j, i],2} ");
                        }
                    }
                    PrintFunc(sb.ToString());
                    sb.Clear();
                }
            }

            public void Call(int call)
            {
                if (Completed)
                {
                    return;
                }

                LastCall = call;
                if (Numbers.ContainsKey(call))
                {
                    Base.Point coords = Numbers[call];
                    int binaryShifted = 1 << (coords.Y * Size + (Size - coords.X - 1));
                    CallState |= binaryShifted;
                    int rowCheck = Row << coords.Y * Size;
                    int colCheck = Col << (Size - coords.X - 1);
                    Completed = (rowCheck & CallState) == rowCheck || (colCheck & CallState) == colCheck;
                    Numbers.Remove(call);
                }
            }

            public int GetScore()
            {
                return Numbers.Keys.Sum() * LastCall;
            }
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool getBest)
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
                if (getBest)
                {
                    BingoBoard winner = boards.Where(b => b.Completed).FirstOrDefault();
                    if (winner != null)
                    {
                        return winner.GetScore().ToString();
                    }
                }
                else
                {
                    if (boards.Count == 1)
                    {
                        if (boards[0].Completed)
                        {
                            return boards[0].GetScore().ToString();
                        }
                    }
                    else
                    {
                        boards.RemoveAll(b => b.Completed);
                    }
                }
            }
            return string.Empty;
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);
    }
}