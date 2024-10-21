using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2023
{
    class Day09 : Core.Day
    {
        public Day09() { }

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

        public override bool SkipTestData => true;

        protected override List<Core.TestDatum> GetTestData()
        {
            List<Core.TestDatum> testData = new List<Core.TestDatum>();
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.One,
                Output = "114",
                RawInput =
@"0 3 6 9 12 15
1 3 6 10 15 21
10 13 16 21 30 45"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "2",
                RawInput =
@"0 3 6 9 12 15
1 3 6 10 15 21
10 13 16 21 30 45"
            });
            return testData;
        }

        public class Oasis
        {
            public List<long> Values { get; set; }
            public long NextValue { get; set; }
            public long PrevValue { get; set; }

            public Oasis()
            {
                Values = new List<long>();
                NextValue = 0;
                PrevValue = 0;
            }

            public static Oasis Parse(string input)
            {
                return new Oasis() { Values = new List<long>(Util.Number.SplitL(input, ' ')) };
            }

            public void AddNextValue()
            {
                NextValue = WorkDown(Values, true);
            }

            public void AddPrevValue()
            {
                PrevValue = WorkDown(Values, false);
            }

            private long WorkDown(List<long> prevHistory, bool isNext)
            {
                List<long> curHistory = new List<long>();
                for (int i = 0; i < prevHistory.Count - 1; ++i)
                {
                    curHistory.Add(prevHistory[i + 1] - prevHistory[i]);
                }

                IEnumerable<long> distinctValues = curHistory.Distinct();
                if (distinctValues.Count() == 1 && distinctValues.First() == 0)
                {
                    if (isNext)
                    {
                        return prevHistory.Last();
                    }
                    else
                    {
                        return prevHistory.First();
                    }
                }
                else
                {
                    long value = WorkDown(curHistory, isNext);
                    if (isNext)
                    {
                        return value + prevHistory.Last();
                    }
                    else
                    {
                        return prevHistory.First() - value;
                    }
                }
            }

            public override string ToString()
            {
                return $"{string.Join(',', Values)} -> {NextValue}";
            }
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool isNext)
        {
            List<Oasis> allOasis = inputs.Select(Oasis.Parse).ToList();
            foreach (Oasis oasis in allOasis)
            {
                if (isNext)
                {
                    oasis.AddNextValue();
                }
                else
                {
                    oasis.AddPrevValue();
                }
            }
            return allOasis.Select(o => isNext ? o.NextValue : o.PrevValue).Sum().ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);
    }
}