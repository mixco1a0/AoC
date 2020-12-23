using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2020
{
    class Day23 : Day
    {
        public Day23() { }
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
                Output = "67384529",
                RawInput =
@"389125467"
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

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            List<int> cups = inputs[0].ToCharArray().Select(c => int.Parse(c.ToString())).ToList();

            int curCupIdx = 0;
            int offset = 0;
            for (int i = 0; i < 100; ++i)
            {
                DebugWriteLine($"-- move {i + 1} --");
                DebugWriteLine($"cups: {string.Join(",", cups)}");
                List<int> removedCups = new List<int>(cups.GetRange(curCupIdx + 1, 3));
                DebugWriteLine($"cups: {string.Join(",", removedCups)}");
                cups.RemoveRange(curCupIdx + 1, 3);

                int destIdx = -1;
                for (int c = 1; c < 10; ++c)
                {
                    int cupCount = cups.Where(cup => cup == cups[curCupIdx] - c).Count();
                    if (cupCount == 1)
                    {
                        DebugWriteLine($"destination: {cups[curCupIdx] - c}");
                        destIdx = cups.IndexOf(cups[curCupIdx] - c);
                        break;
                    }
                }
                if (destIdx < 0)
                {
                    DebugWriteLine($"destination: {cups.Max()}");
                    destIdx = cups.IndexOf(cups.Max());
                }
                cups.InsertRange(destIdx + 1, removedCups);
                cups.Add(cups.First());
                cups.RemoveAt(curCupIdx);
                ++offset;
            }

            while (cups[0] != 1)
            {
                cups.Add(cups.First());
                cups.RemoveAt(0);
            }

            return string.Join("", cups);
        }

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            return "";
        }
    }
}