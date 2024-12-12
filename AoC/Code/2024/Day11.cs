using System;
using System.Collections.Generic;
using System.Linq;
using AoC.Base;

namespace AoC._2024
{
    class Day11 : Core.Day
    {
        public Day11() { }

        public override string GetSolutionVersion(Core.Part part)
        {
            return part switch
            {
                Core.Part.One => "v2",
                Core.Part.Two => "v2",
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
            ];
            return testData;
        }

        private int _BlinkCount { get; }

        private static void UpdateStoneCount(ref Dictionary<long, long> stones, long stone, long count)
        {
            if (stones.ContainsKey(stone))
            {
                stones[stone] += count;
            }
            else
            {
                stones[stone] = count;
            }
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, int maxBlinkCount)
        {
            GetVariable(nameof(_BlinkCount), maxBlinkCount, variables, out int blinkCount);
            Dictionary<long, long[]> stoneCache = [];
            Dictionary<long, long> stones = [];
            foreach (long stone in Util.Number.SplitL(inputs.First(), ' '))
            {
                UpdateStoneCount(ref stones, stone, 1);
            }

            for (int _bc = 0; _bc < blinkCount; ++_bc)
            {
                Dictionary<long, long> stonesAfterBlink = [];
                foreach (var pair in stones)
                {
                    if (stoneCache.TryGetValue(pair.Key, out long[] nextStones))
                    {
                        foreach (long nextStone in nextStones)
                        {
                            UpdateStoneCount(ref stonesAfterBlink, nextStone, pair.Value);
                        }
                        continue;
                    }

                    if (pair.Key == 0)
                    {
                        UpdateStoneCount(ref stonesAfterBlink, 1, pair.Value);
                        stoneCache[0] = [1];
                        continue;
                    }

                    string stoneString = pair.Key.ToString();
                    if (stoneString.Length % 2 == 0)
                    {
                        string s = stoneString.ToString();
                        long lowerStone = long.Parse(s[..(s.Length / 2)]);
                        long upperStone = long.Parse(s[(s.Length / 2)..]);
                        UpdateStoneCount(ref stonesAfterBlink, lowerStone, pair.Value);
                        UpdateStoneCount(ref stonesAfterBlink, upperStone, pair.Value);
                        stoneCache[pair.Key] = [lowerStone, upperStone];
                    }
                    else
                    {
                        long newStone = pair.Key * 2024;
                        UpdateStoneCount(ref stonesAfterBlink, newStone, pair.Value);
                        stoneCache[pair.Key] = [newStone];
                    }
                }
                stones = stonesAfterBlink;
            }

            return stones.Select(pair => pair.Value).Sum().ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, 25);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, 75);
    }
}