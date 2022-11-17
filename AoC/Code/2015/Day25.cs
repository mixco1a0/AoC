using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2015
{
    class Day25 : Core.Day
    {
        public Day25() { }

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
                Output = "1601130",
                RawInput =
@"3 3"
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

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables)
        {
            List<int> coords = inputs.First().Split(" ,.".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Where(c => { int res; return int.TryParse(c, out res); }).Select(int.Parse).ToList();
            long code = 20151125;
            int targetR = coords[0];
            int targetC = coords[1];
            int c = 1, r = 1, maxR = 1;
            while (true)
            {
                code = (code * 252533) % 33554393;
                maxR = Math.Max(maxR, r);
                if (--r < 1)
                {
                    r = maxR + 1;
                    c = 1;
                }
                else
                {
                    ++c;
                }

                if (targetR == r && targetC == c)
                {
                    break;
                }
            }
            return code.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
        { RunPart1Solution(inputs, variables); return "50"; }
    }
}