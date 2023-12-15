using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2023
{
    class Day15 : Core.Day
    {
        public Day15() { }

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
                Output = "1320",
                RawInput =
@"rn=1,cm-,qp=3,cm=2,qp-,pc=4,ot=9,ab=5,pc-,pc=6,ot=7"
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

        private class Sequence
        {
            public string Raw { get; set; }

            public static Sequence Parse(string input)
            {
                Sequence sequence = new Sequence();
                sequence.Raw = input;
                return sequence;
            }

            public long Hash()
            {
                long hash = 0;
                foreach (char raw in Raw)
                {
                    hash += (int)raw;
                    hash *= 17;
                    hash %= 256;
                }
                return hash;
            }

        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables)
        {
            List<Sequence> sequences = Util.String.Split(inputs.First(), ',').Select(Sequence.Parse).ToList();
            return sequences.Select(s => s.Hash()).Sum().ToString();
            //return string.Empty;
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);
    }
}