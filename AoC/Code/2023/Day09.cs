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
                Output = "114",
                RawInput =
@"0 3 6 9 12 15
1 3 6 10 15 21
10 13 16 21 30 45"
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

        public class Oasis
        {
            public List<long> Values { get; set; }
            public long NextValue { get; set; }
            public Action<string> PrintFunc { get; set; }

            public Oasis()
            {
                PrintFunc = (_) => {};
                Values = new List<long>();
            }

            public static Oasis Parse(string input)
            {
                return new Oasis() { Values = new List<long>(Util.Number.SplitL(input, ' ')) };
            }

            public void AddNextValue()
            {
                NextValue = WorkDown(Values);
                PrintFunc($"{string.Join(',', Values)} -> {NextValue}");
            }

            public long WorkDown(List<long> prevHistory)
            {
                List<long> curHistory = new List<long>();
                for (int i = 0; i < prevHistory.Count - 1; ++i)
                {
                    curHistory.Add(prevHistory[i + 1] - prevHistory[i]);
                }

                IEnumerable<long> distinctValues = curHistory.Distinct();
                if (distinctValues.Count() == 1 && distinctValues.First() == 0)
                {
                    PrintFunc($"{string.Join(',', curHistory)} -> 0");
                    return prevHistory.Last();
                }
                else
                {
                    long nextValue = WorkDown(curHistory);
                    PrintFunc($"{string.Join(',', curHistory)} -> {nextValue}");
                    return nextValue + prevHistory.Last();
                }
            }

            public override string ToString()
            {
                return $"{string.Join(',', Values)} -> {NextValue}";
            }
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables)
        {
            List<Oasis> allOasis = inputs.Select(Oasis.Parse).ToList();
            foreach (Oasis oasis in allOasis)
            {
                //oasis.PrintFunc = DebugWriteLine;
                oasis.AddNextValue();
            }
            return allOasis.Select(o => o.NextValue).Sum().ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);
    }
}