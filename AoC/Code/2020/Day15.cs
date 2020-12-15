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
                Output = "",
                RawInput =
@""
            });
            return testData;
        }

        class Turn
        {
            public int Index { get; set; }
            public int Number { get; set; }
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

                if (turns.Last().Index == 2020)
                    return turns.Last().Number.ToString();
            }
            return "";
        }

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            return "";
        }
    }
}