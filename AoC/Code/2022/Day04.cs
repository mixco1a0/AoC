using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2022
{
    class Day04 : Core.Day
    {
        public Day04() { }

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
                Output = "2",
                RawInput =
@"2-4,6-8
2-3,4-5
5-7,7-9
2-8,3-7
6-6,4-6
2-6,4-8"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "4",
                RawInput =
@"2-4,6-8
2-3,4-5
5-7,7-9
2-8,3-7
6-6,4-6
2-6,4-8"
            });
            return testData;
        }

        static Base.Range[] Parse(string input)
        {
            Base.Range[] ranges = new Base.Range[2];
            string[] split = input.Split("-,".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            ranges[0] = new Base.Range(int.Parse(split[0]), int.Parse(split[1]));
            ranges[1] = new Base.Range(int.Parse(split[2]), int.Parse(split[3]));
            return ranges;
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool fullyContainCheck)
        {
            List<Base.Range[]> ranges = inputs.Select(Parse).ToList();
            if (fullyContainCheck)
            {
                return ranges.Where(r => r[0].HasInc(r[1]) || r[1].HasInc(r[0])).Count().ToString();
            }
            else
            {
                return ranges.Where(r => r[0].HasIncOr(r[1]) || r[1].HasIncOr(r[0])).Count().ToString();
            }
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);
    }
}