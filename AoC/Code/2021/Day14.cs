using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2021
{
    class Day14 : Day
    {
        public Day14() { }

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
                //Variables = new Dictionary<string, string>() { { "steps", "5" } },
                Output = "1588",
                RawInput =
@"NNCB

CH -> B
HH -> N
CB -> H
NH -> C
HB -> C
HC -> B
HN -> C
NN -> C
BH -> H
NC -> B
NB -> B
BN -> B
BB -> N
BC -> B
CC -> N
CN -> C"
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
            int steps;
            Util.GetVariable(nameof(steps), 10, variables, out steps);

            string polymer = inputs.First();
            Dictionary<string, char> insertionRules = new Dictionary<string, char>();
            foreach (string input in inputs.Skip(2))
            {
                string[] split = input.Split(" ->".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                insertionRules[split[0]] = split[1][0];
            }

            for (int i = 0; i < steps; ++i)
            {
                StringBuilder sb = new StringBuilder();
                for (int j = 0; j < polymer.Length - 1; ++j)
                {
                    sb.Append(polymer[j]);
                    sb.Append(insertionRules[polymer.Substring(j, 2)]);
                    //sb.Append(polymer[j+1]);
                }
                sb.Append(polymer.Last());
                polymer = sb.ToString();
            }
            IEnumerable<int> counts = polymer.GroupBy(c => c).Select(g => g.Count());
            return (counts.Max() - counts.Min()).ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);
    }
}