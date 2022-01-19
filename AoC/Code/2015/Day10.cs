using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AoC._2015
{
    class Day10 : Core.Day
    {
        public Day10() { }

        public override string GetSolutionVersion(Core.Part part)
        {
            switch (part)
            {
                case Core.Part.One:
                    return "v2";
                case Core.Part.Two:
                    return "v2";
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
                Variables = new Dictionary<string, string> { { "times", "5" } },
                Output = "6",
                RawInput =
@"1"
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

        private string Process(string input)
        {
            StringBuilder processed = new StringBuilder();
            char cur = input[0];
            int count = 1;
            char[] restOfInput = input.Skip(1).ToArray();
            foreach (char c in restOfInput)
            {
                if (c == cur)
                {
                    ++count;
                }
                else
                {
                    processed.Append(count);
                    processed.Append(cur);
                    cur = c;
                    count = 1;
                }
            }
            processed.Append(count);
            processed.Append(cur);
            return processed.ToString();
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, int defaultTimes)
        {
            int times;
            GetVariable(nameof(times), defaultTimes, variables, out times);

            string input = inputs.First();
            for (int i = 0; i < times; ++i)
            {
                input = Process(input);
                //DebugWriteLine($"{i} complete [{input.Length}]");
            }
            return input.Length.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, 40);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, 50);
    }
}