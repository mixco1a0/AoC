using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2024
{
    class Day11 : Core.Day
    {
        public Day11() { }

        public override string GetSolutionVersion(Core.Part part)
        {
            return part switch
            {
                // Core.Part.One => "v1",
                // Core.Part.Two => "v1",
                _ => base.GetSolutionVersion(part),
            };
        }

        protected override List<Core.TestDatum> GetTestData()
        {
            List<Core.TestDatum> testData =
            [
                new Core.TestDatum
                {
                    TestPart = Core.Part.One,
                    Output = "7",
                    Variables = new Dictionary<string, string> { { nameof(_BlinkCount), "1" } },
                    RawInput =
@"0 1 10 99 999"
                },
                new Core.TestDatum
                {
                    TestPart = Core.Part.One,
                    Output = "22",
                    Variables = new Dictionary<string, string> { { nameof(_BlinkCount), "6" } },
                    RawInput =
@"125 17"
                },
                new Core.TestDatum
                {
                    TestPart = Core.Part.Two,
                    Output = "",
                    RawInput =
@""
                },
            ];
            return testData;
        }

        private int _BlinkCount { get; }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool _)
        {
            GetVariable(nameof(_BlinkCount), 25, variables, out int blinkCount);
            List<long> stones = Util.Number.SplitL(inputs.First(), ' ').ToList();
            for (int _bc = 0; _bc < blinkCount; ++_bc)
            {
                List<long> newStones = [];
                foreach(long stone in stones)
                {
                    if (stone == 0)
                    {
                        newStones.Add(1);
                    }
                    else if (stone.ToString().Length % 2 == 0)
                    {
                        string s = stone.ToString();
                        string s1 = s[..(s.Length / 2)];
                        string s2 = s[(s.Length / 2)..];
                        newStones.Add(long.Parse(s1));
                        newStones.Add(long.Parse(s2));
                    }
                    else
                    {
                        newStones.Add(stone * 2024);
                    }
                }
                stones = [.. newStones];
            }
            return stones.Count.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);
    }
}