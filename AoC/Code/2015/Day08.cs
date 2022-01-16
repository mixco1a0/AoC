using System.Collections.Generic;

using AoC.Core;

namespace AoC._2015
{
    class Day08 : Day
    {
        public Day08() { }

        public override string GetSolutionVersion(Part part)
        {
            switch (part)
            {
                case Part.One:
                    return "v1";
                case Part.Two:
                    return "v1";
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
                Output = "15",
                RawInput =
@"''
'abc'
'aaa\'aaa'
'\x27'
'\\'"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.Two,
                Output = "25",
                RawInput =
@"''
'abc'
'aaa\'aaa'
'\x27'
'\\'"
            });
            return testData;
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool expand)
        {
            int total = 0;
            int mem = 0;
            foreach (string input in inputs)
            {
                string actual = input.Replace('\'', '"');
                total += actual.Length;
                if (expand)
                {
                    actual = actual.Replace("\\\\", "||||").Replace("\"", "\\\\\"").Replace("\\x", "\\\\x");
                    mem += actual.Length;
                }
                else
                {
                    string[] splits = actual.Replace("\\\\", "|").Split('\\');
                    foreach (string split in splits)
                    {
                        if (split.StartsWith('x'))
                        {
                            mem += split.Length - 2;
                        }
                        else
                        {
                            mem += split.Length;
                        }
                    }
                    mem -= 2; // the first and last quote
                }
            }
            
            if (expand)
            {
                return (mem - total).ToString();
            }
            return (total - mem).ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);
    }
}