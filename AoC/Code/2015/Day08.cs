using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2015
{
    class Day08 : Day
    {
        public Day08() { }
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
                Output = "",
                RawInput =
@""
            });
            return testData;
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            int total = 0;
            int mem = 0;
            foreach (string input in inputs)
            {
                string actual = input.Replace('\'', '"');
                total += actual.Length;
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
            return (total - mem).ToString();
        }

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            return "";
        }
    }
}