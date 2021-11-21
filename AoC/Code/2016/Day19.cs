using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2016
{
    class Day19 : Day
    {
        public Day19() { }

        public override string GetSolutionVersion(Part part)
        {
            switch (part)
            {
                // case Part.One:
                //     return "v1";
                // case Part.Two:
                //     return "v1";
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
                Output = "3",
                RawInput =
@"5"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.Two,
                Output = "",
                RawInput =
@""
            });
            return testData;
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables)
        {
            int elfCount = int.Parse(inputs.First());
            List<int> elves = Enumerable.Range(1, elfCount).ToList();
            while (elves.Count > 1)
            {
                bool removeFront = elves.Count % 2 != 0;
                elves = elves.Select((e, i) => new {e, i}).Where(p => p.i % 2 == 0).Select(p => p.e).ToList();
                if (elves.Count > 1 && removeFront)
                {
                    elves.RemoveAt(0);
                }
            }
            return elves.First().ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);
    }
}