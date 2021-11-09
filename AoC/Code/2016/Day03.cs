using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2016
{
    class Day03 : Day
    {
        public Day03() { }
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
                Output = "1",
                RawInput =
@"5 10 25
3 4 5
5 10 30"
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

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            int possible = 0;
            foreach (string input in inputs)
            {
                List<int> sides = input.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
                sides.Sort();
                if (sides[0] + sides[1] > sides[2])
                {
                    possible++;
                }
            }
            return possible.ToString();
        }

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            int possible = 0;
            for (int i = 0; i + 2 < inputs.Count;)
            {
                List<int> sidesA = inputs[i].Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
                List<int> sidesB = inputs[i + 1].Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
                List<int> sidesC = inputs[i + 2].Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
                for (int j = 0; j < 3; ++j, ++i)
                {
                    List<int> cur = sidesA.Skip(j).Take(1).Concat(sidesB.Skip(j).Take(1)).Concat(sidesC.Skip(j).Take(1)).ToList();
                    cur.Sort();
                    if (cur[0] + cur[1] > cur[2])
                    {
                        possible++;
                    }
                }
            }
            return possible.ToString();
        }
    }
}