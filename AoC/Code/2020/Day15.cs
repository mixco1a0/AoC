using System.Collections.Generic;
using System.Linq;

namespace AoC._2020
{
    class Day15 : Day
    {
        public Day15() { }
        protected override List<TestDatum> GetTestData()
        {
            List<TestDatum> testData = new List<TestDatum>();
            testData.Add(new TestDatum
            {
                TestPart = TestPart.One,
                Output = "436",
                RawInput =
@"0,3,6"
            });
            testData.Add(new TestDatum
            {
                TestPart = TestPart.Two,
                Output = "175594",
                RawInput =
@"0,3,6"
            });
            return testData;
        }

        class Turn
        {
            public int Index { get; set; }
            public int Number { get; set; }
            public int LastIndex { get; set; }
            public override string ToString()
            {
                return $"{Number} @ {Index}";
            }
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            List<int> numbers = inputs[0].Split(",").Select(int.Parse).ToList();
            List<Turn> turns = new List<Turn>();
            int index = 1;
            foreach (int number in numbers)
            {
                turns.Add(new Turn { Index = index++, Number = number });
                // Debug($"Turn {turns.Last().Index} = {turns.Last().Number}");
            }
            while (true)
            {
                Turn prevTurn = turns.Last();
                int numCount = turns.Where(t => t.Number == prevTurn.Number).Count();
                if (numCount == 1)
                    turns.Add(new Turn { Index = index++, Number = 0 });
                else
                {
                    List<Turn> temp = turns.Where(t => t.Number == prevTurn.Number).ToList();
                    if (temp.Count == 1)
                    {
                        turns.Add(new Turn { Index = index++, Number = numCount });
                    }
                    else
                    {
                        Turn secLast = temp[temp.Count - 2];
                        Turn last = temp[temp.Count - 1];
                        turns.Add(new Turn { Index = index++, Number = last.Index - secLast.Index });
                    }
                }

                // Debug($"Turn {turns.Last().Index} = {turns.Last().Number}");

                if (turns.Last().Index == 2020)
                    return turns.Last().Number.ToString();
            }
        }

        class TurnInfo
        {
            public long Index { get; set; }
            public long PrevIndex { get; set; }
            public void Bump(long idx)
            {
                PrevIndex = Index;
                Index = idx;
            }
            public long GetNext()
            {
                if (PrevIndex == -1)
                    return 0;
                return Index - PrevIndex;
            }
        }

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            List<int> numbers = inputs[0].Split(",").Select(int.Parse).ToList();
            Dictionary<long, TurnInfo> turns = new Dictionary<long, TurnInfo>();
            long index = 1;
            long prevNumber = 0;
            foreach (int number in numbers)
            {
                turns[number] = new TurnInfo { Index = index++, PrevIndex = -1 };
                prevNumber = number;
            }

            while (true)
            {

                TurnInfo turnInfo = turns[prevNumber];
                long curNumber = turnInfo.GetNext();
                // Debug($"Turn {index} = {curNumber}");
                if (index == 30000000)
                    return curNumber.ToString();

                if (!turns.ContainsKey(curNumber))
                    turns[curNumber] = new TurnInfo { Index = index++, PrevIndex = -1 };
                else
                    turns[curNumber].Bump(index++);

                prevNumber = curNumber;


            }
        }
    }
}