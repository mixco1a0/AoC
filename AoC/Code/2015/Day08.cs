using System.Collections.Generic;

namespace AoC._2015
{
    class Day08 : Core.Day
    {
        public Day08() { }

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
                Output = "15",
                RawInput =
@"''
'abc'
'aaa\'aaa'
'\x27'
'\\'"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
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