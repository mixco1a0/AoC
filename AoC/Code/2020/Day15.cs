using System.Collections.Generic;
using System.Linq;

namespace AoC._2020
{
    class Day15 : Day
    {
        public Day15() { }
        public override string GetSolutionVersion(Part part)
        {
            switch (part)
            {
                case Part.One:
                    return "v1";
                case Part.Two:
                    return "v1";
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
                Output = "436",
                RawInput =
@"0,3,6"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.Two,
                Output = "175594",
                RawInput =
@"0,3,6"
            });
            return testData;
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
                {
                    return 0;
                }
                return Index - PrevIndex;
            }
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
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
                if (index == 2020)
                {
                    return curNumber.ToString();
                }

                if (!turns.ContainsKey(curNumber))
                {
                    turns[curNumber] = new TurnInfo { Index = index++, PrevIndex = -1 };
                }
                else
                {
                    turns[curNumber].Bump(index++);
                }

                prevNumber = curNumber;
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
                if (index == 30000000)
                {
                    return curNumber.ToString();
                }

                if (!turns.ContainsKey(curNumber))
                {
                    turns[curNumber] = new TurnInfo { Index = index++, PrevIndex = -1 };
                }
                else
                {
                    turns[curNumber].Bump(index++);
                }

                prevNumber = curNumber;
            }
        }
    }
}