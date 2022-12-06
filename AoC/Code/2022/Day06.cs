using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2022
{
    class Day06 : Core.Day
    {
        public Day06() { }

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
                Output = "7",
                RawInput =
@"mjqjpqmgbljsphdztnvjfqwrcgsmlb"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.One,
                Output = "5",
                RawInput =
@"bvwbjplbgvbhsrlpgdmjqwftvncz"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.One,
                Output = "6",
                RawInput =
@"nppdvjthqldpwncqszvftbrmjlhg"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.One,
                Output = "10",
                RawInput =
@"nznrnfrfntjfmvfwmzdfjlvtqnbhcprsg"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.One,
                Output = "11",
                RawInput =
@"zcfzfwzzqfrljwzlrfnpqdbhtmscgvjw"
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
            string sequence = inputs.First();
            for (int i = 0; i < sequence.Length - 4; ++i)
            {
                if (sequence.Substring(i, 4).DistinctBy(_ => _).Count() == 4)
                {
                    return (i + 4).ToString();
                }
            }
            return string.Empty;
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);
    }
}