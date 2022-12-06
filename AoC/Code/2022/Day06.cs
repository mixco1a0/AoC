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
                Output = "19",
                RawInput =
@"mjqjpqmgbljsphdztnvjfqwrcgsmlb"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "23",
                RawInput =
@"bvwbjplbgvbhsrlpgdmjqwftvncz"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "23",
                RawInput =
@"nppdvjthqldpwncqszvftbrmjlhg"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "29",
                RawInput =
@"nznrnfrfntjfmvfwmzdfjlvtqnbhcprsg"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "26",
                RawInput =
@"zcfzfwzzqfrljwzlrfnpqdbhtmscgvjw"
            });
            return testData;
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, int messageLen)
        {
            string sequence = inputs.First();
            for (int i = 0; i < sequence.Length - messageLen; ++i)
            {
                if (sequence.Substring(i, messageLen).DistinctBy(_ => _).Count() == messageLen)
                {
                    return (i + messageLen).ToString();
                }
            }
            return string.Empty;
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, 4);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, 14);
    }
}