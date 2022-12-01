using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2022
{
    class Day01 : Core.Day
    {
        public Day01() { }

        public override string GetSolutionVersion(Core.Part part)
        {
            switch (part)
            {
                case Core.Part.One:
                    return "v1";
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
                Output = "24000",
                RawInput =
@"1000
2000
3000

4000

5000
6000

7000
8000
9000

10000"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "45000",
                RawInput =
@"1000
2000
3000

4000

5000
6000

7000
8000
9000

10000"
            });
            return testData;
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool top1)
        {
            List<int> cals = new List<int>();
            cals.Add(0);
            int calIdx = 0;
            foreach (string input in inputs)
            {
                if (string.IsNullOrWhiteSpace(input))
                {
                    cals.Add(0);
                    calIdx++;
                }
                else
                {
                    cals[calIdx] += int.Parse(input);
                }
            }
            if (top1)
            {
                return cals.Max().ToString();
            }
            else
            {
                cals.Sort();
                return cals.TakeLast(3).Sum().ToString();
            }
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);
    }
}